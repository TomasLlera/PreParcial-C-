/*// ============= Models/Destino.cs =============
namespace SistemaReservas.Models
{
    public class Destino
    {
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioBase { get; set; }
        public int DuracionDias { get; set; }
        public decimal Total => PrecioBase * DuracionDias;
    }
}

// ============= Models/Reserva.cs =============
using System;
using System.Collections.Generic;

namespace SistemaReservas.Models
{
    public class Reserva
    {
        public string Viajero { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public List<Destino> Destinos { get; set; } = new List<Destino>();
        public string MedioTransporte { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}

// ============= Strategies/ITransporteStrategy.cs =============
using System.Collections.Generic;
using SistemaReservas.Models;

namespace SistemaReservas.Strategies
{
    public interface ITransporteStrategy
    {
        string Nombre { get; }
        decimal CalcularCostoTotal(IEnumerable<Destino> destinos, decimal subtotal);
    }
}

// ============= Strategies/TransporteAvion.cs =============
using System.Collections.Generic;
using SistemaReservas.Models;

namespace SistemaReservas.Strategies
{
    public class TransporteAvion : ITransporteStrategy
    {
        public string Nombre { get { return "Avión"; } }
        
        public decimal CalcularCostoTotal(IEnumerable<Destino> destinos, decimal subtotal)
        {
            return subtotal * 1.10m;
        }
    }
}

// ============= Strategies/TransporteMicro.cs =============
using System.Collections.Generic;
using SistemaReservas.Models;

namespace SistemaReservas.Strategies
{
    public class TransporteMicro : ITransporteStrategy
    {
        public string Nombre => "Micro";
        public decimal CalcularCostoTotal(IEnumerable<Destino> destinos, decimal subtotal) => subtotal * 1.05m;
    }
}

// ============= Strategies/TransporteAuto.cs =============
using System.Collections.Generic;
using System.Linq;
using SistemaReservas.Models;

namespace SistemaReservas.Strategies
{
    public class TransporteAuto : ITransporteStrategy
    {
        public string Nombre { get { return "Auto"; } }
        
        public decimal CalcularCostoTotal(IEnumerable<Destino> destinos, decimal subtotal)
        {
            int cantidadDestinos = destinos.Count();
            decimal costoFijo = cantidadDestinos <= 2 ? 30000m : 50000m;
            return subtotal + costoFijo;
        }
    }
}

// ============= Builders/IReservaBuilder.cs =============
using System;
using SistemaReservas.Models;
using SistemaReservas.Strategies;

namespace SistemaReservas.Builders
{
    public interface IReservaBuilder
    {
        void Reset();
        void SetViajero(string nombre);
        void SetFechaInicio(DateTime fecha);
        void AddDestino(string nombre, decimal precioBase, int duracionDias);
        void SetTransporte(ITransporteStrategy strategy);
        Reserva Build();
    }
}

// ============= Builders/ReservaBuilder.cs =============
using System;
using System.Linq;
using SistemaReservas.Models;
using SistemaReservas.Strategies;

namespace SistemaReservas.Builders
{
    public class ReservaBuilder : IReservaBuilder
    {
        private Reserva _reserva = new Reserva();
        private ITransporteStrategy _transporte;

        public void Reset()
        {
            _reserva = new Reserva();
            _transporte = null;
        }

        public void SetViajero(string nombre)
        {
            _reserva.Viajero = nombre.Trim();
        }

        public void SetFechaInicio(DateTime fecha)
        {
            _reserva.FechaInicio = fecha;
        }
        
        public void AddDestino(string nombre, decimal precioBase, int duracionDias)
        {
            _reserva.Destinos.Add(new Destino { Nombre = nombre.Trim(), PrecioBase = precioBase, DuracionDias = duracionDias });
        }

        public void SetTransporte(ITransporteStrategy strategy)
        {
            _transporte = strategy;
            _reserva.MedioTransporte = strategy.Nombre;
        }

        public Reserva Build()
        {
            if (string.IsNullOrWhiteSpace(_reserva.Viajero)) throw new InvalidOperationException("Falta el nombre del viajero.");
            if (_reserva.FechaInicio == default(DateTime)) throw new InvalidOperationException("Falta la fecha de inicio.");
            if (_reserva.Destinos.Count == 0) throw new InvalidOperationException("La reserva debe tener al menos un destino.");
            if (_transporte == null) throw new InvalidOperationException("Debe seleccionar un medio de transporte.");

            decimal subtotal = _reserva.Destinos.Sum(d => d.Total);
            if (_reserva.Destinos.Count >= 2) subtotal = subtotal * 0.95m; // BONUS: Descuento 5%
            _reserva.Total = _transporte.CalcularCostoTotal(_reserva.Destinos, subtotal);
            return _reserva;
        }
    }
}

// ============= Observers/ReservaConfirmadaEventArgs.cs =============
using System;
using SistemaReservas.Models;

namespace SistemaReservas.Observers
{
    public class ReservaConfirmadaEventArgs : EventArgs
    {
        public Reserva Reserva { get; set; }
        
        public ReservaConfirmadaEventArgs(Reserva reserva)
        {
            Reserva = reserva;
        }
    }
}

// ============= Observers/ClienteObserver.cs =============
using System;
using System.Linq;

namespace SistemaReservas.Observers
{
    public class ClienteObserver
    {
        public void OnReservaConfirmada(object sender, ReservaConfirmadaEventArgs e)
        {
            var r = e.Reserva;
            Console.WriteLine($"\n[Cliente] ¡Gracias {r.Viajero}! Reserva confirmada: ${r.Total:N2}");
            Console.WriteLine($"Inicio: {r.FechaInicio:dd/MM/yyyy} | Transporte: {r.MedioTransporte}");
            Console.WriteLine($"Destinos: {string.Join(", ", r.Destinos.Select(d => d.Nombre))}");
            Console.WriteLine($"Costo/día promedio: ${r.Total / r.Destinos.Sum(d => d.DuracionDias):N2}");
        }
    }
}

// ============= Observers/AgenciaObserver.cs =============
using System;
using System.Linq;

namespace SistemaReservas.Observers
{
    public class AgenciaObserver
    {
        public void OnReservaConfirmada(object sender, ReservaConfirmadaEventArgs e)
        {
            var r = e.Reserva;
            Console.WriteLine($"\n[Agencia] Nueva reserva - Cliente: {r.Viajero} | Total: ${r.Total:N2}");
            foreach (var d in r.Destinos)
                Console.WriteLine($"  • {d.Nombre}: {d.DuracionDias} días x ${d.PrecioBase} = ${d.Total:N2}");
        }
    }
}

// ============= Services/ReservaService.cs =============
using System;
using SistemaReservas.Models;
using SistemaReservas.Observers;

namespace SistemaReservas.Services
{
    public class ReservaService
    {
        public event EventHandler<ReservaConfirmadaEventArgs> ReservaConfirmada;

        public void Confirmar(Reserva reserva)
        {
            if (ReservaConfirmada != null)
                ReservaConfirmada(this, new ReservaConfirmadaEventArgs(reserva));
        }
    }
}

// ============= Services/TransporteFactory.cs =============
using System;
using SistemaReservas.Strategies;

namespace SistemaReservas.Services
{
    public class TransporteFactory
    {
        private readonly TransporteAvion _avion;
        private readonly TransporteMicro _micro;
        private readonly TransporteAuto _auto;

        public TransporteFactory(TransporteAvion avion, TransporteMicro micro, TransporteAuto auto)
        {
            _avion = avion;
            _micro = micro;
            _auto = auto;
        }

        public ITransporteStrategy FromAlias(string alias)
        {
            switch (alias.ToLowerInvariant())
            {
                case "avion": return _avion;
                case "micro": return _micro;
                case "auto": return _auto;
                default: throw new ArgumentException("Tipo de transporte inválido. Use: avion | micro | auto");
            }
        }
    }
}

// ============= Repositories/IRepositorio.cs =============
using System.Collections.Generic;

namespace SistemaReservas.Repositories
{
    public interface IRepositorio<T>
    {
        void Guardar(T entidad);
        IEnumerable<T> ObtenerTodos();
    }
}

// ============= Repositories/RepositorioJson.cs =============
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SistemaReservas.Repositories
{
    public class RepositorioJson<T> : IRepositorio<T>
    {
        private readonly string _ruta;
        private static readonly JsonSerializerOptions _opts = new JsonSerializerOptions { WriteIndented = true };

        public RepositorioJson(string ruta) => _ruta = ruta;

        public void Guardar(T entidad)
        {
            var data = new List<T>();
            if (File.Exists(_ruta))
            {
                try { data = JsonSerializer.Deserialize<List<T>>(File.ReadAllText(_ruta), _opts) ?? new List<T>(); }
                catch (Exception ex) { Console.WriteLine($"Error leyendo archivo: {ex.Message}"); }
            }
            data.Add(entidad);
            try { File.WriteAllText(_ruta, JsonSerializer.Serialize(data, _opts)); }
            catch (Exception ex) { throw new InvalidOperationException($"Error guardando: {ex.Message}"); }
        }

        public IEnumerable<T> ObtenerTodos()
        {
            if (!File.Exists(_ruta)) return Enumerable.Empty<T>();
            try { return JsonSerializer.Deserialize<List<T>>(File.ReadAllText(_ruta), _opts) ?? Enumerable.Empty<T>(); }
            catch (Exception ex) { Console.WriteLine($"Error leyendo reservas: {ex.Message}"); return Enumerable.Empty<T>(); }
        }
    }
}

// ============= Controllers/ViajeFacade.cs =============
using System;
using System.Linq;
using SistemaReservas.Builders;
using SistemaReservas.Models;
using SistemaReservas.Repositories;
using SistemaReservas.Services;

namespace SistemaReservas.Controllers
{
    public class ViajeFacade
    {
        private readonly IReservaBuilder _builder;
        private readonly TransporteFactory _factory;
        private readonly ReservaService _service;
        private readonly IRepositorio<Reserva> _repo;

        public ViajeFacade(IReservaBuilder builder, TransporteFactory factory, ReservaService service, IRepositorio<Reserva> repo)
        {
            _builder = builder;
            _factory = factory;
            _service = service;
            _repo = repo;
            _builder.Reset();
        }

        public void Reset()
        {
            _builder.Reset();
        }

        public void SetDatosViajero(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre del viajero es obligatorio");
            _builder.SetViajero(nombre);
        }

        public void SetFechaInicio(DateTime fecha)
        {
            if (fecha.Date < DateTime.Now.Date) throw new ArgumentException("La fecha debe ser futura");
            _builder.SetFechaInicio(fecha);
        }

        public void AgregarDestino(string nombre, decimal precioBase, int duracionDias)
        {
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre es obligatorio");
            if (precioBase <= 0) throw new ArgumentException("Precio debe ser mayor a cero");
            if (duracionDias <= 0) throw new ArgumentException("Duración debe ser al menos 1 día");
            _builder.AddDestino(nombre, precioBase, duracionDias);
        }

        public void SeleccionarTransporte(string alias)
        {
            _builder.SetTransporte(_factory.FromAlias(alias));
        }

        public void ConfirmarReserva()
        {
            var reserva = _builder.Build();
            _service.Confirmar(reserva);
            _repo.Guardar(reserva);
            Console.WriteLine("\n✓ Reserva guardada en reservas.json");
        }

        public void MostrarReservas()
        {
            var reservas = _repo.ObtenerTodos().ToList();
            if (!reservas.Any())
            {
                Console.WriteLine("\nNo hay reservas.");
                return;
            }
            
            Console.WriteLine("\n=== RESERVAS GUARDADAS ===");
            foreach (var r in reservas)
            {
                Console.WriteLine($"\n{r.Viajero} | {r.FechaInicio:dd/MM/yyyy} | {r.MedioTransporte}");
                Console.WriteLine($"Destinos: {string.Join(", ", r.Destinos.Select(d => d.Nombre))} | Total: ${r.Total:N2}");
            }
        }
    }
}

// ============= Program.cs =============
using System;
using SistemaReservas.Builders;
using SistemaReservas.Controllers;
using SistemaReservas.Models;
using SistemaReservas.Observers;
using SistemaReservas.Repositories;
using SistemaReservas.Services;
using SistemaReservas.Strategies;

namespace SistemaReservas
{
    class Program
    {
        static void Main()
        {
            var repo = new RepositorioJson<Reserva>("reservas.json");
            var builder = new ReservaBuilder();
            
            var avion = new TransporteAvion();
            var micro = new TransporteMicro();
            var auto = new TransporteAuto();
            var factory = new TransporteFactory(avion, micro, auto);
            
            var service = new ReservaService();
            var clienteObs = new ClienteObserver();
            var agenciaObs = new AgenciaObserver();
            
            service.ReservaConfirmada += clienteObs.OnReservaConfirmada;
            service.ReservaConfirmada += agenciaObs.OnReservaConfirmada;

            MostrarMenu(new ViajeFacade(builder, factory, service, repo));
        }

        static void MostrarMenu(ViajeFacade facade)
        {
            string op = "";
            bool datosViajeroIngresados = false;

            while (op != "5")
            {
                Console.Clear();
                Console.WriteLine("═══ SISTEMA DE RESERVAS ═══");
                Console.WriteLine("1) Agregar destino");
                Console.WriteLine("2) Seleccionar transporte");
                Console.WriteLine("3) Confirmar reserva");
                Console.WriteLine("4) Listar reservas");
                Console.WriteLine("5) Salir\n");
                Console.Write("Opción: ");
                op = Console.ReadLine()?.Trim() ?? "";

                try
                {
                    switch (op)
                    {
                        case "1":
                            if (!datosViajeroIngresados)
                            {
                                IniciarReserva(facade);
                                datosViajeroIngresados = true;
                            }
                            AgregarDestino(facade);
                            Pausa();
                            break;

                        case "2":
                            if (!datosViajeroIngresados)
                            {
                                Console.WriteLine("\nDebe agregar al menos un destino primero (opción 1)");
                            }
                            else
                            {
                                SeleccionarTransporte(facade);
                            }
                            Pausa();
                            break;

                        case "3":
                            if (!datosViajeroIngresados)
                            {
                                Console.WriteLine("\nDebe agregar al menos un destino primero (opción 1)");
                            }
                            else
                            {
                                facade.ConfirmarReserva();
                                datosViajeroIngresados = false;
                                facade.Reset();
                            }
                            Pausa();
                            break;

                        case "4":
                            facade.MostrarReservas();
                            Pausa();
                            break;

                        case "5":
                            Console.WriteLine("\n¡Hasta luego!");
                            return;

                        default:
                            Console.WriteLine("\nOpción inválida");
                            Pausa();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n✗ Error: {ex.Message}");
                    Pausa();
                }
            }
        }

        static void IniciarReserva(ViajeFacade facade)
        {
            facade.Reset();
            
            string nombre = "";
            for (int i = 0; i < 3; i++)
            {
                Console.Write("\nNombre del viajero: ");
                nombre = Console.ReadLine() ?? "";
                
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    Console.WriteLine("El nombre no puede estar vacío.");
                    if (i == 2) throw new InvalidOperationException("Demasiados intentos fallidos");
                    continue;
                }
                
                if (EsSoloNumeros(nombre))
                {
                    Console.WriteLine("El nombre no puede ser solo números.");
                    if (i == 2) throw new InvalidOperationException("Demasiados intentos fallidos");
                    continue;
                }
                
                break;
            }
            
            facade.SetDatosViajero(nombre);

            DateTime fecha = default(DateTime);
            for (int i = 0; i < 3; i++)
            {
                Console.Write("Fecha inicio (dd/MM/yyyy): ");
                if (DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out fecha))
                {
                    try
                    {
                        facade.SetFechaInicio(fecha);
                        break;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        if (i == 2) throw new InvalidOperationException("Demasiados intentos fallidos");
                    }
                }
                else
                {
                    Console.WriteLine("Formato inválido. Use dd/MM/yyyy");
                    if (i == 2) throw new InvalidOperationException("Demasiados intentos fallidos");
                }
            }
            Console.WriteLine("\n✓ Reserva iniciada");
            Pausa();
        }

        static void AgregarDestino(ViajeFacade facade)
        {
            string nombre = "";
            for (int i = 0; i < 3; i++)
            {
                Console.Write("\nNombre del destino: ");
                nombre = Console.ReadLine() ?? "";
                
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    Console.WriteLine("El nombre del destino no puede estar vacío.");
                    if (i == 2) throw new InvalidOperationException("Demasiados intentos fallidos");
                    continue;
                }
                
                if (EsSoloNumeros(nombre))
                {
                    Console.WriteLine("El nombre del destino no puede ser solo números.");
                    if (i == 2) throw new InvalidOperationException("Demasiados intentos fallidos");
                    continue;
                }
                
                break;
            }

            decimal precio = 0;
            for (int i = 0; i < 3; i++)
            {
                Console.Write("Precio por día: $");
                string input = Console.ReadLine() ?? "0";
                if (decimal.TryParse(input, out precio) && precio > 0)
                {
                    break;
                }
                Console.WriteLine("Precio inválido. Debe ser un número mayor a cero.");
                if (i == 2) throw new InvalidOperationException("Demasiados intentos fallidos");
            }

            int dias = 0;
            for (int i = 0; i < 3; i++)
            {
                Console.Write("Duración en días: ");
                string input = Console.ReadLine() ?? "0";
                if (int.TryParse(input, out dias) && dias > 0)
                {
                    break;
                }
                Console.WriteLine("Duración inválida. Debe ser un número entero mayor a cero.");
                if (i == 2) throw new InvalidOperationException("Demasiados intentos fallidos");
            }

            facade.AgregarDestino(nombre, precio, dias);
            Console.WriteLine("\n✓ Destino agregado");
        }

        static bool EsSoloNumeros(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return false;
            
            foreach (char c in texto.Trim())
            {
                if (!char.IsDigit(c) && c != ' ')
                    return false;
            }
            return true;
        }

        static void SeleccionarTransporte(ViajeFacade facade)
        {
            Console.WriteLine("\nTransporte: avion (+10%) | micro (+5%) | auto ($30k-50k)");
            Console.Write("Seleccione: ");
            facade.SeleccionarTransporte(Console.ReadLine()?.Trim() ?? "");
            Console.WriteLine("\n✓ Transporte seleccionado");
        }

        static void Pausa()
        {
            Console.WriteLine("\nPresione una tecla...");
            Console.ReadKey();
        }
    }
}*/