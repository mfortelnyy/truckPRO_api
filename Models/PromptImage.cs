using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace truckapi.Models
{
    public class PromptImage
    {
        public string path { get; set; } // Original file path
        public int promptIndex { get; set; } // The prompt index associated with the image

    }
}