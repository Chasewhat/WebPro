using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebPro
{
    public static class MyHtmlHelper
    {
        public static HtmlString ShowPageNavigate(this HtmlHelper htmlHelper, int currentPage, int pageSize, int totalCount)
        {
            var redirectTo = htmlHelper.ViewContext.RequestContext.HttpContext.Request.Url.AbsolutePath;
            pageSize = pageSize == 0 ? 3 : pageSize;
            var totalPages = Math.Max((totalCount + pageSize - 1) / pageSize, 1); //总页数
            var output = new StringBuilder();
            if (totalPages > 1)
            {
                if (currentPage > 1)
                {//处理上一页的连接
                    output.AppendFormat("<li><a href='{0}?pageIndex={1}'>«</a></li> ", redirectTo, currentPage - 1);
                }

                output.Append(" ");
                int currint = 5;
                for (int i = 0; i <= 10; i++)
                {//一共最多显示10个页码，前面5个，后面5个
                    if ((currentPage + i - currint) >= 1 && (currentPage + i - currint) <= totalPages)
                    {
                        if (currint == i)
                        {//当前页处理                           
                            output.AppendFormat("<li class='active'><a href='{0}?pageIndex={1}'>{1}</a></li> ", redirectTo, currentPage);
                        }
                        else
                        {//一般页处理
                            output.AppendFormat("<li><a href='{0}?pageIndex={1}'>{1}</a></li> ", redirectTo, currentPage + i - currint);
                        }
                    }
                    output.Append(" ");
                }
                if (currentPage < totalPages)
                {//处理下一页的链接
                    output.AppendFormat("<li><a href='{0}?pageIndex={1}'>»</a></li> ", redirectTo, currentPage + 1);
                }

                output.Append(" ");
            }
            return new HtmlString(output.ToString());
        }
    }

}