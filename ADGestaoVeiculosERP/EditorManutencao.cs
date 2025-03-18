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

namespace ADGestaoVeiculosERP
{
    public partial class EditorManutencao : Form
    {
        private ErpBS bSO;
        private StdBSInterfPub pSO;
        public EditorManutencao(ErpBS bSO, StdBSInterfPub pSO)
        {
            InitializeComponent();
            this.bSO = bSO;
            this.pSO = pSO;
            GetUltimoId();
        }

        private void GetUltimoId()
        {
            var query = "SELECT MAX(Codigo) + 1 AS ProximoID FROM [PRIPVEIGA].[dbo].AD_TiposDespesas;";
            var SQLID = bSO.Consulta(query);

            var ultimoID = SQLID.DaValor<string>("ProximoID");
            TXT_ID.Text = ultimoID;
        }

        private void BT_Criar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TXT_Nome.Text))
            {
                MessageBox.Show("O campo Descrição é obrigatório!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Interrompe a execução do método
            }

            var queryInserir = $@"
                INSERT INTO AD_TiposDespesas ( Codigo, Descricao)
                 VALUES (
                {TXT_ID.Text},
                '{TXT_Nome.Text}')";
            bSO.DSO.ExecuteSQL(queryInserir);
            this.Close();
        }
    }
}
