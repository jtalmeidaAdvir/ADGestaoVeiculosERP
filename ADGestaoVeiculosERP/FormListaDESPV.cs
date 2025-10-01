
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
    public partial class FormListaDESPV : Form
    {
        private ErpBS100.ErpBS BSO;
        private StdPlatBS100.StdBSInterfPub PSO;
        private DataGridView dataGridViewDocumentos;
        private DataGridView dataGridViewLinhas;
        private ComboBox comboBoxMatricula;
        private Button buttonPesquisar;
        private Button buttonDetalhes;

        public FormListaDESPV(string text, ErpBS100.ErpBS bSO, StdPlatBS100.StdBSInterfPub pSO, string matriculaPredefinida = null)
        {
            InitializeComponent();
            BSO = bSO;
            PSO = pSO;
            this.Text = text;

            InitializeControls();
            CarregarMatriculas();

            // Se foi passada uma matrícula específica, pré-seleciona ela
            if (!string.IsNullOrEmpty(matriculaPredefinida) && comboBoxMatricula.Items.Contains(matriculaPredefinida))
            {
                comboBoxMatricula.SelectedItem = matriculaPredefinida;
                CarregarDocumentosDESPV(matriculaPredefinida);
            }
            else
            {
                CarregarDocumentosDESPV();
            }
        }

        private void InitializeControls()
        {
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // ComboBox para matrícula
            Label lblMatricula = new Label()
            {
                Text = "Matrícula:",
                Location = new Point(10, 15),
                Size = new Size(70, 23)
            };

            comboBoxMatricula = new ComboBox()
            {
                Location = new Point(85, 12),
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboBoxMatricula.Items.Add("Todas");
            comboBoxMatricula.SelectedIndex = 0;

            // Botão pesquisar
            buttonPesquisar = new Button()
            {
                Text = "Pesquisar",
                Location = new Point(245, 12),
                Size = new Size(80, 23)
            };
            buttonPesquisar.Click += ButtonPesquisar_Click;

            // Botão detalhes
            buttonDetalhes = new Button()
            {
                Text = "Ver Linhas",
                Location = new Point(335, 12),
                Size = new Size(80, 23)
            };
            buttonDetalhes.Click += ButtonDetalhes_Click;

            // DataGridView para documentos
            Label lblDocumentos = new Label()
            {
                Text = "Documentos DESPV:",
                Location = new Point(10, 50),
                Size = new Size(150, 20)
            };

            dataGridViewDocumentos = new DataGridView()
            {
                Location = new Point(10, 75),
                Size = new Size(1170, 250),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            // DataGridView para linhas
            Label lblLinhas = new Label()
            {
                Text = "Linhas do Documento:",
                Location = new Point(10, 340),
                Size = new Size(150, 20)
            };

            dataGridViewLinhas = new DataGridView()
            {
                Location = new Point(10, 365),
                Size = new Size(1170, 290),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Adicionar controles ao formulário
            this.Controls.AddRange(new Control[] {
                lblMatricula, comboBoxMatricula, buttonPesquisar, buttonDetalhes,
                lblDocumentos, dataGridViewDocumentos,
                lblLinhas, dataGridViewLinhas
            });
        }

        private void CarregarMatriculas()
        {
            try
            {
                string queryMatriculas = @"
                    SELECT DISTINCT CDU_Matricula 
                    FROM LinhasCompras 
                    WHERE CDU_Matricula IS NOT NULL 
                    ORDER BY CDU_Matricula";

                var resultadoMatriculas = BSO.Consulta(queryMatriculas);

                resultadoMatriculas.Inicio();
                while (!resultadoMatriculas.NoFim())
                {
                    string matricula = resultadoMatriculas.DaValor<string>("CDU_Matricula");
                    if (!string.IsNullOrEmpty(matricula))
                    {
                        comboBoxMatricula.Items.Add(matricula);
                    }
                    resultadoMatriculas.Seguinte();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar matrículas: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarDocumentosDESPV(string matriculaFiltro = null)
        {
            try
            {
                string query = @"
                    SELECT 
                        [CabecCompras].[TipoDoc],
                        MAX([CabecCompras].[NumDoc]) as NumDoc,
                        [CabecCompras].[Entidade],
                        [CabecCompras].[DataDoc],
                        [CabecCompras].[TotalMerc],
                        [CabecCompras].[Matricula],
                        [LinhasCompras].[CDU_Matricula],
                        [CabecCompras].[Serie],
                        [CabecCompras].[Id] as IdCabec
                    FROM [CabecCompras] WITH (NOLOCK) 
                    LEFT JOIN [LinhasCompras] WITH (NOLOCK) ON [CabecCompras].[Id] = [LinhasCompras].[IdCabecCompras]
                    WHERE [CabecCompras].[TipoDoc] IN ('DESPV') 
                    AND [LinhasCompras].[CDU_Matricula] IS NOT NULL";

                if (!string.IsNullOrEmpty(matriculaFiltro) && matriculaFiltro != "Todas")
                {
                    query += $" AND [LinhasCompras].[CDU_Matricula] = '{matriculaFiltro}'";
                }

                query += @"
                    GROUP BY 
                        [CabecCompras].[TipoDoc],
                        [CabecCompras].[Entidade],
                        [CabecCompras].[DataDoc],
                        [CabecCompras].[TotalMerc],
                        [CabecCompras].[Matricula],
                        [LinhasCompras].[CDU_Matricula],
                        [CabecCompras].[Serie],
                        [CabecCompras].[Id]
                    ORDER BY [CabecCompras].[DataDoc] DESC";

                var resultado = BSO.Consulta(query);

                DataTable dt = new DataTable();
                dt.Columns.Add("TipoDoc", typeof(string));
                dt.Columns.Add("NumDoc", typeof(string));
                dt.Columns.Add("Entidade", typeof(string));
                dt.Columns.Add("DataDoc", typeof(DateTime));
                dt.Columns.Add("TotalMerc", typeof(decimal));
                dt.Columns.Add("Matricula", typeof(string));
                dt.Columns.Add("CDU_Matricula", typeof(string));
                dt.Columns.Add("Serie", typeof(string));
                dt.Columns.Add("IdCabec", typeof(string));

                if (resultado.NumLinhas() > 0)
                {
                    resultado.Inicio();
                    while (!resultado.NoFim())
                    {
                        dt.Rows.Add(
                            resultado.DaValor<string>("TipoDoc"),
                            resultado.DaValor<string>("NumDoc"),
                            resultado.DaValor<string>("Entidade"),
                            resultado.DaValor<DateTime>("DataDoc"),
                            resultado.DaValor<decimal>("TotalMerc"),
                            resultado.DaValor<string>("Matricula"),
                            resultado.DaValor<string>("CDU_Matricula"),
                            resultado.DaValor<string>("Serie"),
                            resultado.DaValor<string>("IdCabec")
                        );
                        resultado.Seguinte();
                    }
                }

                dataGridViewDocumentos.DataSource = dt;

                // Ocultar a coluna IdCabec
                if (dataGridViewDocumentos.Columns["IdCabec"] != null)
                {
                    dataGridViewDocumentos.Columns["IdCabec"].Visible = false;
                }

                // Configurar cabeçalhos das colunas
                if (dataGridViewDocumentos.Columns["TipoDoc"] != null)
                    dataGridViewDocumentos.Columns["TipoDoc"].HeaderText = "Tipo Doc";
                if (dataGridViewDocumentos.Columns["NumDoc"] != null)
                    dataGridViewDocumentos.Columns["NumDoc"].HeaderText = "Nº Documento";
                if (dataGridViewDocumentos.Columns["DataDoc"] != null)
                    dataGridViewDocumentos.Columns["DataDoc"].HeaderText = "Data Documento";
                if (dataGridViewDocumentos.Columns["TotalMerc"] != null)
                    dataGridViewDocumentos.Columns["TotalMerc"].HeaderText = "Total Mercadoria";
                if (dataGridViewDocumentos.Columns["CDU_Matricula"] != null)
                    dataGridViewDocumentos.Columns["CDU_Matricula"].HeaderText = "Matrícula";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar documentos: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarLinhasDocumento(string idCabec)
        {
            try
            {
                string queryLinhas = $@"
                    SELECT 
                        [LinhasCompras].[Artigo],
                        [LinhasCompras].[Descricao],
                        [LinhasCompras].[Quantidade],
                        [LinhasCompras].[PrecUnit],
                        [LinhasCompras].[PrecoLiquido],
                        [LinhasCompras].[CDU_Matricula],
                        [LinhasCompras].[CDU_Kms]
                    FROM [LinhasCompras] WITH (NOLOCK)
                    WHERE [LinhasCompras].[IdCabecCompras] = '{idCabec}'
                    ORDER BY [LinhasCompras].[NumLinha]";

                var resultadoLinhas = BSO.Consulta(queryLinhas);

                DataTable dtLinhas = new DataTable();
                dtLinhas.Columns.Add("Artigo", typeof(string));
                dtLinhas.Columns.Add("Descricao", typeof(string));
                dtLinhas.Columns.Add("Quantidade", typeof(decimal));
                dtLinhas.Columns.Add("PrecUnit", typeof(decimal));
                dtLinhas.Columns.Add("PrecoLiquido", typeof(decimal));
                dtLinhas.Columns.Add("CDU_Matricula", typeof(string));
                dtLinhas.Columns.Add("CDU_Kms", typeof(decimal));

                if (resultadoLinhas.NumLinhas() > 0)
                {
                    resultadoLinhas.Inicio();
                    while (!resultadoLinhas.NoFim())
                    {
                        dtLinhas.Rows.Add(
                            resultadoLinhas.DaValor<string>("Artigo"),
                            resultadoLinhas.DaValor<string>("Descricao"),
                            resultadoLinhas.DaValor<decimal>("Quantidade"),
                            resultadoLinhas.DaValor<decimal>("PrecUnit"),
                            resultadoLinhas.DaValor<decimal>("PrecoLiquido"),
                            resultadoLinhas.DaValor<string>("CDU_Matricula"),
                            resultadoLinhas.DaValor<decimal>("CDU_Kms")
                        );
                        resultadoLinhas.Seguinte();
                    }
                }

                dataGridViewLinhas.DataSource = dtLinhas;

                // Configurar cabeçalhos das colunas
                if (dataGridViewLinhas.Columns["Descricao"] != null)
                    dataGridViewLinhas.Columns["Descricao"].HeaderText = "Descrição";
                if (dataGridViewLinhas.Columns["PrecUnit"] != null)
                    dataGridViewLinhas.Columns["PrecUnit"].HeaderText = "Preço Unitário";
                if (dataGridViewLinhas.Columns["PrecoLiquido"] != null)
                    dataGridViewLinhas.Columns["PrecoLiquido"].HeaderText = "Preço Líquido";
                if (dataGridViewLinhas.Columns["CDU_Matricula"] != null)
                    dataGridViewLinhas.Columns["CDU_Matricula"].HeaderText = "Matrícula";
                if (dataGridViewLinhas.Columns["CDU_Kms"] != null)
                    dataGridViewLinhas.Columns["CDU_Kms"].HeaderText = "Quilómetros";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar linhas do documento: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonPesquisar_Click(object sender, EventArgs e)
        {
            string matriculaSelecionada = comboBoxMatricula.SelectedItem?.ToString();
            CarregarDocumentosDESPV(matriculaSelecionada);

            // Limpar as linhas quando pesquisar
            dataGridViewLinhas.DataSource = null;
        }

        private void ButtonDetalhes_Click(object sender, EventArgs e)
        {
            if (dataGridViewDocumentos.SelectedRows.Count > 0)
            {
                DataGridViewRow rowSelecionada = dataGridViewDocumentos.SelectedRows[0];
                string idCabec = rowSelecionada.Cells["IdCabec"].Value?.ToString();

                if (!string.IsNullOrEmpty(idCabec))
                {
                    CarregarLinhasDocumento(idCabec);
                }
            }
            else
            {
                MessageBox.Show("Selecione um documento para ver as linhas.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
