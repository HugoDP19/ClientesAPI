using ClientesAPI.Models; //Modelo (defición de la tabla)
using Microsoft.EntityFrameworkCore; //Acceder BD

namespace ClientesAPI.Data
{
    //Herencia
    public class AppDbContext : DbContext
    {
        //Constructor (inicializar)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {

        }
        
        //ORM tendrá acceso al módelo cliente
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
    }
}
