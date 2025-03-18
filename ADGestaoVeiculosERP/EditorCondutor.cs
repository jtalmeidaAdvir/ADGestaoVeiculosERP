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
    public partial class EditorCondutor : Form
    {
        private ErpBS bSO;
        private StdBSInterfPub pSO;

        public EditorCondutor(ErpBS bSO, StdBSInterfPub pSO)
        {
            InitializeComponent();
            this.bSO = bSO;
            this.pSO = pSO;
            GetUltimoId();
        }

        private void GetUltimoId()
        {
            var query = "SELECT MAX(ID) + 1 AS ProximoID FROM [PRIPVEIGA].[dbo].AD_Condutores;";
            var SQLID = bSO.Consulta(query);

            var ultimoID = SQLID.DaValor<string>("ProximoID");
            TXT_ID.Text = ultimoID;
            DTP_Nascimento.Format = DateTimePickerFormat.Custom;
            DTP_Nascimento.CustomFormat = " ";

            DTP_Validade.Format = DateTimePickerFormat.Custom;
            DTP_Validade.CustomFormat = " ";

        }

        private void BT_Criar_Click(object sender, EventArgs e)
        {
            // Verifica se o campo Nome está vazio
            if (string.IsNullOrWhiteSpace(TXT_Nome.Text))
            {
                MessageBox.Show("O campo Nome é obrigatório!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Interrompe a execução do método
            }
            string dataNascimento = (DTP_Nascimento.CustomFormat == " ") ? "NULL" : $"'{DTP_Nascimento.Value:yyyy-MM-dd}'";
            string dataValidade = (DTP_Validade.CustomFormat == " ") ? "NULL" : $"'{DTP_Validade.Value:yyyy-MM-dd}'";
            var queryInserir = $@"
                INSERT INTO AD_Condutores ( ID, Nome, Abreviatura, Endereco, Localidade, CodigoPostal, Telefone, DataNascimento, Carta, DataValidade, Obs)
                 VALUES (
                {TXT_ID.Text},
                '{TXT_Nome.Text}',
                '{TXT_Abre.Text}',
                '{TXT_Endereco.Text}',
                '{TXT_Localidade.Text}',
                '{TXT_CP.Text}',
                '{TXT_Telefone.Text}',
                {dataNascimento},
                '{TXT_Carta.Text}',
                {dataValidade},
                '{TXT_Obs.Text}')";
            bSO.DSO.ExecuteSQL(queryInserir);
            this.Close();
        }

        private void DTP_Nascimento_Enter(object sender, EventArgs e)
        {
            DTP_Nascimento.CustomFormat = "dd/MM/yyyy";
            DTP_Nascimento.Format = DateTimePickerFormat.Short;
        }

        private void DTP_Validade_Enter(object sender, EventArgs e)
        {
            DTP_Validade.CustomFormat = "dd/MM/yyyy";
            DTP_Validade.Format = DateTimePickerFormat.Short;
        }
    }
}
