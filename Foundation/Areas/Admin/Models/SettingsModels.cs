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
    public class SettingsViewModel
    {
        public int DefaultMembershipPeriod { get; set; }
       
        public DateTime LastUpdateDate { get; set; }
        public CreatorViewModel Creator { get; set; }

        public SettingsViewModel()
        {

        }
        public SettingsViewModel(Settings model)
        {
            DefaultMembershipPeriod = model.DefaultMembershipPeriod;
            
            Creator = new CreatorViewModel()
            {
                Id = model.CreatorId,
                Name = model.Creator.Name,
                Username = model.Creator.UserName
            };

        }
    }
    
    public class SettingsFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public int DefaultMembershipPeriod { get; set; }



        public virtual void UpdateModel(Settings model)
        {
            model.DefaultMembershipPeriod = DefaultMembershipPeriod;
        }
    }
    public class SettingsPostModel : SettingsFormModel
    {
        public override void UpdateModel(Settings model)
        {
            base.UpdateModel(model);
        }
    }

    public class SettingsPutModel : SettingsFormModel
    {
        public override void UpdateModel(Settings model)
        {
            base.UpdateModel(model);
        }
    }

    


  
}