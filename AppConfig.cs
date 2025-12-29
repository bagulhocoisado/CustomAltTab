using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CustomAltTab
{
    public enum DisplayMode
    {
        Wheel,
        Grid
    }

    public enum SlotType
    {
        Application,      // Slot normal para aplicativo
        MinimizeCurrent,  // Minimiza a janela atual
        Cancel           // Cancela e fecha a roda
    }

    [Serializable]
    public class WindowSlot
    {
        public string ExecutableName { get; set; }
        public SlotType Type { get; set; }
        
        public WindowSlot()
        {
            ExecutableName = "";
            Type = SlotType.Application;
        }
    }

    [Serializable]
    public class AppConfig
    {
        private static AppConfig instance;
        private static readonly string configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CustomAltTab",
            "config.xml"
        );

        public static AppConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppConfig();
                }
                return instance;
            }
        }

        public DisplayMode DisplayMode { get; set; }
        public int MinSlots { get; set; }
        public bool ShowEmptySlots { get; set; }
        public bool ShowUnconfiguredWindows { get; set; }
        public List<WindowSlot> WindowSlots { get; set; }

        public AppConfig()
        {
            // Valores padrão
            DisplayMode = DisplayMode.Wheel;
            MinSlots = 8;
            ShowEmptySlots = true;
            ShowUnconfiguredWindows = true;
            WindowSlots = new List<WindowSlot>();
            
            // Adiciona alguns slots padrão
            for (int i = 0; i < 8; i++)
            {
                WindowSlots.Add(new WindowSlot());
            }
        }

        public static void Save()
        {
            try
            {
                string directory = Path.GetDirectoryName(configPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                XmlSerializer serializer = new XmlSerializer(typeof(AppConfig));
                using (StreamWriter writer = new StreamWriter(configPath))
                {
                    serializer.Serialize(writer, Instance);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Erro ao salvar configurações: {ex.Message}",
                    "Erro",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error
                );
            }
        }

        public static void Load()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AppConfig));
                    using (StreamReader reader = new StreamReader(configPath))
                    {
                        instance = (AppConfig)serializer.Deserialize(reader);
                    }
                }
                else
                {
                    instance = new AppConfig();
                    Save(); // Cria arquivo de configuração padrão
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Erro ao carregar configurações: {ex.Message}\nUsando configurações padrão.",
                    "Aviso",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning
                );
                instance = new AppConfig();
            }
        }
    }
}
