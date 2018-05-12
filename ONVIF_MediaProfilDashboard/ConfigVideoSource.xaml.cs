﻿using Newtonsoft.Json;
using ONVIF_MediaProfilDashboard.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ONVIF_MediaProfilDashboard
{

    /// <summary>
    /// Interaction logic for ConfigVideoEncoder.xaml
    /// </summary>
    public partial class ConfigVideoSource : Window
    {
        VideoSourceConfiguration[] configs;
        public Media2Client media;
        public string profileToken;
        public new bool DialogResult { get; private set; }
        InfoOption io;

        bool useProfileToken = false;

        int selectedIndex = 0;

        public ConfigVideoSource()
        {
            InitializeComponent();
            // handle closing event
            this.Closing += Window_Closing;
        }

        internal void setMedia(Media2Client media)
        {
            this.media = media;
            configs = media.GetVideoSourceConfigurations(null, null);
            setComboxItem();
        }

        private void setComboxItem()
        {
            List<string> data = new List<string>();
            foreach (VideoSourceConfiguration element in configs)
            {
                data.Add(element.Name);
            }
            configs_cbox.ItemsSource = data;
            configs_cbox.SelectedItem = configs_cbox.Items.GetItemAt(0);
        }

        private void ok_btn_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationRef[] config = { new ConfigurationRef() };
            config[0].Type = "VideoSource";
            config[0].Token = configs[selectedIndex].token;

            if (!useProfileToken)
            {    
                string name = "new profile";
                profileToken = media.CreateProfile(name, config);
            }
            else
            {
                media.AddConfiguration(profileToken, null, config);
            }

            this.DialogResult = true;
            this.Close();
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedIndex = configs_cbox.SelectedIndex;
            info_config.Text = JsonConvert.SerializeObject(configs[selectedIndex], Formatting.Indented);
        }

        private void show_btn_Click(object sender, RoutedEventArgs e)
        {
            string configsStr = JsonConvert.SerializeObject(configs, Formatting.Indented);
            io = new InfoOption(configsStr);
            io.Show();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (io != null)
            {
                io.Close();
            }
        }
    }
}