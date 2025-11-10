using System;
using System.Collections.Generic;
using Domain;
using Repository;
using Services;

namespace Controllers
{
    // Controlador que coordina toda la lógica del sistema
    public class OrderController
    {
        private readonly IRepository<Order> _repository;
        private readonly OrderService _orderService;
        private readonly IOrderBuilder _builder;
        private IShippingStrategy _currentStrategy;

        // Constructor con Inyección de Dependencias
        public OrderController(
            IRepository<Order> repository,
            OrderService orderService,
            IOrderBuilder builder)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        // Agrega un producto al pedido actual
        public void AddProduct(string name, decimal price, int quantity)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("El nombre del producto no puede estar vacío");

                if (price <= 0)
                    throw new ArgumentException("El precio debe ser mayor a 0");

                if (quantity <= 0)
                    throw new ArgumentException("La cantidad debe ser mayor a 0");

                var product = new Product(name, price, quantity);
                _builder.AddProduct(product);

                Console.WriteLine($"✓ Producto agregado: {product}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al agregar producto: {ex.Message}", ex);
            }
        }

        // Establece los datos del cliente
        public void SetClientData(string clientName, string address)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(clientName))
                    throw new ArgumentException("El nombre del cliente es obligatorio");

                if (string.IsNullOrWhiteSpace(address))
                    throw new ArgumentException("La dirección es obligatoria");

                _builder.SetClientName(clientName);
                _builder.SetAddress(address);

                Console.WriteLine($"✓ Datos del cliente establecidos: {clientName}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al establecer datos del cliente: {ex.Message}", ex);
            }
        }

        // Selecciona y aplica una estrategia de envío
        public void SelectShippingStrategy(int option)
        {
            try
            {
                switch (option)
                {
                    case 1:
                        _currentStrategy = new MotorcycleShipping();
                        break;
                    case 2:
                        _currentStrategy = new CourierShipping();
                        break;
                    case 3:
                        _currentStrategy = new PickupShipping();
                        break;
                    default:
                        throw new ArgumentException("Opción de envío inválida");
                }

                _builder.SetShippingStrategy(_currentStrategy);
                Console.WriteLine($"✓ Tipo de envío seleccionado: {_currentStrategy.GetShippingType()}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al seleccionar envío: {ex.Message}", ex);
            }
        }

        // Recalcula el total con la estrategia actual
        public decimal RecalculateTotal(decimal subtotal)
        {
            if (_currentStrategy == null)
                return subtotal;

            var shippingCost = _currentStrategy.CalculateShippingCost(subtotal);
            return subtotal + shippingCost;
        }

        // Confirma el pedido: valida, notifica y guarda
        public void ConfirmOrder()
        {
            try
            {
                // Construye el pedido (incluye validaciones internas)
                var order = _builder.Build();

                // Notifica a los observadores
                _orderService.ConfirmOrder(order);

                // Guarda en el repositorio
                _repository.Add(order);

                Console.WriteLine($"\n✓ Pedido #{order.Id.ToString().Substring(0, 8)} guardado exitosamente en pedidos.json");
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"No se pudo confirmar el pedido: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al confirmar pedido: {ex.Message}", ex);
            }
        }

        // Obtiene todos los pedidos guardados
        public List<Order> GetAllOrders()
        {
            try
            {
                return _repository.GetAll();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener pedidos: {ex.Message}", ex);
            }
        }

        // Muestra el resumen del pedido antes de confirmar
        public void ShowCurrentOrderSummary()
        {
            // Como el builder ya tiene los datos, podríamos hacer un método auxiliar
            // Por ahora este método se puede usar desde la vista para mostrar info
            Console.WriteLine("\n--- Resumen del Pedido Actual ---");
            if (_currentStrategy != null)
            {
                Console.WriteLine($"Tipo de envío: {_currentStrategy.GetShippingType()}");
            }
        }
    }
}