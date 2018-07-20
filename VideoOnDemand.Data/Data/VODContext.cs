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
        // database tables for storing the video data (tables for storing video-related data in the database)
        // Tell Entity Framework that the entity classes should be added as tables in the database, you need to add them as DbSet properties in the VODContext class.
        public DbSet<Course> Courses { get; set; }
        public DbSet<Download> Downloads { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<Video> Videos { get; set; }

        public VODContext(DbContextOptions<VODContext> options)
            :base(options)
        {

        }
        // data repository that can communicate with the database tables

        // Becasue the UserCourse has the composite key(UserId & CourseId) - specify it in the OnModelCreating()
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Composite key
            builder.Entity<UserCourse>().HasKey(uc => new { uc.UserId, uc.CourseId });

            // Restrict cascading deletes
            // method. A cascading delete will delete all related records to the one being deleted; for instance, if you delete an order, all its order rows will also be deleted
            foreach (var relationship in builder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
