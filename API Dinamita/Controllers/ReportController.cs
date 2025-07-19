using API_Dinamita.Models;
using API_Dinamita.ModelsDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace API_Dinamita.Controllers
{
    [Route("api/Reportes")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ContextDB _context;

        public ReportController(ContextDB context)
        {
            _context = context;
        }

        // GET: api/Reportes
        [Authorize (Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reportes>>> GetReportes()
        {
            return await _context.Reportes.ToListAsync();
        }

        // GET: api/Reporte/5
        [Authorize (Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Reportes>> GetReporte(int id)
        {
            var reporte = await _context.Reportes
      .FirstOrDefaultAsync(p => p.Id_Reporte == id);

            if (reporte == null)
                return NotFound();

            var dto = new ReporteDto
            {
                Nombre_Reporte = reporte.Nombre_Reporte,
                Fecha_Creacion = reporte.Fecha_Creacion,
                Nombre_Evento = reporte.Nombre_Evento,
                N_Ventas = reporte.N_Ventas,
                N_Asistencias = reporte.N_Asistencias,
                Descripcion = reporte.Descripcion
            };

            return Ok(dto);
        }

        // POST: api/Reportes
        [Authorize (Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Reportes>> PostReporte(ReporteDtoCreacion reportess)
        {
            if (reportess == null)
            {
                return BadRequest("Los datos del reporte no son válidos.");
            }

            // Mapear el DTO a la entidad ReporteEmpresa
            var reporte = new Reportes
            {
                Nombre_Reporte = reportess.Nombre_Reporte,
                Fecha_Creacion = DateTime.Now,
                Nombre_Evento = reportess.Nombre_Evento,
                N_Ventas = reportess.N_Ventas,
                N_Asistencias = reportess.N_Asistencias,
                Descripcion = reportess.Descripcion
            };

            // Guardar el reporte en la base de datos
            _context.Reportes.Add(reporte);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostReporte), new { id = reporte.Id_Reporte }, reporte);
        }

        // PUT
        [Authorize (Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReporte(int id, Reportes dto)
        {
            if (id != dto.Id_Reporte)
                return BadRequest();

            // Buscar el reporte de la empresa existente
            var reporte = await _context.Reportes.FindAsync(id);
            if (reporte == null)
                return NotFound();

            // Asignar los valores recibidos en el DTO al objeto de entidad
            reporte.Nombre_Reporte = dto.Nombre_Reporte;
            reporte.Fecha_Creacion = dto.Fecha_Creacion;
            reporte.Nombre_Evento = dto.Nombre_Evento;
            reporte.N_Ventas = dto.N_Ventas;
            reporte.N_Asistencias = dto.N_Asistencias;
            reporte.Descripcion = dto.Descripcion;

            // Marcar el estado de la entidad como modificada
            _context.Entry(reporte).State = EntityState.Modified;

            try
            {
                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReporteExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent(); // Retornar un código de estado 204 si la operación es exitosa
        }

        // DELETE
        [Authorize (Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReporte(int id)
        {
            // Buscar el reporte de la base de datos
            var reporte = await _context.Reportes.FindAsync(id);

            // Si no encontramos el reporte, devolver un error
            if (reporte == null)
                return NotFound();

            // Eliminar el reporte de la base de datos
            _context.Reportes.Remove(reporte);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return NoContent(); // Retornamos una respuesta sin contenido, indicando éxito.
        }

        private bool ReporteExists(int id)
        {
            return _context.Reportes.Any(e => e.Id_Reporte == id);
        }
    }
}
