using Domain.Models;
using Foolproof;
using Foundation.Core;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Foundation.Areas.Public.Models
{

    
    public class AccountViewModel
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Profile { get; set; }
        public string Photo { get; set; }
        public string PhotoUrl { get; set; }
        public string CompanyName { get; set; }

        public AccountViewModel()
        {

        }
        public AccountViewModel(PublicUser model)
        {
            Id = model.Id;
            UserName = model.UserName;
            Name = model.Name;
            Profile = model.Profile;
            Email = model.Email;
            PhoneNumber = model.PhoneNumber;
            Photo = model.Photo;
            if (!String.IsNullOrEmpty(Photo))
                PhotoUrl = UploadHelper.GetPublicUserUrl(Photo);
            CompanyName = model.CompanyName;
        }
    }

    public class AccountFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.FullName, ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_full_name")]
        public string Name { get; set; }

        [CustomRegularExpression(Validation.Email, ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_email")]
        public string Username { get; set; }

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
    public class AccountPostModel : AccountFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public string ActivationCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.Password, ErrorMessageResourceType = typeof(Resources.Errors),
           ErrorMessageResourceName = "common_password")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_confirm_password")]
        public string ConfirmPassword { get; set; }

        public override void UpdateModel(PublicUser model)
        {
            base.UpdateModel(model);
        }
    }

    public class AccountPutModel : AccountFormModel
    {
        
    }

    public class ProfileFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.FullName, ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_full_name")]
        public string Name { get; set; }

        [CustomRegularExpression(Validation.Phone, ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_phone")]
        public string PhoneNumber { get; set; }

        [CustomRegularExpression(Validation.Notes, ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_notes")]
        public string Profile { get; set; }

        [CustomRegularExpression(Validation.FullName, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_full_name")]
        public string CompanyName { get; set; }

        public Attachment PhotoFile { get; set; }


        public virtual void UpdateModel(PublicUser model)
        {
            model.Name = Name;
            model.PhoneNumber = PhoneNumber;
            model.Profile = Profile;
            model.CompanyName = CompanyName;
        }
    }

    public class PasswordFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.Password, ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_password")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_confirm_password")]
        public string ConfirmPassword { get; set; }

        
    }

    

}