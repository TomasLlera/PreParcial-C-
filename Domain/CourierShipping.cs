using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    // Estrategia concreta: Envío por Correo
    public class CourierShipping : IShippingStrategy
    {
        private const decimal PERCENTAGE = 0.10m; // 10% del subtotal

        public decimal CalculateShippingCost(decimal subtotal)
        {
            // El costo es el 10% del subtotal del pedido
            return subtotal * PERCENTAGE;
        }

        public string GetShippingType()
        {
            return "Envío por Correo";
        }
    }
}