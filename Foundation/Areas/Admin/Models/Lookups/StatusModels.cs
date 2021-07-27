using Domain.Models.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Foundation.Areas.Admin.Models.ViewModels
{
    public class StatusViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public long Rank { get; set; }

        public DateTime CreationDate { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public CreatorViewModel Creator { get; set; }

        public StatusViewModel() { }
        public StatusViewModel(Status model)
        {
            Id = model.Id;
            Rank = model.Rank;
            Name = model.Name;
            Color = model.Color;
            CreationDate = model.CreationDate;
            IsDefault = model.IsDefault;
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

    public class StatusFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public string Name { get; set; }
        
        public string Color { get; set; }
        public long Rank { get; set; }
        public bool IsActive { get; set; }
        public void UpdateModel(Status model)
        {
            model.Name = Name;
            model.Color = Color;
            model.Rank = Rank;
            model.IsActive = IsActive;
            model.IsDefault = false;
        }
    }

    public class StatusBriefViewModel
    {
        public StatusBriefViewModel() { }
        public StatusBriefViewModel(Status model) {
            Id = model.Id;
            Name = model.Name;
            Color = model.Color;
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}