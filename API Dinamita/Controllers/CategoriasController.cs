using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_Dinamita.Models;
using Microsoft.AspNetCore.Authorization;
using API_Dinamita.ModelsDto;

namespace API_Dinamita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ContextDB _context;

        public CategoriasController(ContextDB context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categorias>>> GetCategorias()
        {
            return await _context.Categorias.ToListAsync();
        }

        [HttpPut("{Id_Categoria}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutCategorias(int Id_Categoria, CategoriaDto categoriaDto)
        {
            var categoria = await _context.Categorias.FindAsync(Id_Categoria);
            
            categoria.Nombre = categoriaDto.Nombre;
            _context.Entry(categoria).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriasExists(Id_Categoria))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Categorias>> PostCategorias(CategoriaDto categoriaDto)
        {
            _context.Categorias.Add(new Categorias
            {
                Nombre = categoriaDto.Nombre
            });
            await _context.SaveChangesAsync();

            return Ok("Categoria creada exitosamente");
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategorias(int id)
        {
            var categorias = await _context.Categorias.FindAsync(id);
            if (categorias == null)
            {
                return NotFound();
            }

            _context.Categorias.Remove(categorias);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoriasExists(int id)
        {
            return _context.Categorias.Any(e => e.Id_Categoria == id);
        }
    }
}
