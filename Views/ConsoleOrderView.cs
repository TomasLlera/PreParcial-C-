using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Views
{
    // Implementación de la vista para consola
    public class ConsoleOrderView : IOrderView
    {
        // Eventos que se disparan cuando el usuario selecciona una opción
        public event EventHandler AddProductRequested;
        public event EventHandler SetClientDataRequested;
        public event EventHandler SelectShippingRequested;
        public event EventHandler ConfirmOrderRequested;
        public event EventHandler ViewOrdersRequested;
        public event EventHandler ExitRequested;

        private bool _running = true;

        public void Run()
        {
            ShowWelcome();

            while (_running)
            {
                try
                {
                    ShowMenu();
                    var option = ReadOption();
                    ProcessOption(option);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }

            ShowGoodbye();
        }

        private void ShowWelcome()
        {
            Console.Clear();
            Console.WriteLine("╔═══════════════════════════════════════════╗");
            Console.WriteLine("║     SISTEMA DE GESTIÓN DE PEDIDOS        ║");
            Console.WriteLine("║          Programación II                  ║");
            Console.WriteLine("╚═══════════════════════════════════════════╝");
            Console.WriteLine();
        }

        private void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("\n┌───────────────────────────────────────────┐");
            Console.WriteLine("│              MENÚ PRINCIPAL               │");
            Console.WriteLine("├───────────────────────────────────────────┤");
            Console.WriteLine("│ 1. Agregar Producto                       │");
            Console.WriteLine("│ 2. Establecer Datos del Cliente           │");
            Console.WriteLine("│ 3. Seleccionar Tipo de Envío              │");
            Console.WriteLine("│ 4. Confirmar Pedido                       │");
            Console.WriteLine("│ 5. Ver Pedidos Guardados                  │");
            Console.WriteLine("│ 0. Salir                                  │");
            Console.WriteLine("└───────────────────────────────────────────┘");
            Console.Write("\nSeleccione una opción: ");
        }

        private string ReadOption()
        {
            return Console.ReadLine()?.Trim() ?? "";
        }

        private void ProcessOption(string option)
        {
            Console.Clear();
            switch (option)
            {
                case "1":
                    AddProductRequested?.Invoke(this, EventArgs.Empty);
                    break;
                case "2":
                    SetClientDataRequested?.Invoke(this, EventArgs.Empty);
                    break;
                case "3":
                    SelectShippingRequested?.Invoke(this, EventArgs.Empty);
                    break;
                case "4":
                    ConfirmOrderRequested?.Invoke(this, EventArgs.Empty);
                    break;
                case "5":
                    ViewOrdersRequested?.Invoke(this, EventArgs.Empty);
                    break;
                case "0":
                    _running = false;
                    ExitRequested?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    ShowError("Opción inválida. Por favor, seleccione una opción del menú.");
                    break;
            }
        }

        public void ShowMessage(string message)
        {
            Console.WriteLine($"\n{message}");
        }

        public void ShowError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ ERROR: {error}");
            Console.ResetColor();
            PauseForUser();
        }

        public void ShowSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n✓ {message}");
            Console.ResetColor();
            PauseForUser();
        }

        private void ShowGoodbye()
        {
            Console.Clear();
            Console.WriteLine("\n╔═══════════════════════════════════════════╗");
            Console.WriteLine("║   ¡Gracias por usar el sistema!          ║");
            Console.WriteLine("║          ¡Hasta pronto!                   ║");
            Console.WriteLine("╚═══════════════════════════════════════════╝\n");
        }

        private void PauseForUser()
        {
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        // Métodos auxiliares para leer datos del usuario
        public string ReadString(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine()?.Trim() ?? "";
        }

        public decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), out decimal value))
                    return value;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠ Debe ingresar un número decimal válido.");
                Console.ResetColor();
            }
        }

        public int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value))
                    return value;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠ Debe ingresar un número entero válido.");
                Console.ResetColor();
            }
        }
    }
}