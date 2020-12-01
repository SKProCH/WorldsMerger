using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using fNbt;

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

            Run(obj);
        }

        private static void Run(CmdOptions cmdOptions) {
            var main = new NbtFile(cmdOptions.PlayerData);
            var oldMappings = GetMappings(cmdOptions.OldLevelDat);
            var newMappings = GetMappings(cmdOptions.NewLevelDat);
            Process(main.RootTag, oldMappings, newMappings);

            main.SaveToFile(cmdOptions.OutputPath, main.FileCompression);
            Console.WriteLine($"File saved to {cmdOptions.OutputPath}");
        }

        public static Dictionary<string, short> GetMappings(string fileName) {
            Console.WriteLine($"Creating mappings from file - {fileName}");
            var mappings = new NbtFile(fileName);
            var dictionary = new Dictionary<string, short>();
            foreach (var tag in (mappings.RootTag["FML"]["ItemData"] as NbtList)) {
                try {
                    dictionary.Add((tag as NbtCompound)["K"].StringValue.Substring(1), (short) (tag as NbtCompound)["V"].IntValue);
                }
                catch (Exception) {
                    // ignored
                }
            }

            Console.WriteLine($"Завершено");
            return dictionary;
        }

        public static void Process(NbtTag tag, Dictionary<string, short> oldMappings, Dictionary<string, short> newMappings) {
            switch (tag) {
                case NbtCompound nbtCompound: {
                    if (nbtCompound["id"] is NbtShort id) {
                        try {
                            var stringId = oldMappings.First(pair => pair.Value == id.ShortValue).Key;
                            try {
                                var newId = newMappings[stringId];
                                nbtCompound.Remove(id);
                                nbtCompound.Add(new NbtShort("id", newId));
                                Console.WriteLine($"Processed {stringId}");
                            }
                            catch (Exception) {
                                Console.WriteLine($"Tag {stringId} not exists in dictionary");
                            }
                        }
                        catch (Exception) {
                            // ignored
                        }
                    }

                    foreach (var variable in nbtCompound) {
                        Process(variable, oldMappings, newMappings);
                    }

                    break;
                }
                case NbtList nbtList: {
                    foreach (var variable in nbtList) {
                        Process(variable, oldMappings, newMappings);
                    }

                    break;
                }
            }
        }
    }
}