using Domain.Models;
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
    public class ContactViewModel
    {
        public PublicUserBriefViewModel PublicUser { get; set; }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string CompanyName { get; set; }
        public string Profile { get; set; }
        public string Remarks { get; set; }
        public ContactTypeBriefViewModel ContactType { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool IsDeleted { get; set; }

        public IEnumerable<ContactTypeBriefViewModel> ContactTypes { get; set; }

        public ContactViewModel()
        {

        }
        public ContactViewModel(Contact model)
        {
            PublicUser = new PublicUserBriefViewModel(model.PublicUser);

            Id = model.Id;
            Name = model.Name;
            Phone = model.Phone;
            Fax = model.Fax;
            Email = model.Email;
            Address = model.Address;
            CompanyName = model.CompanyName;
            Profile = model.Profile;
            Remarks = model.Remarks;
            CreationDate = model.CreationDate;
            LastUpdateDate = model.LastUpdateDate;
            IsDeleted = model.IsDeleted;
            ContactType = new ContactTypeBriefViewModel(model.ContactType);

        }
    }

    public class ContactFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public long ContactTypeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.FullName, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_full_name")]
        public string Name { get; set; }

        [CustomRegularExpression(Validation.Phone, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_phone")]
        public string Phone { get; set; }

        [CustomRegularExpression(Validation.Phone, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_phone")]
        public string Fax { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.Email, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_email")]
        public string Email { get; set; }

        [CustomRegularExpression(Validation.Address, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_address")]
        public string Address { get; set; }

        public string CompanyName { get; set; }

        [CustomRegularExpression(Validation.Notes, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_notes")]
        public string Profile { get; set; }

        [CustomRegularExpression(Validation.Notes, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_notes")]
        public string Remarks { get; set; }



        public virtual void UpdateModel(Contact model)
        {
            model.ContactTypeId = ContactTypeId;
            model.Name = Name;
            model.Phone = Phone;
            model.Fax = Fax;
            model.Email = Email;
            model.Address = Address;
            model.CompanyName = CompanyName;
            model.Profile = Profile;
            model.Remarks = Remarks;
            model.Name = Name;
        }
    }
    public class ContactPostModel : ContactFormModel
    {
        public string ContactTypes { get; set; }
    }

    public class ContactPutModel : ContactFormModel
    {
        public string ContactTypes { get; set; }
    }

    public class ContactBriefViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ContactType { get; set; }

        public ContactBriefViewModel() { }
        public ContactBriefViewModel(Contact model)
        {
            Id = model.Id;
            Name = model.Name;
            ContactType = model.ContactType.Name;
        }

    }

    public class ContactSearchModel
    {
        public string Username { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool? IsDeleted { get; set; }
        public long? ContactTypeId { get; set; }

        public int? PageNumber { get; set; }
        public int? ItemsPerPage { get; set; }

        public int? Skip { get; set; }

        public ContactSearchModel() { }
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
            if (Email == null)
                Email = "";
            if (Username == null)
                Username = "";
            Search = i =>
                            (!IsDeleted.HasValue || i.IsDeleted == IsDeleted.Value)
                            && (!ContactTypeId.HasValue || i.ContactTypeId == ContactTypeId.Value)
                            && (Email == "" || i.Email.ToLower().Contains(Email))
                            && (Phone == "" || i.Phone.Contains(Phone))
                            && (Name == "" || i.Name.ToLower().Contains(Name.ToLower()))
                            && (Username == "" || i.PublicUser.UserName.ToLower() == Username.ToLower().Trim());
        }
        public Func<Contact, bool> Search;
    }
}