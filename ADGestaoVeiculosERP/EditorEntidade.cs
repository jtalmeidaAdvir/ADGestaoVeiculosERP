using ErpBS100;
using StdPlatBS100;
using System;
using System.Windows.Forms;

namespace ADGestaoVeiculosERP
{
    public partial class EditorEntidade : Form
    {
        private ErpBS bSO;
        private StdBSInterfPub pSO;

        public EditorEntidade(ErpBS bSO, StdBSInterfPub pSO)
        {
            InitializeComponent();
            this.bSO = bSO;
            this.pSO = pSO;
            GetUltimoId();
        }

        private void GetUltimoId()
        {
            var query = "SELECT MAX(ID) + 1 AS ProximoID FROM [PRIPVEIGA].[dbo].AD_Entidades;";
            var SQLID = bSO.Consulta(query);

            var ultimoID = SQLID.DaValor<string>("ProximoID");
            TXT_ID.Text = ultimoID;
        }

        private void BT_Criar_Click(object sender, EventArgs e)
        {
            // Verifica se o campo Nome está vazio
            if (string.IsNullOrWhiteSpace(TXT_Nome.Text))
            {
                MessageBox.Show("O campo Nome é obrigatório!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Interrompe a execução do método
            }
            var queryInserir = $@"
                INSERT INTO AD_Entidades ( ID, Nome, Contribuinte, Endereco, Localidade, CodCodigoPostal, DescCodigoPostal, Telefone, Fax, Obs)
                 VALUES (
                {TXT_ID.Text},
                '{TXT_Nome.Text}',
                '{TXT_Contribuinte.Text}',
                '{TXT_Endereco.Text}',
                '{TXT_Localidade.Text}',
                '{TXT_CP.Text}',
                '{TXT_CPLocal}',
                '{TXT_Telefone.Text}',
                '{TXT_Fax.Text}',
                '{TXT_Obs.Text}')";
            bSO.DSO.ExecuteSQL(queryInserir);
            this.Close();
        }
    }
}
