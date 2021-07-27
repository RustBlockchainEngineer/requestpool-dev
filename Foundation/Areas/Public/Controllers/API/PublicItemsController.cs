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
using System.Web;

namespace Foundation.Areas.Public.Controllers.API
{
    [RoutePrefix("api/items")]
    public class PublicItemsController : ApiPublicController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        long _currentUserId = -1;

        public PublicItemsController()
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
        [ResponseType(typeof(ResponseMsg<IEnumerable<ItemViewModel>>))]
        [HttpGet]
        [Route("{enquiryId}")]
        public HttpResponseMessage All(long enquiryId)
        {
            ResponseMsg<IEnumerable<ItemViewModel>> Msg = new ResponseMsg<IEnumerable<ItemViewModel>>();

            Msg.Content = db.Items
                .Include(i => i.Enquiry.PublicUser)
               .Include(m => m.ItemType)
               .Include(m => m.DynamicProperties)
               .Where(i => !i.IsDeleted && i.EnquiryId == enquiryId && i.Enquiry.PublicUserId == currentUserId)
               .ToList().
               Select(m => new ItemViewModel(m));

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<IEnumerable<ItemViewModel>>))]
        [HttpGet]
        [Route("search")]
        public HttpResponseMessage Search([FromUri] ItemSearchModel searchModel)
        {
            if (searchModel == null)
                searchModel = new ItemSearchModel();
            searchModel.Init();
            ResponseMsg<IEnumerable<ItemSearchViewModel>> Msg = new ResponseMsg<IEnumerable<ItemSearchViewModel>>();


            //var query = db.ItemDynamicProperties
            //   .Include(i => i.Item)
            //   .Include(i => i.Item.ItemType)
            //   .Include(i => i.Item.Enquiry.PublicUser)
            //   .Where(i => i.Item.Enquiry.PublicUserId == currentUserId)
            //   .Where(searchModel.Search)
            //   .OrderByDescending(i => i.Id)
            //   .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value);
            var query = db.ItemDynamicPropertyResponses
               .Include(i => i.ItemResponse.Item)
               .Include(i => i.ItemResponse.Item.ItemType)
               .Include(i => i.ItemResponse.Item.Enquiry.PublicUser)
               .Where(i => i.ItemResponse.Item.Enquiry.PublicUserId == currentUserId)
               .Where(searchModel.Search)
               .OrderByDescending(i => i.Id)
               .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value);
            Msg.Content = query
               .ToList().
               Select(m => new ItemSearchViewModel(m));
            Msg.TotalCount = db.ItemDynamicPropertyResponses
                    .Where(i => i.ItemResponse.Item.Enquiry.PublicUserId == currentUserId)
                    .Where(searchModel.Search).Count();

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<ItemViewModel>))]
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Post(ItemPostModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            var enquiry = db.Enquiries.FirstOrDefault(i => i.Id == model.EnquiryId
                        && i.PublicUserId == currentUserId);
            if (enquiry == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            long invitationsCount = db.Invitations.Count(i => i.EnquiryId == model.EnquiryId && !i.IsDraft);
            if (invitationsCount > 0)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.enquiry_invitations_sent;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            var enquiryItemsPropertiesToDelete = db.EnquiryItemsDynamicProperties.Where(i => i.EnquiryId == model.EnquiryId);
            var itemPropertiesToDelete = db.ItemDynamicProperties.Where(i => i.Item.EnquiryId == model.EnquiryId);
            var itemsToDelete = db.Items.Where(i => i.EnquiryId == model.EnquiryId);

            //db.Database.ExecuteSqlCommand("UPDATE dbo.Items SET CopiedFromId=NULL WHERE CopiedFromId IN (SELECT Id FROM dbo.Items WHERE EnquiryId = "+model.EnquiryId+")");
            db.EnquiryItemsDynamicProperties.RemoveRange(enquiryItemsPropertiesToDelete);
            db.ItemDynamicProperties.RemoveRange(itemPropertiesToDelete);
            db.Items.RemoveRange(itemsToDelete);
            foreach (EnquiryItemsDynamicPropertyFormModel p in model.Properties)
            {
                var itemToSave = new EnquiryItemsDynamicProperty();
                p.UpdateModel(itemToSave);
                itemToSave.EnquiryId = model.EnquiryId;
                itemToSave.CreatorId = currentUserId;
                db.EnquiryItemsDynamicProperties.Add(itemToSave);
            }
            foreach (ItemFormModel i in model.Items)
            {
                var itemToSave = new Item();
                i.UpdateModel(itemToSave, model.EnquiryId);
                bool someValueExists = false;
                foreach (ItemDynamicPropertyFormModel p in i.Properties)
                {
                    if (!String.IsNullOrEmpty(p.Value) && !String.IsNullOrEmpty(p.Value.Trim()))
                    {
                        someValueExists = true;
                        break;
                    }
                }
                if (!someValueExists)
                    continue;
                itemToSave.CreatorId = currentUserId;
                db.Items.Add(itemToSave);
                itemToSave.DynamicProperties = new List<ItemDynamicProperty>();
                foreach (ItemDynamicPropertyFormModel p in i.Properties)
                {
                    var propertyToSave = db.ItemDynamicProperties.Create();
                    p.UpdateModel(propertyToSave);
                    foreach (EnquiryItemsDynamicPropertyFormModel column in model.Properties)
                    {
                        if (column.PropertyId == p.PropertyId)
                        {
                            if (column.IsInfoOnly)
                            {
                                p.IsApplicable = true;
                                p.IsReadOnly = true;
                                p.IsRequired = false;
                            }
                            break;
                        }
                    }
                    propertyToSave.CreatorId = currentUserId;
                    itemToSave.DynamicProperties.Add(propertyToSave);
                    //db.ItemDynamicProperties.Add(propertyToSave);
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

            return All(model.EnquiryId);
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>>))]
        [HttpGet]
        [Route("included-properties/{enquiryId}")]
        public HttpResponseMessage Properties(long enquiryId)
        {
            ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>> Msg = new ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>>();
            var list = db.EnquiryItemsDynamicProperties
              .Include(m => m.Property)
              .Include(m => m.Property.PropertyType)
              .Where(i => i.EnquiryId == enquiryId && i.Enquiry.PublicUserId == currentUserId)
              .ToList();

            Msg.Content = list.Select(m => new EnquiryItemsDynamicPropertyViewModel(m));
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>>))]
        [HttpPut]
        [Route("included-properties/{enquiryId}")]
        public async Task<HttpResponseMessage> UpdateProperties(long enquiryId, EnquiryItemsDynamicPropertiesFormModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            var enquiry = db.Enquiries.FirstOrDefault(i => i.Id == model.EnquiryId
                        && i.PublicUserId == currentUserId);
            if (enquiry == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            long invitationsCount = db.Invitations.Count(i => i.EnquiryId == model.EnquiryId && !i.IsDraft);
            if (invitationsCount > 0)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.enquiry_invitations_sent;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            var enquiryItemsPropertiesToDelete = db.EnquiryItemsDynamicProperties.Where(i => i.EnquiryId == model.EnquiryId);
            db.EnquiryItemsDynamicProperties.RemoveRange(enquiryItemsPropertiesToDelete);
            foreach (EnquiryItemsDynamicPropertyFormModel p in model.Properties)
            {
                var itemToSave = new EnquiryItemsDynamicProperty();
                p.UpdateModel(itemToSave);
                itemToSave.EnquiryId = model.EnquiryId;
                itemToSave.CreatorId = currentUserId;
                db.EnquiryItemsDynamicProperties.Add(itemToSave);
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
            return Properties(model.EnquiryId);

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}


//var list = db.ItemsDynamicProperties
//    .Include(m => m.PropertyType)
//    .GroupJoin(db.EnquiryItemsDynamicProperties.Where(i => i.EnquiryId == enquiryId && !i.Property.IsDeleted),
//    itemsProperty => itemsProperty.Id ,
//    enquiryItemsProperty => enquiryItemsProperty.PropertyId,
//    (itemsProperty, enquiryItemsProperties) => new { itemsProperty, enquiryItemsProperties })
//    .Select(
//        inputElement => new { inputElement.itemsProperty, enquiryItemsProperties = inputElement.enquiryItemsProperties.DefaultIfEmpty() }
//    )
//    .ToList()
//    .Select(x=> new { x.itemsProperty, enquiryItemsProperty = x.enquiryItemsProperties.ElementAtOrDefault(0) })
//    .ToList();

//Msg.Content = list.Select(m=> new EnquiryItemsDynamicPropertyViewModel(m.itemsProperty
//    , m.enquiryItemsProperty == null?new EnquiryItemsDynamicProperty():m.enquiryItemsProperty));