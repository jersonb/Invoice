﻿using Invoice.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Client.Data
{
    public class ApplicationData : DbContext
    {
        public ApplicationData(DbContextOptions<ApplicationData> options)
            : base(options)
        {
        }

        public DbSet<InvoiceData> Invoices { get; set; }
    }
}