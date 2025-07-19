using Microsoft.EntityFrameworkCore;

namespace API_Dinamita.Models
{
    public class ContextDB : DbContext
    {
        public ContextDB(DbContextOptions<ContextDB> options) : base(options)
        {
        }
        public DbSet<Personas> Personas { get; set; }
        public DbSet<Tickets> Tickets { get; set; }
        public DbSet<Categorias> Categorias { get; set; }
        public DbSet<Eventos> Eventos { get; set; }
        public DbSet<Reportes> Reportes { get; set; }

    }
}
