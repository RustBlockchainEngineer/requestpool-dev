using Domain.Models.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Foundation.Areas.Public.Models
{
    public class RegionViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public CityBriefViewModel City { get; set; }

        public RegionViewModel() { }
        public RegionViewModel(Region model)
        {
            Id = model.Id;
            Name = model.Name;
            City = new CityBriefViewModel(model.City);
        }
    }

    public class RegionFormModel
    {
        public void UpdateModel(Region model)
        {

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