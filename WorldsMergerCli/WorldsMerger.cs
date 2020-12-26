using System;
using System.Collections.Generic;
using System.Linq;
using fNbt;
using Microsoft.Extensions.Logging;

namespace WorldsMergerCli {
    public static class WorldsMerger {
        public static Dictionary<string, short> GetMappings(string fileName, ILogger? logger) {
            logger?.Log(LogLevel.Information, $"Creating mappings from file - {fileName}");
            var mappings = new NbtFile(fileName);
            var dictionary = new Dictionary<string, short>();
            var mappingCount = (mappings.RootTag["FML"]["ItemData"] as NbtList).Count;
            for (var index = 0; index < mappingCount; index++) {
                var tag = (mappings.RootTag["FML"]["ItemData"] as NbtList)[index];
                try {
                    dictionary.Add((tag as NbtCompound)["K"].StringValue.Substring(1), (short) (tag as NbtCompound)["V"].IntValue);
                }
                catch (Exception) {
                    // ignored
                }

                if (index % 500 == 0) {
                    logger?.Log(LogLevel.Information, $"Completed {(int)(index / (double)mappingCount * 100)}%: {index} of {mappingCount}");
                }
            }

            logger?.Log(LogLevel.Information, "Completed");
            return dictionary;
            
        }

        public static void Process(NbtTag tag, Dictionary<string, short> oldMappings, Dictionary<string, short> newMappings, ILogger? logger) {
            ProcessInternal(tag, oldMappings, newMappings, logger);
            logger?.Log(LogLevel.Information, "Mapping completed");
        }

        private static void ProcessInternal(NbtTag tag, Dictionary<string, short> oldMappings, Dictionary<string, short> newMappings, ILogger? logger) {
            switch (tag) {
                case NbtCompound nbtCompound: {
                    if (nbtCompound["id"] is NbtShort id) {
                        try {
                            var stringId = oldMappings.First(pair => pair.Value == id.ShortValue).Key;
                            try {
                                var oldValue = id.Value;
                                var newId = newMappings[stringId];
                                nbtCompound.Remove(id);
                                nbtCompound.Add(new NbtShort("id", newId));
                                logger?.Log(LogLevel.Information, $"Processed {stringId}: {oldValue} -> {newId}");
                            }
                            catch (Exception) {
                                logger?.Log(LogLevel.Warning, $"Tag {stringId} not exists in dictionary");
                            }
                        }
                        catch (Exception) {
                            // ignored
                        }
                    }

                    foreach (var variable in nbtCompound) {
                        ProcessInternal(variable, oldMappings, newMappings, logger);
                    }

                    break;
                }
                case NbtList nbtList: {
                    foreach (var variable in nbtList) {
                        ProcessInternal(variable, oldMappings, newMappings, logger);
                    }

                    break;
                }
            }
        }

        public static void Process(string playerDataPath, string oldLevelDatPath, string newLevelDatPath, string outputPath, ILogger? logger = null) {
            var main = new NbtFile(playerDataPath);
            var oldMappings = GetMappings(oldLevelDatPath, logger);
            var newMappings = GetMappings(newLevelDatPath, logger);
            Process(main.RootTag, oldMappings, newMappings, logger);

            main.SaveToFile(outputPath, main.FileCompression);
            logger?.Log(LogLevel.Information, $"File saved to {outputPath}");
        }
    }
}