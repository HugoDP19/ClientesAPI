using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientesAPI.Models
{
    [Table("clientes")] // Corregido a "clientes" (plural)
    public class Cliente
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required, StringLength(70)]
        [Column("apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required, StringLength(70)]
        [Column("nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required, StringLength(9)]
        [Column("telefono")]
        public string Telefono { get; set; } = string.Empty;

        [Required, StringLength(150)]
        [Column("correo")]
        public string Correo { get; set; } = string.Empty;

        [Required, StringLength(8, MinimumLength = 8)]
        [Column("dni", TypeName = "char(8)")]
        public string Dni { get; set; } = string.Empty;

        [Required, StringLength(150)]
        [Column("direccion")]
        public string Direccion { get; set; } = string.Empty;
    }
}