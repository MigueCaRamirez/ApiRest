using API_Dinamita.Helpers;
using API_Dinamita.Models;
using API_Dinamita.ModelsDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Security.Claims;

namespace API_Dinamita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ContextDB _context;
        public TicketsController(ContextDB context)
        {
            _context = context;
        }

        [HttpPost("{cantidad}")]
        [Authorize(Roles = "Usuario")]
        public async Task<ActionResult<TicketDto>> PostTicket(TicketDto ticketDto, int cantidad)
        {
            var evento = await _context.Eventos.FindAsync(ticketDto.Id_Evento);
            if (evento == null)
                return NotFound();

            if (evento.Tickets_Disponible < cantidad)
                return BadRequest("Cantidad no permitida");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            for (int i = 0; i < cantidad; i++)
            {
                string codigo = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
                byte[] codigoQR = GenerarQR(codigo);

                var ticket = new Tickets
                {
                    Precio = ticketDto.Precio,
                    Fecha_Expedicion = DateTime.UtcNow,
                    Nombre_Evento = ticketDto.Nombre_Evento,
                    Categoria = ticketDto.Categoria,
                    Fecha_Entrada = ticketDto.Fecha_Entrada,
                    CodigoAlfanumerico = codigo,
                    CodigoQR = codigoQR,
                    Estado = false,
                    Id_Usuario = userId,
                    Id_Evento = ticketDto.Id_Evento
                };
                _context.Tickets.Add(ticket);
                evento.Tickets_Disponible -= 1;
            }
            await _context.SaveChangesAsync();
            return Ok(new
            {
                mensaje = "Compra realizada exitosamente"
            });
        }

        private byte[] GenerarQR(string codigo)
        {
            var generadorQR = new QRCodeGenerator();
            var datosQR = generadorQR.CreateQrCode(codigo, QRCodeGenerator.ECCLevel.Q);
            var codigoQR = new PngByteQRCode(datosQR);
            return codigoQR.GetGraphic(20);
        }

        [Authorize]
        [HttpGet("{Id_Usuario}")]
        public async Task<ActionResult<CarritoDto>> GetCarrito(int Id_Usuario) {

            var tickets = await _context.Tickets.ToListAsync();
            var ticketsCarrito = tickets.Where(T => T.Id_Usuario == Id_Usuario  &&  T.Estado == false);

            float total = 0;
            if (ticketsCarrito != null)
            {
                List<TicketFront> ticketList = new List<TicketFront>();
                foreach (var ticket in ticketsCarrito)
                {
                    ticketList.Add(ParseTicket(ticket));
                    total = total + ticket.Precio;
                }

                var carrito = new CarritoDto
                {
                    Tickets = ticketList,
                    Total = total
                };
                return Ok(carrito);
            }
            else
            {
                return BadRequest("No ha realizado compras");
            }
            
        }

        private TicketFront ParseTicket(Tickets ticket)
        {
            return new TicketFront
            {
                Id_Ticket = ticket.Id_Ticket,
                Nombre_Evento = ticket.Nombre_Evento,
                Categoria = ticket.Categoria,
                Fecha_Entrada = ticket.Fecha_Entrada,
                CodigoAlfanumerico = ticket.CodigoAlfanumerico,
                CodigoQR = ticket.CodigoQR
            };
        }

        [HttpPost("ValidarQR")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> ValidarQR(string codigoAlfanumerico)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(T => T.CodigoAlfanumerico == codigoAlfanumerico);

            if (ticket == null)
                return BadRequest("Código inválido");

            if (ticket.Estado)
                return BadRequest("El ticket ya ha sido usado");

            ticket.Estado = true;
            _context.SaveChanges();

            return Ok($"Ticket válido para el evento: {ticket.Nombre_Evento}");
        }

    }
}
