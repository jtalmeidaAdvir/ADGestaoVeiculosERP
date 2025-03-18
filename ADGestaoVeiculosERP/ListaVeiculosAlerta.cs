using ErpBS100;
using StdBE100;
using StdPlatBS100;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ADGestaoVeiculosERP
{
    public partial class ListaVeiculosAlerta : Form
    {
        public class ViaturaAtrasada
        {
            public string Numero { get; set; }
            public string Descricao { get; set; }
            public string Matricula { get; set; }
            public int KMAtual { get; set; }
            public int? ManutencaoKM { get; set; }
            public DateTime? ProximaInspecao { get; set; }
            
        }

        ErpBS BSO;
        StdBSInterfPub PSO;
        public StdBELista ListVeiculos { get; set; }

        public ListaVeiculosAlerta(ErpBS100.ErpBS bSO, StdBSInterfPub pSO)
        {
            InitializeComponent();
            BSO = bSO;
            PSO = pSO;


            // Inicializar e configurar o ContextMenuStrip
            contextMenuStrip1 = new ContextMenuStrip();
            var menuItemDrillDown = new ToolStripMenuItem("Drill Down");
            menuItemDrillDown.Click += MenuItemDrillDown_Click;
            contextMenuStrip1.Items.Add(menuItemDrillDown);

            // Atribuindo o ContextMenuStrip à DataGridView
            dataGridView1.ContextMenuStrip = contextMenuStrip1;

            ExibirViaturasAtrasadas();
        }
        // Quando clicar com o botão direito na DataGridView


        private void MenuItemDrillDown_Click(object sender, EventArgs e)
        {
            // Verifique se há linhas selecionadas antes de acessar
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Obtém a linha selecionada
                var linhaSelecionada = dataGridView1.SelectedRows[0];

                // Pega os dados da linha
                string numero = linhaSelecionada.Cells["Numero"].Value.ToString();
                string descricao = linhaSelecionada.Cells["Descricao"].Value.ToString();
                string matricula = linhaSelecionada.Cells["Matricula"].Value.ToString();
                int kmAtual = Convert.ToInt32(linhaSelecionada.Cells["KMAtual"].Value);
                int? ultimaManutencaoKM = linhaSelecionada.Cells["ManutencaoKM"].Value as int?;
                DateTime? proximaInspecao = linhaSelecionada.Cells["ProximaInspecao"].Value as DateTime?;



                this.Hide();
                // Display the details or perform a drilldown action
                CriarViaturas criarViaturaForm = new CriarViaturas(BSO, PSO, matricula);
                criarViaturaForm.ShowDialog();
                this.Close();

            }
            else
            {
                // Caso não tenha nenhuma linha selecionada
                MessageBox.Show("Nenhuma linha selecionada. Por favor, selecione uma linha para o Drill Down.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ExibirViaturasAtrasadas()
        {
            List<ViaturaAtrasada> viaturasAtrasadas = ObterViaturasAtrasadas();

            if (viaturasAtrasadas.Count > 0)
            {
                dataGridView1.DataSource = viaturasAtrasadas;  // Aqui, dataGridView1 é o seu DataGridView

                dataGridView1.DataBindingComplete += (sender, e) =>
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["KMAtual"].Value != null && row.Cells["ManutencaoKM"].Value != null)
                        {
                            // Valida se os valores são inteiros
                            if (int.TryParse(row.Cells["KMAtual"].Value.ToString(), out int kmAtual) &&
                                int.TryParse(row.Cells["ManutencaoKM"].Value.ToString(), out int manutencaoKM))
                            {
                                // Verifica se ManutencaoKM é maior que 0 antes de aplicar a cor
                                if (manutencaoKM > 0 && kmAtual > manutencaoKM) // Destaca em vermelho se KMAtual for maior e ManutencaoKM não for 0
                                {
                                    row.DefaultCellStyle.BackColor = Color.Red;
                                    row.DefaultCellStyle.ForeColor = Color.White; // Melhora a visibilidade
                                }
                                else
                                {
                                    // Caso contrário, remove a cor (caso o comportamento padrão seja diferente)
                                    row.DefaultCellStyle.BackColor = Color.White;
                                    row.DefaultCellStyle.ForeColor = Color.Black;
                                }
                            }
                        }
                    }
                };


            }
            else
            {
                MessageBox.Show("Nenhuma viatura está com manutenção ou inspeção atrasada.", "Tudo em Ordem", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }



        private List<ViaturaAtrasada> ObterViaturasAtrasadas()
        {
            List<ViaturaAtrasada> viaturasAtrasadas = new List<ViaturaAtrasada>();

            try
            {
                // Buscar todas as viaturas ativas
                string queryViaturas = "SELECT * FROM [PRIPVEIGA].[dbo].AD_Viaturas WHERE Activo = 1";
                var resultadoViaturas = BSO.Consulta(queryViaturas);

                if (resultadoViaturas == null || resultadoViaturas.NumLinhas() == 0)
                    return viaturasAtrasadas;

                // Buscar todas as manutenções ordenadas por data (mais recentes primeiro)
                string queryRegistros = "SELECT * FROM [PRIPVEIGA].[dbo].AD_RegistrosManutencao ORDER BY DataEvento DESC";
                var resultadoRegistros = BSO.Consulta(queryRegistros);

                if (resultadoRegistros == null || resultadoRegistros.NumLinhas() == 0)
                    return viaturasAtrasadas;

                // Criar um dicionário para armazenar os registros de manutenção por viatura
                Dictionary<string, List<dynamic>> registrosPorViatura = new Dictionary<string, List<dynamic>>();

                resultadoRegistros.Inicio();
                for (int i = 0; i < resultadoRegistros.NumLinhas(); i++)
                {
                    string idMatricula = resultadoRegistros.DaValor<string>("IdMatricula");
                    int km = resultadoRegistros.DaValor<int>("Quilometros");
                    DateTime dataEvento = resultadoRegistros.DaValor<DateTime>("DataEvento");
                    string descricao = resultadoRegistros.DaValor<string>("Descricao");

                    if (!registrosPorViatura.ContainsKey(idMatricula))
                        registrosPorViatura[idMatricula] = new List<dynamic>();

                    registrosPorViatura[idMatricula].Add(new { km, dataEvento, descricao });

                    resultadoRegistros.Seguinte();
                }

                // Percorrer todas as viaturas e verificar se há atraso
                resultadoViaturas.Inicio();
                for (int i = 0; i < resultadoViaturas.NumLinhas(); i++)
                {
                    try
                    {
                        string idMatricula = resultadoViaturas.DaValor<string>("IdMatricula");
                        int kilometrosAtual = resultadoViaturas.DaValor<int>("KMActuais");
                        string codigo = resultadoViaturas.DaValor<string>("Codigo");

                        if (registrosPorViatura.ContainsKey(idMatricula))
                        {
                            var registros = registrosPorViatura[idMatricula];

                            foreach (var registro in registros)
                            {
                                int manutencaoKM = registro.km;
                                DateTime dataManutencao = registro.dataEvento;
                                string descricao = registro.descricao;

                                // Critério de atraso por quilometragem
                                bool atrasoKM = kilometrosAtual >= manutencaoKM;
                                
                                // Critério de atraso por data (somente se já passou da data atual)
                                bool atrasoData = dataManutencao <= DateTime.Now;
                                if (atrasoData)
                                {
                          
                                    if(dataManutencao.ToString() != "01-01-0001 00:00:00")
                                    {
               
                                        viaturasAtrasadas.Add(new ViaturaAtrasada
                                        {
                                            Numero = codigo,
                                            Descricao = descricao,
                                            Matricula = idMatricula,
                                            KMAtual = kilometrosAtual,
                                            ManutencaoKM = manutencaoKM,
                                            ProximaInspecao = dataManutencao
                                        });
                                    }

                                }
                                if (atrasoKM )
                                {
                                    if(manutencaoKM.ToString() != "0")
                                    {
                                        if(kilometrosAtual > manutencaoKM)
                                        {
                                            viaturasAtrasadas.Add(new ViaturaAtrasada
                                            {
                                                Numero = codigo,
                                                Descricao = descricao,
                                                Matricula = idMatricula,
                                                KMAtual = kilometrosAtual,
                                                ManutencaoKM = manutencaoKM,
                                                ProximaInspecao = dataManutencao
                                            });
                                        }
    
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao processar a viatura {resultadoViaturas.DaValor<string>("IdMatricula")}: {ex.Message}",
                                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    resultadoViaturas.Seguinte();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao consultar as viaturas: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return viaturasAtrasadas;
        }







        private void SetupDataGridView()
        {
            // Limpa as colunas antigas, se houver
            dataGridView1.Columns.Clear();

            // Adiciona novas colunas
            dataGridView1.Columns.Add("Numero", "ID Numero");
            dataGridView1.Columns.Add("Descricao", "descricao");
            dataGridView1.Columns.Add("Matricula", "ID Viatura");
            dataGridView1.Columns.Add("KMAtual", "KM Atual");
            dataGridView1.Columns.Add("ManutencaoKM", "Última Manutenção (KM)");
            dataGridView1.Columns.Add("ProximaInspecao", "Próxima Inspeção");

            // Configura o estilo, se necessário
            dataGridView1.AutoResizeColumns();
        }


        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                // If right-click
                if (e.Button == MouseButtons.Right)
                {
                    // Select the row
                    dataGridView1.Rows[e.RowIndex].Selected = true;
                }
            }
            catch { }
        }
    }
}
