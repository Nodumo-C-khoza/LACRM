using System.ComponentModel.DataAnnotations;

namespace Less_Annoying_CRM_Assessment.Entities
{
    public class LACRMContact
    {
        public string ContactId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string AssignedTo { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public bool IsCompany { get; set; }
    }
}

