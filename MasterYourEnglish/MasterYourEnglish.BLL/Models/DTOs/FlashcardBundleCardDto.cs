namespace MasterYourEnglish.BLL.Models.DTOs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FlashcardBundleCardDto
    {
        public int BundleId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string BundleName { get; set; } = string.Empty;

        public string DifficultyLevel { get; set; } = string.Empty;
    }
}
