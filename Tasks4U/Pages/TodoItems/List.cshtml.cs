using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tasks4U.Data;
using Tasks4U.Models;
using Tasks4U.Models.ViewModels;

namespace Tasks4U.Pages.TodoItems
{
    public class ListModel : PageModel
    {
        private readonly Tasks4UDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private const int PageSize = 4;

        [TempData]
        public string Message { get; set; }

        public TodoItemViewModel[] TodoItems { get; set; }
        public PagingInfo PageInfo { get; set; }

        public ListModel(Tasks4UDbContext dbContext, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> OnGetAsync(int pageNum = 1, string dateTime = null, bool myTasks = false)
        {
            DateTime? dateParam = GetDateFromQueryStr(dateTime);
            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            int totalTasks = await _dbContext.TodoItems
                .CountAsync(item => ((myTasks == false && item.ToUserId == user.DisplayName) ||
                                     (myTasks == true && item.FromUserId == user.DisplayName)) &&
                                     (!dateParam.HasValue || item.When.Date == dateParam.Value.Date) &&
                                     item.Completed == false && item.Rejected == false);

            TodoItems = await _mapper.ProjectTo<TodoItemViewModel>(_dbContext.TodoItems
                .Where(item => ((myTasks == false && item.ToUserId == user.DisplayName) ||
                                 (myTasks == true && item.FromUserId == user.DisplayName)))
                .Where(item => (!dateParam.HasValue || item.When.Date == dateParam.Value.Date) && 
                        item.Completed == false && item.Rejected == false)
                .Skip((pageNum - 1) * PageSize).Take(PageSize)).ToArrayAsync();

            int pages = totalTasks / PageSize;
            if ((totalTasks % PageSize) != 0)
            {
                pages += 1;
            }
            PageInfo = new PagingInfo
            {
                TotalPages = pages,
                CurrentPage = pageNum,
                SelectedDate = dateTime,
                MyTasks = myTasks
            };
            return Page();
        }

        public async Task<IActionResult> OnGetRejectedAsync(int pageNum = 1, string dateTime = null)
        {
            DateTime? dateParam = GetDateFromQueryStr(dateTime);
            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            int totalTasks = await _dbContext.TodoItems
                .CountAsync(item => (item.FromUserId == user.DisplayName) &&
                                     (!dateParam.HasValue || item.When.Date == dateParam.Value.Date) &&
                                     item.Rejected);

            TodoItems = await _mapper.ProjectTo<TodoItemViewModel>(_dbContext.TodoItems
                .Where(item => (item.FromUserId == user.DisplayName) &&
                        (!dateParam.HasValue || item.When.Date == dateParam.Value.Date) &&
                        item.Rejected)
                .Skip((pageNum - 1) * PageSize).Take(PageSize)).ToArrayAsync();

            int pages = totalTasks / PageSize;
            if ((totalTasks % PageSize) != 0)
            {
                pages += 1;
            }
            PageInfo = new PagingInfo
            {
                TotalPages = pages,
                CurrentPage = pageNum,
                SelectedDate = dateTime,
                MyTasks = true 
            };
            return Page();
        }

        public async Task<IActionResult> OnGetCompletedAsync(int pageNum = 1, string dateTime = null)
        {
            DateTime? dateParam = GetDateFromQueryStr(dateTime);

            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            int totalTasks = await _dbContext.TodoItems
              .CountAsync(item => ((item.ToUserId == user.DisplayName) &&
                                   (!dateParam.HasValue || item.When.Date == dateParam.Value.Date) &&
                                   item.Completed == true));

            TodoItems = await _mapper.ProjectTo<TodoItemViewModel>(_dbContext.TodoItems
                .Where(item => ((item.ToUserId == user.DisplayName) &&
                                   (!dateParam.HasValue || item.When.Date == dateParam.Value.Date) &&
                                   item.Completed == true))
                .Skip((pageNum - 1) * PageSize).Take(PageSize)).ToArrayAsync();

            int pages = totalTasks / PageSize;
            if ((totalTasks % PageSize) != 0)
            {
                pages += 1;
            }
            PageInfo = new PagingInfo
            {
                TotalPages = pages,
                CurrentPage = pageNum,
                SelectedDate = dateTime,
                MyTasks = false,
                CompletedTasks = true
            };
            return Page();

        }

        private static DateTime? GetDateFromQueryStr(string dateTime)
        {
            DateTime? dateParam = null;
            if (DateTime.TryParse(dateTime, out DateTime dateTimeValue))
            {
                dateParam = dateTimeValue;
            }

            return dateParam;
        }
    }
}
