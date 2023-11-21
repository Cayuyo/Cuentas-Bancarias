#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
namespace Cuentas_Bancarias.Models;

public class MyContext : DbContext
{
    public MyContext(DbContextOptions options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Transaccion> Transacciones { get; set; }
}