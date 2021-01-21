using System;
using AspnetCoreWebMvcApp03.Areas.Identity.Data;
using AspnetCoreWebMvcApp03.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(AspnetCoreWebMvcApp03.Areas.Identity.IdentityHostingStartup))]
namespace AspnetCoreWebMvcApp03.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}