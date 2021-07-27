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

namespace Foundation.Areas.Public.Controllers.API
{
    [RoutePrefix("api/enquiries")]
    public class PublicEnquiriesController : ApiPublicController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        long _currentUserId = -1;

        public PublicEnquiriesController()
        {
        }

        long currentUserId
        {
            get
            {
                if (_currentUserId == -1)
                    _currentUserId = User.Identity.GetUserId<long>();
                return _currentUserId;
            }
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<EnquiryViewModel>>))]
        [HttpGet]
        [Route("")]
        public HttpResponseMessage All(bool? isTemplate)
        {
            ResponseMsg<IEnumerable<EnquiryViewModel>> Msg = new ResponseMsg<IEnumerable<EnquiryViewModel>>();
            Msg.Content = db.Enquiries
                .Include(i => i.PublicUser)
               .Include(m => m.Project)
               .Include(m => m.Client)
               .Include(m => m.EnquiryType)
               .Include(m=> m.Invitations)
               .Include(m=> m.Status)
               .Include(m=> m.Invitations.Select(i=> i.Recipients))
               .Where(i => i.PublicUserId == currentUserId && (!isTemplate.HasValue || i.IsTemplate == isTemplate))
               .ToList().
               Select(m => new EnquiryViewModel(m));
            

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<IEnumerable<EnquiryViewModel>>))]
        [HttpGet]
        [Route("search")]
        public HttpResponseMessage Get([FromUri]EnquirySearchModel searchModel)
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
               .Where(i => i.PublicUserId == currentUserId)
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
                .Include(i => i.PublicUser)
                .Include(m => m.Project)
                .Include(m => m.Client)
                .Include(m => m.Invitations)
                .Include(m => m.Status)
               .Include(m => m.Invitations.Select(i => i.Recipients))
                .FirstOrDefault(m => m.Id == id && m.PublicUserId == currentUserId);
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
            long invitationsCount = db.Invitations.Count(i => i.EnquiryId == id && !i.IsDraft
            && i.Enquiry.PublicUserId == currentUserId);

            Msg.Content = invitationsCount;

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<EnquiryViewModel>))]
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Post(EnquiryPostModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            if (!model.StatusId.HasValue && model.IsTemplate.Value) 
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            var itemToSave = new Enquiry();
            model.UpdateModel(itemToSave);

            if (model.ProjectId.HasValue)
            {
                var project = db.Projects.FirstOrDefault(i => i.Id == itemToSave.ProjectId
                                && i.Client.PublicUserId == currentUserId);
                if (project == null)
                {
                    ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                    ErrorMsg.Message = Resources.Errors.http_bad_request;
                    return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
                }
                itemToSave.ClientId = project.ClientId;
            }
            else if (model.ClientId.HasValue)
            {
                var client = db.Clients.FirstOrDefault(i => i.Id == itemToSave.ClientId
                                && i.PublicUserId == currentUserId);
                if (client == null)
                {
                    ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                    ErrorMsg.Message = Resources.Errors.http_bad_request;
                    return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
                }

            }

            itemToSave.PublicUserId = currentUserId;
            itemToSave.CreatorId = currentUserId;

            db.Enquiries.Add(itemToSave);
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

            ResponseMsg<EnquiryViewModel> Msg = new ResponseMsg<EnquiryViewModel>();
            db.Entry(itemToSave).Reference(u => u.Project).Load();
            db.Entry(itemToSave).Reference(u => u.PublicUser).Load();
            db.Entry(itemToSave).Reference(u => u.Client).Load();
            db.Entry(itemToSave).Reference(u => u.EnquiryType).Load();
            db.Entry(itemToSave).Reference(u => u.Status).Load();
            Msg.Content = new EnquiryViewModel(itemToSave);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<EnquiryViewModel>))]
        [HttpPut]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Put(long id, EnquiryPutModel model)
        {

            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            if (!model.StatusId.HasValue && model.IsTemplate.Value)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            var itemToSave = db.Enquiries.Include(i => i.PublicUser).FirstOrDefault(i => i.Id == id && i.PublicUserId == currentUserId);
            if (itemToSave == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            model.UpdateModel(itemToSave);

            if (model.ProjectId.HasValue)
            {
                var project = db.Projects.FirstOrDefault(i => i.Id == itemToSave.ProjectId
                                && i.Client.PublicUserId == currentUserId);
                if (project == null)
                {
                    ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                    ErrorMsg.Message = Resources.Errors.http_bad_request;
                    return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
                }
                itemToSave.ClientId = project.ClientId;
            }
            else if (model.ClientId.HasValue)
            {
                var client = db.Clients.FirstOrDefault(i => i.Id == itemToSave.ClientId
                                && i.PublicUserId == currentUserId);
                if (client == null)
                {
                    ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                    ErrorMsg.Message = Resources.Errors.http_bad_request;
                    return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
                }

            }

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
            ResponseMsg<EnquiryViewModel> Msg = new ResponseMsg<EnquiryViewModel>();
            db.Entry(itemToSave).Reference(u => u.Project).Load();
            db.Entry(itemToSave).Reference(u => u.Client).Load();
            db.Entry(itemToSave).Reference(u => u.EnquiryType).Load();
            db.Entry(itemToSave).Reference(u => u.Status).Load();
            Msg.Content = new EnquiryViewModel(itemToSave);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<EnquiryViewModel>))]
        [HttpPost]
        [Route("copy/{enquiryId}")]
        public async Task<HttpResponseMessage> Copy(long enquiryId, EnquiryCopyFormModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            var enquiryToCopy = db.Enquiries.FirstOrDefault(i => i.Id == enquiryId
                      && i.PublicUserId == currentUserId);
            if (enquiryToCopy == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            if (model.IsRevision.GetValueOrDefault() && (String.IsNullOrEmpty(model.RevisionNumber) || String.IsNullOrEmpty(model.RevisionNumber.Trim())))
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            Enquiry enquiryToSave = new Enquiry()
            {
                PublicUserId = currentUserId,
                CreatorId = currentUserId,
                Description = enquiryToCopy.Description,
                Subject = enquiryToCopy.Subject,
                ReferenceNumber = enquiryToCopy.ReferenceNumber,
                RevisionNumber = enquiryToCopy.RevisionNumber,
                PrNumber = enquiryToCopy.PrNumber,
                BoqNumber = enquiryToCopy.BoqNumber,
                EnquiryTypeId = enquiryToCopy.EnquiryTypeId,
                ClientId = enquiryToCopy.ClientId,
                ProjectId = enquiryToCopy.ProjectId,
                Remarks = enquiryToCopy.Remarks,
                IsTemplate = model.IsTemplate.HasValue ? model.IsTemplate.Value : enquiryToCopy.IsTemplate
            };
            if (!enquiryToSave.IsTemplate)
            {
                enquiryToSave.StatusId = (long)SystemStatus.Open;
            }
            if (model.IsRevision.HasValue && model.IsRevision.Value)
            {
                enquiryToSave.ParentId = enquiryId;
                enquiryToSave.Subject = "[Revision] " + enquiryToSave.Subject;
                enquiryToSave.RevisionNumber = model.RevisionNumber;
            }
            else if (enquiryToSave.IsTemplate)
            {
                enquiryToSave.Subject = "[Template] " + enquiryToSave.Subject;
            }
            else
            {
                enquiryToSave.Subject = "[Copy] " + enquiryToSave.Subject;
            }
            db.Enquiries.Add(enquiryToSave);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            string sql = @"INSERT INTO EnquiryItemsDynamicProperties (EnquiryId,PropertyId,Rank,IsInfoOnly,IsPublic,CreatorId,CreationDate,LastUpdateDate,IsDeleted)
                                SELECT @NewEnquiryId,PropertyId,Rank,IsInfoOnly,IsPublic,@CreatorId,GETUTCDATE(),GETUTCDATE(),0
                                FROM EnquiryItemsDynamicProperties WHERE EnquiryId = @EnquiryId AND IsDeleted = 0";
            int affectedRows = 0;
            try
            {
                affectedRows = db.Database.ExecuteSqlCommand(sql,
                    new SqlParameter("@EnquiryId", enquiryToCopy.Id),
                    new SqlParameter("@NewEnquiryId", enquiryToSave.Id),
                    new SqlParameter("@CreatorId", currentUserId));
            }
            catch (Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            if (affectedRows > 0)
            {
                sql = @"INSERT INTO Items (ReferenceNumber,RevisionNumber,Subject,Description,Remarks,ItemTypeId,EnquiryId,CreatorId,CreationDate,LastUpdateDate,IsDeleted,CopiedFromId)
                                SELECT ReferenceNumber,RevisionNumber,Subject,Description,Remarks,ItemTypeId,@NewEnquiryId,@CreatorId,GETUTCDATE(),GETUTCDATE(),0,Id
                                FROM Items WHERE EnquiryId = @EnquiryId AND IsDeleted = 0";
                try
                {
                    affectedRows = db.Database.ExecuteSqlCommand(sql,
                        new SqlParameter("@EnquiryId", enquiryToCopy.Id),
                        new SqlParameter("@NewEnquiryId", enquiryToSave.Id),
                        new SqlParameter("@CreatorId", currentUserId));
                }
                catch (Exception exp)
                {
                    ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                    ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                    return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
                }
                if (affectedRows > 0)
                {
                    //copy properties
                    sql = @"INSERT INTO ItemDynamicProperties (ItemId,DynamicPropertyId,Value,IsApplicable,IsReadOnly,IsRequired,CreationDate,LastUpdateDate,IsDeleted,CreatorId)
                                SELECT I.Id,P.DynamicPropertyId,P.Value,P.IsApplicable,P.IsReadOnly,P.IsRequired,GETUTCDATE(),GETUTCDATE(),0,@CreatorId
                                FROM ItemDynamicProperties P INNER JOIN Items I ON P.ItemId = I.CopiedFromId 
                                WHERE I.EnquiryId = @EnquiryId AND P.IsDeleted = 0";
                    try
                    {
                        affectedRows = db.Database.ExecuteSqlCommand(sql, new SqlParameter("@EnquiryId", enquiryToSave.Id), new SqlParameter("@CreatorId", currentUserId));
                    }
                    catch (Exception exp)
                    {
                        ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                        ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                        return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
                    }
                }
            }

            return await Get(enquiryToSave.Id);

        }
        [ResponseType(typeof(ResponseMsg<string>))]
        [HttpDelete]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Delete(long id)
        {
            var itemToSave = db.Enquiries.FirstOrDefault(i => i.Id == id && i.PublicUserId == currentUserId);

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


        [ResponseType(typeof(ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>>))]
        [HttpGet]
        [Route("preview/properties/{enquiryId}")]
        public HttpResponseMessage PreviewProperties(long enquiryId)
        {
            ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>> Msg = new ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>>();
            var enquiry = db.Enquiries
                
                .Include(m => m.Project)
                .Include(m => m.Client)
                .FirstOrDefault(m => m.Id == enquiryId && m.PublicUserId == currentUserId);

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

                .FirstOrDefault(m => m.Id == enquiryId && m.PublicUserId == currentUserId);

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
