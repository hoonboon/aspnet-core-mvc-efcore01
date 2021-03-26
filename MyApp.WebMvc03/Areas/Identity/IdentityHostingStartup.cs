using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(MyApp.WebMvc03.Areas.Identity.IdentityHostingStartup))]
namespace MyApp.WebMvc03.Areas.Identity
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