using System;
using System.Windows.Forms;

namespace LabWork
{
    static class LaunchForm
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();

            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}