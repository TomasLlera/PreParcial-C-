using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Services
{
    // Observador concreto que representa las notificaciones a Logística
    public class LogisticsObserver : IOrderObserver
    {
        public void OnOrderConfirmed(Order order)
        {
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("📦 NOTIFICACIÓN A LOGÍSTICA");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine($"Nuevo pedido para procesar: #{order.Id.ToString().Substring(0, 8)}");
            Console.WriteLine($"Cliente: {order.ClientName}");
            Console.WriteLine($"Dirección: {order.Address}");
            Console.WriteLine($"Tipo de envío: {order.ShippingType}");
            Console.WriteLine($"Cantidad de productos: {order.Products.Count}");
            Console.WriteLine("Productos:");
            foreach (var product in order.Products)
            {
                Console.WriteLine($"  - {product.Name} x{product.Quantity}");
            }
            Console.WriteLine("Estado: PENDIENTE DE PREPARACIÓN");
            Console.WriteLine("═══════════════════════════════════════════\n");
        }
    }
}