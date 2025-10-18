using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ProductApi.Models
{

    [Table("PRODUCTOS")]
    public class Producto
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("PRODUCTO")]
        [Required]
        public string? Nombre { get; set; }

        [Column("ESTADO_PRODUCTO")]
        [Required]
        public string? EstadoProducto { get; set; }

        [Column("FECHA_TIMESTAMP")]
        public DateTime FechaTimestamp { get; set; } = DateTime.Now;
    }
}
