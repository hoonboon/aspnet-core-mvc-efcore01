using MyApp.WebMvc03.Data;
using MyApp.WebMvc03.Models;
using MyApp.WebMvc03.Models.SchoolViewModels;
using MyApp.WebMvc03.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.WebMvc03.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly SchoolContext _context;
        private readonly ILogger<HomeController> _logger;

        const string SessionKeyTime = "_Time";

        public HomeController(SchoolContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Index() called.");

            if (HttpContext.Session.Get<DateTime>(SessionKeyTime) == default)
            {
                HttpContext.Session.Set<DateTime>(SessionKeyTime, DateTime.Now);
            }
            else
            {
                _logger.LogInformation("SessionKeyTime = " + HttpContext.Session.Get<DateTime>(SessionKeyTime));
            }

            return View();
        }

        public async Task<IActionResult> About()
        {
            IQueryable<StudentCountByEnrollmentDate> data =
                from student in _context.Students
                group student by student.EnrollmentDate into dateGroup
                select new StudentCountByEnrollmentDate()
                {
                    EnrollmentDate = dateGroup.Key,
                    StudentCount = dateGroup.Count()
                };

            return View(await data.AsNoTracking().ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
