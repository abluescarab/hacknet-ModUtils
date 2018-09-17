using System.Collections.Generic;
using System.Linq;

namespace HacknetMods.Utils {
    public class CommandParser {
        public List<string> OptionsPrefixes { get; set; }

        /// <summary>
        /// Holds parsed strings that may be surrounded by single or double quotes.
        /// </summary>
        private struct ParsedText {
            public string Text { get; set; }
            public int NextIndex { get; set; }

            public ParsedText(string text, int nextIndex) {
                Text = text;
                NextIndex = nextIndex;
            }
        }

        /// <summary>
        /// Holds commands and option flags.
        /// </summary>
        public struct Command {
            public string Text { get; set; }
            public string Flag { get; set; }
        }

        public CommandParser(params string[] optionsPrefixes) : this(optionsPrefixes.ToList()) { }

        public CommandParser(List<string> optionsPrefixes) {
            OptionsPrefixes = optionsPrefixes;
        }

        /// <summary>
        /// Parse all arguments, excluding the initial command.
        /// </summary>
        public List<Command> ParseArguments(List<string> args) {
            int index = 1;
            var parsed = new List<Command>();
            Command command = new Command();

            while(index < args.Count) {
                if(string.IsNullOrEmpty(args[index])) {
                    index++;
                    continue;
                }

                string delimiter = args[index][0].ToString();

                if(!args[index].StartsWith("\"") && !args[index].StartsWith("'")) {
                    delimiter = "";
                }

                var parsedText = Parse(args, index, delimiter);
                bool isOption = OptionsPrefixes.Any(p => parsedText.Text.StartsWith(p));
                index = parsedText.NextIndex;

                if(!isOption) {
                    command.Text = parsedText.Text;
                    parsed.Add(command);
                    command = new Command();
                }
                else {
                    command.Flag = parsedText.Text;
                }
            }

            return parsed;
        }

        /// <summary>
        /// Parse individual arguments, including text surrounded by delimiters.
        /// </summary>
        private ParsedText Parse(List<string> args, int startIndex, string delimiter = "") {
            string text = args[startIndex];
            int index = startIndex + 1;

            if(!string.IsNullOrEmpty(delimiter)) {
                do {
                    text += " " + args[index++];
                }
                while(index < args.Count && !args[index - 1].EndsWith(delimiter));
            }

            return new ParsedText(text.Trim('"', '\''), index);
        }
    }
}
