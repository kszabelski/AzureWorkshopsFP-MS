using System.ComponentModel.DataAnnotations;

namespace Common
{
    public class ConstructionProject
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
    }
}