namespace MidiMapper
{
    partial class Principal
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1=new Panel();
            button3=new Button();
            button2=new Button();
            panel6=new Panel();
            label2=new Label();
            comboBoxSortieLoop=new ComboBox();
            comboBoxEntreeLoop=new ComboBox();
            panel3=new Panel();
            comboBox1=new ComboBox();
            checkBox2=new CheckBox();
            panel4=new Panel();
            panel2=new Panel();
            label1=new Label();
            panel1.SuspendLayout();
            panel6.SuspendLayout();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(button3);
            panel1.Controls.Add(button2);
            panel1.Dock=DockStyle.Top;
            panel1.Location=new Point(0, 0);
            panel1.Name="panel1";
            panel1.Size=new Size(998, 70);
            panel1.TabIndex=0;
            // 
            // button3
            // 
            button3.Location=new Point(486, 12);
            button3.Name="button3";
            button3.Size=new Size(150, 46);
            button3.TabIndex=0;
            button3.Text="Save";
            button3.UseVisualStyleBackColor=true;
            button3.Click+=save_Click;
            // 
            // button2
            // 
            button2.Location=new Point(330, 12);
            button2.Name="button2";
            button2.Size=new Size(150, 46);
            button2.TabIndex=0;
            button2.Text="Load";
            button2.UseVisualStyleBackColor=true;
            button2.Click+=load_Click;
            // 
            // panel6
            // 
            panel6.Controls.Add(label2);
            panel6.Controls.Add(comboBoxSortieLoop);
            panel6.Controls.Add(comboBoxEntreeLoop);
            panel6.Dock=DockStyle.Top;
            panel6.Location=new Point(0, 70);
            panel6.Name="panel6";
            panel6.Size=new Size(998, 70);
            panel6.TabIndex=5;
            // 
            // label2
            // 
            label2.AutoSize=true;
            label2.Location=new Point(609, 18);
            label2.Name="label2";
            label2.Size=new Size(67, 32);
            label2.TabIndex=1;
            label2.Text="Loop";
            // 
            // comboBoxSortieLoop
            // 
            comboBoxSortieLoop.FormattingEnabled=true;
            comboBoxSortieLoop.Location=new Point(276, 15);
            comboBoxSortieLoop.Name="comboBoxSortieLoop";
            comboBoxSortieLoop.Size=new Size(242, 40);
            comboBoxSortieLoop.TabIndex=0;
            comboBoxSortieLoop.SelectedIndexChanged+=midiOutLoopPortListBox_SelectionChanged;
            // 
            // comboBoxEntreeLoop
            // 
            comboBoxEntreeLoop.FormattingEnabled=true;
            comboBoxEntreeLoop.Location=new Point(12, 15);
            comboBoxEntreeLoop.Name="comboBoxEntreeLoop";
            comboBoxEntreeLoop.Size=new Size(242, 40);
            comboBoxEntreeLoop.TabIndex=0;
            comboBoxEntreeLoop.SelectedIndexChanged+=midiInLoopPortListBox_SelectionChanged;
            // 
            // panel3
            // 
            panel3.Controls.Add(comboBox1);
            panel3.Controls.Add(checkBox2);
            panel3.Dock=DockStyle.Top;
            panel3.Location=new Point(0, 0);
            panel3.Name="panel3";
            panel3.Size=new Size(998, 70);
            panel3.TabIndex=6;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled=true;
            comboBox1.Location=new Point(481, 16);
            comboBox1.Name="comboBox1";
            comboBox1.Size=new Size(242, 40);
            comboBox1.TabIndex=3;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize=true;
            checkBox2.Location=new Point(59, 18);
            checkBox2.Name="checkBox2";
            checkBox2.Size=new Size(159, 36);
            checkBox2.TabIndex=2;
            checkBox2.Text="checkBox2";
            checkBox2.UseVisualStyleBackColor=true;
            // 
            // panel4
            // 
            panel4.Controls.Add(panel3);
            panel4.Dock=DockStyle.Fill;
            panel4.Location=new Point(0, 190);
            panel4.Name="panel4";
            panel4.Size=new Size(998, 260);
            panel4.TabIndex=7;
            // 
            // panel2
            // 
            panel2.Controls.Add(label1);
            panel2.Dock=DockStyle.Top;
            panel2.Location=new Point(0, 140);
            panel2.Name="panel2";
            panel2.Size=new Size(998, 50);
            panel2.TabIndex=7;
            // 
            // label1
            // 
            label1.AutoSize=true;
            label1.Location=new Point(53, 7);
            label1.Name="label1";
            label1.Size=new Size(160, 32);
            label1.TabIndex=0;
            label1.Text="Midi devices :";
            // 
            // Principal
            // 
            AutoScaleDimensions=new SizeF(13F, 32F);
            AutoScaleMode=AutoScaleMode.Font;
            ClientSize=new Size(998, 450);
            Controls.Add(panel4);
            Controls.Add(panel2);
            Controls.Add(panel6);
            Controls.Add(panel1);
            Name="Principal";
            Text="Form1";
            panel1.ResumeLayout(false);
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel4.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button button3;
        private Button button2;
        private Panel panel6;
        private Label label2;
        private ComboBox comboBoxSortieLoop;
        private ComboBox comboBoxEntreeLoop;
        private Panel panel3;
        private CheckBox checkBox2;
        private Panel panel4;
        private Panel panel2;
        private Label label1;
        private ComboBox comboBox1;
    }
}