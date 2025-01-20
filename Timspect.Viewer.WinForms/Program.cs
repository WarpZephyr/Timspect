namespace Timspect.Viewer.WinForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            MainForm form;
            if (args.Length > 0 && File.Exists(args[0]))
            {
                var image = ImageHelper.ImportImage(args[0]);
                if (image != null)
                {
                    form = new MainForm(image);
                }
                else
                {
                    form = new MainForm();
                }
            }
            else
            {
                form = new MainForm();
            }

            Application.Run(form);
        }
    }
}