using Microsoft.EntityFrameworkCore;
using CRM.Models;

namespace CRM.Data
{
    public class CRMContext : DbContext
    {
        public CRMContext(DbContextOptions<CRMContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<CRM.Models.Role> Role { get; set; }
        public DbSet<CRM.Models.Business> Business { get; set; }
        public DbSet<CRM.Models.Company> Company { get; set; }
        public DbSet<CRM.Models.Note> Note { get; set; }
        public DbSet<CRM.Models.Contact> Contact { get; set; }

    }
}