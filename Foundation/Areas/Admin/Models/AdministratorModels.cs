using Domain.Models;
using Foolproof;
using Foundation.Core;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Foundation.Areas.Admin.Models
{
    public class AdministratorViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool LockoutEnabled { get; set; }
        public long AccessFailedCount { get; set; }
        public DateTime LockoutEndDateUtc { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public CreatorViewModel Creator { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string[] Roles { get; set; }

        public AdministratorViewModel()
        {

        }
        public AdministratorViewModel(Administrator model, string[] roles)
        {
            Id = model.Id;
            Name = model.Name;
            Username = model.UserName;
            Email = model.Email;
            PhoneNumber = model.PhoneNumber;
            LockoutEnabled = model.LockoutEnabled;
            AccessFailedCount = model.AccessFailedCount;
            CreationDate = model.CreationDate;
            LastUpdateDate = model.LastUpdateDate;
            IsActive = model.IsActive;
            IsDeleted = model.IsDeleted;
            Roles = roles;
            Creator = new CreatorViewModel()
            {
                Id = model.CreatorId,
                Name = model.Creator.Name,
                Username = model.Creator.UserName
            };
        }
    }

    public class AdministratorFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.AdminUsername, ErrorMessageResourceType = typeof(Resources.Admin.Errors),
            ErrorMessageResourceName = "administrator_username")]
        public string Username { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.FullName, ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_full_name")]
        public string Name { get; set; }

        [CustomRegularExpression(Validation.Email, ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_email")]
        public string Email { get; set; }

        [CustomRegularExpression(Validation.Phone, ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_phone")]
        public string PhoneNumber { get; set; }

        public string[] Roles { get; set; }

        public bool IsActive { get; set; }
        
        public void UpdateModel(Administrator model)
        {
            model.Name = Name;
            model.UserName = Username;
            model.Email = Email;
            model.PhoneNumber = PhoneNumber;
            model.IsActive = IsActive;
        }
    }
    public class AdministratorPostModel : AdministratorFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.AdminPassword, ErrorMessageResourceType = typeof(Resources.Admin.Errors),
            ErrorMessageResourceName = "administrator_password")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_confirm_password")]
        public string ConfirmPassword { get; set; }

    }

    public class AdministratorPutModel : AdministratorFormModel
    {
        [RequiredIfTrue("IsUpdatePassword", ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.AdminPassword, ErrorMessageResourceType = typeof(Resources.Admin.Errors),
            ErrorMessageResourceName = "administrator_password")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_confirm_password")]
        public string ConfirmPassword { get; set; }

        public bool IsUpdatePassword { get; set; }
    }

    public class AdministratorPasswordPutModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.AdminPassword, ErrorMessageResourceType = typeof(Resources.Admin.Errors),
            ErrorMessageResourceName = "administrator_password")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_confirm_password")]
        public string ConfirmPassword { get; set; }
    }

    public class AdministratorSearchModel
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public int? PageNumber { get; set; }
        public int? ItemsPerPage { get; set; }

        public int? Skip { get; set; }

        public AdministratorSearchModel() { }
        public void Init()
        {
            if (!PageNumber.HasValue)
                PageNumber = 1;
            if (!ItemsPerPage.HasValue)
                ItemsPerPage = AppSettings.ItemsPerPage;
            Skip = (PageNumber.Value - 1) * ItemsPerPage.Value;
            if (Username == null)
                Username = "";
            if (Name == null)
                Name = "";
            Search = i => 
                            (!IsActive.HasValue || i.IsActive == IsActive.Value)
                            && (!IsDeleted.HasValue || i.IsDeleted == IsDeleted.Value)
                            && (i.UserName != SystemRoles.Super && i.UserName != SystemRoles.System)
                            && (Username == "" || i.UserName.ToLower().Contains(Username.ToLower()))
                            && (Name == "" || i.Name.ToLower().Contains(Name.ToLower()));
        }
        public Func<Administrator, bool> Search;
    }

}