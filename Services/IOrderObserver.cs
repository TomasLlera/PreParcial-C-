using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    // Interfaz que deben implementar todos los observadores
    public interface IOrderObserver
    {
        // Método que se ejecuta cuando se confirma un pedido
        void OnOrderConfirmed(Domain.Order order);
    }
}