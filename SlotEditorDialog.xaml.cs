using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace CustomAltTab
{
    public partial class SlotEditorDialog : Window
    {
        private WindowSlot slot;
        private int slotNumber;

        public SlotEditorDialog(WindowSlot slot, int slotNumber)
        {
            InitializeComponent();
            this.slot = slot;
            this.slotNumber = slotNumber;
            
            SlotNumberText.Text = $"Configurar Slot {slotNumber}";
            
            // Define tipo do slot
            SlotTypeCombo.SelectedIndex = (int)slot.Type;
            
            ExecutableNameText.Text = slot.ExecutableName;
            
            LoadRunningProcesses();
            UpdateVisibility();
        }

        private void LoadRunningProcesses()
        {
            var processes = WindowManager.GetOpenWindows()
                .Select(w => w.ProcessName)
                .Distinct()
                .OrderBy(p => p)
                .ToList();
            
            foreach (var process in processes)
            {
                ProcessCombo.Items.Add(process);
            }
        }

        private void SlotTypeCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            if (SlotTypeCombo == null) return;
            
            bool isApplicationSlot = SlotTypeCombo.SelectedIndex == 0;
            
            if (ExecutableLabel != null) ExecutableLabel.Visibility = isApplicationSlot ? Visibility.Visible : Visibility.Collapsed;
            if (ExecutableNameText != null) ExecutableNameText.Visibility = isApplicationSlot ? Visibility.Visible : Visibility.Collapsed;
            if (ProcessLabel != null) ProcessLabel.Visibility = isApplicationSlot ? Visibility.Visible : Visibility.Collapsed;
            if (ProcessCombo != null) ProcessCombo.Visibility = isApplicationSlot ? Visibility.Visible : Visibility.Collapsed;
            if (TipText != null) TipText.Visibility = isApplicationSlot ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ProcessCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ProcessCombo.SelectedItem != null)
            {
                ExecutableNameText.Text = ProcessCombo.SelectedItem.ToString();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            slot.Type = (SlotType)SlotTypeCombo.SelectedIndex;
            slot.ExecutableName = ExecutableNameText.Text;
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ExecutableNameText.Text = "";
        }
    }
}
