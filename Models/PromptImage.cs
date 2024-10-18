using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace truckapi.Models
{
    public class PromptImage
    {
        public string OriginalPath { get; set; } // Original file path
        public int PromptIndex { get; set; } // The prompt index associated with the image

    }
}