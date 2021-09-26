using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tasks4U.Models.ViewModels
{
    public class TodoItemViewModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Кому")]
        public string ToUserName { get; set; }

        [ReadOnly(true)]
        [Required]
        [DisplayName("От")]
        public string FromUserName { get; set; }

        [Required]
        [DisplayName("До какого срока сделать")]
        public DateTime When { get; set; }

        [Required]
        [DisplayName("Краткое описание")]
        [StringLength(500)]
        public string Summary { get; set; }

        [DisplayName("Описание задачи")]
        [StringLength(1000)]
        public string Description { get; set; }

        [DisplayName("Комментарий выполнения")]
        [StringLength(500)]
        public string CompleteMessage { get; set; }

        [DisplayName("Выполнено?")]
        public bool Completed { get; set; }

        [DisplayName("Отменено?")]
        public bool Rejected { get; set; }
    }
}
