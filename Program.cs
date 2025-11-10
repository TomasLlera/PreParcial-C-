using System;
using Domain;
using Repository;
using Services;
using Controllers;
using Views;

namespace OrderManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            // ═══════════════════════════════════════════════════════
            // COMPOSITION ROOT: Aquí "enchufamos" todas las piezas
            // usando Inyección de Dependencias
            // ═══════════════════════════════════════════════════════

            // 1. Crear el Repository para persistir pedidos en JSON
            IRepository<Order> orderRepository = new JsonRepository<Order>("pedidos");

            // 2. Crear el OrderService (Subject del patrón Observer)
            OrderService orderService = new OrderService();

            // 3. Suscribir los observadores al servicio
            orderService.Attach(new ClientObserver());
            orderService.Attach(new LogisticsObserver());

            // 4. Crear el Builder para construir pedidos
            IOrderBuilder orderBuilder = new OrderBuilder();

            // 5. Crear el Controller inyectando sus dependencias
            OrderController controller = new OrderController(
                orderRepository,
                orderService,
                orderBuilder
            );

            // 6. Crear la Vista
            IOrderView view = new ConsoleOrderView();

            // 7. Suscribir los eventos de la vista al controller
            view.AddProductRequested += (s, e) => HandleAddProduct(view, controller);
            view.SetClientDataRequested += (s, e) => HandleSetClientData(view, controller);
            view.SelectShippingRequested += (s, e) => HandleSelectShipping(view, controller);
            view.ConfirmOrderRequested += (s, e) => HandleConfirmOrder(view, controller);
            view.ViewOrdersRequested += (s, e) => HandleViewOrders(view, controller);

            // 8. Iniciar la aplicación
            view.Run();
        }

        // ═══════════════════════════════════════════════════════
        // HANDLERS: Métodos que conectan la Vista con el Controller
        // ═══════════════════════════════════════════════════════

        static void HandleAddProduct(IOrderView view, OrderController controller)
        {
            try
            {
                Console.WriteLine("\n--- AGREGAR PRODUCTO ---");

                var consoleView = view as ConsoleOrderView;
                string name = consoleView.ReadString("Nombre del producto: ");
                decimal price = consoleView.ReadDecimal("Precio: $");
                int quantity = consoleView.ReadInt("Cantidad: ");

                controller.AddProduct(name, price, quantity);
                view.ShowSuccess("Producto agregado correctamente.");
            }
            catch (Exception ex)
            {
                view.ShowError(ex.Message);
            }
        }

        static void HandleSetClientData(IOrderView view, OrderController controller)
        {
            try
            {
                Console.WriteLine("\n--- DATOS DEL CLIENTE ---");

                var consoleView = view as ConsoleOrderView;
                string clientName = consoleView.ReadString("Nombre del cliente: ");
                string address = consoleView.ReadString("Dirección de envío: ");

                controller.SetClientData(clientName, address);
                view.ShowSuccess("Datos del cliente establecidos.");
            }
            catch (Exception ex)
            {
                view.ShowError(ex.Message);
            }
        }

        static void HandleSelectShipping(IOrderView view, OrderController controller)
        {
            try
            {
                Console.WriteLine("\n--- SELECCIONAR TIPO DE ENVÍO ---");
                Console.WriteLine("1. Envío en Moto ($500 fijo)");
                Console.WriteLine("2. Envío por Correo (10% del subtotal)");
                Console.WriteLine("3. Retiro en Local (Gratis)");

                var consoleView = view as ConsoleOrderView;
                int option = consoleView.ReadInt("\nSeleccione opción (1-3): ");

                controller.SelectShippingStrategy(option);
                view.ShowSuccess("Tipo de envío seleccionado.");
            }
            catch (Exception ex)
            {
                view.ShowError(ex.Message);
            }
        }

        static void HandleConfirmOrder(IOrderView view, OrderController controller)
        {
            try
            {
                Console.WriteLine("\n--- CONFIRMAR PEDIDO ---");
                Console.Write("¿Está seguro de confirmar el pedido? (S/N): ");
                string confirm = Console.ReadLine()?.Trim().ToUpper() ?? "";

                if (confirm == "S" || confirm == "SI")
                {
                    controller.ConfirmOrder();
                    view.ShowSuccess("¡Pedido confirmado y guardado exitosamente!");
                }
                else
                {
                    view.ShowMessage("Confirmación cancelada.");
                }
            }
            catch (Exception ex)
            {
                view.ShowError(ex.Message);
            }
        }

        static void HandleViewOrders(IOrderView view, OrderController controller)
        {
            try
            {
                Console.WriteLine("\n╔═══════════════════════════════════════════╗");
                Console.WriteLine("║         PEDIDOS GUARDADOS                 ║");
                Console.WriteLine("╚═══════════════════════════════════════════╝");

                var orders = controller.GetAllOrders();

                if (orders.Count == 0)
                {
                    Console.WriteLine("\nNo hay pedidos guardados aún.");
                }
                else
                {
                    foreach (var order in orders)
                    {
                        Console.WriteLine($"\n─────────────────────────────────────────");
                        Console.WriteLine($"Pedido ID: {order.Id.ToString().Substring(0, 8)}");
                        Console.WriteLine($"Cliente: {order.ClientName}");
                        Console.WriteLine($"Dirección: {order.Address}");
                        Console.WriteLine($"Tipo de envío: {order.ShippingType}");
                        Console.WriteLine($"Productos: {order.Products.Count}");

                        foreach (var product in order.Products)
                        {
                            Console.WriteLine($"  • {product}");
                        }

                        Console.WriteLine($"Subtotal: ${order.Subtotal:N2}");
                        Console.WriteLine($"Costo de envío: ${order.ShippingCost:N2}");
                        Console.WriteLine($"TOTAL: ${order.Total:N2}");
                        Console.WriteLine($"Fecha: {order.ConfirmedDate:dd/MM/yyyy HH:mm}");
                    }
                }

                Console.WriteLine("\n\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                view.ShowError(ex.Message);
            }
        }
    }
}