using QueueManagementSystem.MVC.Models;
using FastReport;
using FastReport.Web;

namespace QueueManagementSystem.MVC.Services
{
    public interface IReportService
    {
        Report GenerateTicketReport(Ticket ticket);

        Report GenerateAnalyticalReport(List<ServiceStat> serviceStats);
        WebReport GenerateWebReport(string reportType, string startDate, string endDate, byte[] qrCodeImage);

    }
}