using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    // Estrategia concreta: Envío en Moto
    public class MotorcycleShipping : IShippingStrategy
    {
        private const decimal BASE_COST = 500m;

        public decimal CalculateShippingCost(decimal subtotal)
        {
            // Costo fijo para envío en moto
            return BASE_COST;
        }

        public string GetShippingType()
        {
            return "Envío en Moto";
        }
    }
}