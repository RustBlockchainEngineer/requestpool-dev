using Domain.Models.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Foundation.Areas.Public.Models
{
  
    public class CityViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }


        public CityViewModel() {
        }
        public CityViewModel(City model)
        {
            Id = model.Id;
            Name = model.Name;
        }
    }

    public class CityFormModel
    {
        public void UpdateModel(City model)
        {

        }
    }

    public class CityBriefViewModel
    {
        public CityBriefViewModel() { }
        public CityBriefViewModel(City model)
        {
            Id = model.Id;
            Name = model.Name;

        }
        public long Id { get; set; }
        public string Name { get; set; }

    }



}