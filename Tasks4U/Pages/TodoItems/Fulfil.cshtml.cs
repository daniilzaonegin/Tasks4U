using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Threading.Tasks;
using Tasks4U.Data;
using Tasks4U.Model;
using Tasks4U.Models.ViewModels;
using Tasks4U.Services;

namespace Tasks4U.Pages.TodoItems
{
    public class FulfilModel : PageModel
    {
        private readonly Tasks4UDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IUrlHelperFactory _urlHelperFactory;

        [BindProperty]
        public FulfilViewModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public FulfilModel(Tasks4UDbContext dbContext, IMapper mapper, IEmailService emailService, IUrlHelperFactory urlHelperFactory)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _emailService = emailService;
            _urlHelperFactory = urlHelperFactory;
        }

        public async Task<IActionResult> OnGetAsync(int id, [FromQuery]string returnUrl)
        {
            TodoItem item = await _dbContext.TodoItems.FindAsync(id);
            if(item==null)
            {
                ModelState.AddModelError(nameof(id), "Task not found");
                return BadRequest(ModelState);
            }

            TodoItemViewModel todo = _mapper.Map<TodoItemViewModel>(item);
            Input = new FulfilViewModel
            {
                TodoItemId = todo.Id,
                TodoItemSummary = todo.Summary,
                CompleteMessage = todo.CompleteMessage,
                IsCompleted = todo.Completed
            };
            ReturnUrl = returnUrl;
            return Page();
        } 

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TodoItem item = await _dbContext.TodoItems.FindAsync(Input.TodoItemId);
            if(item==null)
            {
                ModelState.AddModelError(nameof(Input.TodoItemId), "Task not found");
                return BadRequest(ModelState);
            }

            if (item.Completed)
            {
                ModelState.AddModelError(nameof(Input.TodoItemId), "Task was already completed");
                return BadRequest(ModelState);
            }

            item.CompleteMessage = Input.CompleteMessage;
            item.Completed = true;
            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(PageContext);

            await _dbContext.SaveChangesAsync();
            await _emailService.SendEmailAsync(item.FromUserId, item.ToUserId, $"«адача '{item.Summary}' выполнена",
                $"ѕользователь {item.ToUserId} выполнил поставленную вами задачу.<br/> " +
                $"ѕосмотреть текст выполнени€ вы можете по <a href='{urlHelper.PageLink("/TodoItems/List")}'>ссылке</a>");
            TempData[nameof(ListModel.Message)] = "Task was completed successfully";
            return RedirectToPage("/TodoItems/List");
        }
    }
}
