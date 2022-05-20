using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Rock_Models
{
    public class ApplicationType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }

    }
}
