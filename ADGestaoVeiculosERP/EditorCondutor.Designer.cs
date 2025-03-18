namespace ADGestaoVeiculosERP
{
    partial class EditorCondutor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorCondutor));
            this.BT_Criar = new System.Windows.Forms.Button();
            this.TXT_ID = new System.Windows.Forms.TextBox();
            this.TXT_Obs = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.TXT_Telefone = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.TXT_CP = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.TXT_Localidade = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TXT_Endereco = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TXT_Nome = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TXT_Abre = new System.Windows.Forms.TextBox();
            this.DTP_Nascimento = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.TXT_Carta = new System.Windows.Forms.TextBox();
            this.DTP_Validade = new System.Windows.Forms.DateTimePicker();
            this.label12 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BT_Criar
            // 
            this.BT_Criar.Location = new System.Drawing.Point(611, 205);
            this.BT_Criar.Name = "BT_Criar";
            this.BT_Criar.Size = new System.Drawing.Size(201, 38);
            this.BT_Criar.TabIndex = 41;
            this.BT_Criar.Text = "Criar Condutor";
            this.BT_Criar.UseVisualStyleBackColor = true;
            this.BT_Criar.Click += new System.EventHandler(this.BT_Criar_Click);
            // 
            // TXT_ID
            // 
            this.TXT_ID.Enabled = false;
            this.TXT_ID.Location = new System.Drawing.Point(103, 15);
            this.TXT_ID.Name = "TXT_ID";
            this.TXT_ID.Size = new System.Drawing.Size(58, 23);
            this.TXT_ID.TabIndex = 40;
            // 
            // TXT_Obs
            // 
            this.TXT_Obs.Location = new System.Drawing.Point(611, 15);
            this.TXT_Obs.Multiline = true;
            this.TXT_Obs.Name = "TXT_Obs";
            this.TXT_Obs.Size = new System.Drawing.Size(201, 172);
            this.TXT_Obs.TabIndex = 39;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(572, 15);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(32, 15);
            this.label10.TabIndex = 38;
            this.label10.Text = "Obs:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 160);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 15);
            this.label9.TabIndex = 36;
            this.label9.Text = "Data Nascim.:";
            // 
            // TXT_Telefone
            // 
            this.TXT_Telefone.Location = new System.Drawing.Point(391, 122);
            this.TXT_Telefone.Name = "TXT_Telefone";
            this.TXT_Telefone.Size = new System.Drawing.Size(151, 23);
            this.TXT_Telefone.TabIndex = 35;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(329, 125);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 15);
            this.label8.TabIndex = 34;
            this.label8.Text = "Telefone:";
            // 
            // TXT_CP
            // 
            this.TXT_CP.Location = new System.Drawing.Point(103, 122);
            this.TXT_CP.Name = "TXT_CP";
            this.TXT_CP.Size = new System.Drawing.Size(210, 23);
            this.TXT_CP.TabIndex = 31;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 125);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 15);
            this.label6.TabIndex = 30;
            this.label6.Text = "Codigo Postal:";
            // 
            // TXT_Localidade
            // 
            this.TXT_Localidade.Location = new System.Drawing.Point(103, 83);
            this.TXT_Localidade.Name = "TXT_Localidade";
            this.TXT_Localidade.Size = new System.Drawing.Size(439, 23);
            this.TXT_Localidade.TabIndex = 29;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 86);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 15);
            this.label5.TabIndex = 28;
            this.label5.Text = "Localidade:";
            // 
            // TXT_Endereco
            // 
            this.TXT_Endereco.Location = new System.Drawing.Point(103, 44);
            this.TXT_Endereco.Name = "TXT_Endereco";
            this.TXT_Endereco.Size = new System.Drawing.Size(439, 23);
            this.TXT_Endereco.TabIndex = 27;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(35, 47);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 15);
            this.label4.TabIndex = 26;
            this.label4.Text = "Endereço:";
            // 
            // TXT_Nome
            // 
            this.TXT_Nome.Location = new System.Drawing.Point(167, 15);
            this.TXT_Nome.Name = "TXT_Nome";
            this.TXT_Nome.Size = new System.Drawing.Size(162, 23);
            this.TXT_Nome.TabIndex = 25;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 15);
            this.label1.TabIndex = 22;
            this.label1.Text = "Nome:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(336, 18);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 15);
            this.label2.TabIndex = 42;
            this.label2.Text = "Abreviatura:";
            // 
            // TXT_Abre
            // 
            this.TXT_Abre.Location = new System.Drawing.Point(418, 15);
            this.TXT_Abre.Name = "TXT_Abre";
            this.TXT_Abre.Size = new System.Drawing.Size(124, 23);
            this.TXT_Abre.TabIndex = 43;
            // 
            // DTP_Nascimento
            // 
            this.DTP_Nascimento.Location = new System.Drawing.Point(103, 156);
            this.DTP_Nascimento.Name = "DTP_Nascimento";
            this.DTP_Nascimento.Size = new System.Drawing.Size(210, 23);
            this.DTP_Nascimento.TabIndex = 44;
            this.DTP_Nascimento.Enter += new System.EventHandler(this.DTP_Nascimento_Enter);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(56, 189);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(40, 15);
            this.label11.TabIndex = 45;
            this.label11.Text = "Carta:";
            // 
            // TXT_Carta
            // 
            this.TXT_Carta.Location = new System.Drawing.Point(103, 186);
            this.TXT_Carta.Name = "TXT_Carta";
            this.TXT_Carta.Size = new System.Drawing.Size(173, 23);
            this.TXT_Carta.TabIndex = 46;
            // 
            // DTP_Validade
            // 
            this.DTP_Validade.Location = new System.Drawing.Point(377, 189);
            this.DTP_Validade.Name = "DTP_Validade";
            this.DTP_Validade.Size = new System.Drawing.Size(165, 23);
            this.DTP_Validade.TabIndex = 48;
            this.DTP_Validade.Enter += new System.EventHandler(this.DTP_Validade_Enter);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(283, 191);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(87, 15);
            this.label12.TabIndex = 47;
            this.label12.Text = "Data Validade:";
            // 
            // EditorCondutor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(845, 269);
            this.Controls.Add(this.DTP_Validade);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.TXT_Carta);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.DTP_Nascimento);
            this.Controls.Add(this.TXT_Abre);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BT_Criar);
            this.Controls.Add(this.TXT_ID);
            this.Controls.Add(this.TXT_Obs);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.TXT_Telefone);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.TXT_CP);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.TXT_Localidade);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TXT_Endereco);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.TXT_Nome);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditorCondutor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Registo de Condutores";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BT_Criar;
        private System.Windows.Forms.TextBox TXT_ID;
        private System.Windows.Forms.TextBox TXT_Obs;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox TXT_Telefone;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox TXT_CP;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TXT_Localidade;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TXT_Endereco;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TXT_Nome;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TXT_Abre;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DateTimePicker DTP_Nascimento;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox TXT_Carta;
        private System.Windows.Forms.DateTimePicker DTP_Validade;
        private System.Windows.Forms.Label label12;
    }
}