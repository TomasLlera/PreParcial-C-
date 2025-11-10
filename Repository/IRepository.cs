using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    // Interfaz genérica para el patrón Repository
    public interface IRepository<T> where T : class
    {
        // Agrega una entidad
        void Add(T entity);

        // Obtiene todas las entidades
        List<T> GetAll();

        // Actualiza una entidad existente
        void Update(Predicate<T> predicate, T entity);

        // Elimina una entidad
        void Delete(Predicate<T> predicate);

        // Obtiene una entidad por predicado
        T GetBy(Predicate<T> predicate);
    }
}