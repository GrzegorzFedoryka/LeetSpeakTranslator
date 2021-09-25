using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeetSpeakTranslator.Models
{
    public class LeetSpeakResponseError
    {
        public class LeetSpeakError
        {
            public int Code { get; set; }
            public string Message { get; set; }
        }

        public LeetSpeakError Error { get; set; }
    }
    
}
