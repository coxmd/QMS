using FastReport;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using QueueManagementSystem.MVC.Services;
using System.Data;
using System.Windows.Forms;

namespace QueueManagementSystem.MVC.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public ReportsController(IReportService reportService, IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _reportService = reportService;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        public IActionResult Index(string reportType, DateTime? startDate, DateTime? endDate)
        {
            var report = new WebReport();

            // Choose report file based on reportType
            string reportFileName = reportType switch
            {
                "AverageWaiting" => "AverageWaiting.frx",
                "TotalCustomersServed" => "CustomersServed.frx",
                _ => "AverageWaiting.frx" // Default report
            };

            if (!string.IsNullOrEmpty(reportType) && startDate.HasValue && endDate.HasValue)
            {

                string reportPath = Path.Combine(_hostingEnvironment.WebRootPath, "reports", reportFileName);
                report.Report.Load(reportPath);

                // If date range is provided, pass it as parameters to the report
                if (startDate.HasValue && endDate.HasValue)
                {
                    report.Report.SetParameterValue("DateFrom", startDate.Value.ToString("yyyy-MM-dd"));
                    report.Report.SetParameterValue("DateTo", endDate.Value.ToString("yyyy-MM-dd"));
                    report.Report.SetParameterValue("Tz", "EAT");
                }

                ViewBag.WebReport = report;
                ViewBag.ShowReport = true;
            }
            else
            {
                ViewBag.ShowReport = false;
            }

            return View();
        }

    }
}
