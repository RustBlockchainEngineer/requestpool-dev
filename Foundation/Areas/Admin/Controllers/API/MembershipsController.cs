using Foundation.Core;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Foundation.Areas.Admin.Models.ViewModels;
using System.Data.Entity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Mapster;
using Domain.Models;
using Foundation.Areas.Admin.Models;
using System.IO;
using System.Web.Http.ModelBinding;
using MobileServices.Core;

namespace Foundation.Areas.Admin.Controllers.API
{
    [RoutePrefix("admin/api/membership")]
    [CustomAuthorize(SystemRoles.ManageMemberships)]

    public class MembershipController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public MembershipController()
        {
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<MembershipViewModel>>))]
        [HttpGet]
        [Route("")]
        public HttpResponseMessage Get()
        {
            ResponseMsg<IEnumerable<MembershipViewModel>> Msg = new ResponseMsg<IEnumerable<MembershipViewModel>>();
            Msg.Content = db.Memberships.Include(m => m.MembershipPlan)
                .Include(m=> m.DowngradeToMembershipPlan)
                .Include(m => m.PublicUser)
                .Include(m => m.Creator).ToList().Select(m =>
                    new MembershipViewModel(m));
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<MembershipViewModel>))]
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Get(long id)
        {
            ResponseMsg<MembershipViewModel> Msg = new ResponseMsg<MembershipViewModel>();
            var item = db.Memberships.Find(id);
            if (item == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            db.Entry(item).Reference(u => u.Creator).Load();
            db.Entry(item).Reference(u => u.PublicUser).Load();
            db.Entry(item).Reference(u => u.MembershipPlan).Load();
            if(item.DowngradeToMembershipPlanId.HasValue)
                db.Entry(item).Reference(u => u.DowngradeToMembershipPlan).Load();

            Msg.Content = new MembershipViewModel(item);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<MembershipViewModel>>))]
        [HttpGet]
        [Route("user/{id}")]
        public HttpResponseMessage GetByUser(long id)
        {
            ResponseMsg<IEnumerable<MembershipViewModel>> Msg = new ResponseMsg<IEnumerable<MembershipViewModel>>();
            Msg.Content = db.Memberships.Where(m => m.PublicUserId == id)
                .Include(m => m.MembershipPlan)
                .Include(m => m.DowngradeToMembershipPlan)
                .Include(m => m.PublicUser)
                .Include(m => m.Creator).ToList().Select(m =>
                    new MembershipViewModel(m));
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<MembershipViewModel>))]
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Post(MembershipPostModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            if (model.EndDate < model.StartDate)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            var currentUserId = User.Identity.GetUserId<long>();
            var itemToSave = new Membership();
            model.UpdateModel(itemToSave);
            itemToSave.CreationDate = DateTime.UtcNow;
            itemToSave.CreatorId = currentUserId;
            db.Memberships.Add(itemToSave);
            try
            {
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            ResponseMsg<MembershipViewModel> Msg = new ResponseMsg<MembershipViewModel>();
            db.Memberships.Attach(itemToSave);
            db.Entry(itemToSave).Reference(u => u.Creator).Load();
            db.Entry(itemToSave).Reference(u => u.PublicUser).Load();
            db.Entry(itemToSave).Reference(u => u.MembershipPlan).Load();

            if (itemToSave.DowngradeToMembershipPlanId.HasValue)
                db.Entry(itemToSave).Reference(u => u.DowngradeToMembershipPlan).Load();
            Msg.Content = new MembershipViewModel(itemToSave);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<MembershipViewModel>))]
        [HttpPut]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Put(long id, MembershipPutModel model)
        {

            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            var itemToSave = db.Memberships.Find(id);
            if (itemToSave == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            if (model.EndDate < model.StartDate)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            model.UpdateModel(itemToSave);
            try
            {
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            ResponseMsg<MembershipViewModel> Msg = new ResponseMsg<MembershipViewModel>();
            db.Entry(itemToSave).Reference(u => u.Creator).Load();
            db.Entry(itemToSave).Reference(u => u.PublicUser).Load();
            db.Entry(itemToSave).Reference(u => u.MembershipPlan).Load();
            if (itemToSave.DowngradeToMembershipPlanId.HasValue)
                db.Entry(itemToSave).Reference(u => u.DowngradeToMembershipPlan).Load();
            Msg.Content = new MembershipViewModel(itemToSave);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<string>))]
        [HttpDelete]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Delete(long id)
        {
            var itemToSave = db.Memberships.Find(id);
            if (itemToSave == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            itemToSave.IsDeleted = true;
            try
            {
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            ResponseMsg<string> Msg = new ResponseMsg<string>();
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<CurrentMembershipViewModel>))]
        [HttpGet]
        [Route("current/{publicUserId}")]
        public HttpResponseMessage Current(long publicUserId)
        {
            ResponseMsg<CurrentMembershipViewModel> Msg = new ResponseMsg<CurrentMembershipViewModel>();
            var item = db.Memberships.Include(m => m.MembershipPlan).FirstOrDefault(m => m.PublicUserId == publicUserId
                        && (m.EndDate == null || DbFunctions.DiffDays(DateTime.UtcNow, m.EndDate) >= 0));
            if (item == null)
            {
                item = new Membership() { MembershipPlan = new MembershipPlan() };
                //ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                //ErrorMsg.Message = Resources.Errors.http_not_found;
                //return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            Msg.Content = new CurrentMembershipViewModel(item);
            Msg.Content.Consumption = GetConsumption(item,publicUserId);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        protected ConsumptionViewModel GetConsumption(Membership membership,long publicUserId)
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
            consumption.EnquiriesPerMonth = db.Enquiries.Count(m => m.PublicUserId == publicUserId && !m.IsDeleted
                    && DbFunctions.DiffDays(startDate, m.CreationDate) >= 0
                    && DbFunctions.DiffDays(m.CreationDate, endDate) >= 0);
            //ItemFields
            consumption.ItemFields = db.ItemsDynamicProperties.Count(m => m.PublicUserId == publicUserId && !m.IsDeleted);
            //InvitationsPerMonth
            consumption.InvitationsPerMonth = db.Invitations.Count(m => m.Enquiry.PublicUserId == publicUserId && !m.IsDeleted
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
