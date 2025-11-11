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
            IRepository<Order> orderRepository = new JsonRepository<Order>("pedidos");
            OrderService orderService = new OrderService();
            orderService.Attach(new ClientObserver());
            orderService.Attach(new LogisticsObserver());

            IOrderBuilder orderBuilder = new OrderBuilder();

            OrderController controller = new OrderController(
                orderRepository,
                orderService,
                orderBuilder
            );

            IOrderView view = new ConsoleOrderView();
            
            // 🔹 Instanciás el manejador que contiene los HandleX
            var handler = new OrderConsoleHandler(view, controller);

            // 🔹 Suscribís los eventos a sus métodos internos
            view.AddProductRequested += (s, e) => handler.HandleAddProduct();
            view.SetClientDataRequested += (s, e) => handler.HandleSetClientData();
            view.SelectShippingRequested += (s, e) => handler.HandleSelectShipping();
            view.ConfirmOrderRequested += (s, e) => handler.HandleConfirmOrder();
            view.ViewOrdersRequested += (s, e) => handler.HandleViewOrders();

            // 🔹 Ejecutás la vista principal
            view.Run();
        }
    }
}
