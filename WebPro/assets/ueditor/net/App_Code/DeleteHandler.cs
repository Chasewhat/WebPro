using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


public class DeleteHandler : Handler
{
    public DeleteHandler(HttpContext context) : base(context) { }

    public override void Process()
    {
        string errors = "";
        try
        {
            string path = Request["path"];
            var localPath = Server.MapPath(path);
            if (File.Exists(localPath))
            {
                File.Delete(localPath);
            }
        }
        catch (Exception ex)
        {
            errors = ex.Message;
        }
        if (string.IsNullOrEmpty(errors))
        {
            WriteJson(new { path = Request["path"] });
        }
        else
        {
            WriteJson(new { path = Request["path"], error = errors });
        }
        
    }
}