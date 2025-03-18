using ErpBS100;
using StdBE100;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ADGestaoVeiculosERP
{
    public partial class ListaViaturas : Form
    {
        ErpBS BSO;
        StdBSInterfPub PSO;
        public StdBELista ListVeiculos { get; set; }
        public ListaViaturas(ErpBS100.ErpBS mBSO, StdPlatBS100.StdBSInterfPub mPSO)
        {
            InitializeComponent();
            BSO = mBSO;
            PSO = mPSO;
            GetVeiculos();
        }



        private void GetVeiculos(string marcaFiltro = null, string modeloFiltro = null, bool? activoFiltro = null)
        {
            Loadform();

            // Começa a construção da consulta SQL com um SELECT básico
            var query = "SELECT * FROM [PRIPVEIGA].[dbo].AD_Viaturas WHERE Activo = 1"; // 1=1 é apenas uma condição sempre verdadeira

            // Adiciona as condições de filtro na consulta, se fornecidas
            if (!string.IsNullOrEmpty(marcaFiltro))
            {
                query += $" AND Marca = '{marcaFiltro}'";
            }

            if (!string.IsNullOrEmpty(modeloFiltro))
            {
                query += $" AND Modelo = '{modeloFiltro}'";
            }

            if (activoFiltro.HasValue)
            {
                query += $" AND Activo = {(activoFiltro.Value ? 1 : 0)}"; // Supondo que "Activo" seja 1 para verdadeiro e 0 para falso
            }
            // Adiciona a ordenação pelo campo 'Codigo', assumindo que é um número
            query += " ORDER BY CAST(Codigo AS INT)";

            ListVeiculos = BSO.Consulta(query);

            // Preenche o DataGridView com os dados filtrados
            for (int i = 0; i < ListVeiculos.NumLinhas(); i++)
            {
                var numero = ListVeiculos.DaValor<string>("Codigo");
                var marca = ListVeiculos.DaValor<string>("Marca");
                var modelo = ListVeiculos.DaValor<string>("Modelo");
                var matricula = ListVeiculos.DaValor<string>("IdMatricula");
                var activa = ListVeiculos.DaValor<bool>("Activo");

                dataGridView1.Rows.Add(numero, marca, modelo, matricula, activa);
                ListVeiculos.Seguinte();
            }
        }


        private void Loadform()
        {
            // Initialize the context menu and add the event handlers
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            // Drill Down option
            ToolStripMenuItem drillDownItem = new ToolStripMenuItem("Drill Down");
            drillDownItem.Click += toolStripMenuItem1_Click;
            contextMenu.Items.Add(drillDownItem);

            // Lista de Custo option
            ToolStripMenuItem listaCustoItem = new ToolStripMenuItem("Lista de Custo");
            listaCustoItem.Click += listaDeCustoToolStripMenuItem_Click;
            contextMenu.Items.Add(listaCustoItem);

            // Attach the context menu to the DataGridView
            dataGridView1.ContextMenuStrip = contextMenu;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the selected row
                var selectedRow = dataGridView1.SelectedRows[0];

            var matricula = selectedRow.Cells["Matricula"].Value.ToString();
            this.Hide();
            // Display the details or perform a drilldown action
            CriarViaturas criarViaturaForm = new CriarViaturas(BSO, PSO, matricula); 
            criarViaturaForm.ShowDialog();
            this.Close();
            }
            catch { }
        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                // If right-click
                if (e.Button == MouseButtons.Right)
                {
                    dataGridView1.ClearSelection();
                    // Select the row
                    dataGridView1.Rows[e.RowIndex].Selected = true;
                }
            }
            catch { }

        }

        private async void listaDeCustoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                // Pega a linha selecionada
                var selectedRow = dataGridView1.SelectedRows[0];

                // Obtém a matrícula do veículo
                var matricula = selectedRow.Cells["Matricula"].Value.ToString();



                // Inicia o processo de abrir o novo formulário em segundo plano
                await Task.Run(() =>
                {
                    // Cria e mostra o formulário ListaCusto
                    var listaCustoForm = new ListaCusto(BSO, PSO, matricula);
                    listaCustoForm.ShowDialog(); // Isso vai bloquear apenas essa thread, mas não a UI
                });

                // Após o fechamento do formulario ListaCustoForm, reabre o formulário atual
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao abrir Lista de Custo: " + ex.Message);
            }
        }



    }
}
