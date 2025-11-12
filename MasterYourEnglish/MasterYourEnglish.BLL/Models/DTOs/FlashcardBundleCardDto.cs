using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterYourEnglish.BLL.Models.DTOs
{
    public class FlashcardBundleCardDto
    {
        public int BundleId { get; set; }
        public string CategoryName { get; set; } = "";
        public string BundleName { get; set; } = "";
    }
}
