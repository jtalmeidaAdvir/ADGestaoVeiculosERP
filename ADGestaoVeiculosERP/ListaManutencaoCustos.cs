using ErpBS100;
using StdPlatBS100;
using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADGestaoVeiculosERP
{
    public partial class ListaManutencao : Form
    {
        private string _matricula;
        private ErpBS _BSO;
        private StdBSInterfPub _PSO;

        public ListaManutencao(string matricula, ErpBS BSO, StdBSInterfPub PSO)
        {
            InitializeComponent();
            _matricula = matricula;
            _BSO = BSO;
            _PSO = PSO;

            ConfigurarDataGridView();
            Task.Run(() => GetValoresLista());
        }

        private void ConfigurarDataGridView()
        {
            dgvManutencaoCustos.ReadOnly = true;
            dgvManutencaoCustos.AllowUserToAddRows = false;
            dgvManutencaoCustos.RowHeadersVisible = false;
            dgvManutencaoCustos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvManutencaoCustos.CellFormatting += DgvManutencaoCustos_CellFormatting;
        }

        // Novo método para aplicar o filtro ao pesquisar
        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            Task.Run(() => GetValoresLista(txtFiltro.Text)); // Passa o texto do filtro
        }

        private void GetValoresLista(string filtroDescricao = "")
        {
            try
            {
                DataTable dt = CriarTabela();

                // Primeira consulta - LinhasCompras
                string query = $@"
        SELECT L.Artigo, L.Descricao, L.CDU_Kms, L.DataDoc, C.TipoDoc, C.NumDoc
        FROM LinhasCompras L
        INNER JOIN CabecCompras C ON L.IdCabecCompras = C.Id
        WHERE C.TipoDoc IN ('COMBV', 'VFA', 'DESPV') 
        AND L.CDU_Matricula LIKE '%{_matricula}%'
        {(string.IsNullOrEmpty(filtroDescricao) ? "" : $"AND L.Descricao LIKE '%{filtroDescricao}%'")} 
        ORDER BY C.TipoDoc, L.DataDoc DESC";

                var resultado = _BSO.Consulta(query);

                if (resultado.NumLinhas() == 0)
                {
                    dt.Rows.Add(DBNull.Value, "Sem registros", DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);
                }
                else
                {
                    string ultimoTipoDoc = null;

                    resultado.Inicio();
                    while (!resultado.NoFim())
                    {
                        string tipoDoc = resultado.DaValor<string>("TipoDoc");

                        // Adiciona linha separadora se mudar de TipoDoc
                        if (ultimoTipoDoc != tipoDoc)
                        {
                            dt.Rows.Add(DBNull.Value, $"🔹 {tipoDoc}", DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);
                            ultimoTipoDoc = tipoDoc;
                        }

                        dt.Rows.Add(
                            resultado.DaValor<string>("Artigo"),
                            resultado.DaValor<string>("Descricao"),
                            resultado.DaValor<string>("CDU_Kms"),
                            resultado.DaValor<string>("DataDoc"),
                            resultado.DaValor<string>("TipoDoc"),
                            resultado.DaValor<string>("NumDoc")
                        );

                        resultado.Seguinte();
                    }
                }

                // Segunda consulta - AD_F3MManutencoes
                var queryListaF3M = $@"
                SELECT * 
                FROM [PRIPVEIGA].[dbo].[AD_F3MManutencoes] 
                WHERE NumViatura LIKE '%{_matricula}%' 
                {(string.IsNullOrEmpty(filtroDescricao) ? "" : $"AND CodCombustivel LIKE '%{filtroDescricao}%'")}";


                var listaF3M = _BSO.Consulta(queryListaF3M);
                var numf3m = listaF3M.NumLinhas();

                if (numf3m == 0)
                {
                    dt.Rows.Add(DBNull.Value, "Sem registros F3M", DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);
                }
                else
                {
                    listaF3M.Inicio();
                    string ultimoTipoDoc2 = null;

                    for (int i = 0; i < numf3m; i++)
                    {
                        string tipoDoc = "Histórico";

                        // Adiciona linha separadora se mudar de TipoDoc
                        if (ultimoTipoDoc2 != tipoDoc)
                        {
                            dt.Rows.Add(DBNull.Value, $"🔹 {tipoDoc}", DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);
                            ultimoTipoDoc2 = tipoDoc;
                        }

                        dt.Rows.Add(
                            listaF3M.DaValor<string>("NumMovimento"),  // NumMovimento
                            listaF3M.DaValor<string>("CodCombustivel"),  // CodCombustivel
                            listaF3M.DaValor<string>("KilometrosActual"), // KilometrosActual
                            listaF3M.DaValor<string>("Data"), // Data
                            tipoDoc,  // TipoDoc
                            "0"  // NumDoc
                        );

                        listaF3M.Seguinte();
                    }
                }

                // Atualiza a DataGridView
                dgvManutencaoCustos.Invoke(new Action(() => { dgvManutencaoCustos.DataSource = dt; }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar os dados: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private DataTable CriarTabela()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Artigo", typeof(string));
            dt.Columns.Add("Descricao", typeof(string));
            dt.Columns.Add("Kms", typeof(string));
            dt.Columns.Add("DataDoc", typeof(string));
            dt.Columns.Add("TipoDoc", typeof(string));
            dt.Columns.Add("NumDoc", typeof(string));
            return dt;
        }



        private void DgvManutencaoCustos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Verifica se é uma linha de separação (sem valores nas colunas principais)
            if (dgvManutencaoCustos.Rows[e.RowIndex].Cells[1].Value != null &&
                dgvManutencaoCustos.Rows[e.RowIndex].Cells[1].Value.ToString().StartsWith("🔹"))
            {
                dgvManutencaoCustos.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Black;
                dgvManutencaoCustos.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                dgvManutencaoCustos.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(dgvManutencaoCustos.Font, FontStyle.Bold);
            }
        }
    }

}
