
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using RecetarioApi.Models;

namespace RecetarioApi.Data
{
    public class JsonRecetaRepository : IRecetaRepository
    {
        private readonly string _path;
        private readonly object _lock = new();
        private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web) { WriteIndented = true };

        public JsonRecetaRepository(string filePath)
        {
            _path = filePath;
            EnsureFile();
        }

        private void EnsureFile()
        {
            var dir = Path.GetDirectoryName(_path) ?? ".";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            if (!File.Exists(_path)) File.WriteAllText(_path, "[]");
        }

        private List<Receta> ReadAll()
        {
            lock (_lock)
            {
                var json = File.ReadAllText(_path);
                return JsonSerializer.Deserialize<List<Receta>>(json, _options) ?? new List<Receta>();
            }
        }

        private void WriteAll(List<Receta> list)
        {
            lock (_lock)
            {
                var json = JsonSerializer.Serialize(list, _options);
                File.WriteAllText(_path, json);
            }
        }

        public IEnumerable<Receta> GetAll() => ReadAll();

        public Receta? Get(int id) => ReadAll().FirstOrDefault(r => r.Id == id);

        public Receta Create(Receta receta)
        {
            lock (_lock)
            {
                var list = ReadAll();
                if (receta.Id == 0)
                {
                    receta.Id = list.Any() ? list.Max(r => r.Id) + 1 : 1;
                }
                // ensure unique id if provided
                if (list.Any(r => r.Id == receta.Id))
                {
                    receta.Id = list.Max(r => r.Id) + 1;
                }
                list.Add(receta);
                WriteAll(list);
                return receta;
            }
        }

        public bool Update(Receta receta)
        {
            lock (_lock)
            {
                var list = ReadAll();
                var idx = list.FindIndex(r => r.Id == receta.Id);
                if (idx == -1) return false;
                list[idx] = receta;
                WriteAll(list);
                return true;
            }
        }

        public bool Delete(int id)
        {
            lock (_lock)
            {
                var list = ReadAll();
                var removed = list.RemoveAll(r => r.Id == id) > 0;
                if (removed) WriteAll(list);
                return removed;
            }
        }
    }
}