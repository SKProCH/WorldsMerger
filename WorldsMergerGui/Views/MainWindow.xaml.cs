using Avalonia;
using Avalonia.Controls;
using Avalonia.Diagnostics;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace WorldsMergerGui.Views {
    public class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
            DevTools.Attach(this, KeyGesture.Parse("F12"));
        }
    }
}