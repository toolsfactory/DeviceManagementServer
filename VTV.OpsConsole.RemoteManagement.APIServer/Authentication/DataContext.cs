using Microsoft.EntityFrameworkCore;

namespace VTV.OpsConsole.RemoteManagement.APIServer.Authentication
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
