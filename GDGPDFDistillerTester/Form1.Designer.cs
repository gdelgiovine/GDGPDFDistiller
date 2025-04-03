namespace GDGPDFDistillerTester
{
    partial class Form1
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
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            propertyGrid1 = new PropertyGrid();
            propertyGrid2 = new PropertyGrid();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(355, 38);
            button1.Name = "button1";
            button1.Size = new Size(148, 23);
            button1.TabIndex = 0;
            button1.Text = "Convert HTML -> PDF";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(355, 80);
            button2.Name = "button2";
            button2.Size = new Size(148, 23);
            button2.TabIndex = 1;
            button2.Text = "Convert PS -> PDF";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(355, 118);
            button3.Name = "button3";
            button3.Size = new Size(148, 23);
            button3.TabIndex = 2;
            button3.Text = "Convert PCL -> PDF";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // propertyGrid1
            // 
            propertyGrid1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            propertyGrid1.Location = new Point(12, 12);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.Size = new Size(337, 426);
            propertyGrid1.TabIndex = 3;
            // 
            // propertyGrid2
            // 
            propertyGrid2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            propertyGrid2.Location = new Point(509, 12);
            propertyGrid2.Name = "propertyGrid2";
            propertyGrid2.Size = new Size(337, 426);
            propertyGrid2.TabIndex = 4;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(851, 450);
            Controls.Add(propertyGrid2);
            Controls.Add(propertyGrid1);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button button2;
        private Button button3;
        private PropertyGrid propertyGrid1;
        private PropertyGrid propertyGrid2;
    }
}
