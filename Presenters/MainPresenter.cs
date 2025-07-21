using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurante.Presenters
{
    internal class MainPresenter
    {
        private Menu mainView;
        private int? idtipousuario;
        private readonly string sqlConnectionString;
        private readonly int idsucursal;
        private readonly int idusuario;

        public MainPresenter(Menu mainView, string sqlConnectionString, int? tipousuario, string nombre, int idsucursal, int idusuario)
        {
            this.mainView = mainView;
            idtipousuario = tipousuario;
            this.mainView.Show();

            this.idusuario = idusuario;

            

            
            this.idsucursal = idsucursal;
            this.sqlConnectionString = sqlConnectionString;
        }

    }
}
