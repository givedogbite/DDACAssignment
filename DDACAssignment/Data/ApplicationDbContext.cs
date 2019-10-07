using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DDACAssignment.Models;

namespace DDACAssignment.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<DDACAssignment.Models.Room> Room { get; set; }
        public DbSet<DDACAssignment.Models.Reservation> Reservation { get; set; }
        public DbSet<DDACAssignment.Models.Image> Image { get; set; }
    }
}
