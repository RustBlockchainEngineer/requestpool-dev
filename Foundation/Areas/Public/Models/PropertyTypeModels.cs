using Domain.Models;
using Domain.Models.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Foundation.Areas.Public.Models
{
    public class PropertyTypeBriefViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string UiRegex { get; set; }

        public PropertyTypeBriefViewModel()
        {
        }
        public PropertyTypeBriefViewModel(PropertyType model)
        {
            Id = model.Id;
            Name = model.Name;
            UiRegex = model.UiRegex;
        }
    }
}