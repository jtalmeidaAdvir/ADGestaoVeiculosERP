using StdBE100;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADGestaoVeiculosERP
{
    public partial class DetalhesDespesasAntigas : Form
    {
        private string numero;
        ErpBS100.ErpBS BSO;
        StdPlatBS100.StdBSInterfPub PSO;

        public DetalhesDespesasAntigas(ErpBS100.ErpBS _BSO, StdPlatBS100.StdBSInterfPub _PSO, string _numero)
        {
            InitializeComponent();
            numero = _numero;
            BSO = _BSO;
            PSO = _PSO;
            GetValores();
        }

        private void GetValores()
        {
            var query = $@"SELECT * 
            FROM [PRIPVEIGA].[dbo].[AD_F3MDespesas] WHERE Numero = '{numero}'";

            var dadosViatura = BSO.Consulta(query);

            var matricula = dadosViatura.DaValor<string>("NumViatura");

            var query2 = $@"SELECT * 
            FROM [PRIPVEIGA].[dbo].[AD_Viaturas] WHERE IdMatricula = '{matricula}'";

            var listavitura = BSO.Consulta(query2);

            InsereValores2(listavitura);

            InsereValores(dadosViatura);
        }

        private void InsereValores2(StdBELista listavitura)
        {
            txt_numViatura.Text = listavitura.DaValor<string>("Codigo");
            txt_modelo.Text = listavitura.DaValor<string>("Modelo");
            txt_marca.Text = listavitura.DaValor<string>("Marca");
        }


        private void InsereValores(StdBELista dadosViatura)
        {
            txt_numero.Text = dadosViatura.DaValor<string>("Numero");
            txt_matricula.Text = dadosViatura.DaValor<string>("NumViatura");
            txt_despesa.Text = dadosViatura.DaValor<string>("CodTipoDespesa");
            txt_obs.Text = dadosViatura.DaValor<string>("Obs");
            txt_valor.Text = dadosViatura.DaValor<string>("Valor");
            string dataStr = dadosViatura.DaValor<string>("Data");
            if (DateTime.TryParse(dataStr, out DateTime dataConvertida))
            {
                dtp_data.Value = dataConvertida;
            }
            else
            {
                // MessageBox.Show("Data inválida: " + dataStr);
            }
        }
    }
}
