using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CustomAltTab
{
    public class WindowInfo
    {
        public IntPtr Handle { get; set; }
        public string Title { get; set; }
        public string ProcessName { get; set; }
        public ImageSource Icon { get; set; }
        public bool IsPlaceholder { get; set; }

        public WindowInfo()
        {
            IsPlaceholder = false;
        }
    }

    public static class WindowManager
    {
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_APPWINDOW = 0x00040000;
        private const int SW_RESTORE = 9;
        private const uint WM_GETICON = 0x007F;
        private const int ICON_SMALL = 0;
        private const int ICON_BIG = 1;
        private const uint GW_OWNER = 4;

        public static List<WindowInfo> GetOpenWindows()
        {
            List<WindowInfo> windows = new List<WindowInfo>();
            HashSet<string> addedProcesses = new HashSet<string>();

            EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
            {
                if (!IsWindowVisible(hWnd))
                    return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0)
                    return true;

                // Verifica se é uma janela de aplicativo válida
                IntPtr exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
                if (((long)exStyle & WS_EX_TOOLWINDOW) != 0)
                    return true;

                IntPtr owner = GetWindow(hWnd, GW_OWNER);
                if (owner != IntPtr.Zero)
                    return true;

                StringBuilder builder = new StringBuilder(length + 1);
                GetWindowText(hWnd, builder, builder.Capacity);
                string title = builder.ToString();

                if (string.IsNullOrWhiteSpace(title))
                    return true;

                uint processId;
                GetWindowThreadProcessId(hWnd, out processId);

                try
                {
                    Process process = Process.GetProcessById((int)processId);
                    string processName = process.ProcessName;

                    // Ignora o próprio aplicativo
                    if (processName.Equals("CustomAltTab", StringComparison.OrdinalIgnoreCase))
                        return true;

                    // Obtém o ícone
                    ImageSource icon = GetWindowIcon(hWnd, process);

                    windows.Add(new WindowInfo
                    {
                        Handle = hWnd,
                        Title = title,
                        ProcessName = processName,
                        Icon = icon
                    });
                }
                catch
                {
                    // Ignora processos que não podem ser acessados
                }

                return true;
            }, IntPtr.Zero);

            return windows;
        }

        private static ImageSource GetWindowIcon(IntPtr hWnd, Process process)
        {
            try
            {
                // Tenta obter o ícone da janela
                IntPtr iconHandle = SendMessage(hWnd, WM_GETICON, (IntPtr)ICON_BIG, IntPtr.Zero);
                if (iconHandle == IntPtr.Zero)
                {
                    iconHandle = SendMessage(hWnd, WM_GETICON, (IntPtr)ICON_SMALL, IntPtr.Zero);
                }

                if (iconHandle == IntPtr.Zero)
                {
                    // Tenta obter do executável
                    try
                    {
                        var icon = System.Drawing.Icon.ExtractAssociatedIcon(process.MainModule.FileName);
                        if (icon != null)
                        {
                            return Imaging.CreateBitmapSourceFromHIcon(
                                icon.Handle,
                                Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions());
                        }
                    }
                    catch { }
                }
                else
                {
                    return Imaging.CreateBitmapSourceFromHIcon(
                        iconHandle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
            catch { }

            // Retorna ícone padrão se não conseguir obter
            return CreateDefaultIcon();
        }

        private static ImageSource CreateDefaultIcon()
        {
            // Cria um ícone padrão simples
            var drawingVisual = new DrawingVisual();
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                dc.DrawRectangle(Brushes.Gray, null, new Rect(0, 0, 32, 32));
                dc.DrawRectangle(Brushes.White, null, new Rect(8, 8, 16, 16));
            }

            var rtb = new RenderTargetBitmap(32, 32, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(drawingVisual);
            return rtb;
        }

        public static void ActivateWindow(IntPtr hWnd)
        {
            try
            {
                // Restaura a janela se estiver minimizada
                ShowWindow(hWnd, SW_RESTORE);
                
                // Traz a janela para frente
                SetForegroundWindow(hWnd);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao ativar janela: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
