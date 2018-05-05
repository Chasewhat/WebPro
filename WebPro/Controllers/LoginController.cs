using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPro.Models;

namespace WebPro.Controllers
{
    public class LoginController : Controller
    {
        private WebEntities db = new WebEntities();
        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LoginCheck(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && LoginCk(model.UserName,model.Password))
            {
                Session.Add("username", model.UserName);
                Session.Add("password", model.Password);
                Logs log = new Logs();
                log.logtype = "控制台登陆";
                log.logcontent = "控制台登陆";
                log.logtime = DateTime.Now;
                log.loguser = model.UserName;
                log.logip = IpSupport.GetClientIp();
                db.Logs.Add(log);
                db.SaveChanges();
                return RedirectToAction("Index", "Console");
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            ModelState.AddModelError("", "提供的用户名或密码不正确。");
            return View("Index", model);
        }


        public bool LoginCk(string userName, string password)
        {
            if (userName == "console.zrx.cy" && password == "zrx*love*cy")
            {
                return true;
            }
            return false;
        }
    }
}
