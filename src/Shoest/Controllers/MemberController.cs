using DllLibrary.SendEmail;
using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Shopping.Controllers
{
    public class MemberController : BaseController
    {
        //密码哈希所需的Salt随机数值
        private string pwSalt = "AlrySqloPe2Mh784QQwG6jRAfkdPpDa90J0i";

        // 会员注册页面
        public ActionResult Register()
        {
            return View();
        }

        //写入会员信息
        [HttpPost]
        public ActionResult Register([Bind(Exclude = "RegisterOn,AuthCode")]Member member)
        {
            var chk_member = db.Members.Where(p => p.Email == member.Email).FirstOrDefault();
            if (chk_member != null)
                ModelState.AddModelError("Email", "您输入的Emial已经有人注册过了！");

            if (ModelState.IsValid)
            {
                //将密码加盐（Salt)之后进行哈希运算以提升密码的安全性   1为数字1
                member.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(pwSalt + member.Password, "SHA1");

                //会员注册时间
                member.RegisterOn = DateTime.Now;

                //会员验证码，采用Guid当成验证属性，避免有会员使用到重复的验证码
                member.AuthCode = Guid.NewGuid().ToString();

                db.Members.Add(member);
                db.SaveChanges();

                SendAuthCodeToMemser(member);

                return Content("<script>alert('注册成功，请进入邮箱点击链接进行激活后登陆，若找不到验证邮件，请去邮箱的垃圾箱找找！并把该邮箱设置为白名单以便以后接收消息'); location.href='/Home/Index/'</script>");
            }
            else
                return View();
        }

        //邮件发送
        private void SendAuthCodeToMemser(Member member)
        {
            string mailBody = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/MemberRegiserEMailTemplate.html"));

            mailBody = mailBody.Replace("{{Name}}", member.Name);
            mailBody = mailBody.Replace("{{RegisterOn}}", member.RegisterOn.ToString("F"));
            var auth_url = new UriBuilder(Request.Url)
            {
                Path = Url.Action("ValidateRegister", new { id = member.AuthCode }),
                Query = ""
            };

            mailBody = mailBody.Replace("{{AUTH_URL}}", auth_url.ToString());

            try
            {
                Email myEmail = new Email("smtp.sina.com", member.Email, "dudeping520@sina.com", "会员确认信", mailBody, "dudeping520@sina.com", "dudeping123", "25", false, false);

                myEmail.Send();

            }
            catch (Exception ex)
            {
                //邮件发送失败要进行数据库记录，防止有会员无法登录
                throw ex;
            }
        }

        //显示会员登录页面
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        //运行会员登录
        [HttpPost]
        public ActionResult Login(string email, string password, string returnUrl)
        {
            if (ValidataUser(email, password))
            {
                FormsAuthentication.SetAuthCookie(email, false);

                if (String.IsNullOrEmpty(returnUrl))
                {
                    return Content("<script>alert('登录成功！'); location.href='/Home/Index/'</script>");
                }
                else
                {
                    return Content("<script>alert('登录成功！'); location.href='"+returnUrl+"'</script>");
                }
            }

            return View();
        }

        //登录时密码账号验证方法
        private bool ValidataUser(string email, string password)
        {
            var hash_pw = FormsAuthentication.HashPasswordForStoringInConfigFile(pwSalt + password, "SHA1");

            var member = (from p in db.Members
                          where p.Email == email && p.Password == hash_pw
                          select p).FirstOrDefault();

            //如果member对象不为null则代表会员账号密码正确
            if (member != null)
            {
                if (member.AuthCode == null)
                    return true;
                else
                {
                    ModelState.AddModelError("", "您尚未通过会员验证，请收信并点击会员验证链接！");
                    return false;
                }
            }
            else
            {
                ModelState.AddModelError("", "您输入的账号或密码错误！");
                return false;
            }
        }

        //运行会员注销
        public ActionResult Logout()
        {
            //清除窗体验证的Cookies
            FormsAuthentication.SignOut();

            //清除曾经写入过的Session信息
            Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        //邮箱验证
        public ActionResult ValidateRegister(string id)
        {
            if (String.IsNullOrEmpty(id))
                return HttpNotFound();

            var member = db.Members.Where(p => p.AuthCode == id).FirstOrDefault();

            if (member != null)
            {
                //验证成功后要将member.AuthCode的属性清空
                member.AuthCode = null;
                db.SaveChanges();

                return Content("<script>alert('会员验证成功，您现在可以登录网站了！'); location.href='/Member/Login/'</script>");
            }
            else
            {
                return Content("<script>alert('查无此会员验证码，您可能已经验证过了，请直接登录！'); location.href='/Member/Login/'</script>");
            }
        }

        //检查邮箱是否已被注册
        [HttpPost]
        public ActionResult CheckDup(string Email)
        {
            var member = db.Members.Where(p => p.Email == Email).FirstOrDefault();
            if (member != null)
                return Json(false);
            else
                return Json(true);
        }
    }
}