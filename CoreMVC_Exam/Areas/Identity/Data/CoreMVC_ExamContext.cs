using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreMVC_Exam.Areas.Identity.Data;
using CoreMVC_Exam.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoreMVC_Exam.Data
{
    public class CoreMVC_ExamContext : IdentityDbContext<CoreMVC_ExamUser>
    {
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Category> Categories  { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Client> Clients { get; set; }

        public CoreMVC_ExamContext(DbContextOptions<CoreMVC_ExamContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
