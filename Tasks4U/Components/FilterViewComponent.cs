using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tasks4U.Data;
using Tasks4U.Model;
using Tasks4U.Models;

namespace Tasks4U.Components
{
    public class FilterViewComponent : ViewComponent
    {
        private readonly Tasks4UDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public FilterViewComponent(Tasks4UDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<ViewViewComponentResult> InvokeAsync(string selectedDate, bool myTasks = false)
        {
            IQueryable<TodoItem> selectedItems = _dbContext.TodoItems;

            bool rejectedTasks = HttpContext.Request.RouteValues.ContainsKey("handler") && 
                                HttpContext.Request.RouteValues["handler"]?.ToString() == "Rejected";

            bool completedTasks = HttpContext.Request.RouteValues.ContainsKey("handler") &&
                                HttpContext.Request.RouteValues["handler"]?.ToString() == "Completed";

            ApplicationUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            string currentUserName = user.DisplayName;
            if (myTasks)
                selectedItems = selectedItems.Where(i => i.FromUserId == currentUserName);
            else
                selectedItems = selectedItems.Where(i => i.ToUserId == currentUserName);

            selectedItems = completedTasks ? 
                selectedItems.Where(i => i.Completed) : selectedItems.Where(i => !i.Completed);


            selectedItems = rejectedTasks ?
                selectedItems.Where(i => i.Rejected) : selectedItems.Where(i => !i.Rejected);
            
            string[] dates = selectedItems.GroupBy(i => i.When.Date).Select(g => g.Key.ToShortDateString()).ToArray();
            ViewData["selectedDate"] = selectedDate ?? "";
            ViewData["myTasks"] = myTasks;
            ViewData["completedTasks"] = completedTasks;
            ViewData["rejectedTasks"] = rejectedTasks;
            return View(dates);
        }
    }
}
