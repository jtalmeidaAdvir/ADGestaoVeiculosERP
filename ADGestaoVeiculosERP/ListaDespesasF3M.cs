using ErpBS100;
using StdPlatBS100;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ADGestaoVeiculosERP.ListaCusto;

namespace ADGestaoVeiculosERP
{
    public partial class ListaDespesasF3M : Form
    {
        private string _matricula;
        private ErpBS _BSO;
        private StdBSInterfPub _PSO;
        public ListaDespesasF3M(string matricula, ErpBS BSO, StdBSInterfPub PSO)
        {
            InitializeComponent();
            _matricula = matricula;
            _BSO = BSO;
            _PSO = PSO;

            GetValores(_matricula);
        }

        private void GetValores(string viatura)
        {
            dataGridView1.Rows.Clear();

            var query = $@" SELECT * 
 FROM [PRIPVEIGA].[dbo].[AD_F3MDespesas]  WHERE NumViatura = '{viatura}'
 Order By Data DESC";
            var result = _BSO.Consulta(query);

            var num = result.NumLinhas();
            result.Inicio();
            for (int i = 0; i < num; i++)
            {
                var NumViatura = result.DaValor<string>("NumViatura");
                var Data = result.DaValor<string>("Data");
                var Valor = result.DaValor<string>("Valor");
                dataGridView1.Rows.Add(NumViatura, Data, Valor);

                result.Seguinte();
            }
        }
    }
}
