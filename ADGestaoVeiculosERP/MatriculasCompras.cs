using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;

namespace ADGestaoVeiculosERP
{
    public partial class MatriculasCompras : Form
    {
        private ErpBS100.ErpBS BSO;
        private CmpBE100.CmpBEDocumentoCompra DocumentoCompra;
        private List<string> matriculas;  // Lista para armazenar as matrículas

        public MatriculasCompras(ErpBS100.ErpBS bSO, CmpBE100.CmpBEDocumentoCompra documentoCompra)
        {
            InitializeComponent();
            BSO = bSO;
            DocumentoCompra = documentoCompra;
            this.Load += new EventHandler(MatriculasCompras_Load);
        }

        private async void MatriculasCompras_Load(object sender, EventArgs e)
        {
            await CarregarDadosAsync(); // Carregar os dados de forma assíncrona
        }

        // Carregar dados de forma assíncrona para manter o UI responsivo
        private async Task CarregarDadosAsync()
        {
            // Realiza a consulta ao banco de dados de forma assíncrona
            var query = "SELECT IdMatricula FROM [PRIPVEIGA].[dbo].AD_Viaturas WHERE Activo = 1";
            var lista = await Task.Run(() => BSO.Consulta(query));

            // Inicializa a lista de matrículas
            matriculas = new List<string>();

            var num = lista.NumLinhas();
            lista.Inicio();

            // Carrega todas as matrículas em uma lista
            for (int i = 0; i < num; i++)
            {
                matriculas.Add(lista.DaValor<string>("IdMatricula"));
                lista.Seguinte();
            }

            // Preenche o ComboBox com as matrículas após o carregamento completo
            comboBox1.Items.AddRange(matriculas.ToArray());
        }

        // Método para inserção otimizada
        private void btInserir_Click(object sender, EventArgs e)
        {
            // Evitar calcular o número de linhas em cada iteração
            var linhas = this.DocumentoCompra.Linhas.NumItens;

            // Verificar se a matrícula foi selecionada de forma segura
            var matriculaComboboxSelecionado = comboBox1.SelectedItem as string;
            if (string.IsNullOrEmpty(matriculaComboboxSelecionado))
            {
                MessageBox.Show("Selecione uma matrícula para inserir.");
                return;
            }

            // Loop para atualizar todas as linhas do documento
            for (int i = 1; i <= linhas; i++)
            {
                var linha = this.DocumentoCompra.Linhas.GetEdita(i);
                if (!string.IsNullOrEmpty(linha.Artigo))  // Verifica se o artigo não é vazio
                {
                    linha.CamposUtil["CDU_Matricula"].Valor = matriculaComboboxSelecionado;
                }
            }

            this.Close(); // Fecha o formulário após a inserção
        }
    }
}
