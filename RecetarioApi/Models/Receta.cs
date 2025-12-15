
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecetarioApi.Models
{
    public class Receta
    {
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public List<string> Ingredientes { get; set; } = new();

        [Required]
        [MinLength(1)]
        public List<string> Pasos { get; set; } = new();

        public string Categoria { get; set; } = string.Empty;

        public bool Favorito { get; set; } = false;
    }
}