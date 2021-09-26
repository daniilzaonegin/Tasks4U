using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tasks4U.Model
{
    [Table("TodoItems")]
    public class TodoItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string ToUserId { get; set; }

        [Required]
        [MaxLength(128)]
        public string FromUserId { get; set; }

        [Required]
        public DateTime When { get; set; }

        [Required]
        [MaxLength(255)]
        public string Summary { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(500)]
        public string CompleteMessage { get; set; }

        public bool Completed { get; set; }

        [MaxLength(500)]
        public string RejectMessage { get; set; }

        public bool Rejected { get; set; }
    }
}
