using ErpBS100;
using StdPlatBS100;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static ADGestaoVeiculosERP.ListaCusto;
using Excel = Microsoft.Office.Interop.Excel;
namespace ADGestaoVeiculosERP
{
    public partial class ListaCusto : Form
    {

        public class Fatura
        {
            public string Matricula { get; set; }  // Alterado de NumDoc para Matricula
            public List<Viatura> Viaturas { get; set; }
            public decimal CustoTotal { get; set; }
        }

        // Classe para representar as viaturas
        public class Viatura
        {
            public string Matricula { get; set; }
            public string Artigo { get; set; }
            public string Descricao { get; set; }
            public decimal PrecUnit { get; set; }
            public int Quantidade { get; set; }
            public string DataDoc { get; set; }
            public decimal PrecoLiquido { get; set; }
            public string TipoDoc { get; set; }
            public string NumDoc { get; set; }
        }


        private ErpBS bSO;
        private StdBSInterfPub pSO;
        private readonly string _matricula;

        public ListaCusto(ErpBS bSO, StdBSInterfPub pSO, string matricula)
        {
            InitializeComponent();
            this.bSO = bSO;
            this.pSO = pSO;
            this._matricula = matricula;
            // Adicionar eventos à ComboBox e TextBox
            datePickerInicio.ValueChanged += FiltroAlterado;
            datePickerFim.ValueChanged += FiltroAlterado;
            TB_Matricula.TextChanged += TextBoxFiltro_TextChanged;

            // Inicializa a lista com a data padrão (exemplo: últimos 30 dias)
            datePickerInicio.Value = new DateTime(DateTime.Today.Year, 1, 1);

            datePickerFim.Value = DateTime.Today;
            if (!string.IsNullOrEmpty(_matricula))
            {
                TB_Matricula.Text = _matricula;
                GetValoresParaLista(datePickerInicio.Value, datePickerFim.Value, _matricula);
            }
            else
            {
                GetValoresParaLista(datePickerInicio.Value, datePickerFim.Value, TB_Matricula.Text);
            }
            // Inicializa a lista com a série padrão (ou a primeira série disponível)
            
        }

        private void FiltroAlterado(object sender, EventArgs e)
        {
            // Chama a função para filtrar com base na data e matrícula
            GetValoresParaLista(datePickerInicio.Value, datePickerFim.Value, TB_Matricula.Text);
        }

        private void TextBoxFiltro_TextChanged(object sender, EventArgs e)
        {


            // Obter valores dos filtros
            string matricula = TB_Matricula.Text;
            DateTime dataInicio = datePickerInicio.Value.Date;
            DateTime dataFim = datePickerFim.Value.Date;

            // Chamar o método de filtragem passando a matrícula e intervalo de datas
            GetValoresParaLista(dataInicio, dataFim, matricula);
        }

        private void GetValoresParaLista(DateTime dataInicio, DateTime dataFim, string matricula)
        {
            List<Fatura> faturas = ObterFaturas(dataInicio, dataFim, matricula);

            // Configuração do DataGridView
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = false;

            // Adicionar as colunas do DataGridView e configurar o nome das colunas
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Matrícula", DataPropertyName = "Matricula", Name = "Matricula" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Artigo", DataPropertyName = "Artigo", Name = "Artigo" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Descrição", DataPropertyName = "Descricao", Name = "Descricao" });  
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "PrecUnit (€)", DataPropertyName = "PrecUnit", Name = "PrecUnit" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Quantidade", DataPropertyName = "Quantidade", Name = "Quantidade" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Preço Líquido (€)", DataPropertyName = "PrecoLiquido", Name = "PrecoLiquido" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Data Doc", DataPropertyName = "DataDoc", Name = "DataDoc" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tipo Doc", DataPropertyName = "TipoDoc", Name = "TipoDoc" });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Num Doc", DataPropertyName = "NumDoc", Name = "NumDoc" });

            // Criar uma lista plana para exibir no DataGridView
            List<object> listaFlat = new List<object>();

            decimal totalGeral = 0.0m; // Variável para armazenar o total geral de todos os agrupamentos
            decimal totalPrecoUnitarioGeral = 0.0m; // Total do preço unitário geral
            int totalQuantidadeGeral = 0; // Total geral das quantidades

            foreach (var fatura in faturas)
            {
                // Inicializar os totais para esta fatura
                decimal totalPrecoLiquidoFatura = 0.0m;
                decimal totalPrecoUnitarioFatura = 0.0m;
                int totalQuantidadeFatura = 0;

                listaFlat.Add(new { Matricula = fatura.Matricula, Artigo = "", PrecUnit = 0.0m, DataDoc = "", Quantidade = 0, PrecoLiquido = 0.0m, Descricao = "", TipoDoc = "", NumDoc = "" });

                // Adiciona as viaturas dentro da fatura e calcula os totais para esta fatura
                foreach (var viatura in fatura.Viaturas)
                {
                    listaFlat.Add(new { Matricula = "", Artigo = viatura.Artigo, PrecUnit = viatura.PrecUnit, DataDoc = viatura.DataDoc, Quantidade = viatura.Quantidade, PrecoLiquido = viatura.PrecoLiquido, Descricao = viatura.Descricao, TipoDoc = viatura.TipoDoc, NumDoc = viatura.NumDoc });
                    totalPrecoLiquidoFatura += viatura.PrecoLiquido;
                    totalPrecoUnitarioFatura += viatura.PrecUnit * viatura.Quantidade;
                    totalQuantidadeFatura += viatura.Quantidade;
                }

                // Adicionar a linha de total para esta fatura (Subtotal)
                listaFlat.Add(new { Matricula = "Subtotal", Artigo = "", PrecUnit = totalPrecoUnitarioFatura, DataDoc = "", Quantidade = totalQuantidadeFatura, PrecoLiquido = totalPrecoLiquidoFatura, Descricao = "", TipoDoc = "", NumDoc = "" });

                // Atualizar o total geral
                totalGeral += totalPrecoLiquidoFatura;
                totalPrecoUnitarioGeral += totalPrecoUnitarioFatura;
                totalQuantidadeGeral += totalQuantidadeFatura;
            }

            // Adicionar a linha de total geral no final
            listaFlat.Add(new { Matricula = "Total", Artigo = "", PrecUnit = totalPrecoUnitarioGeral, DataDoc = "", Quantidade = totalQuantidadeGeral, PrecoLiquido = totalGeral, Descricao = "", TipoDoc = "", NumDoc = "" });

            // Definir a fonte de dados para o DataGridView
            dataGridView1.DataSource = listaFlat;

            // Assinar o evento RowPostPaint para aplicar a formatação
            dataGridView1.RowPostPaint += (sender, e) =>
            {
                // Verificar se a célula "Matricula" contém um valor, o que indica que é uma linha de fatura ou de total
                if (dataGridView1.Rows[e.RowIndex].Cells["Matricula"].Value != null &&
                    !string.IsNullOrEmpty(dataGridView1.Rows[e.RowIndex].Cells["Matricula"].Value.ToString()))
                {
                    // Verificar se é uma linha de total
                    if (dataGridView1.Rows[e.RowIndex].Cells["Matricula"].Value.ToString() == "Subtotal")
                    {
                        // Alterar a aparência para destacar a linha de total da fatura
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.Font = new System.Drawing.Font("Calibri", 10, System.Drawing.FontStyle.Bold);
                    }
                    else if (dataGridView1.Rows[e.RowIndex].Cells["Matricula"].Value.ToString() == "Total")
                    {
                        // Alterar a aparência para destacar a linha de total geral
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.Font = new System.Drawing.Font("Calibri", 10, System.Drawing.FontStyle.Bold);
                    }
                    else
                    {
                        // Definir o fundo como cinza e o texto como branco para faturas
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.Gray;

                        dataGridView1.Rows[e.RowIndex].Cells[0].Style.ForeColor = System.Drawing.Color.White;
                        for (int colIndex = 1; colIndex < dataGridView1.Columns.Count; colIndex++)
                        {
                            dataGridView1.Rows[e.RowIndex].Cells[colIndex].Style.ForeColor = System.Drawing.Color.Gray;
                        }
                    }
                }
            };
        }

        private List<Fatura> ObterFaturas(DateTime dataInicio, DateTime dataFim, string matricula)
        {
            List<Fatura> faturas = new List<Fatura>();

            try
            {
                string dataInicioStr = dataInicio.ToString("yyyy-MM-dd");
                string dataFimStr = dataFim.ToString("yyyy-MM-dd");


                // Obter faturas
                string queryCabec = $@"
                        SELECT Id, NumDoc 
                        FROM CabecCompras 
                        WHERE TipoDoc IN ('COMBV', 'VFA', 'DESPV')
                        AND DataDoc BETWEEN '{dataInicioStr}' AND '{dataFimStr}'";
                var listaCabec = bSO.Consulta(queryCabec);
                if (listaCabec.NumLinhas() == 0) return faturas; // Sem registros

                Dictionary<string, string> dicNumDoc = new Dictionary<string, string>();
                listaCabec.Inicio();
                while (!listaCabec.NoFim())
                {
                    string idCabec = listaCabec.DaValor<string>("Id");
                    string numDocDB = listaCabec.DaValor<string>("NumDoc");
                    dicNumDoc[idCabec] = numDocDB;
                    listaCabec.Seguinte();
                }

                string idsCabecStr = string.Join(",", dicNumDoc.Keys.Select(id => $"'{id}'"));
                string queryLinhas = $@"
                                    SELECT 
                                    lc.CDU_Matricula, 
                                    lc.Artigo, 
                                    lc.IdCabecCompras, 
                                    lc.PrecUnit, 
                                    lc.DataDoc, 
                                    lc.Quantidade, 
                                    lc.PrecoLiquido, 
                                    lc.Descricao,  
                                    cc.TipoDoc, 
                                    cc.NumDoc
                                FROM LinhasCompras lc
                                JOIN CabecCompras cc ON lc.IdCabecCompras = cc.Id
                                WHERE lc.CDU_Matricula IS NOT NULL 
                                AND lc.IdCabecCompras IN ({idsCabecStr})";

                // Filtrar por matrícula, se necessário
                if (!string.IsNullOrEmpty(matricula))
                {
                    queryLinhas += $" AND  lc.CDU_Matricula LIKE '%{matricula}%'";
                }

                var listaLinhas = bSO.Consulta(queryLinhas);
                listaLinhas.Inicio();
                // Agrupar as linhas de compra por Matrícula
                while (!listaLinhas.NoFim())
                {
                    string matriculaFromDB = listaLinhas.DaValor<string>("CDU_Matricula");

                    var fatura = faturas.FirstOrDefault(f => f.Matricula == matriculaFromDB);
                    if (fatura == null)
                    {
                        fatura = new Fatura
                        {
                            Matricula = matriculaFromDB,
                            Viaturas = new List<Viatura>()
                        };
                        faturas.Add(fatura);
                    }

                    // Obter a descrição do artigo
                    string artigo = listaLinhas.DaValor<string>("Artigo");
                    string desc = listaLinhas.DaValor<string>("Descricao");
                    string descricaoQuery = $"SELECT Descricao FROM Artigo WHERE Artigo = '{artigo}'";
                    var descricao = bSO.Consulta(descricaoQuery);
                    string descricaoArtigo = descricao.NumLinhas() > 0 ? descricao.DaValor<string>("Descricao") : string.Empty;
                    var tipoDoc = listaLinhas.DaValor<string>("TipoDoc");
                    var numDoc = listaLinhas.DaValor<string>("NumDoc");
                    // Adicionar a viatura com a descrição
                    fatura.Viaturas.Add(new Viatura
                    {
                        Matricula = matriculaFromDB,
                        Artigo = artigo,
                        Descricao = desc, 
                        PrecUnit = Math.Abs(listaLinhas.DaValor<decimal>("PrecUnit")),
                        DataDoc = listaLinhas.DaValor<DateTime>("DataDoc").ToString("dd/MM/yyyy"),
                        Quantidade = Math.Abs(listaLinhas.DaValor<int>("Quantidade")),
                        PrecoLiquido = Math.Abs(listaLinhas.DaValor<decimal>("PrecoLiquido")),
                        TipoDoc = tipoDoc,
                        NumDoc = numDoc,
                    });

          
                    listaLinhas.Seguinte();
                }

                // Agora, adicionar as despesas
                string queryDespesas = $@"
            SELECT NumViatura, Artigo, CodTipoDespesa, Data, Valor
            FROM [PRIPVEIGA].[dbo].AD_F3MDespesas
            WHERE Data BETWEEN '{dataInicioStr}' AND '{dataFimStr}'";

                var listaDespesas = bSO.Consulta(queryDespesas);
                listaDespesas.Inicio();

                // Agrupar as despesas por Matrícula
                while (!listaDespesas.NoFim())
                {
                    string matriculaFromDB = listaDespesas.DaValor<string>("NumViatura");

                    // Procurar a fatura pela matrícula
                    var fatura = faturas.FirstOrDefault(f => f.Matricula == matriculaFromDB);
                    if (fatura == null)
                    {

                        fatura = new Fatura
                        {
                            Matricula = matriculaFromDB,
                            Viaturas = new List<Viatura>()
                        };
                        faturas.Add(fatura);
                    }

                    // Adicionar a despesa à fatura
                    fatura.Viaturas.Add(new Viatura
                    {
                        Matricula = matriculaFromDB,
                        Artigo = listaDespesas.DaValor<string>("Artigo"),
                        Descricao = listaDespesas.DaValor<string>("CodTipoDespesa"),
                        PrecUnit = 0,  // A despesa não tem preço unitário
                        DataDoc = listaDespesas.DaValor<DateTime>("Data").ToString("dd/MM/yyyy"),
                        Quantidade = 1,  // A quantidade é 1 por cada despesa
                        PrecoLiquido = listaDespesas.DaValor<decimal>("Valor")
                    });

                    listaDespesas.Seguinte();
                }



                // Calcular o custo total por fatura
                foreach (var fatura in faturas)
                {
                    fatura.CustoTotal = fatura.Viaturas.Sum(v => v.PrecUnit);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao obter os dados: " + ex.Message);
            }

            return faturas;
        }


        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Não há dados para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Salvar como",
                FileName = "Lista de Custos por Viatura.xlsx"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filePath = saveFileDialog.FileName;

                    // Verifique se o arquivo já existe
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath); // Deletar o arquivo se já existir
                    }

                    // Criar uma aplicação Excel
                    Excel.Application excelApp = new Excel.Application();
                    excelApp.Visible = false;

                    // Criar um novo workbook
                    Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);
                    Excel.Worksheet worksheet = (Excel.Worksheet)workbook.ActiveSheet;
                    worksheet.Name = "Lista de Custos por Viatura";

                    // Escrever os cabeçalhos
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        worksheet.Cells[1, i + 1] = dataGridView1.Columns[i].HeaderText;
                    }

                    // Escrever os dados e aplicar a formatação
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGridView1.Columns.Count; j++)
                        {
                            if (dataGridView1.Rows[i].Cells[j].Value != null)
                            {
                                worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                            }
                        }


                        string matriculaValue = worksheet.Cells[i + 2, 1].Value?.ToString();
                        // Escrever os cabeçalhos e formatar a primeira célula do cabeçalho
                        for (int y = 0; y < dataGridView1.Columns.Count; y++)
                        {
                            worksheet.Cells[1, y + 1] = dataGridView1.Columns[y].HeaderText;

                            if (i == 0) // Primeira célula do cabeçalho (coluna "Matrícula")
                            {
                                worksheet.Cells[1, y + 1].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow); // Cor de fundo
                                worksheet.Cells[1, y + 1].Font.Bold = true; // Texto em negrito
                            }
                        }
                        if (!string.IsNullOrEmpty(matriculaValue) && matriculaValue.Contains("-"))
                        {
                            for (int j = 1; j <= 7; j++) 
                            {
                                if(j == 1)
                                {
                                    worksheet.Cells[i + 2, j].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                                }
                                else
                                {
                                    worksheet.Cells[i + 2, j].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkGray);
                                }
                                worksheet.Cells[i + 2, j].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkGray);
                                
                            }
                        }
                        else if (matriculaValue == "Subtotal")
                        {
                            for (int j = 1; j <= 7; j++)  
                            {
                                worksheet.Cells[i + 2, j].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                                worksheet.Cells[i + 2, j].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            }
                        }
                        else if (matriculaValue == "Total")
                        {
                            for (int j = 1; j <= 7; j++) 
                            {
                                worksheet.Cells[i + 2, j].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightBlue);
                                worksheet.Cells[i + 2, j].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                            }
                        }
                    }

                    // Salvar o arquivo
                    workbook.SaveAs(filePath);
                    workbook.Close();
                    excelApp.Quit();

                    MessageBox.Show("Exportação concluída com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao exportar: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


    }
}
