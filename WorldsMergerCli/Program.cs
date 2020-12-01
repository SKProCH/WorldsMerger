using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fNbt;

namespace WorldsMergerCli {
    class Program {
        static void Main(string[] args) {
            Console.Write("Путь к файлу игрока: ");
            var filePath = Path.GetFullPath(Console.ReadLine());
            var main = new NbtFile(filePath);
            Console.Write("Путь к старому level.dat: ");
            var oldMappings = GetMappings(Console.ReadLine());
            Console.Write("Путь к новому level.dat: ");
            var newMappings = GetMappings(Console.ReadLine());
            Process(main.RootTag, oldMappings, newMappings);
            var patchedPath = Path.Combine(Path.GetDirectoryName(filePath), $"{Path.GetFileNameWithoutExtension(filePath)}_patched{Path.GetExtension(filePath)}");
            main.SaveToFile(patchedPath, main.FileCompression);
            Console.WriteLine($"Файл сохранен - {patchedPath}");

            Console.ReadKey();
        }
        
        public static Dictionary<string, short> GetMappings(string fileName) {
            Console.WriteLine($"Создание маппингов из файла - {fileName}");
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