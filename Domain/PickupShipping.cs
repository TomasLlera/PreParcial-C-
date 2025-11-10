using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    // Estrategia concreta: Retiro en Local
    public class PickupShipping : IShippingStrategy
    {
        public decimal CalculateShippingCost(decimal subtotal)
        {
            // Retiro en local no tiene costo
            return 0m;
        }

        public string GetShippingType()
        {
            return "Retiro en Local";
        }
    }
}