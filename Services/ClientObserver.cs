using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Services
{
    // Observador concreto que representa las notificaciones al Cliente
    public class ClientObserver : IOrderObserver
    {
        public void OnOrderConfirmed(Order order)
        {
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("📧 NOTIFICACIÓN AL CLIENTE");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine($"Hola {order.ClientName},");
            Console.WriteLine($"Tu pedido #{order.Id.ToString().Substring(0, 8)} ha sido confirmado.");
            Console.WriteLine($"Dirección de envío: {order.Address}");
            Console.WriteLine($"Tipo de envío: {order.ShippingType}");
            Console.WriteLine($"Total a pagar: ${order.Total:N2}");
            Console.WriteLine($"Fecha: {order.ConfirmedDate:dd/MM/yyyy HH:mm}");
            Console.WriteLine("¡Gracias por tu compra!");
            Console.WriteLine("═══════════════════════════════════════════\n");
        }
    }
}