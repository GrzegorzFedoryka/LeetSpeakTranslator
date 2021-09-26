using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeetSpeakTranslator.Models
{
    public class TranslatorAPIResponse
    {
        public APISuccess Success { get; set; }
        public APIContents Contents { get; set; }
    }

    public class APISuccess
    {
        public int Total { get; set; }
    }
    public class APIContents
    {
        public string Translated { get; set; }
        public string Text { get; set; }
        public string Translation { get; set; }
    }
}
