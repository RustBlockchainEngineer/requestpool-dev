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
using Domain.Models.Lookups;
using Domain.Models;
using Foundation.Areas.Admin.Models;
using System.IO;
using Foundation.Areas.Public.Models;
using System.Data.Common;
using System.Data.SqlClient;
using MobileServices.Core;

namespace Foundation.Areas.Admin.Controllers.API
{
    [RoutePrefix("admin/api/enquiries")]
    [CustomAuthorize(SystemRoles.ViewEnquiries)]

    public class EnquiriesController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public EnquiriesController()
        {
        }
        
        


        [ResponseType(typeof(ResponseMsg<IEnumerable<EnquiryViewModel>>))]
        [HttpGet]
        [Route("search")]
        public HttpResponseMessage Search([FromUri]EnquirySearchModel searchModel)
        {
            if (searchModel == null)
                searchModel = new EnquirySearchModel();
            searchModel.Init();
            ResponseMsg<IEnumerable<EnquiryViewModel>> Msg = new ResponseMsg<IEnumerable<EnquiryViewModel>>();

            Msg.Content = db.Enquiries
                .Include(i => i.PublicUser)
               .Include(m => m.Project)
               .Include(m => m.Client)
               .Include(m => m.EnquiryType)
               .Include(m => m.Invitations)
               .Include(m => m.Status)
               .Include(m => m.Invitations.Select(i => i.Recipients))
               .Where(searchModel.Search)
               .OrderByDescending(i => i.Id)
               .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value)
               .ToList().
               Select(m => new EnquiryViewModel(m));
            Msg.TotalCount = db.Enquiries.Where(searchModel.Search).Count();

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<EnquiryViewModel>))]
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Get(long id)
        {

            ResponseMsg<EnquiryViewModel> Msg = new ResponseMsg<EnquiryViewModel>();
            var item = db.Enquiries
                .Include(m => m.Project)
                .Include(m => m.Client)
                .Include(i => i.PublicUser)
                .Include(m => m.Invitations)
                .Include(m => m.Status)
               .Include(m => m.Invitations.Select(i => i.Recipients))
                .FirstOrDefault(m => m.Id == id);
            if (item == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            db.Entry(item).Reference(u => u.Project).Load();
            db.Entry(item).Reference(u => u.EnquiryType).Load();
            Msg.Content = new EnquiryViewModel(item);

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<EnquiryViewModel>))]
        [HttpGet]
        [Route("{id}/invitations-count")]
        public HttpResponseMessage GetInvitationsCount(long id)
        {

            ResponseMsg<long> Msg = new ResponseMsg<long>();
            long invitationsCount = db.Invitations.Count(i => i.EnquiryId == id && !i.IsDraft);

            Msg.Content = invitationsCount;

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>>))]
        [HttpGet]
        [Route("preview/properties/{enquiryId}")]
        public HttpResponseMessage PreviewProperties(long enquiryId)
        {
            ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>> Msg = new ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>>();
            var enquiry = db.Enquiries
                .Include(m => m.Project)
                .Include(m => m.Client)
                .FirstOrDefault(m => m.Id == enquiryId);

            if (enquiry == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg);
            }

            var list = db.EnquiryItemsDynamicProperties
               .Include(m => m.Property)
               .Include(m => m.Property.PropertyType)
               .Where(i => i.EnquiryId == enquiry.Id && i.IsPublic)
               .ToList();
            foreach (var item in list)
            {
                if (item.Property.IsDeleted)
                {
                    item.Property.Name = item.Property.Name.Replace("[Deleted]", "");
                }
            }
            Msg.Content = list.Select(m => new EnquiryItemsDynamicPropertyViewModel(m));

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<EnquiryPreviewViewModel>))]
        [HttpGet]
        [Route("preview/{enquiryId}")]
        public HttpResponseMessage Preview([FromUri]long enquiryId)
        {
            ResponseMsg<EnquiryPreviewViewModel> Msg = new ResponseMsg<EnquiryPreviewViewModel>();
            var enquiry = db.Enquiries
                .Include(m => m.Project)
                .Include(m => m.Client)
                .Include(i => i.Items)
                .Include(i => i.Items.Select(m => m.DynamicProperties))
                .Include(i => i.PublicUser)

                .FirstOrDefault(m => m.Id == enquiryId);

            if (enquiry == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg);
            }

            Msg.Content = new EnquiryPreviewViewModel(enquiry.Items);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
