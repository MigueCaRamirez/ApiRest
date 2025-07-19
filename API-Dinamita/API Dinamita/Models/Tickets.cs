
using System.ComponentModel.DataAnnotations;
namespace API_Dinamita.Models
{
    public class Tickets
    {
        [Key]
        public int Id_Ticket { get; set; }
        public float Precio { get; set; }
        public DateTime Fecha_Expedicion { get; set; }
        public string Nombre_Evento { get; set; }
        public string? Categoria { get; set; }
        public DateTime Fecha_Entrada { get; set; }
        public string CodigoAlfanumerico { get; set; }
        public byte[] CodigoQR { get; set; }
        public bool Estado { get; set; }
        public int Id_Usuario { get; set; }
        public int Id_Evento { get; set; }

    }
}
