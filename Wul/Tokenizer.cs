using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Wul
{
    class Tokenizer
    {
        public static IEnumerable<Token> Tokenize(string source, string fileName = null)
        {
            var lines = source.Split("\n");
            int i = 0;
            var tokens = lines.SelectMany(l =>
            {
                i++;
                var split = Regex.Split(l, @"\s");
                return split
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(s => new Token(s, fileName, i, 0));
            });
            return tokens;
        }
    }
}