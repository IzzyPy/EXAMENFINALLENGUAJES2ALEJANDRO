
using System.Collections.Generic;
using RecetarioApi.Models;

namespace RecetarioApi.Data
{
    public interface IRecetaRepository
    {
        IEnumerable<Receta> GetAll();
        Receta? Get(int id);
        Receta Create(Receta receta);
        bool Update(Receta receta);
        bool Delete(int id);
    }
}