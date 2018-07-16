using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoOnDemand.Data.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

// This database context will be used to add entity classes that represent tables in the database, and to call the database from the other projects.

namespace VideoOnDemand.Data.Data
{
    public class VODContext : IdentityDbContext<User>
    {
        public VODContext(DbContextOptions<VODContext> options)
            :base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
