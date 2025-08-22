using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurante
{
    internal class EventosGlobales
    {
        // Evento que se dispara cuando se entrega un pedido
        public static event Action PedidoEntregado;

        // Método para disparar el evento


        public static void DispararPedidoEntregado()
        {
            PedidoEntregado?.Invoke();
        }
    }
}
