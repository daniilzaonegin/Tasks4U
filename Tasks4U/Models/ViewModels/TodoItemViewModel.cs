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
        [DisplayName("To Whom")]
        public string ToUserName { get; set; }

        [ReadOnly(true)]
        [Required]
        [DisplayName("From Whom")]
        public string FromUserName { get; set; }

        [Required]
        [DisplayName("Todo Date")]
        public DateTime When { get; set; }

        [Required]
        [DisplayName("Summary")]
        [StringLength(500)]
        public string Summary { get; set; }

        [DisplayName("Task Description")]
        [StringLength(1000)]
        public string Description { get; set; }

        [DisplayName("Complete message")]
        [StringLength(500)]
        public string CompleteMessage { get; set; }

        [DisplayName("Completed?")]
        public bool Completed { get; set; }

        [DisplayName("Rejected?")]
        public bool Rejected { get; set; }
    }
}
