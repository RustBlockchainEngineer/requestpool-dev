﻿using Domain.Models;
using Domain.Models.Lookups;
using Foolproof;
using Foundation.Areas.Public.Models.ViewModels;
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
    public class RecipientResponseAttachmentViewModel
    {
        public PublicUserBriefViewModel PublicUser { get; set; }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Filename { get; set; }
        public string OriginalFileName { get; set; }
        public string Url { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool IsDeleted { get; set; }

        public RecipientResponseAttachmentViewModel()
        {

        }
        public RecipientResponseAttachmentViewModel(RecipientResponseAttachment model)
        {
            Id = model.Id;
            Title = model.Title;
            Description = model.Description;
            Filename = model.FileName;
            OriginalFileName = model.OriginalFileName;
            Url = UploadHelper.GetResponsesUrl(Filename);
            CreationDate = model.CreationDate;
            IsDeleted = model.IsDeleted;
            PublicUser = new PublicUserBriefViewModel(model.Recipient.PublicUser);
        }
    }
    public class RecipientResponseAttachmentFormModel
    {


        //[CustomRegularExpression(Validation.Title, ErrorMessageResourceType = typeof(Resources.Errors),
        //            ErrorMessageResourceName = "common_title")]
        //[StringLength(100, ErrorMessageResourceType = typeof(Resources.Admin.Errors), ErrorMessageResourceName = "common_length_of_100")]
        //public string Title { get; set; }

        [CustomRegularExpression(Validation.Notes, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_notes")]
        public string Description { get; set; }

        public virtual void UpdateModel(RecipientResponseAttachment model)
        {
            //model.Title = Title;
            model.Description = Description;
        }
    }
    public class RecipientResponseAttachmentPostModel : RecipientResponseAttachmentFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.Title, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_title")]
        public string OriginalFileName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public string Content { get; set; }

        public override void UpdateModel(RecipientResponseAttachment model)
        {
            base.UpdateModel(model);
            model.OriginalFileName = OriginalFileName;
            model.Title = OriginalFileName;
        }
    }

    public class RecipientResponseAttachmentPutModel : RecipientResponseAttachmentFormModel
    {
    }
    
}