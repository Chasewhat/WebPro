//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebPro.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Blogs
    {
        public int id { get; set; }
        public string title { get; set; }
        public string keyword { get; set; }
        public string showContent { get; set; }
        public string content { get; set; }
        public string imgurls { get; set; }
        public System.DateTime publishTime { get; set; }
        public string author { get; set; }
        public int viewCount { get; set; }
        public int orderNum { get; set; }
        public int sourceId { get; set; }
    }
}
