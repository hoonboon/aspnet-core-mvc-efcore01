using AspnetCoreWebMvcApp03.Data;
using AspnetCoreWebMvcApp03.Models;
using AspnetCoreWebMvcApp03.Models.SchoolViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreWebMvcApp03.Controllers
{
    public class HomeController : Controller
    {
        private readonly SchoolContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(SchoolContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Index() called.");

            return View();
        }

        public async Task<IActionResult> About()
        {
            List<StudentCountByEnrollmentDate> results = new List<StudentCountByEnrollmentDate>();

            var conn = _context.Database.GetDbConnection();

            try
            {
                await conn.OpenAsync();
                using (var command = conn.CreateCommand())
                {
                    string query = "SELECT EnrollmentDate, COUNT(*) AS StudentCount" 
                        + " FROM Student" 
                        + " GROUP BY EnrollmentDate";
                    command.CommandText = query;
                    DbDataReader reader = await command.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new StudentCountByEnrollmentDate()
                            {
                                EnrollmentDate = reader.GetDateTime(0),
                                StudentCount = reader.GetInt32(1)
                            };
                            results.Add(row);
                        }
                    }
                    await reader.DisposeAsync();
                }
            }
            finally
            {
                await conn.CloseAsync();
            }

            return View(results);

            //IQueryable<StudentCountByEnrollmentDate> data =
            //    from student in _context.Students
            //    group student by student.EnrollmentDate into dateGroup
            //    select new StudentCountByEnrollmentDate()
            //    {
            //        EnrollmentDate = dateGroup.Key,
            //        StudentCount = dateGroup.Count()
            //    };

            //return View(await data.AsNoTracking().ToListAsync());
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
