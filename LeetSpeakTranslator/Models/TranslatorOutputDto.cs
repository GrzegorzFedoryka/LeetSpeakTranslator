using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeetSpeakTranslator.Models
{
    public class TranslatorOutputDto
    {
        public string InputText { get; set; }
        public string ConvertedText { get; set; }
        public string ErrorMessage { get; set; }
        public string Code { get; set; }
    }
}
