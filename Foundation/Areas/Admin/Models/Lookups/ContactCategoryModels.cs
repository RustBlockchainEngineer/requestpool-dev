using Domain.Models.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Foundation.Areas.Admin.Models.ViewModels
{
  
    public class ContactCategoryViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public DateTime CreationDate { get; set; }
        public bool IsDeleted { get; set; }

        public CreatorViewModel Creator { get; set; }

        public ContactCategoryViewModel() {
        }
        public ContactCategoryViewModel(Country model)
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

    public class ContactCategoryFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public string Name { get; set; }
        
        public void UpdateModel(ContactType model)
        {
            model.Name = Name;
        }
    }

    public class ContactCategoryBriefViewModel
    {
        public ContactCategoryBriefViewModel() { }
        public ContactCategoryBriefViewModel(ContactType model) {
            Id = model.Id;
            Name = model.Name;
        }
        public long Id { get; set; }
        public string Name { get; set; }
    }


   
}