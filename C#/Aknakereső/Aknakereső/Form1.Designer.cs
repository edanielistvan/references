namespace Aknakereső
{
    partial class Form1
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
            this.btn_kever = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_marad = new System.Windows.Forms.TextBox();
            this.p_game = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // btn_kever
            // 
            this.btn_kever.Location = new System.Drawing.Point(12, 12);
            this.btn_kever.Name = "btn_kever";
            this.btn_kever.Size = new System.Drawing.Size(131, 23);
            this.btn_kever.TabIndex = 0;
            this.btn_kever.Text = "Keverés (új játék)";
            this.btn_kever.UseVisualStyleBackColor = true;
            this.btn_kever.Click += new System.EventHandler(this.Btn_kever_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(392, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Hátralévő bombák száma:";
            // 
            // txt_marad
            // 
            this.txt_marad.Location = new System.Drawing.Point(528, 12);
            this.txt_marad.Name = "txt_marad";
            this.txt_marad.ReadOnly = true;
            this.txt_marad.Size = new System.Drawing.Size(44, 20);
            this.txt_marad.TabIndex = 2;
            // 
            // p_game
            // 
            this.p_game.Location = new System.Drawing.Point(2, 41);
            this.p_game.Name = "p_game";
            this.p_game.Size = new System.Drawing.Size(581, 520);
            this.p_game.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 562);
            this.Controls.Add(this.p_game);
            this.Controls.Add(this.txt_marad);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_kever);
            this.Name = "Form1";
            this.Text = "Aknakereső";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_kever;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_marad;
        private System.Windows.Forms.Panel p_game;
    }
}

