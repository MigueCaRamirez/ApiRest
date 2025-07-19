
namespace API_Dinamita.ModelsDto
{
    public class ReporteDtoCreacion
    {
        public string Nombre_Reporte { get; set; }
        public string Nombre_Evento { get; set; }
        public int N_Ventas { get; set; }
        public int N_Asistencias { get; set; }
        public string Descripcion { get; set; }
    }

    public class ReporteDto
    {
        public string Nombre_Reporte { get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public string Nombre_Evento { get; set; }
        public int N_Ventas { get; set; }
        public int N_Asistencias { get; set; }
        public string Descripcion { get; set; }
    }
}