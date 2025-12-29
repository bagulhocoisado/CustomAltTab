using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Interop;

namespace CustomAltTab
{
    public partial class OverlayWindow : Window
    {
        private List<WindowInfo> windows;
        private int selectedIndex = 0;
        private List<WindowInfo> selectedAppWindows = new List<WindowInfo>();
        private int selectedAppWindowIndex = 0;
        private Dictionary<FrameworkElement, Storyboard> hoverAnimations = new Dictionary<FrameworkElement, Storyboard>();
        
        // Win32 API para minimizar janela
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        
        private const int SW_MINIMIZE = 6;

        public OverlayWindow()
        {
            InitializeComponent();
            this.KeyDown += OverlayWindow_KeyDown;
            this.KeyUp += OverlayWindow_KeyUp;
            this.MouseWheel += OverlayWindow_MouseWheel;
            this.Loaded += OverlayWindow_Loaded;
            
            // Fazer a janela ficar sempre no topo
            this.Topmost = true;
        }

        private void OverlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshWindows();
            
            if (AppConfig.Instance.DisplayMode == DisplayMode.Wheel)
            {
                DrawWheel();
            }
            else
            {
                DrawGrid();
            }
            
            // Animação de entrada
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            var scaleTransform = new ScaleTransform(0.8, 0.8, this.Width / 2, this.Height / 2);
            MainCanvas.RenderTransform = scaleTransform;
            
            var scaleX = new DoubleAnimation(0.8, 1, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            var scaleY = new DoubleAnimation(0.8, 1, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            
            this.BeginAnimation(Window.OpacityProperty, fadeIn);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleX);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleY);
        }

        private void RefreshWindows()
        {
            windows = WindowManager.GetOpenWindows();
            
            // Organiza as janelas de acordo com as configurações
            var organizedWindows = new List<WindowInfo>();
            var usedWindows = new HashSet<IntPtr>();
            
            // Primeiro, adiciona janelas/ações configuradas em suas posições
            for (int i = 0; i < AppConfig.Instance.WindowSlots.Count; i++)
            {
                var slot = AppConfig.Instance.WindowSlots[i];
                
                // Verifica se é um slot de ação especial
                if (slot.Type == SlotType.MinimizeCurrent)
                {
                    organizedWindows.Add(new WindowInfo
                    {
                        Title = "Minimizar Atual",
                        ProcessName = "MinimizeCurrent",
                        IsPlaceholder = false
                    });
                    continue;
                }
                else if (slot.Type == SlotType.Cancel)
                {
                    organizedWindows.Add(new WindowInfo
                    {
                        Title = "Cancelar",
                        ProcessName = "Cancel",
                        IsPlaceholder = false
                    });
                    continue;
                }
                
                // Slot normal de aplicativo
                if (!string.IsNullOrEmpty(slot.ExecutableName))
                {
                    var matchingWindow = windows.FirstOrDefault(w => 
                        w.ProcessName.Equals(slot.ExecutableName, StringComparison.OrdinalIgnoreCase) &&
                        !usedWindows.Contains(w.Handle));
                    
                    if (matchingWindow != null)
                    {
                        organizedWindows.Add(matchingWindow);
                        usedWindows.Add(matchingWindow.Handle);
                    }
                    else if (AppConfig.Instance.ShowEmptySlots)
                    {
                        organizedWindows.Add(new WindowInfo
                        {
                            Title = slot.ExecutableName,
                            ProcessName = slot.ExecutableName,
                            IsPlaceholder = true
                        });
                    }
                }
                else if (AppConfig.Instance.ShowEmptySlots)
                {
                    organizedWindows.Add(new WindowInfo
                    {
                        Title = "Empty Slot",
                        IsPlaceholder = true
                    });
                }
            }
            
            // Adiciona janelas restantes não configuradas
            if (AppConfig.Instance.ShowUnconfiguredWindows)
            {
                foreach (var window in windows)
                {
                    if (!usedWindows.Contains(window.Handle))
                    {
                        organizedWindows.Add(window);
                    }
                }
            }
            
            windows = organizedWindows;
        }

        private void DrawWheel()
        {
            MainCanvas.Children.Clear();
            
            double centerX = this.Width / 2;
            double centerY = this.Height / 2;
            double radius = 250;
            int slotCount = Math.Max(windows.Count, AppConfig.Instance.MinSlots);
            
            // Desenha círculo central
            var centerCircle = new Ellipse
            {
                Width = 100,
                Height = 100,
                Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(180, 40, 40, 40)),
                Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 100, 100, 100)),
                StrokeThickness = 2
            };
            Canvas.SetLeft(centerCircle, centerX - 50);
            Canvas.SetTop(centerCircle, centerY - 50);
            MainCanvas.Children.Add(centerCircle);
            
            // Texto central
            var centerText = new TextBlock
            {
                Text = "Alt+Tab",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.White,
                TextAlignment = TextAlignment.Center
            };
            Canvas.SetLeft(centerText, centerX - 35);
            Canvas.SetTop(centerText, centerY - 10);
            MainCanvas.Children.Add(centerText);
            
            // Desenha slots em círculo
            for (int i = 0; i < slotCount; i++)
            {
                double angle = (Math.PI * 2 * i / slotCount) - Math.PI / 2;
                double x = centerX + radius * Math.Cos(angle);
                double y = centerY + radius * Math.Sin(angle);
                
                bool isSelected = i == selectedIndex;
                var window = i < windows.Count ? windows[i] : null;
                
                DrawSlot(x, y, window, isSelected, i);
            }
            
            // Botão de configuração
            DrawConfigButton();
        }

        private void DrawGrid()
        {
            MainCanvas.Children.Clear();
            
            int columns = (int)Math.Ceiling(Math.Sqrt(windows.Count));
            int rows = (int)Math.Ceiling((double)windows.Count / columns);
            
            double slotWidth = 200;
            double slotHeight = 150;
            double spacing = 20;
            
            double totalWidth = columns * slotWidth + (columns - 1) * spacing;
            double totalHeight = rows * slotHeight + (rows - 1) * spacing;
            
            double startX = (this.Width - totalWidth) / 2;
            double startY = (this.Height - totalHeight) / 2;
            
            for (int i = 0; i < windows.Count; i++)
            {
                int row = i / columns;
                int col = i % columns;
                
                double x = startX + col * (slotWidth + spacing);
                double y = startY + row * (slotHeight + spacing);
                
                bool isSelected = i == selectedIndex;
                DrawGridSlot(x, y, slotWidth, slotHeight, windows[i], isSelected, i);
            }
            
            // Botão de configuração
            DrawConfigButton();
        }

        private void DrawSlot(double x, double y, WindowInfo window, bool isSelected, int index)
        {
            double size = isSelected ? 100 : 80;
            
            // Verifica se é um slot especial
            var slot = index < AppConfig.Instance.WindowSlots.Count ? AppConfig.Instance.WindowSlots[index] : null;
            bool isSpecialSlot = slot != null && (slot.Type == SlotType.MinimizeCurrent || slot.Type == SlotType.Cancel);
            
            // Container para animações
            var slotContainer = new Canvas();
            
            // Sombra externa (glow effect)
            if (isSelected)
            {
                var glowEllipse = new Ellipse
                {
                    Width = size + 30,
                    Height = size + 30,
                    Fill = new RadialGradientBrush
                    {
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop(System.Windows.Media.Color.FromArgb(100, 0, 150, 255), 0),
                            new GradientStop(System.Windows.Media.Color.FromArgb(0, 0, 150, 255), 1)
                        }
                    }
                };
                Canvas.SetLeft(glowEllipse, x - (size + 30) / 2);
                Canvas.SetTop(glowEllipse, y - (size + 30) / 2);
                MainCanvas.Children.Add(glowEllipse);
                
                // Animação de pulso
                var pulseAnimation = new DoubleAnimation
                {
                    From = 0.8,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(1),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };
                glowEllipse.BeginAnimation(UIElement.OpacityProperty, pulseAnimation);
            }
            
            // Background do slot com gradiente
            var slotBg = new Ellipse
            {
                Width = size,
                Height = size,
                RenderTransform = new ScaleTransform(1, 1, size / 2, size / 2),
                RenderTransformOrigin = new System.Windows.Point(0.5, 0.5)
            };
            
            // Cor baseada no tipo de slot
            if (isSpecialSlot)
            {
                if (slot.Type == SlotType.MinimizeCurrent)
                {
                    slotBg.Fill = new RadialGradientBrush
                    {
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop(System.Windows.Media.Color.FromArgb(220, 255, 165, 0), 0),
                            new GradientStop(System.Windows.Media.Color.FromArgb(180, 200, 100, 0), 1)
                        }
                    };
                }
                else // Cancel
                {
                    slotBg.Fill = new RadialGradientBrush
                    {
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop(System.Windows.Media.Color.FromArgb(220, 220, 50, 50), 0),
                            new GradientStop(System.Windows.Media.Color.FromArgb(180, 150, 30, 30), 1)
                        }
                    };
                }
            }
            else if (window != null && !window.IsPlaceholder)
            {
                slotBg.Fill = new RadialGradientBrush
                {
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(System.Windows.Media.Color.FromArgb(230, 80, 80, 90), 0),
                        new GradientStop(System.Windows.Media.Color.FromArgb(200, 40, 40, 50), 1)
                    }
                };
            }
            else
            {
                slotBg.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(100, 40, 40, 40));
            }
            
            slotBg.Stroke = new SolidColorBrush(isSelected 
                ? System.Windows.Media.Color.FromArgb(255, 0, 200, 255)
                : System.Windows.Media.Color.FromArgb(150, 100, 100, 100));
            slotBg.StrokeThickness = isSelected ? 4 : 2;
            
            // Efeito de sombra
            slotBg.Effect = new DropShadowEffect
            {
                Color = isSelected ? System.Windows.Media.Color.FromRgb(0, 150, 255) : Colors.Black,
                BlurRadius = isSelected ? 25 : 15,
                ShadowDepth = 5,
                Opacity = isSelected ? 0.8 : 0.6
            };
            
            Canvas.SetLeft(slotBg, x - size / 2);
            Canvas.SetTop(slotBg, y - size / 2);
            MainCanvas.Children.Add(slotBg);
            
            // Conteúdo do slot
            if (isSpecialSlot)
            {
                // Ícone especial
                var iconText = new TextBlock
                {
                    Text = slot.Type == SlotType.MinimizeCurrent ? "━" : "✕",
                    FontSize = size * 0.5,
                    FontWeight = FontWeights.Bold,
                    Foreground = System.Windows.Media.Brushes.White,
                    TextAlignment = TextAlignment.Center,
                    Width = size,
                    Height = size,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Canvas.SetLeft(iconText, x - size / 2);
                Canvas.SetTop(iconText, y - size * 0.25);
                MainCanvas.Children.Add(iconText);
                
                // Label
                var labelText = new TextBlock
                {
                    Text = slot.Type == SlotType.MinimizeCurrent ? "Minimizar" : "Cancelar",
                    FontSize = 12,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = System.Windows.Media.Brushes.White,
                    TextAlignment = TextAlignment.Center,
                    Width = size + 60
                };
                Canvas.SetLeft(labelText, x - (size + 60) / 2);
                Canvas.SetTop(labelText, y + size / 2 + 8);
                MainCanvas.Children.Add(labelText);
            }
            else if (window != null && !window.IsPlaceholder)
            {
                // Ícone do aplicativo
                var icon = new System.Windows.Controls.Image
                {
                    Width = size * 0.55,
                    Height = size * 0.55,
                    Source = window.Icon,
                    Effect = new DropShadowEffect
                    {
                        Color = Colors.Black,
                        BlurRadius = 5,
                        ShadowDepth = 2,
                        Opacity = 0.5
                    }
                };
                Canvas.SetLeft(icon, x - size * 0.275);
                Canvas.SetTop(icon, y - size * 0.375);
                MainCanvas.Children.Add(icon);
                
                // Nome da janela com fundo semi-transparente
                var nameBg = new Border
                {
                    Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(180, 0, 0, 0)),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(10, 4, 10, 4),
                    Child = new TextBlock
                    {
                        Text = window.Title.Length > 20 ? window.Title.Substring(0, 17) + "..." : window.Title,
                        FontSize = 12,
                        FontWeight = FontWeights.Medium,
                        Foreground = System.Windows.Media.Brushes.White,
                        TextAlignment = TextAlignment.Center
                    }
                };
                Canvas.SetLeft(nameBg, x - 60);
                Canvas.SetTop(nameBg, y + size / 2 + 10);
                MainCanvas.Children.Add(nameBg);
                
                // Contador de janelas múltiplas
                var sameAppCount = windows.Count(w => !w.IsPlaceholder && 
                    w.ProcessName.Equals(window.ProcessName, StringComparison.OrdinalIgnoreCase));
                if (sameAppCount > 1)
                {
                    var countBadge = new Border
                    {
                        Width = 24,
                        Height = 24,
                        Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(230, 255, 100, 0)),
                        CornerRadius = new CornerRadius(12),
                        BorderBrush = System.Windows.Media.Brushes.White,
                        BorderThickness = new Thickness(2),
                        Child = new TextBlock
                        {
                            Text = sameAppCount.ToString(),
                            FontSize = 11,
                            FontWeight = FontWeights.Bold,
                            Foreground = System.Windows.Media.Brushes.White,
                            TextAlignment = TextAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    };
                    Canvas.SetLeft(countBadge, x + size / 2 - 20);
                    Canvas.SetTop(countBadge, y - size / 2 + 5);
                    MainCanvas.Children.Add(countBadge);
                }
            }
            else if (window != null && window.IsPlaceholder)
            {
                // Placeholder com ícone de adicionar
                var placeholderIcon = new TextBlock
                {
                    Text = "+",
                    FontSize = size * 0.4,
                    Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(150, 200, 200, 200)),
                    TextAlignment = TextAlignment.Center,
                    Width = size
                };
                Canvas.SetLeft(placeholderIcon, x - size / 2);
                Canvas.SetTop(placeholderIcon, y - size * 0.2);
                MainCanvas.Children.Add(placeholderIcon);
                
                var placeholderText = new TextBlock
                {
                    Text = window.Title.Length > 12 ? window.Title.Substring(0, 10) + "..." : window.Title,
                    FontSize = 10,
                    Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(150, 200, 200, 200)),
                    TextAlignment = TextAlignment.Center,
                    Width = size + 20
                };
                Canvas.SetLeft(placeholderText, x - (size + 20) / 2);
                Canvas.SetTop(placeholderText, y + size / 2 + 5);
                MainCanvas.Children.Add(placeholderText);
            }
            
            // Interatividade
            slotBg.Tag = index;
            slotBg.MouseEnter += (s, e) => Slot_MouseEnter(s, e);
            slotBg.MouseLeftButtonDown += Slot_Click;
            slotBg.Cursor = Cursors.Hand;
        }

        private void DrawGridSlot(double x, double y, double width, double height, WindowInfo window, bool isSelected, int index)
        {
            var slotBorder = new Border
            {
                Width = width,
                Height = height,
                Background = new SolidColorBrush(window != null && !window.IsPlaceholder 
                    ? System.Windows.Media.Color.FromArgb(200, 50, 50, 50)
                    : System.Windows.Media.Color.FromArgb(80, 40, 40, 40)),
                BorderBrush = new SolidColorBrush(isSelected 
                    ? System.Windows.Media.Color.FromArgb(255, 0, 150, 255)
                    : System.Windows.Media.Color.FromArgb(150, 80, 80, 80)),
                BorderThickness = new Thickness(isSelected ? 3 : 2),
                CornerRadius = new CornerRadius(8)
            };
            
            Canvas.SetLeft(slotBorder, x);
            Canvas.SetTop(slotBorder, y);
            MainCanvas.Children.Add(slotBorder);
            
            if (window != null && !window.IsPlaceholder)
            {
                var stackPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                
                // Ícone
                var icon = new System.Windows.Controls.Image
                {
                    Width = 64,
                    Height = 64,
                    Source = window.Icon,
                    Margin = new Thickness(0, 10, 0, 10)
                };
                stackPanel.Children.Add(icon);
                
                // Nome
                var nameText = new TextBlock
                {
                    Text = window.Title.Length > 25 ? window.Title.Substring(0, 22) + "..." : window.Title,
                    FontSize = 14,
                    Foreground = System.Windows.Media.Brushes.White,
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    MaxWidth = width - 20
                };
                stackPanel.Children.Add(nameText);
                
                // Processo
                var processText = new TextBlock
                {
                    Text = window.ProcessName,
                    FontSize = 10,
                    Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 180, 180, 180)),
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 5, 0, 0)
                };
                stackPanel.Children.Add(processText);
                
                Canvas.SetLeft(stackPanel, x + (width - stackPanel.ActualWidth) / 2);
                Canvas.SetTop(stackPanel, y + (height - stackPanel.ActualHeight) / 2);
                slotBorder.Child = stackPanel;
            }
            
            slotBorder.Tag = index;
            slotBorder.MouseEnter += GridSlot_MouseEnter;
            slotBorder.MouseLeftButtonDown += Slot_Click;
        }

        private void DrawConfigButton()
        {
            var configButton = new Border
            {
                Width = 50,
                Height = 50,
                Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(180, 60, 60, 60)),
                BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 100, 100, 100)),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(25),
                Cursor = Cursors.Hand
            };
            
            var configIcon = new TextBlock
            {
                Text = "⚙",
                FontSize = 28,
                Foreground = System.Windows.Media.Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            configButton.Child = configIcon;
            
            Canvas.SetRight(configButton, 20);
            Canvas.SetTop(configButton, 20);
            MainCanvas.Children.Add(configButton);
            
            configButton.MouseLeftButtonDown += ConfigButton_Click;
        }

        private void Slot_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is int index)
            {
                selectedIndex = index;
                if (AppConfig.Instance.DisplayMode == DisplayMode.Wheel)
                {
                    DrawWheel();
                }
                else
                {
                    DrawGrid();
                }
            }
        }

        private void GridSlot_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is int index)
            {
                selectedIndex = index;
                DrawGrid();
            }
        }

        private void Slot_Click(object sender, MouseButtonEventArgs e)
        {
            ActivateSelectedWindow();
        }

        private void ConfigButton_Click(object sender, MouseButtonEventArgs e)
        {
            var configWindow = new ConfigurationWindow();
            this.Close();
            configWindow.ShowDialog();
        }

        private void OverlayWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                selectedIndex = (selectedIndex + 1) % windows.Count;
                if (AppConfig.Instance.DisplayMode == DisplayMode.Wheel)
                {
                    DrawWheel();
                }
                else
                {
                    DrawGrid();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Left || e.Key == Key.Up)
            {
                selectedIndex = (selectedIndex - 1 + windows.Count) % windows.Count;
                if (AppConfig.Instance.DisplayMode == DisplayMode.Wheel)
                {
                    DrawWheel();
                }
                else
                {
                    DrawGrid();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Right || e.Key == Key.Down)
            {
                selectedIndex = (selectedIndex + 1) % windows.Count;
                if (AppConfig.Instance.DisplayMode == DisplayMode.Wheel)
                {
                    DrawWheel();
                }
                else
                {
                    DrawGrid();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                ActivateSelectedWindow();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                this.Close();
                e.Handled = true;
            }
        }

        private void OverlayWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                ActivateSelectedWindow();
            }
        }

        private void OverlayWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Scroll para alternar entre múltiplas janelas do mesmo app
            if (selectedIndex < windows.Count && windows[selectedIndex] != null)
            {
                var currentWindow = windows[selectedIndex];
                if (!currentWindow.IsPlaceholder)
                {
                    var sameAppWindows = windows.Where(w => 
                        !w.IsPlaceholder && 
                        w.ProcessName.Equals(currentWindow.ProcessName, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    
                    if (sameAppWindows.Count > 1)
                    {
                        int currentAppIndex = sameAppWindows.IndexOf(currentWindow);
                        if (e.Delta > 0)
                        {
                            currentAppIndex = (currentAppIndex + 1) % sameAppWindows.Count;
                        }
                        else
                        {
                            currentAppIndex = (currentAppIndex - 1 + sameAppWindows.Count) % sameAppWindows.Count;
                        }
                        
                        selectedIndex = windows.IndexOf(sameAppWindows[currentAppIndex]);
                        if (AppConfig.Instance.DisplayMode == DisplayMode.Wheel)
                        {
                            DrawWheel();
                        }
                        else
                        {
                            DrawGrid();
                        }
                    }
                }
            }
        }

        private void ActivateSelectedWindow()
        {
            if (selectedIndex < windows.Count && windows[selectedIndex] != null)
            {
                // Verifica se é um slot especial
                if (selectedIndex < AppConfig.Instance.WindowSlots.Count)
                {
                    var slot = AppConfig.Instance.WindowSlots[selectedIndex];
                    
                    if (slot.Type == SlotType.Cancel)
                    {
                        // Apenas fecha sem fazer nada
                        this.Close();
                        return;
                    }
                    else if (slot.Type == SlotType.MinimizeCurrent)
                    {
                        // Minimiza a janela atual (foreground)
                        IntPtr currentWindow = GetForegroundWindow();
                        if (currentWindow != IntPtr.Zero)
                        {
                            ShowWindow(currentWindow, SW_MINIMIZE);
                        }
                        this.Close();
                        return;
                    }
                }
                
                // Slot normal - ativa a janela
                if (!windows[selectedIndex].IsPlaceholder)
                {
                    var window = windows[selectedIndex];
                    WindowManager.ActivateWindow(window.Handle);
                }
            }
            this.Close();
        }
    }
}
