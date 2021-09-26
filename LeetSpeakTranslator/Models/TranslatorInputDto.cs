using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeetSpeakTranslator.Models
{
    public class TranslatorInputDto
    {
        public string Text { get; set; }
        public TranslatorsEnum Translators { get; set; }
    }
}
