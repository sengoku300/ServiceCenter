using System;
using CoreMVC_Exam.Areas.Identity.Data;
using CoreMVC_Exam.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(CoreMVC_Exam.Areas.Identity.IdentityHostingStartup))]
namespace CoreMVC_Exam.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            //builder.ConfigureServices((context, services) =>
            //{
            //    services.AddDbContext<CoreMVC_ExamContext>(options =>
            //        options.UseSqlServer(
            //            context.Configuration.GetConnectionString("DefaultConnection")));

            //    services.AddDefaultIdentity<CoreMVC_ExamUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //        .AddEntityFrameworkStores<CoreMVC_ExamContext>();
            //});
        }
    }
}