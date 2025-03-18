namespace ADGestaoVeiculosERP
{
    partial class FormEditorVenda
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditorVenda));
            this.label1 = new System.Windows.Forms.Label();
            this.NUD_KMS = new System.Windows.Forms.NumericUpDown();
            this.Enviar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_KMS)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Kilometros:";
            // 
            // NUD_KMS
            // 
            this.NUD_KMS.Location = new System.Drawing.Point(90, 12);
            this.NUD_KMS.Maximum = new decimal(new int[] {
            1569325055,
            23283064,
            0,
            0});
            this.NUD_KMS.Name = "NUD_KMS";
            this.NUD_KMS.Size = new System.Drawing.Size(182, 23);
            this.NUD_KMS.TabIndex = 3;
            // 
            // Enviar
            // 
            this.Enviar.Location = new System.Drawing.Point(194, 41);
            this.Enviar.Name = "Enviar";
            this.Enviar.Size = new System.Drawing.Size(78, 39);
            this.Enviar.TabIndex = 4;
            this.Enviar.Text = "OK";
            this.Enviar.UseVisualStyleBackColor = true;
            this.Enviar.Click += new System.EventHandler(this.Enviar_Click);
            // 
            // FormEditorVenda
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(294, 91);
            this.Controls.Add(this.Enviar);
            this.Controls.Add(this.NUD_KMS);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditorVenda";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Atualização de Quilometragem";
            ((System.ComponentModel.ISupportInitialize)(this.NUD_KMS)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown NUD_KMS;
        private System.Windows.Forms.Button Enviar;
    }
}