using Domain.Models;
using Foolproof;
using Foundation.Areas.Admin.Models.ViewModels;
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
    public class PublicUserViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string Photo { get; set; }
        public string PhotoUrl { get; set; }
        public string CompanyName { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public PublicUserViewModel()
        {

        }
        public PublicUserViewModel(PublicUser model)
        {
            Id = model.Id;
            Name = model.Name;
            Username = model.UserName;
            PhoneNumber = model.PhoneNumber;
            CompanyName = model.CompanyName;
            Photo = model.Photo;
            if(!String.IsNullOrEmpty(Photo))
                PhotoUrl = UploadHelper.GetPublicUserUrl(Photo);

            CreationDate = model.CreationDate;
            LastUpdateDate = model.LastUpdateDate;
        }
    }
    
    public class PublicUserFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.Email, ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_email")]
        public string Username { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.FullName, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_full_name")]
        public string Name { get; set; }

        

        [CustomRegularExpression(Validation.Phone, ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_phone")]
        public string PhoneNumber { get; set; }

        [CustomRegularExpression(Validation.FullName, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_full_name")]
        public string CompanyName { get; set; }

        public Attachment PhotoFile { get; set; }


        public virtual void UpdateModel(PublicUser model)
        {
            model.Name = Name;
            model.UserName = model.Email = Username;
            model.PhoneNumber = PhoneNumber;
            model.CompanyName = CompanyName;

        }
    }
    public class PublicUserPostModel : PublicUserFormModel
    {
        public override void UpdateModel(PublicUser model)
        {
            base.UpdateModel(model);
        }
    }

    public class PublicUserPutModel : PublicUserFormModel
    {
        public override void UpdateModel(PublicUser model)
        {
            base.UpdateModel(model);
        }
    }

    public class PublicUserBriefViewModel
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string CompanyName { get; set; }


        public PublicUserBriefViewModel() { }
        public PublicUserBriefViewModel(PublicUser model)
        {
            Id = model.Id;
            Name = model.Name;
            Username = model.UserName;
            PhoneNumber = model.PhoneNumber;
            CompanyName = model.CompanyName;
        }
        
    }
    public class GelocationPostModel
    {
        [RequiredIfNotEmpty("Longitude", ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.Latitude, ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_geolocation")]
        public string Latitude { get; set; }

        [RequiredIfNotEmpty("Latitude", ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.Longitude, ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_geolocation")]
        public string Longitude { get; set; }
    }

}