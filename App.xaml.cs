using System;
using System.Windows;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CustomAltTab
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Garante que apenas uma instância está rodando
            System.Threading.Mutex mutex = new System.Threading.Mutex(true, "CustomAltTabMutex", out bool createdNew);
            if (!createdNew)
            {
                MessageBox.Show(
                    "Custom Alt+Tab já está em execução!",
                    "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                Application.Current.Shutdown();
                return;
            }

            // Cria ícone na bandeja do sistema
            var notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = CreateCustomIcon();
            notifyIcon.Visible = true;
            notifyIcon.Text = "Custom Alt+Tab";
            
            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            contextMenu.Items.Add("Configurações", null, (s, args) => ShowConfiguration());
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Sair", null, (s, args) => Application.Current.Shutdown());
            
            notifyIcon.ContextMenuStrip = contextMenu;
            notifyIcon.DoubleClick += (s, args) => ShowConfiguration();
            
            // Mantém a referência para não ser coletada pelo GC
            this.Resources["NotifyIcon"] = notifyIcon;
        }

        private Icon CreateCustomIcon()
        {
            // Cria um ícone simples 32x32
            Bitmap bitmap = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Fundo circular azul
                using (var brush = new SolidBrush(Color.FromArgb(255, 0, 120, 215)))
                {
                    g.FillEllipse(brush, 2, 2, 28, 28);
                }
                
                // Borda branca
                using (var pen = new Pen(Color.White, 2))
                {
                    g.DrawEllipse(pen, 2, 2, 28, 28);
                }
                
                // Texto "AT" (Alt+Tab)
                using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
                using (var textBrush = new SolidBrush(Color.White))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString("AT", font, textBrush, new RectangleF(0, 0, 32, 32), sf);
                }
            }
            
            IntPtr hIcon = bitmap.GetHicon();
            Icon icon = Icon.FromHandle(hIcon);
            
            return icon;
        }

        private void ShowConfiguration()
        {
            var configWindow = new ConfigurationWindow();
            configWindow.Show();
            configWindow.Activate();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (this.Resources["NotifyIcon"] is System.Windows.Forms.NotifyIcon notifyIcon)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            }
            base.OnExit(e);
        }
    }
}
