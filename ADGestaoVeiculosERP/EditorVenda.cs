
using Primavera.Extensibility.BusinessEntities;
using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;
using Primavera.Extensibility.Sales.Editors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ADGestaoVeiculosERP
{
    public class EditorVenda : EditorVendas
    {
        public override void DepoisDeGravar(string Filial, string Tipo, string Serie, int NumDoc, ExtensibilityEventArgs e)
        {
            try
            {
                if(this.DocumentoVenda.Tipodoc == "FR")
                {
                    var numero = this.DocumentoVenda.Linhas.NumItens;

                    for (int i = 1; i <= numero + 1; i++)
                    {
                        var linha = this.DocumentoVenda.Linhas.GetEdita(i);
                        var matricula = linha.CamposUtil["CDU_MAtricula"].Valor.ToString();
                        var kms = linha.CamposUtil["CDU_Kms"].Valor.ToString();
                        var matriculasComAviso = new HashSet<string>();
                        var query2 = $"SELECT * FROM [PRIPVEIGA].[dbo].AD_Viaturas where IdMatricula = '{matricula}'";
                        var viatura2 = BSO.Consulta(query2);

                        if (!string.IsNullOrEmpty(kms) && kms != "0")
                        {
                            var UpdateKMS = $@"UPDATE [PRIPVEIGA].[dbo].AD_Viaturas
                            SET KMActuais = {kms}
                            WHERE IdMatricula = '{matricula}'";
                            BSO.DSO.ExecuteSQL(UpdateKMS);
                        }

                        var totalDespesa = this.DocumentoVenda.Linhas.GetEdita(i).PrecUnit * this.DocumentoVenda.Linhas.GetEdita(i).Quantidade;
                        var viaturaTotalDespesa = viatura2.DaValor<decimal>("TotalDespesas");

                        var calculadoDespesas = (double)viaturaTotalDespesa + totalDespesa;
                        string TotalDespesas = calculadoDespesas.ToString("F2").Replace(",", ".");
                        var update = $@"UPDATE [PRIPVEIGA].[dbo].AD_Viaturas
                            SET 
                                TotalDespesas = {TotalDespesas}
                            WHERE IdMatricula = '{matricula}'";
                        BSO.DSO.ExecuteSQL(update);



                        // AVISO (somente se a matrícula ainda não tiver sido avisada)
                        if (!matriculasComAviso.Contains(matricula))
                        {
                            var query = $"SELECT * FROM [PRIPVEIGA].[dbo].AD_RegistrosManutencao where IdMatricula = '{matricula}'";
                            var viatura = BSO.Consulta(query);
                            var numvia = viatura.NumLinhas();
                            viatura.Inicio();

                            for (int y = 0; y < numvia; y++)
                            {
                                var quilometros = viatura.DaValor<int>("Quilometros");
                                var dataDoc = this.DocumentoVenda.DataDoc;
                                var dataString = viatura.DaValor<DateTime>("DataEvento");
                                var infoData = viatura.DaValor<string>("Descricao");

                                if (!string.IsNullOrEmpty(dataString.ToString()))
                                {
                                    DateTime data = DateTime.ParseExact(dataString.ToString(), "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                                    if (data.ToString() != "01-01-0001 00:00:00" && data < dataDoc)
                                    {
                                        MessageBox.Show($"Atenção: O veículo {matricula} necessita de '{infoData}', a data do evento é anterior à data do documento.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        matriculasComAviso.Add(matricula); // Adiciona ao HashSet para não repetir
                                        break; // Sai do loop pois já avisamos esta matrícula
                                    }
                                }

                                var intkms = int.Parse(kms);
                                if (quilometros != 0 && intkms >= quilometros)
                                {
                                    MessageBox.Show($"Atenção: O seu veículo {matricula} precisa de '{infoData}' urgente.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    matriculasComAviso.Add(matricula); // Adiciona ao HashSet para não repetir
                                    break; // Sai do loop pois já avisamos esta matrícula
                                }

                                viatura.Seguinte();
                            }
                        }
                    }


                }
            }
            catch { }
        }
    }
}
