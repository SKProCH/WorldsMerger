using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WorldsMergerGui.Models;

namespace WorldsMergerGui.ViewModels {
    public class MainWindowViewModel : ViewModelBase {
        public ISubject<bool> CanRunProcess = new Subject<bool>();
        private ReactiveLogger _reactiveLogger;

        public MainWindowViewModel() {
            BrowseOldLevelData = ReactiveCommand.Create<Window, Task>(async window => { OldLevelData = await BrowsePath(window, OldLevelData); });
            BrowseNewLevelData = ReactiveCommand.Create<Window, Task>(async window => { NewLevelData = await BrowsePath(window, NewLevelData); });
            BrowsePlayerData = ReactiveCommand.Create<Window, Task>(async window => { PlayerData = await BrowsePath(window, PlayerData); });
            BrowseOutputPath = ReactiveCommand.Create<Window, Task>(async window => { OutputPath = await BrowsePath(window, OutputPath); });
            RunCommand = ReactiveCommand.Create(Run, CanRunProcess.ObserveOn(RxApp.MainThreadScheduler));
            CanRunProcess.OnNext(true);
            _reactiveLogger = new ReactiveLogger();
            _reactiveLogger.OnLog.Subscribe(s => LogText += s + Environment.NewLine);
        }

        private void Run() {
            CanRunProcess.OnNext(false);
            Task.Run(() => {
                LogText = "";
                _reactiveLogger.Log(LogLevel.Information, "Starting...");
                try {
                    WorldsMergerCli.WorldsMerger.Process(PlayerData, OldLevelData, NewLevelData, OutputPath, _reactiveLogger);
                }
                catch (Exception e) {
                    _reactiveLogger.Log(LogLevel.Critical, e.ToString());
                }
                finally {
                    CanRunProcess.OnNext(true);
                }
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

        [Reactive] public string OldLevelData { get; set; }

        [Reactive] public string NewLevelData { get; set; }

        [Reactive] public string PlayerData { get; set; }

        [Reactive] public string OutputPath { get; set; }

        public ReactiveCommand<Window, Task> BrowseOldLevelData { get; set; }
        public ReactiveCommand<Window, Task> BrowseNewLevelData { get; set; }
        public ReactiveCommand<Window, Task> BrowsePlayerData { get; set; }
        public ReactiveCommand<Window, Task> BrowseOutputPath { get; set; }

        [Reactive] public string LogText { get; set; }

        public ReactiveCommand<Unit, Unit> RunCommand { get; set; }
    }
}