namespace Timspect.Viewer.WinForms
{
    public partial class MainForm : Form
    {
        public MainForm(Image image) : this()
        {
            UpdateImage(image);
        }

        public MainForm()
        {
            InitializeComponent();
        }

        #region MainForm

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            var data = e.Data;
            if (data == null)
                return;

            if (data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data;
            if (data == null)
                return;

            object? obj = data.GetData(DataFormats.FileDrop);
            if (obj == null)
                return;

            string[] paths = (string[])obj;
            ImportFile(paths[0]);
        }

        #endregion

        #region FileMenu

        private void OpenButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                InitialDirectory = "C:\\Users",
                Title = "Select a file to load.",
                Filter = "All files (*)|*"
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                ImportFile(fileDialog.FileName);
            }
        }

        #endregion

        #region Import

        private void ImportFile(string path)
        {
            var image = ImageHelper.ImportImage(path);
            if (image != null)
            {
                UpdateImage(image);
            }
        }

        private void UpdateImage(Image image)
        {
            TIMPictureBox.Image = image;

            // Set size values
            if (TIMPictureBox.Image != null)
            {
                int width = TIMPictureBox.Image.Width;
                int height = TIMPictureBox.Image.Height;
                ClientSize = new Size(TIMPictureBox.Location.X + width, TIMPictureBox.Location.Y + height);
                TIMPictureBox.ClientSize = new Size(width, height);
            }
        }

        #endregion
    }
}
