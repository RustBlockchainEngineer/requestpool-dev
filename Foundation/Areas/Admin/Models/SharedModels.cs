using Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Foundation.Areas.Admin.Models
{
    public class CreatorViewModel
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
    }

   

    public class ClientBriefViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class ProjectBriefViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
    }

    public class RoleViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public RoleViewModel() { }
        public RoleViewModel(ApplicationRole model)
        {
            Id = model.Id;
            Name = model.Name;
        }
    }

    public class Attachment
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public string Content { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public string OriginalFileName { get; set; }
    }
}