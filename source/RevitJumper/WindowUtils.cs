using System;
using System.Diagnostics;
using System.Windows.Interop;

namespace RevitJumper
{
    public static class WindowsUtil
    {
        public static bool? ShowHostDialog(this System.Windows.Window window)
        {
            Process process = Process.GetCurrentProcess();

            IntPtr mainWindowHandle = process.MainWindowHandle;

            var helper = new WindowInteropHelper(window);

            helper.Owner = mainWindowHandle;

            return window.ShowDialog();
        }
    }
}
