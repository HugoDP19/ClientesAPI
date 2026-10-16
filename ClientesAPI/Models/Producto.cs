using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientesAPI.Models
{
    [Table("productos")] // Coincide con CREATE TABLE productos
    public class Producto
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required, StringLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Column("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [Column("precio", TypeName = "decimal(7,2)")]
        public decimal Precio { get; set; }

        [Required]
        [Column("garantia")]
        public byte Garantia { get; set; } // TINYINT en SQL Server equivale a byte en C#

        [StringLength(40)]
        [Column("numserie", TypeName = "varchar(40)")]
        public string? Numserie { get; set; } // Permite NULL (string?)
    }
}