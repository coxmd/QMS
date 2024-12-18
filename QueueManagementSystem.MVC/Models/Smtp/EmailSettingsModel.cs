﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QueueManagementSystem.MVC.Models.Smtp
{
    public class EmailSettingsModel
    {
        [Key]
        public int Id { get; set; }
        public string? MailServer { get; set; }
        public int MailPort { get; set; }
        public string? Sender { get; set; }
        public string? Password { get; set; }
        public string? SenderName { get; set; }
        public bool EnableSsl { get; set; }

    }
}
