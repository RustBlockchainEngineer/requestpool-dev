using Domain.Models;
using Domain.Models.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mapping
{
    class AdministratorMapping : EntityTypeConfiguration<Administrator>
    {
        public AdministratorMapping()
        {
        }
    }

}
