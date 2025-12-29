using System;
using System.Windows;

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
            notifyIcon.Icon = new System.Drawing.Icon(SystemIcons.Application, 40, 40);
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
