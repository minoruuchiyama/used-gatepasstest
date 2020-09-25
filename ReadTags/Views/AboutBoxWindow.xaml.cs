using System;
using System.Windows;
using System.Diagnostics;
using ReadTags.ViweModels;

namespace ReadTags.Views {
    public partial class AboutBoxWindow : Window {

        AboundBoxWindowViewModel AboundBoxWindowViewModel = new AboundBoxWindowViewModel();

        public AboutBoxWindow() {
            InitializeComponent();
            DataContext = AboundBoxWindowViewModel;
        }
    }
}
