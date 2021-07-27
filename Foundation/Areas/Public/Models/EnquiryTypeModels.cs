using Domain.Models.Lookups;
using Foundation.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Foundation.Areas.Public.Models.ViewModels
{
  
    public class EnquiryTypeViewModel
    {
        public PublicUserBriefViewModel PublicUser { get; set; }

        public long Id { get; set; }
        public string Name { get; set; }

        public DateTime CreationDate { get; set; }
        public bool IsDeleted { get; set; }


        public EnquiryTypeViewModel() {
        }
        public EnquiryTypeViewModel(EnquiryType model)
        {
            PublicUser = new PublicUserBriefViewModel(model.PublicUser);

            Id = model.Id;
            Name = model.Name;
            CreationDate = model.CreationDate;
            IsDeleted = model.IsDeleted;
        }
    }

    public class EnquiryTypeFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public string Name { get; set; }
        

        public void UpdateModel(EnquiryType model)
        {
            model.Name = Name;
        }
    }

    public class EnquiryTypeBriefViewModel
    {
        public EnquiryTypeBriefViewModel() { }
        public EnquiryTypeBriefViewModel(EnquiryType model) {
            Id = model.Id;
            Name = model.Name;
        }
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class EnquiryTypeSearchModel
    {
        public string Username { get; set; }

        public string Name { get; set; }
        public bool? IsDeleted { get; set; }

        public int? PageNumber { get; set; }
        public int? ItemsPerPage { get; set; }

        public int? Skip { get; set; }

        public EnquiryTypeSearchModel() { }
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
        public Func<EnquiryType, bool> Search;
    }



}