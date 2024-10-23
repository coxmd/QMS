using QueueManagementSystem.MVC.Models;
using FastReport;
using FastReport.Web;
using FastReport.Utils;
using FastReport.Data;
using System.Drawing;

namespace QueueManagementSystem.MVC.Services
{
    public class ReportService : IReportService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public ReportService(IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            RegisteredObjects.AddConnection(typeof(PostgresDataConnection));
        }
        public Report GenerateTicketReport(Ticket ticket)
        {   
            //TODO: catch exceptions for null or invalid tickets
            Report report = new Report();
            string reportPath = Path.Combine(_hostingEnvironment.WebRootPath, "reports", "Ticket.frx");
            report.Load(reportPath);
            report.SetParameterValue("TicketNo", ticket.TicketNumber);
            report.SetParameterValue("serviceID", ticket.ServiceName);
            report.SetParameterValue("printTime", ticket.PrintTime);
            
            report.Prepare();

            return report;
        }

        public Report GenerateAnalyticalReport(List<ServiceStat> serviceStats)
        {
            Report report = new Report();
            string reportPath = Path.Combine(_hostingEnvironment.WebRootPath, "reports", "ServiceStats.frx");
            report.Load(reportPath);
            report.Dictionary.RegisterData(serviceStats, "serviceStats", true);
            DataBand db1 = (DataBand)report.FindObject("Data1");
            db1.DataSource = report.GetDataSource("serviceStats");
            report.Prepare();

            return report;
        }

        public WebReport GenerateWebReport(string reportType, string startDate, string endDate, byte[] qrCodeImage)
        {
            var report = new WebReport();

            // Load the appropriate report template based on report type
            var reportPath = reportType switch
            {
                "AverageWaitingTime" => Path.Combine(_hostingEnvironment.WebRootPath, "reports", "AverageWaiting.frx"),
                "CustomersServed" => Path.Combine(_hostingEnvironment.WebRootPath, "reports", "CustomersServed.frx"),
                _ => throw new Exception("Invalid report type")
            };

            report.Report.Load(reportPath);

            // Set up the PostgreSQL connection
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            // Create a new PostgresDataConnection for FastReport
            var pgConnection = new PostgresDataConnection();
            //pgConnection.ConnectionString = connectionString;

            //// Register the connection in the report
            //report.Report.Dictionary.Connections.Add(pgConnection);


            // Ensure the PostgreSQL connection is registered and available
            report.Report.Dictionary.Connections.Clear();
            //report.Report.Dictionary.RegisterData(serviceStats, "serviceStats", true);

            DataBand db1 = (DataBand)report.Report.FindObject("Data1");

            report.Report.SetParameterValue("URL", "Queue/TicketTrackingPage");
            report.Report.SetParameterValue("StartDate", startDate);
            report.Report.SetParameterValue("EndDate", endDate);

            report.Report.PrepareAsync();

            return report;
        }
    }
}
