using Domain.Models;
using Domain.Models.Lookups;
using Foolproof;
using Foundation.Areas.Public.Models.ViewModels;
using Foundation.Core;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace Foundation.Areas.Public.Models
{
    public class SenderViewModel
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public SenderViewModel() { }
        public SenderViewModel(PublicUser model)
        {
            Id = model.Id;
            Name = model.Name;
            Email = model.Email;
            Phone = model.PhoneNumber;
        }

    }
 
}