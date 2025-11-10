using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain
{
    // Interfaz que define el contrato para las estrategias de envío
    public interface IShippingStrategy
    {
        // Calcula el costo de envío basado en el subtotal del pedido
        decimal CalculateShippingCost(decimal subtotal);

        // Retorna el nombre del tipo de envío
        string GetShippingType();
    }
}