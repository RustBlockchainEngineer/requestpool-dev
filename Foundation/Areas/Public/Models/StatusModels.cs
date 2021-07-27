using Domain.Models.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Foundation.Areas.Public.Models
{
    public class StatusViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public long Rank { get; set; }

        public StatusViewModel() { }
        public StatusViewModel(Status model)
        {
            Id = model.Id;
            Rank = model.Rank;
            Name = model.Name;
            Color = model.Color;
        }
    }

    public class StatusFormModel
    {
      
        public void UpdateModel(Status model)
        {
           
        }
    }

    public class StatusBriefViewModel
    {
        public StatusBriefViewModel() { }
        public StatusBriefViewModel(Status model)
        {
            Id = model.Id;
            Name = model.Name;
            Color = model.Color;
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}