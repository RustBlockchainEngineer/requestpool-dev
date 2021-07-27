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

namespace Foundation.Areas.Admin.Models
{
    public class PublicUserViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string Otp { get; set; }

        public string Remarks { get; set; }
        public string Photo { get; set; }
        public string PhotoUrl { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string LatestIP { get; set; }
        public string CompanyName { get; set; }

        public bool LockoutEnabled { get; set; }
        public long AccessFailedCount { get; set; }
        public DateTime LockoutEndDateUtc { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public CreatorViewModel Creator { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public PublicUserViewModel()
        {

        }
        public PublicUserViewModel(PublicUser model)
        {
            Id = model.Id;
            Name = model.Name;
            Username = model.UserName;
            PhoneNumber = model.PhoneNumber;
            Otp = model.Otp;
            Photo = model.Photo;
            if(!String.IsNullOrEmpty(Photo))
                PhotoUrl = UploadHelper.GetPublicUserUrl(Photo);

            Latitude = model.Latitude;
            Longitude = model.Longitude;
            LatestIP = model.LatestIP;
            CompanyName = model.CompanyName;
            Remarks = model.Remarks;
            LockoutEnabled = model.LockoutEnabled;
            AccessFailedCount = model.AccessFailedCount;
            CreationDate = model.CreationDate;
            LastUpdateDate = model.LastUpdateDate;
            IsActive = model.IsActive;
            IsDeleted = model.IsDeleted;
            
            Creator = new CreatorViewModel()
            {
                Id = model.CreatorId,
                Name = model.Creator.Name,
                Username = model.Creator.UserName
            };
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
       
        [CustomRegularExpression(Validation.Notes, ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_notes")]
        public string Remarks { get; set; }


        public bool IsActive { get; set; }

        public Attachment PhotoFile { get; set; }


        public virtual void UpdateModel(PublicUser model)
        {
            model.Name = Name;
            model.UserName = model.Email = Username;
            model.PhoneNumber = PhoneNumber;
            model.Remarks = Remarks;
            model.IsActive = IsActive;

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


        public PublicUserBriefViewModel() { }
        public PublicUserBriefViewModel(PublicUser model)
        {
            Id = model.Id;
            Name = model.Name;
            Username = model.UserName;
            PhoneNumber = model.PhoneNumber;
        }
        
    }


    public class PublicUserSearchModel
    {
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public int? PageNumber { get; set; }
        public int? ItemsPerPage { get; set; }

        public int? Skip { get; set; }

        public PublicUserSearchModel() { }
        public void Init()
        {
            if (!PageNumber.HasValue)
                PageNumber = 1;
            if (!ItemsPerPage.HasValue)
                ItemsPerPage = AppSettings.ItemsPerPage;
            Skip = (PageNumber.Value - 1) * ItemsPerPage.Value;
            if (Phone == null)
                Phone = "";
            if (Name == null)
                Name = "";
            if (Username == null)
                Username = "";
            Search = i =>
                            (!IsActive.HasValue || i.IsActive == IsActive.Value)
                            && (!IsDeleted.HasValue || i.IsDeleted == IsDeleted.Value)
                            && (Username=="" || i.UserName.ToLower().Contains(Username))
                            && (Phone == "" || i.PhoneNumber.Contains(Phone))
                            && (Name == "" || i.Name.ToLower().Contains(Name.ToLower()));
        }
        public Func<PublicUser, bool> Search;
    }
}