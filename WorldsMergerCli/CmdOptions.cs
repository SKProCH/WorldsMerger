using CommandLine;

namespace WorldsMergerCli {
    public class CmdOptions {
        [Option('o', "oldld", Required = false, HelpText = "Path to old level.dat")]
        public string OldLevelDat { get; set; }
        
        [Option('n', "newld", Required = false, HelpText = "Path to new level.dat")]
        public string NewLevelDat { get; set; }
        
        [Option('p', "playerdata", Required = false, HelpText = "Path to player data file")]
        public string PlayerData { get; set; }
        
        [Option('o', "output", Required = false, HelpText = "The path where the file should be saved")]
        public string OutputPath { get; set; }
    }
}