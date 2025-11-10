using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    // Implementación concreta del Builder para construir pedidos
    public class OrderBuilder : IOrderBuilder
    {
        private Order _order;

        public OrderBuilder()
        {
            Reset();
        }

        public void Reset()
        {
            _order = new Order();
        }

        public IOrderBuilder SetClientName(string clientName)
        {
            _order.ClientName = clientName;
            return this; // Retorna this para permitir method chaining
        }

        public IOrderBuilder SetAddress(string address)
        {
            _order.Address = address;
            return this;
        }

        public IOrderBuilder AddProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product), "El producto no puede ser nulo");

            _order.Products.Add(product);
            return this;
        }

        public IOrderBuilder SetShippingStrategy(IShippingStrategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException(nameof(strategy), "La estrategia de envío no puede ser nula");

            // Calcula el costo de envío usando la estrategia
            _order.ShippingCost = strategy.CalculateShippingCost(_order.Subtotal);
            _order.ShippingType = strategy.GetShippingType();
            return this;
        }

        public Order Build()
        {
            // Validaciones antes de construir el pedido
            if (string.IsNullOrWhiteSpace(_order.ClientName))
                throw new InvalidOperationException("El nombre del cliente es obligatorio");

            if (string.IsNullOrWhiteSpace(_order.Address))
                throw new InvalidOperationException("La dirección es obligatoria");

            if (_order.Products.Count == 0)
                throw new InvalidOperationException("El pedido debe tener al menos un producto");

            if (string.IsNullOrWhiteSpace(_order.ShippingType))
                throw new InvalidOperationException("Debe seleccionar un tipo de envío");

            // Establece la fecha de confirmación
            _order.ConfirmedDate = DateTime.Now;

            // Retorna el pedido construido
            var result = _order;
            Reset(); // Prepara el builder para un nuevo pedido
            return result;
        }
    }
}