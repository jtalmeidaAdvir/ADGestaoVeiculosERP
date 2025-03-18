using CmpBE100;
using ErpBS100;
using StdBE100;
using StdPlatBS100;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADGestaoVeiculosERP
{
    public partial class FormEditorVenda : Form
    {
        private ErpBS bSO;
        private StdBSInterfPub pSO;
        private CmpBE100.CmpBEDocumentoCompra documento;
        private CmpBELinhaDocumentoCompra linha;
        private int kmAtuais;
        private string matricula;

        public FormEditorVenda(ErpBS bSO, StdBSInterfPub pSO, CmpBE100.CmpBEDocumentoCompra documentoCompra)
        {
            InitializeComponent();
            this.bSO = bSO;
            this.pSO = pSO;
            this.documento = documentoCompra;
            GetViaturaInfo();
        }

        private void GetViaturaInfo()
        {
             var viaturaSql = GetKilometros();
            NUD_KMS.Value = viaturaSql;
        }

        private int GetKilometros()
        {

            var numeroLinhas = this.documento.Linhas.NumItens;

            for (int i = 1; i <= numeroLinhas; i++)
            {
                var linha = this.documento.Linhas.GetEdita(i);

                if (linha.CamposUtil["CDU_MAtricula"].Valor != null &&
                    !string.IsNullOrEmpty(linha.CamposUtil["CDU_MAtricula"].Valor.ToString()))
                {
                    matricula = linha.CamposUtil["CDU_MAtricula"].Valor.ToString();
                    break; // Sai do loop assim que encontrar um valor
                }
            }

            if (!string.IsNullOrEmpty(matricula))
            {
                var query = $"SELECT KMActuais FROM [PRIPVEIGA].[dbo].AD_Viaturas WHERE IdMatricula = '{matricula}'";
                var viatura = bSO.Consulta(query);

                kmAtuais = viatura.DaValor<int>("KMActuais");

                return kmAtuais;
            }
            this.Close();
            return 0;

        }


        private void Enviar_Click(object sender, EventArgs e)
        {
            var query = $"SELECT * FROM [PRIPVEIGA].[dbo].AD_RegistrosManutencao where IdMatricula = '{matricula}'";
            var viatura = bSO.Consulta(query);

            var query2 = $"SELECT * FROM [PRIPVEIGA].[dbo].AD_Viaturas where IdMatricula = '{matricula}'";
            var viatura2 = bSO.Consulta(query2);


            var num = viatura.NumLinhas();
            double totalDespesa = 0;
            if (num > 0) {
                for (int i = 0; i < num; i++)
                {
                    var quilometros = viatura.DaValor<int>("Quilometros");
                    var dataDoc = this.documento.DataDoc;


                    var dataString = viatura.DaValor<DateTime>("DataEvento");
                    var infoData = viatura.DaValor<string>("Descricao");

                    if (!string.IsNullOrEmpty(dataString.ToString()))
                    {
                        
                        DateTime data = DateTime.ParseExact(dataString.ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        if (data.ToString() != "01/01/0001 00:00:00")
                        {

                        
                            if (data < dataDoc)
                            {
                                MessageBox.Show($"Atenção: O veículo necessita de '{infoData}', a data do evento é anterior à data do documento.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    if (quilometros != 0 && kmAtuais >= quilometros)
                    {
                        MessageBox.Show("Atenção: O seu veículo precisa de manutenção urgente.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    viatura.Seguinte();
                }


            }

            var numeroLinhas = this.documento.Linhas.NumItens;
            for (int i = 1; i < numeroLinhas + 1; i++)
            {
                totalDespesa += this.documento.Linhas.GetEdita(i).PrecUnit * this.documento.Linhas.GetEdita(i).Quantidade;
                
            }


            var viaturaTotalDespesa = viatura2.DaValor<decimal>("TotalDespesas");
            
            var calculadoDespesas = (double)viaturaTotalDespesa + totalDespesa;

            string TotalDespesas = calculadoDespesas.ToString("F2").Replace(",", ".");
            var update = $@"UPDATE [PRIPVEIGA].[dbo].AD_Viaturas
                            SET KMActuais = {NUD_KMS.Value},
                                TotalDespesas = {TotalDespesas}
                            WHERE IdMatricula = '{matricula}'";
            bSO.DSO.ExecuteSQL(update);
           
            this.Close();
            
        }
    }
}
