using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;

namespace WorldsMergerGui.ViewModels {
    public class MainWindowViewModel : ViewModelBase {
        public MainWindowViewModel() {
            BrowseOldLevelData  = ReactiveCommand.Create<Window, Task>(async window => {
                OldLevelData = await BrowsePath(window, OldLevelData);
            }); 
            BrowseNewLevelData = ReactiveCommand.Create<Window, Task>(async window => {
                NewLevelData = await BrowsePath(window, NewLevelData);
            }); 
            BrowsePlayerData = ReactiveCommand.Create<Window, Task>(async window => {
                PlayerData = await BrowsePath(window, PlayerData);
            }); 
            BrowseOutputPath = ReactiveCommand.Create<Window, Task>(async window => {
                OutputPath = await BrowsePath(window, OutputPath);
            }); 
        }

        private async Task<string> BrowsePath(Window window, string oldPath) {
            var openFileDialog = new OpenFileDialog() {AllowMultiple = false};
            if (oldPath != null) {
                openFileDialog.Directory = Path.GetDirectoryName(oldPath);
            }

            var files = await openFileDialog.ShowAsync(window);
            return files.Length == 0 ? oldPath : files.First();
        }

        public string OldLevelData { get; set; }

        public string NewLevelData { get; set; }

        public string PlayerData { get; set; }

        public string OutputPath { get; set; }

        public ReactiveCommand<Window, Task> BrowseOldLevelData { get; set; }
        public ReactiveCommand<Window, Task> BrowseNewLevelData { get; set; }
        public ReactiveCommand<Window, Task> BrowsePlayerData { get; set; }
        public ReactiveCommand<Window, Task> BrowseOutputPath { get; set; }
    }
}