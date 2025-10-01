using ErpBS100;
using StdPlatBS100;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace ADGestaoVeiculosERP
{
    public partial class CriarViaturas : Form
    {
        ErpBS BSO;
        StdBSInterfPub PSO;
        string MatriculaStrig;
        public CriarViaturas(ErpBS mBSO, StdBSInterfPub mPSO, string matricula)
        {
            InitializeComponent();
            BSO = mBSO;
            PSO = mPSO;
            MatriculaStrig = matricula;

            if (MatriculaStrig == "")
            {
                GetUltimoNumeroViatura();
                CB_Activo.Checked = true;
            }
            else
            {
                GetVeiculos();
            }


            DTP_DataInspecao.CustomFormat = " ";
            DTP_DataInspecao.Format = DateTimePickerFormat.Custom;
        }


        private void GetVeiculos()
        {
            var query = $@"SELECt * FROM [PRIPVEIGA].[dbo].AD_Viaturas WHERE IdMatricula = '{MatriculaStrig}'";
            var veiculoEscolhido = BSO.Consulta(query);

            TXT_Codigo.Text = veiculoEscolhido.DaValor<string>("Codigo");
            TXT_Matricula.Text = veiculoEscolhido.DaValor<string>("IdMatricula");
            TXT_Marca.Text = veiculoEscolhido.DaValor<string>("Marca");
            TXT_Modelo.Text = veiculoEscolhido.DaValor<string>("Modelo");
            TXT_Cor.Text = veiculoEscolhido.DaValor<string>("Cor");
            TXT_Categoria.Text = veiculoEscolhido.DaValor<string>("Categoria");
            NUD_Cilindrada.Text = veiculoEscolhido.DaValor<string>("Cilindrada");
            NUD_Lugares.Text = veiculoEscolhido.DaValor<string>("Lugares");
            TXT_NumCondutor.Text = veiculoEscolhido.DaValor<string>("NumCondutor");
            CB_Combustivel.SelectedItem = veiculoEscolhido.DaValor<string>("Combustivel");
            NUD_Kilometrosiniciais.Text = veiculoEscolhido.DaValor<string>("KilometrosIniciais");
            TXT_TotalDespesas.Text = veiculoEscolhido.DaValor<string>("TotalDespesas");
            TXT_TotalCombustivel.Text = veiculoEscolhido.DaValor<string>("TotalCombustivel");
            TXT_Obs.Text = veiculoEscolhido.DaValor<string>("Obs");
            CB_Activo.Checked = Convert.ToBoolean(veiculoEscolhido.DaValor<string>("Activo"));
            NUD_Tara.Text = veiculoEscolhido.DaValor<string>("Tara");
            TXT_TipoViatura.Text = veiculoEscolhido.DaValor<string>("TipoViatura");
            NUD_MedioConsumo.Text = veiculoEscolhido.DaValor<string>("MediaConsumo");
            NUD_KMActuais.Text = veiculoEscolhido.DaValor<string>("KMActuais");
            NUD_DiaPrestacao.Text = veiculoEscolhido.DaValor<string>("DiaPrestacao");

            dtp_Data.Value = veiculoEscolhido.DaValor<DateTime>("Data");


            // Verificação para garantir que DataCompra seja tratada corretamente
            DateTime? dataCompra = veiculoEscolhido.DaValor<string>("DataCompra") != null
                ? (DateTime?)Convert.ToDateTime(veiculoEscolhido.DaValor<string>("DataCompra"))
                : null;

            if (dataCompra == null || dataCompra == DateTime.MinValue)
            {
                DTP_DataCompra.Value = DateTime.Now; // ou qualquer valor padrão
                DTP_DataCompra.Checked = false; // Marca o controle como desmarcado
            }
            else
            {
                DTP_DataCompra.Value = dataCompra.Value;
                DTP_DataCompra.Checked = true; // Marca o controle com a data escolhida
            }

            // Verificação para garantir que PrazoPagamento seja tratado corretamente
            DateTime? prazoPagamento = veiculoEscolhido.DaValor<string>("PrazoPagamento") != null
                ? (DateTime?)Convert.ToDateTime(veiculoEscolhido.DaValor<string>("PrazoPagamento"))
                : null;

            if (prazoPagamento == null || prazoPagamento == DateTime.MinValue)
            {
                DTP_PrazoPagamento.Value = DateTime.Now; // ou qualquer valor padrão
                DTP_PrazoPagamento.Checked = false; // Marca o controle como desmarcado
            }
            else
            {
                DTP_PrazoPagamento.Value = prazoPagamento.Value;
                DTP_PrazoPagamento.Checked = true; // Marca o controle com a data escolhida
            }

            TXT_ObsCompra.Text = veiculoEscolhido.DaValor<string>("ObsCompra");

            var codEntidade = veiculoEscolhido.DaValor<string>("CodEntidade");

            if (codEntidade == "0") // Verifica se codEntidade é null ou vazio
            {
                TXT_CodEntidade.Text = "";
                TXT_NomeEntidade.Text = "";
            }
            else
            {
                var queryCondutor = $@"SELECT Nome FROM [PRIPVEIGA].[dbo].AD_Entidades WHERE ID ='{codEntidade}'";
                var resultCondutor = BSO.Consulta(queryCondutor);
                TXT_NomeEntidade.Text = resultCondutor.DaValor<string>("Nome");
            }

            var numCondutor = veiculoEscolhido.DaValor<string>("NumCondutor");

            if (numCondutor == "0") // Verifica se NumCondutor é null ou vazio
            {
                TXT_NumCondutor.Text = "";
                TXT_NomeCondutor.Text = "";
            }
            else
            {
                var queryCondutor = $@"SELECT Nome FROM [PRIPVEIGA].[dbo].AD_Condutores WHERE ID ='{numCondutor}'";
                var resultCondutor = BSO.Consulta(queryCondutor);
                TXT_NomeCondutor.Text = resultCondutor.DaValor<string>("Nome");
            }

            CarregarMatriculas();
        }

        private void GetUltimoNumeroViatura()
        {
            var query = $"SELECT MAX(CAST(Codigo AS INT)) + 1 AS ProximoCodigo FROM [PRIPVEIGA].[dbo].AD_Viaturas;";
            var result = BSO.Consulta(query);
            TXT_Codigo.Text = result.DaValor<string>("ProximoCodigo");
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnF4_Click(object sender, EventArgs e)
        {
            MetodoGetVeiculos();
        }
        private void TXT_Codigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                MetodoGetVeiculos();
            }
        }
        private void MetodoGetVeiculos()
        {
            Dictionary<string, string> veiculo = new Dictionary<string, string>();
            GetVeiculos(ref veiculo);

            if (veiculo.Count > 0)
            {
                SetInfoVeiculo(veiculo);
            }
        }

        private void SetInfoVeiculo(Dictionary<string, string> veiculo)
        {
            TXT_Codigo.Text = veiculo["Codigo"];
            TXT_Matricula.Text = veiculo["IdMatricula"];
            TXT_Marca.Text = veiculo["Marca"];
            TXT_Modelo.Text = veiculo["Modelo"];
            TXT_Cor.Text = veiculo["Cor"];
            TXT_Categoria.Text = veiculo["Categoria"];
            NUD_Cilindrada.Text = veiculo["Cilindrada"];
            NUD_Lugares.Text = veiculo["Lugares"];
            TXT_NumCondutor.Text = veiculo["NumCondutor"];
            CB_Combustivel.SelectedItem = veiculo["Combustivel"];
            NUD_Kilometrosiniciais.Text = veiculo["KilometrosIniciais"];

            TXT_TotalDespesas.Text = veiculo["TotalDespesas"];
            TXT_TotalCombustivel.Text = veiculo["TotalCombustivel"];
            TXT_Obs.Text = veiculo["Obs"];
            CB_Activo.Checked = Convert.ToBoolean(veiculo["Activo"]);
            NUD_Tara.Text = veiculo["Tara"];
            TXT_TipoViatura.Text = veiculo["TipoViatura"];
            NUD_MedioConsumo.Text = veiculo["MediaConsumo"];
            NUD_KMActuais.Text = veiculo["KMActuais"];
            NUD_DiaPrestacao.Text = veiculo["DiaPrestacao"];

            if (veiculo["DataCompra"] != "")
            {
                DTP_DataCompra.Value = DateTime.Parse(veiculo["DataCompra"]);
            }
            if (veiculo["Data"] != "")
            {
                dtp_Data.Value = DateTime.Parse(veiculo["Data"]);
            }
            TXT_CodEntidade.Text = veiculo["CodEntidade"];
            TXT_TipoCompra.Text = veiculo["TipoCompra"];
            NUD_ValorMensal.Text = veiculo["ValorMensal"];


            if (veiculo["PrazoPagamento"] != "")
            {
                DTP_PrazoPagamento.Value = DateTime.Parse(veiculo["PrazoPagamento"]);
            }

            TXT_ObsCompra.Text = veiculo["ObsCompra"];


            if (veiculo["CodEntidade"] != "0")
            {
                var identidade = veiculo["CodEntidade"];
                var queryCondutor = $@"SELECT Nome FROM  [PRIPVEIGA].[dbo].AD_Entidades WHERE ID ={identidade}";
                var resultCondutor = BSO.Consulta(queryCondutor);
                TXT_NomeEntidade.Text = resultCondutor.DaValor<string>("Nome");
            }
            else
            {
                TXT_CodEntidade.Text = "";
                TXT_NomeEntidade.Text = "";
            }


            if (veiculo["NumCondutor"] != "0")
            {
                var idcondutor = veiculo["NumCondutor"];
                var queryCondutor = $@"SELECT Nome FROM  [PRIPVEIGA].[dbo].AD_Condutores WHERE ID ={idcondutor}";
                var resultCondutor = BSO.Consulta(queryCondutor);
                TXT_NomeCondutor.Text = resultCondutor.DaValor<string>("Nome");
            }
            else
            {
                TXT_NumCondutor.Text = "";
                TXT_NomeCondutor.Text = "";
            }
            CarregarMatriculas();
            tabcontrol.Enabled = true;
        }

        private void GetVeiculos(ref Dictionary<string, string> veiculo)
        {
            string NomeLista = "Veiculos";
            string Campos = "Codigo,IdMatricula, Marca, Modelo, Cor, Categoria, Cilindrada, NumCondutor, Combustivel, KilometrosIniciais, KilometrosOutros, TotalDespesas,TotalCombustivel,Obs,Activo,Tara,TipoViatura,MediaConsumo,CustoMedioKM,CustoMedioKMDespesas,MedidaPneus,KMActuais,DiaPrestacao,DataCompra,CodEntidade,TipoCompra,ValorMensal,PrazoPagamento,ObsCompra,Lugares,Manutencao,KmsFaltaManutencao,Data";
            string Tabela = "[PRIPVEIGA].[dbo].AD_Viaturas (NOLOCK)";
            string Where = "Activo = 1";
            string CamposF4 = "Codigo,IdMatricula, Marca, Modelo, Cor, Categoria, Cilindrada, NumCondutor, Combustivel, KilometrosIniciais, KilometrosOutros, TotalDespesas,TotalCombustivel,Obs,Activo,Tara,TipoViatura,MediaConsumo,CustoMedioKM,CustoMedioKMDespesas,MedidaPneus,KMActuais,DiaPrestacao,DataCompra,CodEntidade,TipoCompra,ValorMensal,PrazoPagamento,ObsCompra,Lugares,Manutencao,KmsFaltaManutencao,Data";
            string orderby = "Codigo, IdMatricula";

            List<string> ResQuery = new List<string>();

            OpenF4List(Campos, Tabela, Where, CamposF4, orderby, NomeLista, this, ref ResQuery);

            if (ResQuery.Count > 0)
            {
                string[] colunas = CamposF4.Split(',');
                for (int i = 0; i < colunas.Length; i++)
                {
                    if (i < ResQuery.Count)
                    {
                        veiculo[colunas[i].Trim()] = ResQuery[i].ToString();
                    }
                }
            }
        }



        private void BT_F4Condutor_Click(object sender, EventArgs e)
        {
            MetodoGetCondutores();
        }

        private void MetodoGetCondutores()
        {
            Dictionary<string, string> condutores = new Dictionary<string, string>();
            GetCondutores(ref condutores);

            if (condutores.Count > 0)
            {
                SetInfoCondutor(condutores);
            }
        }

        private void SetInfoCondutor(Dictionary<string, string> condutores)
        {
            TXT_NumCondutor.Text = condutores["ID"];
            TXT_NomeCondutor.Text = condutores["Nome"];
        }

        private void GetCondutores(ref Dictionary<string, string> condutores)
        {
            string NomeLista = "Condutores";
            string Campos = "ID,Nome,Abreviatura,Endereco,Localidade,CodigoPostal,Telefone,DataNascimento,Carta,DataValidade,Obs,Activo,Telemovel";
            string Tabela = "[PRIPVEIGA].[dbo].AD_Condutores (NOLOCK)";
            string Where = "";
            string CamposF4 = "ID,Nome,Abreviatura,Endereco,Localidade,CodigoPostal,Telefone,DataNascimento,Carta,DataValidade,Obs,Activo,Telemovel";
            string orderby = "ID, Nome";

            List<string> ResQuery = new List<string>();

            OpenF4List(Campos, Tabela, Where, CamposF4, orderby, NomeLista, this, ref ResQuery);

            if (ResQuery.Count > 0)
            {
                string[] colunas = CamposF4.Split(',');
                for (int i = 0; i < colunas.Length; i++)
                {
                    if (i < ResQuery.Count)
                    {
                        condutores[colunas[i].Trim()] = ResQuery[i].ToString();
                    }
                }
            }
        }

        private void BT_F4Entidade_Click(object sender, EventArgs e)
        {
            MetodoGetEntidades();
        }

        private void MetodoGetEntidades()
        {
            Dictionary<string, string> entidades = new Dictionary<string, string>();
            GetEntidades(ref entidades);

            if (entidades.Count > 0)
            {
                SetInfoEntidades(entidades);
            }
        }

        private void SetInfoEntidades(Dictionary<string, string> entidades)
        {
            TXT_CodEntidade.Text = entidades["ID"];
            TXT_NomeEntidade.Text = entidades["Nome"];
        }

        private void GetEntidades(ref Dictionary<string, string> entidades)
        {
            string NomeLista = "Entidades";
            string Campos = "ID,Nome";
            string Tabela = "[PRIPVEIGA].[dbo].AD_Entidades (NOLOCK)";
            string Where = "";
            string CamposF4 = "ID,Nome";
            string orderby = "ID, Nome";

            List<string> ResQuery = new List<string>();

            OpenF4List(Campos, Tabela, Where, CamposF4, orderby, NomeLista, this, ref ResQuery);

            if (ResQuery.Count > 0)
            {
                string[] colunas = CamposF4.Split(',');
                for (int i = 0; i < colunas.Length; i++)
                {
                    if (i < ResQuery.Count)
                    {
                        entidades[colunas[i].Trim()] = ResQuery[i].ToString();
                    }
                }
            }
        }

        private void BT_Gravar_Click(object sender, EventArgs e)
        {
            string queryCheck = $"SELECT * FROM [PRIPVEIGA].[dbo].AD_Viaturas WHERE IdMatricula = '{TXT_Matricula.Text}' AND Codigo = {TXT_Codigo.Text}";
            var resultCheck = BSO.Consulta(queryCheck);
            if (string.IsNullOrEmpty(TXT_Matricula.Text))
            {
                MessageBox.Show("A Matricula é obrigatorio!");
            }
            else
            {
                if (resultCheck.NumLinhas() > 0)
                {
                    UpdateViatura();
                }
                else
                {
                    CreateViatura();
                }
            }
        }

        private void CreateViatura()
        {
            if (TXT_Matricula.Text != null)
            {

                var queryViaturaExiste = $@"SELECT * FROM [PRIPVEIGA].[dbo].AD_Viaturas WHERE IdMatricula = '{TXT_Matricula.Text}'";

                var ExisteViatura = BSO.Consulta(queryViaturaExiste);
                if (ExisteViatura.NumLinhas() > 0)
                {
                    MessageBox.Show("A matrícula do veículo já existe. Por favor, verifique e tente novamente.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                var combustivel = CB_Combustivel?.SelectedItem?.ToString();
                var isActive = CB_Activo.Checked ? 1 : 0;
                int numCond = int.TryParse(TXT_NumCondutor.Text, out int num) ? num : 0;
                // Create the SQL insert query
                string queryInsert = $@"
                INSERT INTO [PRIPVEIGA].[dbo].AD_Viaturas (
                    IdMatricula, Codigo, Marca, Modelo, Cor, Categoria, Cilindrada, Lugares, NumCondutor, Combustivel, KilometrosIniciais, Obs, Activo, Tara, TipoViatura, MediaConsumo, KMActuais, DiaPrestacao,
                    DataCompra, CodEntidade, TipoCompra, ValorMensal, PrazoPagamento, ObsCompra
                ) 
                VALUES (
                    '{TXT_Matricula.Text}',
                    '{TXT_Codigo.Text}',
                    '{(string.IsNullOrEmpty(TXT_Marca.Text) ? "" : TXT_Marca.Text)}',
                    '{(string.IsNullOrEmpty(TXT_Modelo.Text) ? "" : TXT_Modelo.Text)}',
                    '{(string.IsNullOrEmpty(TXT_Cor.Text) ? "" : TXT_Cor.Text)}',
                    '{(string.IsNullOrEmpty(TXT_Categoria.Text) ? "" : TXT_Categoria.Text)}',
                    {(NUD_Cilindrada.Value == 0 ? "NULL" : NUD_Cilindrada.Value.ToString())},
                    {(NUD_Lugares.Value == 0 ? "NULL" : NUD_Lugares.Value.ToString())},
                    {numCond},
                    '{(combustivel != null ? combustivel.ToString() : "NULL")}',
                    {(NUD_Kilometrosiniciais.Value == 0 ? "NULL" : NUD_Kilometrosiniciais.Value.ToString("0.00", CultureInfo.InvariantCulture))},
                    '{(string.IsNullOrEmpty(TXT_Obs.Text) ? "" : TXT_Obs.Text)}',
                    {isActive},
                    {(NUD_Tara.Value == 0 ? "NULL" : NUD_Tara.Value.ToString("0.00", CultureInfo.InvariantCulture))},
                    '{(string.IsNullOrEmpty(TXT_TipoViatura.Text) ? "" : TXT_TipoViatura.Text)}',
                    {(NUD_MedioConsumo.Value == 0 ? "NULL" : NUD_MedioConsumo.Value.ToString("0.00", CultureInfo.InvariantCulture))},
                    {(NUD_KMActuais.Value == 0 ? "NULL" : NUD_KMActuais.Value.ToString())},
                    {(NUD_DiaPrestacao.Value == 0 ? "NULL" : NUD_DiaPrestacao.Value.ToString())},
                    '{DTP_DataCompra.Value:yyyy-MM-dd}',
                   {(string.IsNullOrEmpty(TXT_CodEntidade.Text) ? "0" : TXT_CodEntidade.Text)},

                    '{(string.IsNullOrEmpty(TXT_TipoCompra.Text) ? "" : TXT_TipoCompra.Text)}',
                    {(NUD_ValorMensal.Value == 0 ? "NULL" : NUD_ValorMensal.Value.ToString("0.00", CultureInfo.InvariantCulture))},
                    '{DTP_PrazoPagamento.Value:yyyy-MM-dd}',
                    '{(string.IsNullOrEmpty(TXT_ObsCompra.Text) ? "" : TXT_ObsCompra.Text)}'
                )";


                // Execute the insert query
                BSO.DSO.ExecuteSQL(queryInsert);

                // Clear the fields after creating the entry
                LimparCampos();
            }
        }

        private void UpdateViatura()
        {

            var combustivel = CB_Combustivel?.SelectedItem?.ToString();
            string dataCompraFormatada = DTP_DataCompra.Value.ToString("yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
            string dataPrazoPagamentoFormatada = DTP_PrazoPagamento.Value.ToString("yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
            var dataCompraSQL = "NULL";
            var dataPrazoPagamentoSQL = "NULL";
            if (DTP_DataCompra.Value == DateTime.Now && !DTP_DataCompra.Checked)
            {
            }
            else
            {
                dataCompraSQL = DTP_DataCompra.Checked ? $"'{dataCompraFormatada}'" : $"'{dataCompraFormatada}'";
                dataPrazoPagamentoSQL = DTP_DataCompra.Checked ? $"'{dataPrazoPagamentoFormatada}'" : $"'{dataPrazoPagamentoFormatada}'";
            }
            string queryUpdate = $@"
                        UPDATE [PRIPVEIGA].[dbo].AD_Viaturas 
                        SET Marca = '{(string.IsNullOrEmpty(TXT_Marca.Text) ? "" : TXT_Marca.Text)}',
                        Modelo = '{(string.IsNullOrEmpty(TXT_Modelo.Text) ? "" : TXT_Modelo.Text)}',
                        Cor = '{(string.IsNullOrEmpty(TXT_Cor.Text) ? "" : TXT_Cor.Text)}', 
                        Categoria = '{(string.IsNullOrEmpty(TXT_Categoria.Text) ? "" : TXT_Categoria.Text)}',
                        Cilindrada = {(NUD_Cilindrada.Value == 0 ? "NULL" : NUD_Cilindrada.Value.ToString())},
                        Lugares = {(NUD_Lugares.Value == 0 ? "NULL" : NUD_Lugares.Value.ToString())},
                        NumCondutor = {(string.IsNullOrEmpty(TXT_NumCondutor.Text) ? "0" : int.Parse(TXT_NumCondutor.Text).ToString())},


                        Combustivel = '{(combustivel != null ? combustivel.ToString() : "NULL")}',
                        KilometrosIniciais =  {(NUD_Kilometrosiniciais.Value == 0 ? "NULL" : NUD_Kilometrosiniciais.Value.ToString("0.00", CultureInfo.InvariantCulture))},
                        Obs = '{(string.IsNullOrEmpty(TXT_Obs.Text) ? "" : TXT_Obs.Text)}',
                        Activo = {(CB_Activo.Checked ? 1 : 0)},
                        Tara = {(NUD_Tara.Value == 0 ? "NULL" : NUD_Tara.Value.ToString("0.00", CultureInfo.InvariantCulture))},
                        TipoViatura = '{(string.IsNullOrEmpty(TXT_TipoViatura.Text) ? "" : TXT_TipoViatura.Text)}',
                        MedidaPneus = '{(string.IsNullOrEmpty(TXT_MedidaPneus.Text) ? "" : TXT_MedidaPneus.Text)}',
                        KMActuais = {(NUD_KMActuais.Value == 0 ? "NULL" : NUD_KMActuais.Value.ToString())},
                        DiaPrestacao = {(NUD_DiaPrestacao.Value == 0 ? "NULL" : NUD_DiaPrestacao.Value.ToString())},
                        DataCompra = '{DTP_DataCompra.Value:yyyy-MM-dd}',
                        CodEntidade = {(string.IsNullOrEmpty(TXT_CodEntidade.Text) ? "0" : int.Parse(TXT_CodEntidade.Text).ToString())},

                        TipoCompra = '{(string.IsNullOrEmpty(TXT_TipoCompra.Text) ? "" : TXT_TipoCompra.Text)}',
                        ValorMensal = {(NUD_ValorMensal.Value == 0 ? "NULL" : NUD_ValorMensal.Value.ToString("0.00", CultureInfo.InvariantCulture))},
                        PrazoPagamento =  '{DTP_PrazoPagamento.Value:yyyy-MM-dd}',
                        ObsCompra = '{(string.IsNullOrEmpty(TXT_ObsCompra.Text) ? "" : TXT_ObsCompra.Text)}'
                        WHERE IdMatricula = '{TXT_Matricula.Text}'";
            BSO.DSO.ExecuteSQL(queryUpdate);
            LimparCampos();
        }

        private void BT_Novo_Click(object sender, EventArgs e)
        {


            LimparCampos();
        }

        private void LimparCampos()
        {
            GetUltimoNumeroViatura();
            TXT_Modelo.Text = "";
            TXT_NomeCondutor.Text = "";
            TXT_NomeEntidade.Text = "";
            TXT_Matricula.Text = "";
            TXT_Marca.Text = "";
            TXT_Cor.Text = "";
            TXT_Categoria.Text = "";
            NUD_Cilindrada.Value = 0;
            NUD_Lugares.Value = 0;
            TXT_NumCondutor.Text = "";
            CB_Combustivel.SelectedItem = null;

            NUD_Kilometrosiniciais.Value = 0;
            TXT_Obs.Text = "";
            CB_Activo.Checked = true;
            NUD_Tara.Value = 0;
            TXT_TipoViatura.Text = "";
            NUD_MedioConsumo.Value = 0;
            TXT_MedidaPneus.Text = "";
            NUD_KMActuais.Value = 0;
            NUD_DiaPrestacao.Value = 0;
            DTP_DataCompra.Value = DateTime.Now;
            TXT_CodEntidade.Text = "";
            TXT_TipoCompra.Text = "";
            NUD_ValorMensal.Value = 0;
            DTP_PrazoPagamento.Value = DateTime.Now;
            TXT_ObsCompra.Text = "";
            TXT_TotalCombustivel.Text = "";
            TXT_TotalDespesas.Text = "";

            dataGridView1.Rows.Clear();
            TXT_Descricao.Text = "";
            NUD_Kilometrosiniciais.Value = 0;
            DTP_DataInspecao.CustomFormat = " ";

        }

        private void BT_Adcionar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TXT_Matricula.Text))
            {
                MessageBox.Show("Selecione uma matrícula!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(TXT_Descricao.Text))
            {
                MessageBox.Show("A descrição não pode estar vazia!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            string descricao = TXT_Descricao.Text;
            int quilometragem = (int)NUD_Quilometros.Value;
            string data = DTP_DataInspecao.CustomFormat == " " ? "" : DTP_DataInspecao.Value.ToShortDateString();

            dataGridView1.Rows.Add(descricao, quilometragem, data);
            DateTime? dataConvertida = null;

            if (DateTime.TryParseExact(data, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime tempData))
            {
                dataConvertida = tempData;
            }

            // Se dataConvertida for null, passamos "NULL" para a query, caso contrário, formatamos a data para o padrão yyyy-MM-dd
            string dataSQL = dataConvertida.HasValue ? $"'{dataConvertida.Value.ToString("yyyy-MM-dd")}'" : "NULL";
            string queryMatricula = $@"INSERT INTO [PRIPVEIGA].[dbo].AD_RegistrosManutencao (IdMatricula, Descricao, Quilometros, DataEvento) VALUES ('{TXT_Matricula.Text}', '{descricao}', {quilometragem}, {dataSQL})";

            BSO.DSO.ExecuteSQL(queryMatricula);




            // Limpando os campos após adicionar
            TXT_Descricao.Clear();
            NUD_Quilometros.Value = 0;
            DTP_DataInspecao.CustomFormat = " ";
        }

        private void BT_Remover_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0) // Verifica se há uma linha selecionada
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    string descricao = row.Cells[0].Value.ToString();
                    int quilometros = Convert.ToInt32(row.Cells[1].Value);
                    dataGridView1.Rows.Remove(row); // Remove a linha selecionada
                    string queryDelete = $@"DELETE FROM [PRIPVEIGA].[dbo].AD_RegistrosManutencao WHERE Descricao = '{descricao}' AND Quilometros = {quilometros} AND IdMatricula= '{TXT_Matricula.Text}' ";
                    BSO.DSO.ExecuteSQL(queryDelete);
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecione uma linha para remover.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DTP_DataInspecao_ValueChanged(object sender, EventArgs e)
        {
            DTP_DataInspecao.CustomFormat = "dd/MM/yyyy";
        }

        private void CarregarMatriculas()
        {
            dataGridView1.Rows.Clear();
            var queryMatricula = $@"SELECT * FROM [PRIPVEIGA].[dbo].AD_RegistrosManutencao WHERE IdMatricula = '{TXT_Matricula.Text}'";
            var listaRegistos = BSO.Consulta(queryMatricula);

            var numregistos = listaRegistos.NumLinhas();
            for (int i = 0; i < numregistos; i++)
            {
                var descricao = listaRegistos.DaValor<string>("Descricao");
                var quilometros = listaRegistos.DaValor<string>("Quilometros");
                var data = listaRegistos.DaValor<string>("DataEvento");


                dataGridView1.Rows.Add(descricao, quilometros, data);
                listaRegistos.Seguinte();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MetodoTipoDespesas();
        }

        private void MetodoTipoDespesas()
        {
            Dictionary<string, string> despesas = new Dictionary<string, string>();
            GetTipoDespesas(ref despesas);

            if (despesas.Count > 0)
            {
                TXT_Descricao.Text = despesas["Descricao"];
            }
        }

        private void GetTipoDespesas(ref Dictionary<string, string> despesas)
        {
            string NomeLista = "Despesas";
            string Campos = "Descricao";
            string Tabela = "[PRIPVEIGA].[dbo].AD_TiposDespesas (NOLOCK)";
            string Where = "";
            string CamposF4 = "Descricao";
            string orderby = "Descricao";

            List<string> ResQuery = new List<string>();

            OpenF4List(Campos, Tabela, Where, CamposF4, orderby, NomeLista, this, ref ResQuery);

            if (ResQuery.Count > 0)
            {
                string[] colunas = CamposF4.Split(',');
                for (int i = 0; i < colunas.Length; i++)
                {
                    if (i < ResQuery.Count)
                    {
                        despesas[colunas[i].Trim()] = ResQuery[i].ToString();
                    }
                }
            }
        }



        private void BT_CriaEntidade_Click(object sender, EventArgs e)
        {
            EditorEntidade criarViaturaForm = new EditorEntidade(BSO, PSO); // Cria uma instância do formulário CriarViatura
            criarViaturaForm.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EditorCondutor criarViaturaForm = new EditorCondutor(BSO, PSO); // Cria uma instância do formulário CriarViatura
            criarViaturaForm.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            EditorManutencao criarViaturaForm = new EditorManutencao(BSO, PSO); // Cria uma instância do formulário CriarViatura
            criarViaturaForm.ShowDialog();
        }

        private void listaCustoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var matri = TXT_Matricula.Text;
                ListaCusto listaCustoForm = new ListaCusto(BSO, PSO, matri);
                listaCustoForm.ShowDialog();


                //ListaCustos(matri);
                // var query = PSO.Listas.BSO.Listas.CarregaLista("ListasTrabalho", "5D89F3F4-7D32-11E3-90FB-000C29012999");

                // Control control = new Control();


                // PSO.Listas.BSO.Listas.TrataF4Id("CabecCompras", "", this, control, "Movimentos de Compra - Viaturas", "783D5F9B-9ED3-4EEC-8BED-E5A2D20F3A26");//3427ADAD-2D31-4F2A-9319-12240E7B034E

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }


        }
        private void ListaCustos(string matricula)
        {
            string NomeLista = "Lista Custo";
            string Campos = "[LinhasCompras].[CDU_Matricula], [CabecCompras].[TipoDoc], [CabecCompras].[Serie], " +
                            "[CabecCompras].[NumDoc], [CabecCompras].[TipoEntidade], [CabecCompras].[Entidade], " +
                            "[LinhasCompras].[Artigo], [LinhasCompras].[PrecUnit]";

            string Tabela = "[CabecCompras] WITH (NOLOCK) LEFT JOIN [LinhasCompras] WITH (NOLOCK) ON [CabecCompras].[Id] = [LinhasCompras].[IdCabecCompras]";

            string Where = $"([CabecCompras].[TipoDoc] IN ('COMBV','DESPV')) AND ([LinhasCompras].[CDU_Matricula] = '{matricula}')";

            string CamposF4 = "[LinhasCompras].[CDU_Matricula]";
            string orderby = "[LinhasCompras].[CDU_Matricula]";

            List<string> ResQuery = new List<string>();

            OpenF4List(Campos, Tabela, Where, CamposF4, orderby, NomeLista, this, ref ResQuery);
        }

        private void OpenF4List(string campos, string tabela, string where, string camposF4, string orderby, string nomeLista, Form frm, ref List<string> resQuery)
        {
            string strSQL = "select distinct " + campos + " FROM " + tabela;

            if (where.Length > 0)
            {
                strSQL += " WHERE " + where;
            }

            strSQL += " Order by " + orderby;
            string result = Convert.ToString(PSO.Listas.GetF4SQL(nomeLista, strSQL, camposF4, frm));

            if (!string.IsNullOrEmpty(result))
            {
                string[] itemQuery = result.Split('\t');
                resQuery.AddRange(itemQuery);
            }
        }
        private void listaManutencaoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var matri = TXT_Matricula.Text;
            ListaManutencao listaCustoForm = new ListaManutencao(matri, BSO, PSO);
            listaCustoForm.ShowDialog();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) // Garante que não é o cabeçalho
            {
                string coluna = dataGridView1.Columns[e.ColumnIndex].HeaderText; // Nome da coluna
                string novoValor = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString(); // Novo valor
                string descricaoOriginal = dataGridView1.Rows[e.RowIndex].Cells[0].Value?.ToString();
                string kms = dataGridView1.Rows[e.RowIndex].Cells[1].Value?.ToString();
                string data2 = dataGridView1.Rows[e.RowIndex].Cells[2].Value?.ToString();
                string Where = "";

                // Exemplo: Adicionar verificação para identificar qual coluna foi alterada e montar a query de UPDATE
                string setClause = "";
                if (coluna == "Descricao")
                {
                    setClause = $"Descricao = '{novoValor}' ";
                    Where = $"AND Quilometros = {kms}";
                }
                else if (coluna == "Quilómetros")
                {
                    // Verifique se o valor é numérico antes de tentar convertê-lo
                    if (decimal.TryParse(novoValor, out decimal quilometragem))
                    {
                        setClause = $"Quilometros = {quilometragem} ";
                        Where = $"AND Descricao = '{descricaoOriginal}'";
                    }
                    else
                    {
                        return;
                    }
                }
                else if (coluna == "Data")
                {
                    DateTime? dataConvertida = null;
                    // Tente converter para DateTime, se possível
                    if (DateTime.TryParse(novoValor, out DateTime data))
                    {
                        dataConvertida = data;
                    }


                    string dataSQL = dataConvertida.HasValue ? $"'{dataConvertida.Value.ToString("yyyy-MM-dd")}'" : "NULL";

                    setClause = $"DataEvento = {dataSQL}";
                    Where = $"AND Descricao = '{descricaoOriginal}'";
                }

                // Agora, montamos a query para o UPDATE
                if (!string.IsNullOrEmpty(setClause))
                {
                    string queryUpdate = $@"
                UPDATE [PRIPVEIGA].[dbo].AD_RegistrosManutencao 
                SET {setClause}
                WHERE IdMatricula = '{TXT_Matricula.Text}' {Where} "; // Condição baseada no identificador

                    // Aqui, você executaria a query no banco de dados. Exemplo:
                    // ExecuteQuery(queryUpdate);

                    BSO.DSO.ExecuteSQL(queryUpdate);

                    // Opcional: Aqui você poderia executar a query de atualização no banco de dados, usando um método para executar a SQL.
                    // ExecuteQuery(queryUpdate); 
                }
                else
                {
                    MessageBox.Show("Alteração não reconhecida.");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ListaDespesasF3M listaDespesasF3M = new ListaDespesasF3M(TXT_Matricula.Text, BSO, PSO);
            listaDespesasF3M.ShowDialog();
        }

        private void listaDasDespesasDESPVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormListaDESPV formListaDESPV = new FormListaDESPV("Lista de Documentos DESPV", BSO, PSO, TXT_Matricula.Text);
            formListaDESPV.ShowDialog();
        }
    }
}
