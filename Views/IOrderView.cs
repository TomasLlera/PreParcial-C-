using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Views
{
    // Interfaz que define el contrato de la vista
    public interface IOrderView
    {
        // Eventos que la vista dispara al controlador
        event EventHandler AddProductRequested;
        event EventHandler SetClientDataRequested;
        event EventHandler SelectShippingRequested;
        event EventHandler ConfirmOrderRequested;
        event EventHandler ViewOrdersRequested;
        event EventHandler ExitRequested;

        // Métodos para mostrar información al usuario
        void ShowMessage(string message);
        void ShowError(string error);
        void ShowSuccess(string message);

        // Método principal que muestra el menú
        void Run();
    }
}