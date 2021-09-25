using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeetSpeakTranslator.Models
{
    public class LeetSpeakResponse
    {
        public LeetSpeakSuccess Success { get; set; }
        public LeetSpeakContents Contents { get; set; }
    }

    public class LeetSpeakSuccess
    {
        public int Total { get; set; }
    }
    public class LeetSpeakContents
    {
        public string Translated { get; set; }
        public string Text { get; set; }
        public string Translation { get; set; }
    }
}
