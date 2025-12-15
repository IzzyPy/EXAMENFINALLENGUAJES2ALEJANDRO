using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RecetarioApi.Data;
using RecetarioApi.Models;

namespace RecetarioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecetasController : ControllerBase
    {
        private readonly IRecetaRepository _repo;

        public RecetasController(IRecetaRepository repo)
        {
            _repo = repo;
        }

        // GET api/recetas?ingrediente=tomate&categoria=ensalada&favorito=true
        [HttpGet]
        public ActionResult<IEnumerable<Receta>> GetAll([FromQuery] string? ingrediente, [FromQuery] string? categoria, [FromQuery] bool? favorito)
        {
            var items = _repo.GetAll();

            if (!string.IsNullOrWhiteSpace(ingrediente))
                items = items.Where(r => r.Ingredientes.Any(i => i.Contains(ingrediente, System.StringComparison.OrdinalIgnoreCase)));

            if (!string.IsNullOrWhiteSpace(categoria))
                items = items.Where(r => r.Categoria.Contains(categoria, System.StringComparison.OrdinalIgnoreCase));

            if (favorito.HasValue)
                items = items.Where(r => r.Favorito == favorito.Value);

            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public ActionResult<Receta> Get(int id)
        {
            var receta = _repo.Get(id);
            if (receta == null) return NotFound();
            return Ok(receta);
        }

        [HttpPost]
        public ActionResult<Receta> Create([FromBody] Receta receta)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = _repo.Create(receta);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Receta receta)
        {
            if (id != receta.Id) return BadRequest("El id de la ruta y del cuerpo no coinciden.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!_repo.Update(receta)) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (!_repo.Delete(id)) return NotFound();
            return NoContent();
        }

        [HttpPut("{id:int}/favorito")]
        public IActionResult SetFavorito(int id, [FromBody] bool favorito)
        {
            var receta = _repo.Get(id);
            if (receta == null) return NotFound();
            receta.Favorito = favorito;
            _repo.Update(receta);
            return NoContent();
        }
    }
}