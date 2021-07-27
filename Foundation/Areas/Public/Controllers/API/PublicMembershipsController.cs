using Foundation.Core;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Mapster;
using Domain.Models;
using System.IO;
using System.Web.Http.ModelBinding;
using Foundation.Areas.Public.Models;

namespace Foundation.Areas.Public.Controllers.API
{
    [RoutePrefix("api/membership")]
    public class PublicMembershipController : ApiPublicController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private AdministratorUserManager _administratorUserManager;
        public AdministratorUserManager AdministratorUserManager
        {
            get
            {
                return _administratorUserManager ?? Request.GetOwinContext().GetUserManager<AdministratorUserManager>();
            }
            private set
            {
                _administratorUserManager = value;
            }
        }

        long _currentUserId = -1;
        long currentUserId
        {
            get
            {
                if (_currentUserId == -1)
                    _currentUserId = User.Identity.GetUserId<long>();
                return _currentUserId;
            }
        }
        public PublicMembershipController()
        {
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<MembershipViewModel>>))]
        [HttpGet]
        [Route("")]
        public HttpResponseMessage All()
        {
            ResponseMsg<IEnumerable<MembershipViewModel>> Msg = new ResponseMsg<IEnumerable<MembershipViewModel>>();
            Msg.Content = db.Memberships.Include(m => m.MembershipPlan)
                .Include(m=> m.DowngradeToMembershipPlan)
                .Where(m => m.PublicUserId == currentUserId && !m.IsDeleted)
                .ToList().Select(m =>new MembershipViewModel(m));
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<MembershipViewModel>))]
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Get(long id)
        {
            ResponseMsg<MembershipViewModel> Msg = new ResponseMsg<MembershipViewModel>();
            var item = db.Memberships.FirstOrDefault(m=> m.Id == id && m.PublicUserId == currentUserId);
            if (item == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            db.Entry(item).Reference(u => u.MembershipPlan).Load();
            Msg.Content = new MembershipViewModel(item);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }
        [ResponseType(typeof(ResponseMsg<CurrentMembershipViewModel>))]
        [HttpGet]
        [Route("current")]
        public HttpResponseMessage Current()
        {
            ResponseMsg<CurrentMembershipViewModel> Msg = new ResponseMsg<CurrentMembershipViewModel>();
            var item = db.Memberships.Include(m => m.MembershipPlan).FirstOrDefault(m => m.PublicUserId == currentUserId
                        && (m.EndDate == null || DbFunctions.DiffDays(DateTime.UtcNow, m.EndDate) >= 0));
            if (item == null)
            {
                MembershipPlan defaultDowngradePlan = null;
                var lastMembership = db.Memberships.Where(i => i.PublicUserId == currentUserId)
                    .OrderByDescending(i => i.EndDate)
                    .FirstOrDefault();
                if(lastMembership == null || !lastMembership.DowngradeToMembershipPlanId.HasValue)
                {
                    defaultDowngradePlan = db.MembershipPlans.FirstOrDefault(i => i.IsDefaultDowngrade);
                }
                else
                {
                    defaultDowngradePlan = lastMembership.DowngradeToMembershipPlan;
                }
                if(defaultDowngradePlan != null)
                {
                    var system = AdministratorUserManager.FindByName(SystemRoles.System);
                    item = new Membership()
                    {
                        CreatorId = system.Id,
                        MembershipPlanId = defaultDowngradePlan.Id,
                        PublicUserId = currentUserId,
                        StartDate = DateTime.UtcNow
                    };
                    db.Memberships.Add(item);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception exp) { }
                }
                if (item == null)
                {
                    item = new Membership() { MembershipPlan = new MembershipPlan() };
                }
            }
            Msg.Content = new CurrentMembershipViewModel(item);
            Msg.Content.Consumption = GetConsumption(item);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<ConsumptionViewModel>))]
        [HttpGet]
        [Route("consumption")]
        public async Task<HttpResponseMessage> Consumption()
        {
            ResponseMsg<ConsumptionViewModel> Msg = new ResponseMsg<ConsumptionViewModel>();
            var membership = db.Memberships.Include(m=> m.MembershipPlan )
                .FirstOrDefault(m => m.PublicUserId == currentUserId
                && (m.EndDate == null || DbFunctions.DiffDays(DateTime.UtcNow, m.EndDate) >= 0));
            if (membership == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            Msg.Content = GetConsumption(membership);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        protected ConsumptionViewModel GetConsumption(Membership membership)
        {
            int startYear = DateTime.UtcNow.Year;
            int startMonth = DateTime.UtcNow.Month;
            DateTime startDate, endDate;
            if (membership.StartDate.Day > DateTime.UtcNow.Day)
            {
                if (DateTime.UtcNow.Month == 1)
                {
                    startYear--;
                    startMonth = 12;
                }
                else
                {
                    startMonth--;
                }
            }
            startDate = new DateTime(startYear, startMonth, membership.StartDate.Day);
            endDate = startDate.AddMonths(1).AddDays(-1);

            var consumption = new ConsumptionViewModel();
            consumption.StartDate = startDate;
            consumption.EndDate = endDate;
            //EnquiriesPerMonth
            consumption.EnquiriesPerMonth = db.Enquiries.Count(m => m.PublicUserId == currentUserId && !m.IsDeleted
                    && DbFunctions.DiffDays(startDate, m.CreationDate) >= 0
                    && DbFunctions.DiffDays(m.CreationDate, endDate) >= 0);
            //ItemFields
            consumption.ItemFields = db.ItemsDynamicProperties.Count(m => m.PublicUserId == currentUserId && !m.IsDeleted);
            //InvitationsPerMonth
            consumption.InvitationsPerMonth = db.Invitations.Count(m => m.Enquiry.PublicUserId == currentUserId && !m.IsDeleted
                    && DbFunctions.DiffDays(startDate, m.CreationDate) >= 0
                    && DbFunctions.DiffDays(m.CreationDate, endDate) >= 0);

            return consumption;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
