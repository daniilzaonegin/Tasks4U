using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tasks4U.Data;
using Tasks4U.Model;
using Tasks4U.Models;
using Tasks4U.Models.ViewModels;
using Tasks4U.Pages.TodoItems;
using Tasks4U.Services;

namespace Tasks4U.Pages.Home
{
    public class TodoItemModel : PageModel
    {
        private readonly Tasks4UDbContext _dbcontext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly ILogger<TodoItemModel> _logger;

        [BindProperty]
        public TodoItemViewModel Input { get; set; } = new TodoItemViewModel();

        public ApplicationUser CurrentUser { get; set; }

        public SelectList RegisteredUsers { get; private set; }

        public TodoItemModel(
            Tasks4UDbContext dbcontext,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IEmailService emailService,
            IUrlHelperFactory urlHelperFactory,
            ILogger<TodoItemModel> logger)
        {
            _dbcontext = dbcontext;
            _userManager = userManager;
            _mapper = mapper;
            _emailService = emailService;
            _urlHelperFactory = urlHelperFactory;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            await InitilizeFormFields();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                await InitilizeFormFields();
                return Page();
            }

            TodoItem todoItem = _mapper.Map<TodoItem>(Input);

            await _dbcontext.TodoItems.AddAsync(todoItem);
            await _dbcontext.SaveChangesAsync();
            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(PageContext);
            await _emailService.SendEmailAsync(Input.ToUserName,
                Input.FromUserName,
                $"{Input.FromUserName} создал задачу для вас",
                @$"{Input.FromUserName} создал задачу '{todoItem.Summary}' для вас.
Перейдите по <a href='{urlHelper.PageLink("/TodoItems/List")}'>ссылке</a>, чтобы посмотреть ee."
            );

            TempData[nameof(ListModel.Message)] = "Task was successfully created!";
            return RedirectToPage("/TodoItems/List");
        }

        private async Task InitilizeFormFields()
        {
            CurrentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            Input.FromUserName = CurrentUser.DisplayName;
            RegisteredUsers = new SelectList(_userManager.Users.Select(u => u.DisplayName));
        }
    }
}
