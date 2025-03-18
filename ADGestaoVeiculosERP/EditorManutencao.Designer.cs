namespace ADGestaoVeiculosERP
{
    partial class EditorManutencao
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorManutencao));
            this.BT_Criar = new System.Windows.Forms.Button();
            this.TXT_ID = new System.Windows.Forms.TextBox();
            this.TXT_Nome = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BT_Criar
            // 
            this.BT_Criar.Location = new System.Drawing.Point(211, 35);
            this.BT_Criar.Name = "BT_Criar";
            this.BT_Criar.Size = new System.Drawing.Size(157, 38);
            this.BT_Criar.TabIndex = 41;
            this.BT_Criar.Text = "Criar Despesa";
            this.BT_Criar.UseVisualStyleBackColor = true;
            this.BT_Criar.Click += new System.EventHandler(this.BT_Criar_Click);
            // 
            // TXT_ID
            // 
            this.TXT_ID.Enabled = false;
            this.TXT_ID.Location = new System.Drawing.Point(87, 9);
            this.TXT_ID.Name = "TXT_ID";
            this.TXT_ID.Size = new System.Drawing.Size(58, 20);
            this.TXT_ID.TabIndex = 40;
            // 
            // TXT_Nome
            // 
            this.TXT_Nome.Location = new System.Drawing.Point(151, 9);
            this.TXT_Nome.Name = "TXT_Nome";
            this.TXT_Nome.Size = new System.Drawing.Size(217, 20);
            this.TXT_Nome.TabIndex = 25;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Descricao:";
            // 
            // EditorManutencao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(403, 89);
            this.Controls.Add(this.BT_Criar);
            this.Controls.Add(this.TXT_ID);
            this.Controls.Add(this.TXT_Nome);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditorManutencao";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Registo de Manutenções";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BT_Criar;
        private System.Windows.Forms.TextBox TXT_ID;
        private System.Windows.Forms.TextBox TXT_Nome;
        private System.Windows.Forms.Label label1;
    }
}