using Domain.Models.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Foundation.Areas.Admin.Models.ViewModels
{
  
    public class EnquiryTypeViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public DateTime CreationDate { get; set; }
        public bool IsDeleted { get; set; }

        public CreatorViewModel Creator { get; set; }

        public EnquiryTypeViewModel() {
        }
        public EnquiryTypeViewModel(Country model)
        {
            Id = model.Id;
            Name = model.Name;
            CreationDate = model.CreationDate;
            IsDeleted = model.IsDeleted;

            Creator = new CreatorViewModel()
            {
                Id = model.Creator.Id,
                Name = model.Creator.Name,
                Username = model.Creator.UserName
            };
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


   
}