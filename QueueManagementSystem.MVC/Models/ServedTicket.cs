using System;

namespace QueueManagementSystem.MVC.Models
{
    public class ServedTicket
    {
        public int Id {get; set;}
        public string TicketNumber{get; set;}
        public string ServiceName {get; set;}
        public DateTime PrintTime {get; set;}
        public DateTime ShowTime {get; set;}
        public DateTime FinishTime {get; set;}
        public int ServiceId { get; set;}
        //Foreign key to ServicePoint
        public int ServicePointId { get; set; }
        public ServicePoint? ServicePoint { get; set; }
    }
}