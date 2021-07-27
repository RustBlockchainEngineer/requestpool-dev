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
    public class ItemViewModel
    {
        public PublicUserBriefViewModel PublicUser { get; set; }

        public long Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string RevisionNumber { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public List<ItemDynamicPropertyViewModel> Properties { get; set; }

        public ItemTypeBriefViewModel ItemType { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public ItemViewModel()
        {

        }
        public ItemViewModel(Item model)
        {
            
            PublicUser = new PublicUserBriefViewModel(model.Enquiry.PublicUser);
            Id = model.Id;
            Subject = model.Subject;
            ReferenceNumber = model.ReferenceNumber;
            RevisionNumber = model.RevisionNumber;
            Description = model.Description;
            Remarks = model.Remarks;
            CreationDate = model.CreationDate;
            LastUpdateDate = model.LastUpdateDate;
            if(model.ItemTypeId.HasValue)
                ItemType = new ItemTypeBriefViewModel(model.ItemType);
            Properties = new List<ItemDynamicPropertyViewModel>();
            if (model.DynamicProperties.Count() > 0)
            {
                Properties = model.DynamicProperties.Select(p => new ItemDynamicPropertyViewModel(p)).ToList();
            }
        }
    }

    public class ItemSearchViewModel
    {
        public PublicUserBriefViewModel PublicUser { get; set; }

        public long Id { get; set; }
        public long ItemId { get; set; }
        public long DynamicPropertyId { get; set; }
        public string Value { get; set; }
        public EnquiryBriefViewModel Enquiry { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public ItemSearchViewModel()
        {

        }
        public ItemSearchViewModel(ItemDynamicPropertyResponse model)
        {

            PublicUser = new PublicUserBriefViewModel(model.ItemResponse.Item.Enquiry.PublicUser);
            Id = model.Id;
            ItemId = model.ItemResponse.ItemId;
            DynamicPropertyId = model.PropertyId;
            Value = model.Value;
            Enquiry = new EnquiryBriefViewModel(model.ItemResponse.Item.Enquiry);

            CreationDate = model.CreationDate;
            LastUpdateDate = model.LastUpdateDate;
        }
    }

    public class ItemFormModel
    {
        public long? Id { get; set; }
        public long? ItemTypeId { get; set; }
        public string ReferenceNumber { get; set; }
        public string RevisionNumber { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public List<ItemDynamicPropertyFormModel> Properties { get; set; }
        public virtual void UpdateModel(Item model, long enquiryId)
        {
            model.EnquiryId = enquiryId;
            model.ItemTypeId = ItemTypeId;
            model.Subject = Subject;
            model.ReferenceNumber = ReferenceNumber;
            model.RevisionNumber = RevisionNumber;
            model.Description = Description;
            model.Remarks = Remarks;
        }
    }
    public class ItemPostModel
    {
        public long EnquiryId { get; set; }
        public List<ItemFormModel> Items { get; set; }
        public List<EnquiryItemsDynamicPropertyFormModel> Properties { get; set; }


    }


    public class ItemBriefViewModel
    {
        public long Id { get; set; }
        public string Subject { get; set; }

        public ItemBriefViewModel() { }
        public ItemBriefViewModel(Item model)
        {
            Id = model.Id;
            Subject = model.Subject;
        }

    }

    public class ItemSearchModel
    {
        public string Username { get; set; }
        public long? ItemTypeId { get; set; }

        public long? propertyId { get; set; }
        public string propertyValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? PageNumber { get; set; }
        public int? ItemsPerPage { get; set; }

        public int? Skip { get; set; }

        public ItemSearchModel() { }
        public void Init()
        {
            if (!PageNumber.HasValue)
                PageNumber = 1;
            if (!ItemsPerPage.HasValue)
                ItemsPerPage = AppSettings.ItemsPerPage;
            Skip = (PageNumber.Value - 1) * ItemsPerPage.Value;
            if (Username == null)
                Username = "";
            if (propertyValue == null)
                propertyValue = "";
            Search = i => !i.IsDeleted
                           && (!ItemTypeId.HasValue || i.ItemResponse.Item.ItemTypeId == ItemTypeId.Value)
                           &&(i.Value != null && i.Value.Trim() != "")
                           && (!propertyId.HasValue || i.PropertyId == propertyId )
                           && (propertyValue=="" || (i.Value.ToLower().Contains(propertyValue.Trim().ToLower())))
                           && (Username == "" || i.ItemResponse.Item.Enquiry.PublicUser.UserName.ToLower() == Username.ToLower().Trim());

        }
        public Func<ItemDynamicPropertyResponse, bool> Search;
    }

    public class ReceivedItemViewModel
    {
        public long Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string RevisionNumber { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }

        public ItemTypeBriefViewModel ItemType { get; set; }

        public List<ItemDynamicPropertyViewModel> Properties { get; set; }

        public ReceivedItemViewModel()
        {

        }
        public ReceivedItemViewModel(Item model)
        {
            Id = model.Id;
            Subject = model.Subject;
            ReferenceNumber = model.ReferenceNumber;
            RevisionNumber = model.RevisionNumber;
            Description = model.Description;
            if (model.ItemTypeId.HasValue)
                ItemType = new ItemTypeBriefViewModel(model.ItemType);
            Properties = new List<ItemDynamicPropertyViewModel>();
            if (model.DynamicProperties.Count() > 0)
            {
                Properties = model.DynamicProperties.Select(p => new ItemDynamicPropertyViewModel(p)).ToList();
            }
        }
    }

    public class EnquiryItemsDynamicPropertyViewModel
    {
        public long PropertyId { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
        public bool? IsPublic { get; set; }
        public bool? IsInfoOnly { get; set; }

        public PropertyTypeBriefViewModel PropertyType { get; set; }

        public EnquiryItemsDynamicPropertyViewModel()
        {

        }
        public EnquiryItemsDynamicPropertyViewModel(EnquiryItemsDynamicProperty model)
        {
            PropertyId = model.Property.Id;
            Name = model.Property.Name;
            Rank = model.Rank;
            IsPublic = model.IsPublic;
            IsInfoOnly = model.IsInfoOnly;
            PropertyType = new PropertyTypeBriefViewModel(model.Property.PropertyType);
        }
    }

    public class EnquiryItemsDynamicPropertiesFormModel
    {
        public long EnquiryId { get; set; }
        public List<EnquiryItemsDynamicPropertyFormModel> Properties { get; set; }
    }


    public class EnquiryItemsDynamicPropertyFormModel
    {
        public long PropertyId { get; set; }
        public int Rank { get; set; }
        public bool IsPublic { get; set; }
        public bool IsInfoOnly { get; set; }
        public string Formula { get; set; }


        public void UpdateModel(EnquiryItemsDynamicProperty model)
        {
            model.PropertyId = PropertyId;
            model.Rank = Rank;
            model.IsPublic = IsPublic;
            model.IsInfoOnly = IsInfoOnly;
        }
    }


    public class ItemDynamicPropertyViewModel
    {
        public long PropertyId { get; set; }
        public long ItemId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsApplicable { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsRequired { get; set; }

        public PropertyTypeBriefViewModel PropertyType { get; set; }
        public ItemDynamicPropertyViewModel()
        {

        }
        public ItemDynamicPropertyViewModel(ItemDynamicProperty property)
        {
            PropertyId = property.DynamicPropertyId;
            ItemId = property.ItemId;
            Name = property.DynamicProperty.Name;
            Value = property.Value;
            IsApplicable = property.IsApplicable;
            IsReadOnly = property.IsReadOnly;
            IsRequired = property.IsRequired;
            PropertyType = new PropertyTypeBriefViewModel(property.DynamicProperty.PropertyType);
        }
    }
    public class ItemDynamicPropertyFormModel
    {
        public long PropertyId { get; set; }
        public string Value { get; set; }
        public bool IsApplicable { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsRequired { get; set; }

        public virtual void UpdateModel(ItemDynamicProperty model)
        {
            model.DynamicPropertyId = PropertyId;
            model.Value = Value;
            model.IsApplicable = IsApplicable;
            model.IsReadOnly = IsReadOnly;
            model.IsRequired = IsRequired;
        }
    }

}