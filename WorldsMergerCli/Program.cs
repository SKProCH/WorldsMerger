using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;

namespace WorldsMergerCli {
    class Program {
        static void Main(string[] args) {
            Parser.Default.ParseArguments<CmdOptions>(args)
                  .WithParsed(RunOptions)
                  .WithNotParsed(HandleParseError);
        }

        private static void HandleParseError(IEnumerable<Error> obj) {
            Console.WriteLine("Arguments parse failed:");
            foreach (var error in obj) {
                Console.WriteLine(error.Tag);
            }

            Environment.Exit(-1);
        }

        private static void RunOptions(CmdOptions obj) {
            if (obj.OldLevelDat == null) {
                Console.WriteLine("Enter path to old level.dat");
                obj.OldLevelDat = Console.ReadLine();
            }

            if (obj.NewLevelDat == null) {
                Console.WriteLine("Enter path to new level.dat");
                obj.NewLevelDat = Console.ReadLine();
            }

            if (obj.PlayerData == null) {
                Console.WriteLine("Enter path to player data file");
                obj.PlayerData = Console.ReadLine();
            }

            if (obj.OutputPath == null) {
                obj.OutputPath = Path.Combine(Path.GetDirectoryName(obj.PlayerData),
                    $"{Path.GetFileNameWithoutExtension(obj.PlayerData)}_patched{Path.GetExtension(obj.PlayerData)}");
            }

            obj.PlayerData = Path.GetFullPath(obj.PlayerData);
            obj.OutputPath = Path.GetFullPath(obj.OutputPath);
            obj.NewLevelDat = Path.GetFullPath(obj.NewLevelDat);
            obj.OldLevelDat = Path.GetFullPath(obj.OldLevelDat);

            WorldsMerger.Process(obj.PlayerData, obj.OldLevelDat, obj.NewLevelDat, obj.OutputPath, new ConsoleLogger());
        }
    }
}