using Domain.Models;
using Domain.Models.Lookups;
using Foolproof;
using Foundation.Areas.Public.Models.ViewModels;
using Foundation.Core;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace Foundation.Areas.Public.Models
{
    public class InvitationResponseViewModel
    {
        public bool IsDraftResponse { get; set; }
        public List<ItemResponseViewModel> Responses { get; set; }
        public List<ReceivedItemViewModel> Items { get; set; }

        public InvitationResponseViewModel()
        {

        }
        public InvitationResponseViewModel(bool isDraftResponse, IEnumerable<ItemResponse> responses, IEnumerable<Item> items)
        {
            IsDraftResponse = isDraftResponse;
            Responses = responses.Select(r => new ItemResponseViewModel(r)).ToList();
            Items = items.Select(i => new ReceivedItemViewModel(i)).ToList();
        }
    }
    public class InvitationResponseFormModel
    {
        public long ItemId { get; set; }
        public string Response { get; set; }
        public List<ItemDynamicPropertyResponseFormModel> Properties { get; set; }

        public virtual void UpdateModel(ItemResponse model)
        {
            model.ItemId = ItemId;
            model.Response = Response;
        }
    }

    public class ItemDynamicPropertyResponseFormModel
    {
        public long PropertyId { get; set; }
        public string Value { get; set; }

        public virtual void UpdateModel(ItemDynamicPropertyResponse model)
        {
            model.PropertyId = PropertyId;
            model.Value = Value;
        }
    }
    public class InvitationResponsePostModel
    {
        public long InvitationId { get; set; }
        public bool IsDraftResponse { get; set; }
        public List<InvitationResponseFormModel> Items { get; set; }



    }
    public class RecipientResponseViewModel
    {
        public long Id { get; set; }
        public long ContactId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsFailed { get; set; }
        public string FailureReason { get; set; }
        public DateTime? ResponseSubmitDate { get; set; }
        public bool IsDraftResponse { get; set; }

        public List<ItemResponseViewModel> ItemsResponse { get; set; }
        public RecipientResponseViewModel()
        {

        }
        public RecipientResponseViewModel(Recipient model,IEnumerable<ItemResponse> items)
        {
            Id = model.Id;
            ContactId = model.ContactId;
            Name = model.Contact.Name;
            Email = model.Contact.Email;
            Phone = model.Contact.Phone;
            IsFailed = model.IsFailed;
            FailureReason = model.FailureReason;
            ResponseSubmitDate = model.ResponseSubmitDate;
            IsDraftResponse = model.IsDraftResponse;
            ItemsResponse = items.Select(i => new ItemResponseViewModel(i)).ToList();// model.ItemsResponse.Select
        }
    }
    public class ItemResponseViewModel
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public string Response { get; set; }
        public List<ItemDynamicPropertyResponseViewModel> Properties { get; set; }

        public ItemResponseViewModel()
        {

        }
        public ItemResponseViewModel(ItemResponse model)
        {
            Id = model.Id;
            ItemId = model.ItemId;
            Response = model.Response;
            Properties = new List<ItemDynamicPropertyResponseViewModel>();
            if (model.DynamicPropertiesResponses.Count() > 0)
            {
                Properties = model.DynamicPropertiesResponses.Select(r => new ItemDynamicPropertyResponseViewModel(r)).ToList();
            }
        }
    }
    public class ItemDynamicPropertyResponseViewModel
    {
        public long PropertyId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public ItemDynamicPropertyResponseViewModel() { }
        public ItemDynamicPropertyResponseViewModel(ItemDynamicPropertyResponse model)
        {
            PropertyId = model.PropertyId;
            Name = model.Property.Name;
            Value = model.Value;
        }

    }
    

}