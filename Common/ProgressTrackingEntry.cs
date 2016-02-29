using System;
using System.ComponentModel.DataAnnotations;

namespace Common
{
    public class ProgressTrackingEntry
    {
        public int Id { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Display(Name = "Picture")]
        public string PictureReference { get; set; }

        [Display(Name = "Entry date")]
        public DateTime EntryDate { get; set; }
        
        public int ConstructionProjectId { get; set; }

        public virtual ConstructionProject ConstructionProject { get; set; }

        public string ThumbnailPictureReference { get; set; }
    }
}
