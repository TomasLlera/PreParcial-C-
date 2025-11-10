using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Services
{
    // Subject del patrón Observer: mantiene la lista de observadores y los notifica
    public class OrderService
    {
        private readonly List<IOrderObserver> _observers = new List<IOrderObserver>();

        // Suscribe un observador para recibir notificaciones
        public void Attach(IOrderObserver observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        // Desuscribe un observador
        public void Detach(IOrderObserver observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            _observers.Remove(observer);
        }

        // Notifica a todos los observadores cuando se confirma un pedido
        public void ConfirmOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            Console.WriteLine("\n🔔 Confirmando pedido y notificando observadores...\n");

            // Notifica a todos los observadores suscritos
            foreach (var observer in _observers)
            {
                try
                {
                    observer.OnOrderConfirmed(order);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al notificar observador: {ex.Message}");
                }
            }
        }
    }
}