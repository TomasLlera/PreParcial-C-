using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    // Interfaz que define los pasos para construir un pedido
    public interface IOrderBuilder
    {
        // Establece los datos del cliente
        IOrderBuilder SetClientName(string clientName);

        // Establece la dirección de envío
        IOrderBuilder SetAddress(string address);

        // Agrega un producto al pedido
        IOrderBuilder AddProduct(Product product);

        // Establece la estrategia de envío
        IOrderBuilder SetShippingStrategy(IShippingStrategy strategy);

        // Reinicia el builder para crear un nuevo pedido
        void Reset();

        // Construye y retorna el pedido final
        Order Build();
    }
}