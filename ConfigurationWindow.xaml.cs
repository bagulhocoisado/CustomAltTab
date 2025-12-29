using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CustomAltTab
{
    public partial class ConfigurationWindow : Window
    {
        public ConfigurationWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Modo de exibição
            DisplayModeCombo.SelectedIndex = AppConfig.Instance.DisplayMode == DisplayMode.Wheel ? 0 : 1;
            
            // Slots
            MinSlotsSlider.Value = AppConfig.Instance.MinSlots;
            MinSlotsText.Text = AppConfig.Instance.MinSlots.ToString();
            
            // Checkboxes
            ShowEmptySlotsCheck.IsChecked = AppConfig.Instance.ShowEmptySlots;
            ShowUnconfiguredCheck.IsChecked = AppConfig.Instance.ShowUnconfiguredWindows;
            
            // Lista de slots
            RefreshSlotsList();
        }

        private void RefreshSlotsList()
        {
            SlotsListBox.Items.Clear();
            
            for (int i = 0; i < AppConfig.Instance.WindowSlots.Count; i++)
            {
                var slot = AppConfig.Instance.WindowSlots[i];
                var item = new ListBoxItem
                {
                    Content = $"Slot {i + 1}: {(string.IsNullOrEmpty(slot.ExecutableName) ? "(vazio)" : slot.ExecutableName)}",
                    Tag = i
                };
                SlotsListBox.Items.Add(item);
            }
        }

        private void DisplayModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DisplayModeCombo.SelectedIndex >= 0)
            {
                AppConfig.Instance.DisplayMode = DisplayModeCombo.SelectedIndex == 0 
                    ? DisplayMode.Wheel 
                    : DisplayMode.Grid;
            }
        }

        private void MinSlotsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MinSlotsText != null)
            {
                int value = (int)e.NewValue;
                MinSlotsText.Text = value.ToString();
                AppConfig.Instance.MinSlots = value;
            }
        }

        private void ShowEmptySlotsCheck_Changed(object sender, RoutedEventArgs e)
        {
            AppConfig.Instance.ShowEmptySlots = ShowEmptySlotsCheck.IsChecked == true;
        }

        private void ShowUnconfiguredCheck_Changed(object sender, RoutedEventArgs e)
        {
            AppConfig.Instance.ShowUnconfiguredWindows = ShowUnconfiguredCheck.IsChecked == true;
        }

        private void AddSlot_Click(object sender, RoutedEventArgs e)
        {
            AppConfig.Instance.WindowSlots.Add(new WindowSlot());
            RefreshSlotsList();
        }

        private void EditSlot_Click(object sender, RoutedEventArgs e)
        {
            if (SlotsListBox.SelectedItem is ListBoxItem item && item.Tag is int index)
            {
                var slot = AppConfig.Instance.WindowSlots[index];
                var dialog = new SlotEditorDialog(slot, index + 1);
                if (dialog.ShowDialog() == true)
                {
                    RefreshSlotsList();
                }
            }
            else
            {
                MessageBox.Show("Selecione um slot para editar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RemoveSlot_Click(object sender, RoutedEventArgs e)
        {
            if (SlotsListBox.SelectedItem is ListBoxItem item && item.Tag is int index)
            {
                var result = MessageBox.Show(
                    $"Deseja remover o Slot {index + 1}?", 
                    "Confirmar", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    AppConfig.Instance.WindowSlots.RemoveAt(index);
                    RefreshSlotsList();
                }
            }
            else
            {
                MessageBox.Show("Selecione um slot para remover.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            AppConfig.Save();
            MessageBox.Show("Configurações salvas com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            AppConfig.Load(); // Recarrega para descartar mudanças
            this.Close();
        }
    }
}
