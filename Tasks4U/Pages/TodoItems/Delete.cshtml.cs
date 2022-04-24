using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Tasks4U.Data;
using Tasks4U.Model;
using Tasks4U.Services;

namespace Tasks4U.Pages.TodoItems
{
    public class DeleteModel : PageModel
    {
        private readonly Tasks4UDbContext _dbContext;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IEmailService _emailService;

        [BindProperty]
        public int Id { get; set; }

        [DisplayName("Comment")]
        [BindProperty]
        [StringLength(500)]
        public string Message { get; set; }

        public bool IsRejected { get; set; }

        [DisplayName("Task")]
        public string TaskSummary { get; set; }


        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        public DeleteModel(Tasks4UDbContext dbContext, IUrlHelperFactory urlHelperFactory, IEmailService emailService)
        {
            _dbContext = dbContext;
            _urlHelperFactory = urlHelperFactory;
            _emailService = emailService;
        }

        public async Task<IActionResult> OnGetAsync([FromQuery] int id)
        {
            TodoItem item = await _dbContext.TodoItems.FindAsync(id);
            if (item == null) {
                ModelState.AddModelError(nameof(id), "Task not found");
                return BadRequest(ModelState);
            }
            Id = id;
            Message = item.RejectMessage;
            IsRejected = item.Rejected;
            TaskSummary = item.Summary;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
             TodoItem itemToDelete = await _dbContext.TodoItems.FindAsync(Id);
            if(itemToDelete == null)
            {
                ModelState.AddModelError(nameof(Id), "Task not found");
                return BadRequest(ModelState);
            }

            if (itemToDelete.Rejected)
            {
                ModelState.AddModelError(nameof(Id), "Task is already rejected");
                return BadRequest(ModelState);
            }

            itemToDelete.Rejected = true;
            itemToDelete.RejectMessage = Message ?? "";

            await _dbContext.SaveChangesAsync();
            await _emailService.SendEmailAsync(itemToDelete.ToUserId,
                itemToDelete.FromUserId,
                $"������������ {itemToDelete.FromUserId} ������ ������� '{itemToDelete.Summary}'",
                $"������������ {itemToDelete.FromUserId} ������ ������� '{itemToDelete.Summary}'.<br/>" +
                $"������ ������ ��� �� ������ :)");
            TempData["Message"] = "Task was deleted successfully!";

            return Redirect(ReturnUrl);
        }
    }
}
