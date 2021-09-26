using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tasks4U.Models.ViewModels;

namespace Tasks4U.Infrastructure
{
    [HtmlTargetElement("div", Attributes = "page-info")]
    public class PageLinkTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _factory;

        public PagingInfo PageInfo { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PageLinkTagHelper(IUrlHelperFactory factory)
        {
            _factory = factory;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = _factory.GetUrlHelper(ViewContext);
            for (int i = 1; i <= PageInfo.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                string currentPage = ViewContext.RouteData.Values["page"].ToString();
                string handlerVal = "";
                if (ViewContext.RouteData.Values.TryGetValue("handler", out object handlerObj))
                {
                    handlerVal = handlerObj?.ToString() ?? "";
                }
                if (PageInfo.SelectedDate != null)
                {
                    tag.Attributes.Add("href", urlHelper.Page($"{currentPage}", 
                        new { pageNum = i, dateTime = PageInfo.SelectedDate, myTasks=PageInfo.MyTasks, 
                            handler = handlerVal }));
                }
                else
                {
                    tag.Attributes.Add("href", urlHelper.Page($"{currentPage}", new { pageNum = i, myTasks=PageInfo.MyTasks,
                        handler = handlerVal}));
                }

                tag.InnerHtml.Append(i.ToString());
                if (i == PageInfo.CurrentPage)
                    tag.AddCssClass("btn btn-primary");
                else
                    tag.AddCssClass("btn btn-outline-primary");
                output.Content.AppendHtml(tag);
            }
        }
    }
}
