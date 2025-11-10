using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Repository
{
    // Implementación del Repository que persiste en archivos JSON
    public class JsonRepository<T> : IRepository<T> where T : class
    {
        private readonly string _fileName;
        private readonly string _directory;
        private readonly JsonSerializerOptions _options;

        public JsonRepository(string fileName, string directory = "Data")
        {
            _fileName = fileName;
            _directory = directory;
            _options = new JsonSerializerOptions { WriteIndented = true };

            // Crea el directorio si no existe
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
        }

        public void Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var data = GetAll();
            data.Add(entity);
            Save(data);
        }

        public List<T> GetAll()
        {
            return Load();
        }

        public T GetBy(Predicate<T> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var data = Load();
            return data.Find(predicate);
        }

        public void Update(Predicate<T> predicate, T entity)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var data = Load();
            int index = data.FindIndex(predicate);

            if (index != -1)
            {
                data[index] = entity;
                Save(data);
            }
            else
            {
                throw new InvalidOperationException("No se encontró la entidad para actualizar");
            }
        }

        public void Delete(Predicate<T> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var data = Load();
            data.RemoveAll(predicate);
            Save(data);
        }

        private void Save(List<T> data)
        {
            try
            {
                string path = GetFilePath();
                string json = JsonSerializer.Serialize(data, _options);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al guardar en {_fileName}: {ex.Message}", ex);
            }
        }

        private List<T> Load()
        {
            try
            {
                string path = GetFilePath();

                if (!File.Exists(path))
                    return new List<T>();

                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<List<T>>(json, _options) ?? new List<T>();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al cargar desde {_fileName}: {ex.Message}", ex);
            }
        }

        private string GetFilePath()
        {
            return Path.Combine(_directory, $"{_fileName}.json");
        }
    }
}