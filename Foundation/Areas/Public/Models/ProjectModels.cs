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
    public class ProjectViewModel
    {
        public PublicUserBriefViewModel PublicUser { get; set; }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }

        public ClientBriefViewModel Client { get; set; }


        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool IsDeleted { get; set; }

        public ProjectViewModel()
        {

        }
        public ProjectViewModel(Project model)
        {
            
            PublicUser = new PublicUserBriefViewModel(model.Client.PublicUser);
            Id = model.Id;
            Title = model.Title;
            Code = model.Code;
            Description = model.Description;
            Remarks = model.Remarks;
            CreationDate = model.CreationDate;
            LastUpdateDate = model.LastUpdateDate;
            IsDeleted = model.IsDeleted;

            Client = new ClientBriefViewModel(model.Client);
        }
    }

    public class ProjectFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public long ClientId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.Title, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_title")]
        public string Title { get; set; }

        [CustomRegularExpression(Validation.Code, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_code")]
        public string Code { get; set; }

        [CustomRegularExpression(Validation.Description, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_description")]
        public string Description { get; set; }

        
        [CustomRegularExpression(Validation.Notes, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_notes")]
        public string Remarks { get; set; }



        public virtual void UpdateModel(Project model)
        {
            model.ClientId = ClientId;
            model.Title = Title;
            model.Code = Code;
            model.Description = Description;
            model.Remarks = Remarks;
        }
    }
    public class ProjectPostModel : ProjectFormModel
    {

    }

    public class ProjectPutModel : ProjectFormModel
    {

    }
    public class ProjectBriefViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }

        public ProjectBriefViewModel() { }
        public ProjectBriefViewModel(Project model)
        {
            Id = model.Id;
            Title = model.Title;
        }

    }

    public class ProjectSearchModel
    {
        public string Username { get; set; }

        public string Title { get; set; }
        public long? ClientId { get; set; }
        public string Code { get; set; }
        public bool? IsDeleted { get; set; }

        public int? PageNumber { get; set; }
        public int? ItemsPerPage { get; set; }

        public int? Skip { get; set; }

        public ProjectSearchModel() { }
        public void Init()
        {
            if (!PageNumber.HasValue)
                PageNumber = 1;
            if (!ItemsPerPage.HasValue)
                ItemsPerPage = AppSettings.ItemsPerPage;
            Skip = (PageNumber.Value - 1) * ItemsPerPage.Value;
            if (Title == null)
                Title = "";
            if (Code == null)
                Code = "";
            if (Username == null)
                Username = "";
            Search = i =>
                            (!IsDeleted.HasValue || i.IsDeleted == IsDeleted.Value)
                            && (!ClientId.HasValue || i.ClientId == ClientId)
                            && (Title == "" || i.Title.ToLower().Contains(Title))
                            && (Code == "" || i.Code.Contains(Code))
                            && (Username == "" || i.Client.PublicUser.UserName.ToLower() == Username.ToLower().Trim());

        }
        public Func<Project, bool> Search;
    }
}