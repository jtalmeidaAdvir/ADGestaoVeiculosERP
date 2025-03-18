using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ADGestaoVeiculosERP
{
    public partial class FormMenu : Form
    {
        private ErpBS100.ErpBS BSO;
        StdPlatBS100.StdBSInterfPub PSO;

        public FormMenu(ErpBS100.ErpBS bSO, StdPlatBS100.StdBSInterfPub pSO)
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.Manual;

            Screen screen = Screen.FromPoint(Cursor.Position);
            Rectangle area = screen.WorkingArea;

            // Calcula a posição para o canto superior direito
            int x = area.Right - this.Width;  // Lado direito do monitor
            int y = area.Top + 155;                 // Topo do monitor

            int height = area.Bottom - y;

            this.Location = new Point(x, y);
            this.Height = height;




            BSO = bSO;
            PSO = pSO;
            this.Load += Menu_Load;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CriarViaturas criarViaturaForm = new CriarViaturas(BSO, PSO, ""); // Cria uma instância do formulário CriarViatura
            criarViaturaForm.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var numeroVeiculosAtrasados = GetVeiculosAtrasados();
            button3.Text = $"Atrasos em Manutenção ({numeroVeiculosAtrasados})";
            ListaVeiculosAlerta criarViaturaForm = new ListaVeiculosAlerta(BSO, PSO); // Cria uma instância do formulário CriarViatura
            criarViaturaForm.ShowDialog();
        }
        private void Menu_Load(object sender, EventArgs e)
        {
            var numeroVeiculosAtrasados = GetVeiculosAtrasados();
            button3.Text = $"Atrasos em Manutenção ({numeroVeiculosAtrasados})"; // Chama o método para atualizar o botão com os atrasos
        }

        private int GetVeiculosAtrasados()
        {
            int numeroViaturasAtrasadas = 0;

            try
            {
                string queryViaturas = "SELECT * FROM [PRIPVEIGA].[dbo].AD_Viaturas WHERE Activo = 1";
                var resultadoViaturas = BSO.Consulta(queryViaturas);

                if (resultadoViaturas == null || resultadoViaturas.NumLinhas() == 0)
                    return numeroViaturasAtrasadas;

                string queryRegistros = "SELECT * FROM [PRIPVEIGA].[dbo].AD_RegistrosManutencao ORDER BY DataEvento DESC";
                var resultadoRegistros = BSO.Consulta(queryRegistros);

                if (resultadoRegistros == null || resultadoRegistros.NumLinhas() == 0)
                    return numeroViaturasAtrasadas;

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
                                bool atrasoKM = kilometrosAtual > manutencaoKM && manutencaoKM > 0;
                                bool atrasoData = dataManutencao <= DateTime.Now && dataManutencao != DateTime.MinValue;

                                if (atrasoKM || atrasoData)
                                {
                                    numeroViaturasAtrasadas++;
                                    break;
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

            return numeroViaturasAtrasadas;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ListaViaturas criarViaturaForm = new ListaViaturas(BSO, PSO); // Cria uma instância do formulário CriarViatura
            criarViaturaForm.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ListaCusto criarViaturaForm = new ListaCusto(BSO, PSO, ""); // Cria uma instância do formulário CriarViatura
            criarViaturaForm.Show();
        }
    }
}
