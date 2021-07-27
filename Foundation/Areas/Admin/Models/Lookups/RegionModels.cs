using Domain.Models.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Foundation.Areas.Admin.Models.ViewModels
{
    public class RegionViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public CityBriefViewModel City { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public CreatorViewModel Creator { get; set; }

        public RegionViewModel() { }
        public RegionViewModel(Region model)
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
            City = new CityBriefViewModel(model.City);
        }
    }

    public class RegionFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public long CityId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public string Name { get; set; }
        
        public bool IsActive { get; set; }

        public void UpdateModel(Region model)
        {
            model.CityId = CityId;
            model.Name = Name;
            model.IsActive = IsActive;
        }
    }

    public class RegionBriefViewModel
    {
        public RegionBriefViewModel() { }
        public RegionBriefViewModel(Region model)
        {
            Id = model.Id;
            Name = model.Name;
        }
        public long Id { get; set; }
        public string Name { get; set; }
    }
}