using Domain.Models.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Foundation.Areas.Admin.Models.ViewModels
{
  
    public class CountryViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public CreatorViewModel Creator { get; set; }

        public CountryViewModel() {
        }
        public CountryViewModel(Country model)
        {
            Id = model.Id;
            Name = model.Name;
            CreationDate = model.CreationDate;
            IsActive = model.IsActive;
            IsDeleted = model.IsDeleted;

            Creator = new CreatorViewModel()
            {
                Id = model.Creator.Id,
                Name = model.Creator.Name,
                Username = model.Creator.UserName
            };
        }
    }

    public class CountryFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public string Name { get; set; }
        
        public bool IsActive { get; set; }

        public void UpdateModel(Country model)
        {
            model.Name = Name;
            model.IsActive = IsActive;
        }
    }

    public class CountryBriefViewModel
    {
        public CountryBriefViewModel() { }
        public CountryBriefViewModel(Country model) {
            Id = model.Id;
            Name = model.Name;
        }
        public long Id { get; set; }
        public string Name { get; set; }
    }


   
}