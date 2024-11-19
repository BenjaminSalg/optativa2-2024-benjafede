using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace App.Models
{
    public class SqlContext : DbContext
    {
        public SqlContext(DbContextOptions<SqlContext> options) : base(options)
        {
        }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Accion> Acciones { get; set; }
    }
}
