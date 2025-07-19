namespace API_Dinamita.ModelsDto
{
    public class TicketDto
    {
        public float Precio { get; set; }
        public string Nombre_Evento { get; set; }
        public string Categoria { get; set; }
        public DateTime Fecha_Entrada { get; set; }
        public int Id_Evento { get; set; }
    }

    public class TicketFront
    {
        public int Id_Ticket { get; set; }
        public string Nombre_Evento { get; set; }
        public string? Categoria { get; set; }
        public DateTime Fecha_Entrada { get; set; }
        public string CodigoAlfanumerico { get; set; }
        public byte[] CodigoQR { get; set; }

    }
}
