using ClientesAPI.Data;
using ClientesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/productos -> Obtiene todos los productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> Listar()
        {
            var productos = await _context.Productos.AsNoTracking().ToListAsync();
            return Ok(productos);
        }

        // GET: api/productos/1 -> Obtiene un producto por su ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Producto>> ObtenerPorId(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound(new { mensaje = $"No existe el producto con ID: {id}" });
            }

            return Ok(producto);
        }

        // POST: api/productos -> Registra un nuevo producto
        [HttpPost]
        public async Task<ActionResult<Producto>> Registrar([FromBody] Producto producto)
        {
            // Evita asignar un ID manual para respetar el IDENTITY de la base de datos
            producto.Id = 0;

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
        }

        // PUT: api/productos/1 -> Actualiza un producto existente
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Producto producto)
        {
            // Validación 1: Coincidencia entre el ID de la URL y el cuerpo
            if (id != producto.Id)
            {
                return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID del cuerpo" });
            }

            // Validación 2: Verificar si existe el producto
            var existente = await _context.Productos.FindAsync(id);

            if (existente == null)
            {
                return NotFound(new { mensaje = $"No existe el producto con ID: {id}" });
            }

            // Actualización de campos
            existente.Nombre = producto.Nombre;
            existente.Descripcion = producto.Descripcion;
            existente.Precio = producto.Precio;
            existente.Garantia = producto.Garantia;
            existente.Numserie = producto.Numserie;

            await _context.SaveChangesAsync();

            return NoContent(); // Retorna 204
        }

        // DELETE: api/productos/1 -> Elimina un producto por ID
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound(new { mensaje = $"No existe el producto con ID: {id}" });
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent(); // Retorna 204
        }
    }
}