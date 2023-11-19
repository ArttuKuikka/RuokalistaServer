using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RuokalistaServer.Models;

namespace RuokalistaServer.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<RuokalistaServer.Models.Ruokalista> Ruokalista { get; set; }
        public DbSet<RuokalistaServer.Models.BackgroundForWeek> BackgroundForWeek { get; set; }
        public DbSet<RuokalistaServer.Models.VoteModel> Votes { get; set; }
    }
}