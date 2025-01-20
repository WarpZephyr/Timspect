namespace Timspect.Viewer.WinForms
{
    partial class MainForm
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
            TIMPictureBox = new PictureBox();
            FormMenuStrip = new MenuStrip();
            FileMenu = new ToolStripMenuItem();
            OpenButton = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)TIMPictureBox).BeginInit();
            FormMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // TIMPictureBox
            // 
            TIMPictureBox.BackColor = Color.Black;
            TIMPictureBox.Dock = DockStyle.Fill;
            TIMPictureBox.Location = new Point(0, 24);
            TIMPictureBox.Name = "TIMPictureBox";
            TIMPictureBox.Size = new Size(464, 377);
            TIMPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            TIMPictureBox.TabIndex = 0;
            TIMPictureBox.TabStop = false;
            // 
            // FormMenuStrip
            // 
            FormMenuStrip.Items.AddRange(new ToolStripItem[] { FileMenu });
            FormMenuStrip.Location = new Point(0, 0);
            FormMenuStrip.Name = "FormMenuStrip";
            FormMenuStrip.Size = new Size(464, 24);
            FormMenuStrip.TabIndex = 1;
            FormMenuStrip.Text = "menuStrip1";
            // 
            // FileMenu
            // 
            FileMenu.DropDownItems.AddRange(new ToolStripItem[] { OpenButton });
            FileMenu.Name = "FileMenu";
            FileMenu.Size = new Size(37, 20);
            FileMenu.Text = "File";
            // 
            // OpenButton
            // 
            OpenButton.Name = "OpenButton";
            OpenButton.Size = new Size(180, 22);
            OpenButton.Text = "Open";
            OpenButton.Click += OpenButton_Click;
            // 
            // MainForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(464, 401);
            Controls.Add(TIMPictureBox);
            Controls.Add(FormMenuStrip);
            MainMenuStrip = FormMenuStrip;
            Name = "MainForm";
            Text = "TIM Viewer";
            DragDrop += MainForm_DragDrop;
            DragEnter += MainForm_DragEnter;
            ((System.ComponentModel.ISupportInitialize)TIMPictureBox).EndInit();
            FormMenuStrip.ResumeLayout(false);
            FormMenuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox TIMPictureBox;
        private MenuStrip FormMenuStrip;
        private ToolStripMenuItem FileMenu;
        private ToolStripMenuItem OpenButton;
    }
}
