using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tasks4U.Models.ViewModels
{
    public class FulfilViewModel
    {
        [Required]
        [DisplayName("Комментарий")]
        public string CompleteMessage { get; set; }

        [Required]
        public int TodoItemId { get; set; }

        [DisplayName("Задача")]
        public string TodoItemSummary { get; set; }

        public bool IsCompleted { get; set; }
    }
}
