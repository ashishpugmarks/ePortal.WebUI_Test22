using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePortal.ViewModels
{
    public class PersonalityQuotesViewModel
    {
        public long QUOTES_ID { get; set; }
        public string QUOTES { get; set; }
        public short STATUS { get; set; }
        public long PERSONALITY_ID { get; set; }
        public string PERSONALITYNAME { get; set; }
        public string NATIONALITY { get; set; }
        public string OCCUPATION { get; set; }
        public byte[] BACKGROUND_IMAGE { get; set; }
        public string IMAGE_CONTENTTYPE { get; set; }
        public string IMAGE_NAME { get; set; }
    }
}
