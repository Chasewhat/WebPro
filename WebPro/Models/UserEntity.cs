using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebPro.Models
{
    public class UserEntity
    {
    }

    public class PagerInfo
    {
        public int RecordCount { get; set; }

        public int CurrentPageIndex { get; set; }

        public int PageSize { get; set; }
    }


    public class PagerQuery<TPager, TEntityList, TBlogRight>
    {
        public PagerQuery(TPager pager, TEntityList entityList, TBlogRight blogRight)
        {
            this.Pager = pager;
            this.EntityList = entityList;
            this.BlogRight = blogRight;
        }
        public TPager Pager { get; set; }
        public TEntityList EntityList { get; set; }

        public TBlogRight BlogRight { get; set; }
    }

    public class PagerTimeQuery<TPager, TEntityList>
    {
        public PagerTimeQuery(TPager pager, TEntityList entityList)
        {
            this.Pager = pager;
            this.EntityList = entityList;
        }
        public TPager Pager { get; set; }
        public TEntityList EntityList { get; set; }
    }

    public class DetailQuery<TBlog, TBlogRight>
    {
        public DetailQuery(TBlog blogs, TBlogRight blogRight)
        {
            this.Blogs = blogs;
            this.BlogRight = blogRight;
        }
        public TBlog Blogs { get; set; }

        public TBlogRight BlogRight { get; set; }
    }

    public class BlogRight<TTagList, TMostList>
    {
        public BlogRight(TTagList tag, TMostList best)
        {
            this.Tag = tag;
            this.Best = best;
        }
        public TTagList Tag { get; set; }
        public TMostList Best { get; set; }
    }

    public class PhotoForPage
    {
        public int id { get; set; }
        public int photoGroup { get; set; }
        public string imgurls { get; set; }
        public string imgFolder { get; set; }
    }

    public static class GlobalVar
    {
        public static string skin = "white";
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }
    }


    public class InitialPreviewConfig
    {
        public int key { get; set; }
        public string url { get; set; }

        public DataExtra extra { get; set; }

        public InitialPreviewConfig(int key, string url,DataExtra extra)
        {
            this.key = key;
            this.url = url;
            this.extra = extra;
        }

        public InitialPreviewConfig()
        { }
    }

    public class DataExtra
    {
        public string path{get;set;}

        public DataExtra(string path)
        {
            this.path=path;
        }
    }
}