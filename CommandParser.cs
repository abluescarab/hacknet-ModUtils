using System.Collections.Generic;
using System.Linq;

namespace HacknetMods.Utils {
    public class CommandParser {
        public List<string> OptionsPrefixes { get; set; }

        public struct Option {
            public string Text { get; set; }
            public int NextIndex { get; set; }

            public Option(string text, int nextIndex) {
                Text = text;
                NextIndex = nextIndex;
            }
        }

        public CommandParser(params string[] optionsPrefixes) : this(optionsPrefixes.ToList()) { }

        public CommandParser(List<string> optionsPrefixes) {
            OptionsPrefixes = optionsPrefixes;
        }

        public List<string[]> ParseArguments(List<string> args) {
            int index = 1;
            var parsed = new List<string[]>();
            bool isOption = false;
            var arg = new string[2];

            while(index < args.Count) {
                Option option;

                if(args[index].StartsWith("\"")) {
                    option = ParseFullOption(args, index, "\"");
                }
                else if(args[index].StartsWith("'")) {
                    option = ParseFullOption(args, index, "'");
                }
                else {
                    option = ParseFullOption(args, index);
                }

                arg[!isOption ? 0 : 1] = option.Text;
                isOption = OptionsPrefixes.Any(p => arg[index].StartsWith(p));
                index = option.NextIndex;

                if(!isOption) {
                    parsed.Add(arg);
                    arg = new string[2];
                }
            }

            return parsed;
        }

        private Option ParseFullOption(List<string> args, int startIndex, string delimiter = "") {
            string option = args[startIndex];
            int index = startIndex + 1;

            if(!string.IsNullOrEmpty(delimiter)) {
                do {
                    option += " " + args[index++];
                }
                while(index < args.Count && !args[index - 1].EndsWith(delimiter));
            }

            return new Option(option.Trim('"', '\''), index);
        }
    }
}
