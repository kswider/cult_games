using Cult_games_server.Models;
using Microsoft.EntityFrameworkCore;

namespace Cult_games_server.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
    }
}
