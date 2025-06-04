using Primavera.Extensibility.BusinessEntities.ExtensibilityService.EventArgs;
using Primavera.Extensibility.Purchases.Editors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;


namespace ADGestaoVeiculosERP
{
    public class EditorCompra : EditorCompras
    {
        public override void AntesDeGravar(ref bool Cancel, ExtensibilityEventArgs e)
        {

            var tipodoc = this.DocumentoCompra.Tipodoc;
            if (tipodoc == "COMBV")
            {
                var linhas = this.DocumentoCompra.Linhas.NumItens;

                for (global::System.Int32 i = 1; i < linhas + 1; i++)
                {

                    var linha = this.DocumentoCompra.Linhas.GetEdita(i);

                    if (linha.Artigo != "")
                    {
                        var campo = linha.CamposUtil["CDU_Kms"].Valor.ToString();
                        var campo2 = linha.CamposUtil["CDU_UltimoKms"].Valor;
                        if (campo == "0")
                        {
                            MessageBox.Show("Os quilômetros são obrigatórios.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Cancel = true;
                            break;
                        }


                        var Kms = Convert.ToInt32(linha.CamposUtil["CDU_Kms"].Valor);
                        var Ulkms = Convert.ToInt32(linha.CamposUtil["CDU_UltimoKms"].Valor);

                        if (Ulkms > Kms)
                        {
                            MessageBox.Show("Aviso: O último quilómetro registado é maior do que o quilómetro atual!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                    }

                }
            };





        }

        public override void DepoisDeGravar(string Filial, string Tipo, string Serie, int NumDoc, ExtensibilityEventArgs e)
        {
            try
            {


                if (this.DocumentoCompra.Tipodoc == "VFA")
                {
                    var numero = this.DocumentoCompra.Linhas.NumItens;
                    bool encontrouDespvi = false;
                    for (int i = 1; i <= numero + 1; i++)
                    {
                        var linha = this.DocumentoCompra.Linhas.GetEdita(i);
                        var artigo = BSO.Base.Artigos.Edita(linha.Artigo);

                        if (artigo.Familia == "DESPVI")
                        {
                            encontrouDespvi = true;
                            break; // Sai do loop assim que encontrar o primeiro
                        }
                    }
                    if (encontrouDespvi)
                    {
                        FormEditorVenda criarViaturaForm = new FormEditorVenda(BSO, PSO, this.DocumentoCompra);
                        criarViaturaForm.ShowDialog();
                    }
                }

                if (this.DocumentoCompra.Tipodoc == "COMBV")
                {
                    var numero = this.DocumentoCompra.Linhas.NumItens;

                    for (int i = 1; i <= numero + 1; i++)
                    {
                        var linha = this.DocumentoCompra.Linhas.GetEdita(i);
                        var matricula = linha.CamposUtil["CDU_MAtricula"].Valor.ToString();
                        var kms = linha.CamposUtil["CDU_Kms"].Valor.ToString();
                        var query2 = $"SELECT * FROM [PRIPVEIGA].[dbo].AD_Viaturas where IdMatricula = '{matricula}'";
                        var viatura2 = BSO.Consulta(query2);
                        var matriculasComAviso = new HashSet<string>();

                        if (kms != "0")
                        {
                            var UpdateKMS = $@"UPDATE [PRIPVEIGA].[dbo].AD_Viaturas
                            SET KMActuais = {kms}
                            WHERE IdMatricula = '{matricula}'";
                            BSO.DSO.ExecuteSQL(UpdateKMS);
                        }

                        var totalCombustivel = DocumentoCompra.Linhas.GetEdita(i).PrecUnit * DocumentoCompra.Linhas.GetEdita(i).Quantidade;
                        var viaturaTotalCombustivel = viatura2.DaValor<decimal>("TotalCombustivel");
                        var calculadoCombustivel = (double)viaturaTotalCombustivel + totalCombustivel;
                        string TotalCombustivel = calculadoCombustivel.ToString("F2").Replace(",", ".");

                        var update = $@"UPDATE [PRIPVEIGA].[dbo].AD_Viaturas
                            SET TotalCombustivel = {TotalCombustivel}
                            WHERE IdMatricula = '{matricula}'";
                        BSO.DSO.ExecuteSQL(update);


                        // AVISO (somente se a matrícula ainda não tiver sido avisada)
                        if (!matriculasComAviso.Contains(matricula))
                        {
                            var query = $"SELECT TOP 1 * FROM [PRIPVEIGA].[dbo].AD_RegistrosManutencao where IdMatricula = '{matricula}'";
                            var viatura = BSO.Consulta(query);
                            var numvia = viatura.NumLinhas();
                            viatura.Inicio();

                            for (int y = 0; y < numvia; y++)
                            {
                                var quilometros = viatura.DaValor<int>("Quilometros");
                                var dataDoc = this.DocumentoCompra.DataDoc;
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

                if (this.DocumentoCompra.Tipodoc == "DESPV")
                {
                    var numero = this.DocumentoCompra.Linhas.NumItens;
                    for (int i = 1; i <= numero + 1; i++)
                    {
                        var linha = this.DocumentoCompra.Linhas.GetEdita(i);
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

                        var totalDespesa = this.DocumentoCompra.Linhas.GetEdita(i).PrecUnit * this.DocumentoCompra.Linhas.GetEdita(i).Quantidade;
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
                                var dataDoc = this.DocumentoCompra.DataDoc;
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
            catch (Exception ex)
            {
            }

        }

        public override void TeclaPressionada(int KeyCode, int Shift, ExtensibilityEventArgs e)
        {



            var tipodoc = this.DocumentoCompra.Tipodoc;
            if (tipodoc == "COMBV")
            {
                var linhas = this.DocumentoCompra.Linhas.NumItens;
                for (int i = 1; i < linhas + 1; i++)
                {
                    var linha = this.DocumentoCompra.Linhas.GetEdita(i);
                    if (linha.Artigo != "")
                    {

                        if (KeyCode == 77 && Shift == 2)
                        {
                            MatriculasCompras matriculasCompras = new MatriculasCompras(BSO, this.DocumentoCompra);
                            matriculasCompras.ShowDialog();
                            atualiza();
                            break;
                        }
                    }
                }
                if (KeyCode == 77 && Shift == 2)
                {

                }
                else
                {
                    FormMenu menu = new FormMenu(BSO, PSO);
                    menu.Show();
                }
            }
            else
            {
                FormMenu menu = new FormMenu(BSO, PSO);
                menu.Show();
            }
        }
        private void atualiza()
        {
            var linhas = this.DocumentoCompra.Linhas.NumItens;

            for (global::System.Int32 i = 1; i < linhas + 1; i++)
            {

                var linha = this.DocumentoCompra.Linhas.GetEdita(i);

                if (linha.Artigo != "")
                {
                    var campo = linha.CamposUtil["CDU_Matricula"].Valor.ToString();
                    var campoUltimo = linha.CamposUtil["CDU_UltimoKms"].Valor.ToString();

                    if (campo != "")
                    {
                        var iddoc = this.DocumentoCompra.ID;
                        var datadoc = this.DocumentoCompra.DataDoc;
                        var datadocFormatted = Convert.ToDateTime(datadoc).ToString("yyyy-MM-dd HH:mm:ss");
                        var query2 = $@"
    SELECT TOP 1 CDU_UltimoKms, * 
    FROM LinhasCompras 
    WHERE CDU_Matricula = '{campo}' 
          AND DataDoc < '{datadocFormatted}'
    ORDER BY DataDoc DESC";

                        var query = $@"SELECT * FROM [PRIPVEIGA].[dbo].AD_Viaturas WHERE IdMatricula = '{campo}'";

                        var veiculo = BSO.Consulta(query2);
                        var num = veiculo.NumLinhas();


                        var veiculo2 = BSO.Consulta(query);

                        if (num != 0)
                        {
                            var kmsAtuais = veiculo.DaValor<int>("CDU_Kms");
                            linha.CamposUtil["CDU_UltimoKms"].Valor = kmsAtuais;
                        }
                        else
                        {
                            var kmsAtuais = veiculo2.DaValor<int>("KMActuais");
                            linha.CamposUtil["CDU_UltimoKms"].Valor = kmsAtuais;
                        }




                    }
                }

            }
        }
        public override void ValidaLinha(int NumLinha, ExtensibilityEventArgs e)
        {
            atualiza();
        }
    }
}