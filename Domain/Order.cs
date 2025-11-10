using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ClientName { get; set; }
        public string Address { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();

        // Tipo de envío seleccionado
        public string ShippingType { get; set; }

        // Costo del envío
        public decimal ShippingCost { get; set; }

        // Fecha de confirmación
        public DateTime ConfirmedDate { get; set; }

        // Calcula el subtotal de todos los productos
        public decimal Subtotal => Products.Sum(p => p.Subtotal);

        // Total = Subtotal + Costo de envío
        public decimal Total => Subtotal + ShippingCost;

        public Order() { }

        public override string ToString()
        {
            return $"Pedido #{Id} - Cliente: {ClientName} - Total: ${Total}";
        }
    }
}