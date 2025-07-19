using API_Dinamita.Models;
using API_Dinamita.ModelsDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Eventos = API_Dinamita.Models.Eventos;

namespace API_Dinamita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventosController : ControllerBase
    {
        private readonly ContextDB _context;

        // Constructor: aquí recibo la base de datos para poder trabajar con ella
        public EventosController(ContextDB context)
        {
            _context = context;
        }

        // Obtener todos los eventos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoGetDto>>> GetEventos()
        {
            var eventos = await _context.Eventos.ToListAsync();
            // Si no existe, aviso
            if (eventos == null)
            {
                return NotFound();
            }

            // Armo la lista de eventosdto
            LinkedList<EventoGetDto> eventosdto = new LinkedList<EventoGetDto>();
            foreach (var evento in eventos)
            {
                var eventodto = new EventoGetDto
                {
                    Id_Evento = evento.Id_Evento,
                    Nombre_Evento = evento.Nombre_Evento,
                    Descripcion = evento.Descripcion,
                    Nombre_Lugar = evento.Nombre_Lugar,
                    Direccion_Lugar = evento.Direccion_Lugar,
                    Fecha = evento.Fecha,
                    Aforo_Max = evento.Aforo_Max,
                    Tickets_Disponible = evento.Tickets_Disponible,
                    Estado = evento.Estado,
                    PrecioTicket = evento.PrecioTicket, 
                    Categoria = evento.Categoria,
                    Imagen = evento.Imagen != null ? Convert.ToBase64String(evento.Imagen) : null
                };
                eventosdto.AddLast(eventodto);
            }

            return Ok(eventosdto);
        }

        // Crear un nuevo evento (solo admins)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Eventofrom>> PostEvento([FromForm] Eventofrom eventofrom)
        {
            // Si no me mandan datos, aviso
            if (eventofrom == null)
            {
                return BadRequest("Los datos del reporte no son válidos.");
            }

            // Armo el evento con los datos que recibo
            var evento = new Eventos
            {
                Nombre_Evento = eventofrom.Nombre_Evento,
                Descripcion = eventofrom.Descripcion,
                Nombre_Lugar = eventofrom.Nombre_Lugar,
                Direccion_Lugar = eventofrom.Direccion_Lugar,
                Fecha = eventofrom.Fecha,
                Aforo_Max = eventofrom.Aforo_Max,
                PrecioTicket = eventofrom.PrecioTicket,
                Tickets_Disponible = eventofrom.Aforo_Max,
                Estado = true,
                Categoria = eventofrom.Categoria
            };

            // Si la imagen fue subida, la guardamos en la propiedad del evento
            if (eventofrom.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await eventofrom.Imagen.CopyToAsync(memoryStream);
                    evento.Imagen = memoryStream.ToArray();
                }
            }

            // Guardo el evento en la base de datos
            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();

            // Aviso que se guardó bien
            return Ok("Evento guardado correctamente");
        }

        // Modificar un evento existente (solo admins)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvento(int id, EventosDto evento)
        {
            // Busco el evento que quiero modificar
            var eventoExistente = await _context.Eventos.FindAsync(id);
            if (eventoExistente == null)
            {
                return NotFound("Evento no existe");
            }

            // Actualizo los datos del evento
            eventoExistente.Nombre_Evento = evento.Nombre_Evento;
            eventoExistente.Descripcion = evento.Descripcion;
            eventoExistente.Nombre_Lugar = evento.Nombre_Lugar;
            eventoExistente.Direccion_Lugar = evento.Direccion_Lugar;
            eventoExistente.Fecha = evento.Fecha; 
            eventoExistente.Aforo_Max = evento.Aforo_Max;
            eventoExistente.PrecioTicket = evento.PrecioTicket;
            eventoExistente.Tickets_Disponible = evento.Tickets_Disponible;
            eventoExistente.Estado = evento.Estado;
            eventoExistente.Categoria = evento.Categoria;

            // Si la imagen es diferente, la actualizo
            if (evento.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await evento.Imagen.CopyToAsync(memoryStream);
                    eventoExistente.Imagen = memoryStream.ToArray();
                }
            }

            // Marco el evento como modificado
            _context.Entry(eventoExistente).State = EntityState.Modified;

            try
            {
                // Guardo los cambios
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Si el evento no existe, aviso
                if (!EventoExists(id))
                {
                    return NotFound("Fallo la modificacion en base de datos");
                }
                else
                {
                    throw;
                }
            }

            // Aviso que se modificó bien
            return Ok("Evento modificado correctamente");
        }


        // Eliminar un evento (solo admins)
        [HttpDelete("{Id_Evento}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEvento(int Id_Evento)
        {
            // Busco el evento
            var evento = await _context.Eventos.FindAsync(Id_Evento);

            // Si no existe, aviso
            if (evento == null)
                return NotFound();

            // Lo elimino de la base de datos
            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();

            // Aviso que se eliminó bien
            return Ok("Evento eliminado exitosamente");
        }

        // Función para saber si un evento existe
        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.Id_Evento == id);
        }
    }
}