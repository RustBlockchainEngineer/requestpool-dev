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
    public class ClientViewModel
    {
        public long Id { get; set; }
        public PublicUserBriefViewModel PublicUser { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string Profile { get; set; }
        public string Remarks { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool IsDeleted { get; set; }

        public ClientViewModel()
        {

        }
        public ClientViewModel(Client model)
        {
            PublicUser = new PublicUserBriefViewModel(model.PublicUser);
            Id = model.Id;
            Name = model.Name;
            Phone = model.Phone;
            Fax = model.Fax;
            Email = model.Email;
            Address = model.Address;
            Website = model.Website;
            Profile = model.Profile;
            Remarks = model.Remarks;
            CreationDate = model.CreationDate;
            LastUpdateDate = model.LastUpdateDate;
            IsDeleted = model.IsDeleted;
        }
    }

    public class ClientFormModel
    {
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

        [CustomRegularExpression(Validation.Email, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_email")]
        public string Email { get; set; }

        [CustomRegularExpression(Validation.Address, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_address")]
        public string Address { get; set; }

        [CustomRegularExpression(Validation.Url, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_url")]
        public string Website { get; set; }

        [CustomRegularExpression(Validation.Notes, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_notes")]
        public string Profile { get; set; }

        [CustomRegularExpression(Validation.Notes, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_notes")]
        public string Remarks { get; set; }



        public virtual void UpdateModel(Client model)
        {
            model.Name = Name;
            model.Phone = Phone;
            model.Fax = Fax;
            model.Email = Email;
            model.Address = Address;
            model.Website = Website;
            model.Profile = Profile;
            model.Remarks = Remarks;
            model.Name = Name;
        }
    }
    public class ClientPostModel : ClientFormModel
    {

    }

    public class ClientPutModel : ClientFormModel
    {

    }

    public class ClientBriefViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public ClientBriefViewModel() { }
        public ClientBriefViewModel(Client model)
        {
            Id = model.Id;
            Name = model.Name;
        }

    }

    public class ClientSearchModel
    {
        public string Username { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool? IsDeleted { get; set; }

        public int? PageNumber { get; set; }
        public int? ItemsPerPage { get; set; }

        public int? Skip { get; set; }

        public ClientSearchModel() { }
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
                            && (Email == "" || i.Email.ToLower().Contains(Email))
                            && (Phone == "" || i.Phone.Contains(Phone))
                            && (Name == "" || i.Name.ToLower().Contains(Name.ToLower()))
                            && (Username == "" || i.PublicUser.UserName.ToLower() == Username.ToLower().Trim());
        }
        public Func<Client, bool> Search;
    }
}