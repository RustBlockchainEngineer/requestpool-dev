using Domain.Models;
using Foolproof;
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
    public class ItemsDynamicPropertyViewModel
    {
        public PublicUserBriefViewModel PublicUser { get; set; }

        public long Id { get; set; }
        public string Name { get; set; }
        public PropertyTypeBriefViewModel PropertyType { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool IsDeleted { get; set; }

        public ItemsDynamicPropertyViewModel()
        {

        }
        public ItemsDynamicPropertyViewModel(ItemsDynamicProperty model)
        {
            PublicUser = new PublicUserBriefViewModel(model.PublicUser);

            Id = model.Id;
            Name = model.Name;
            CreationDate = model.CreationDate;
            LastUpdateDate = model.LastUpdateDate;
            IsDeleted = model.IsDeleted;
            PropertyType = new PropertyTypeBriefViewModel(model.PropertyType);

        }
    }

    public class ItemsDynamicPropertyFormModel
    {
        public long PropertyTypeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.FullName, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_full_name")]
        public string Name { get; set; }


        public virtual void UpdateModel(ItemsDynamicProperty model)
        {
            model.Name = Name;
            model.PropertyTypeId = PropertyTypeId;
        }
    }
    public class ItemsDynamicPropertyPostModel : ItemsDynamicPropertyFormModel
    {

    }

    public class ItemsDynamicPropertyPutModel : ItemsDynamicPropertyFormModel
    {

    }

    public class ItemsDynamicPropertyBriefViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public ItemsDynamicPropertyBriefViewModel() { }
        public ItemsDynamicPropertyBriefViewModel(ItemsDynamicProperty model)
        {
            Id = model.Id;
            Name = model.Name;
        }

    }

    public class ItemsDynamicPropertySearchModel
    {
        public string Username { get; set; }

        public string Name { get; set; }
        public bool? IsDeleted { get; set; }

        public int? PageNumber { get; set; }
        public int? ItemsPerPage { get; set; }

        public int? Skip { get; set; }

        public ItemsDynamicPropertySearchModel() { }
        public void Init()
        {
            if (!PageNumber.HasValue)
                PageNumber = 1;
            if (!ItemsPerPage.HasValue)
                ItemsPerPage = AppSettings.ItemsPerPage;
            Skip = (PageNumber.Value - 1) * ItemsPerPage.Value;
            if (Name == null)
                Name = "";
            if (Username == null)
                Username = "";
            Search = i =>
                            (!IsDeleted.HasValue || i.IsDeleted == IsDeleted.Value)
                            && (Name == "" || i.Name.ToLower().Contains(Name.ToLower()))
                            && (Username == "" || i.PublicUser.UserName.ToLower() == Username.ToLower().Trim());
        }
        public Func<ItemsDynamicProperty, bool> Search;
    }


}