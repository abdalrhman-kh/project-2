using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using project_2.Models;

namespace project_2.Data
{
    public class DataDbContext: IdentityDbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options):base(options)
        {
            
        }
        public DbSet<Dentist> Dentist { get; set; }
        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<Schedule> Schedule { get; set; }
        public DbSet<OTP> OTPs { get; set; }
    }
}
