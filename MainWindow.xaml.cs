using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CustomAltTab
{
    public partial class MainWindow : Window
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int VK_TAB = 0x09;
        private const int VK_MENU = 0x12; // Alt key
        private const int VK_LMENU = 0xA4;
        private const int VK_RMENU = 0xA5;

        private IntPtr hookID = IntPtr.Zero;
        private LowLevelKeyboardProc proc;
        private DateTime altPressTime;
        private bool isAltPressed = false;
        private bool isTabPressed = false;
        private bool overlayShown = false;
        private DispatcherTimer holdTimer;
        private DispatcherTimer fastSwitchTimer;
        private ConfigurationWindow configWindow;

        public MainWindow()
        {
            InitializeComponent();
            proc = HookCallback;
            hookID = SetHook(proc);
            
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            
            holdTimer = new DispatcherTimer();
            holdTimer.Interval = TimeSpan.FromMilliseconds(250); // 250ms para considerar "segurado"
            holdTimer.Tick += HoldTimer_Tick;
            
            fastSwitchTimer = new DispatcherTimer();
            fastSwitchTimer.Interval = TimeSpan.FromMilliseconds(100);
            fastSwitchTimer.Tick += FastSwitchTimer_Tick;

            LoadConfiguration();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UnhookWindowsHookEx(hookID);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                
                if (wParam == (IntPtr)WM_KEYDOWN)
                {
                    if (vkCode == VK_MENU || vkCode == VK_LMENU || vkCode == VK_RMENU)
                    {
                        if (!isAltPressed)
                        {
                            isAltPressed = true;
                            altPressTime = DateTime.Now;
                        }
                    }
                    else if (vkCode == VK_TAB && isAltPressed)
                    {
                        if (!isTabPressed)
                        {
                            isTabPressed = true;
                            holdTimer.Start();
                            return (IntPtr)1; // Bloqueia o Alt+Tab padrão
                        }
                    }
                }
                else if (wParam == (IntPtr)WM_KEYUP)
                {
                    if (vkCode == VK_MENU || vkCode == VK_LMENU || vkCode == VK_RMENU)
                    {
                        isAltPressed = false;
                        holdTimer.Stop();
                        
                        if (overlayShown)
                        {
                            Dispatcher.Invoke(() => HideOverlay());
                        }
                        else if (isTabPressed)
                        {
                            // Alt+Tab rápido - comportamento padrão do Windows
                            Dispatcher.Invoke(() => PerformFastSwitch());
                        }
                        
                        isTabPressed = false;
                    }
                    else if (vkCode == VK_TAB)
                    {
                        isTabPressed = false;
                    }
                }
            }
            
            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        private void HoldTimer_Tick(object sender, EventArgs e)
        {
            if (isAltPressed && isTabPressed && !overlayShown)
            {
                ShowOverlay();
                holdTimer.Stop();
            }
        }

        private void ShowOverlay()
        {
            overlayShown = true;
            var overlayWindow = new OverlayWindow();
            overlayWindow.ShowDialog();
        }

        private void HideOverlay()
        {
            overlayShown = false;
        }

        private void PerformFastSwitch()
        {
            // Simula Alt+Tab padrão do Windows
            keybd_event(VK_MENU, 0, 0, 0);
            keybd_event(VK_TAB, 0, 0, 0);
            keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, 0);
        }

        private void FastSwitchTimer_Tick(object sender, EventArgs e)
        {
            fastSwitchTimer.Stop();
        }

        private void LoadConfiguration()
        {
            // Carrega configuração do arquivo
            AppConfig.Load();
        }

        private void ShowConfiguration()
        {
            if (configWindow == null || !configWindow.IsVisible)
            {
                configWindow = new ConfigurationWindow();
                configWindow.Show();
            }
            else
            {
                configWindow.Activate();
            }
        }

        // Windows API
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern void keybd_event(int bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private const int KEYEVENTF_KEYUP = 0x0002;
    }
}
