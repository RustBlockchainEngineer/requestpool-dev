using Domain.Models;
using Foolproof;
using Foundation.Core;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace Foundation.Areas.Public.Models
{


    public class RecipientFormModel
    {
        

        public virtual void UpdateModel(Recipient model)
        {
            
        }
    }
    public class RecipientPostModel : RecipientFormModel
    {

    }

    public class RecipientPutModel : RecipientFormModel
    {

    }

    public class RecipientBriefViewModel
    {
        public long Id { get; set; }
        public long ContactId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsFailed { get; set; }
        public string FailureReason { get; set; }
        public RecipientBriefViewModel() { }
        public RecipientBriefViewModel(Recipient model)
        {
            Id = model.Id;
            ContactId = model.ContactId;
            Name = model.Contact.Name;
            Email = model.Contact.Email;
            Phone = model.Contact.Phone;
            IsFailed = model.IsFailed;
            FailureReason = model.FailureReason;
        }

    }

   
}