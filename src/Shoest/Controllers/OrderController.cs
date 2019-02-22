using Shopping.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopping.Models
{
    [Authorize] //必须登录会员才能使用订单结账功能
    public class OrderController : BaseController
    {
        // 显示订单的窗体
        public ActionResult Complete()
        {
            return View();
        }

        //将订单信息与购物车信息写入数据库
        [HttpPost]
        public ActionResult Complete(OrderHeader form)
        {
            var member = db.Members.Where(p => p.Email == User.Identity.Name).FirstOrDefault();
            if (member == null) return RedirectToAction("Index", "Home");

            if (this.Carts.Count == 0) return RedirectToAction("Index", "Cart");

            //将订单信息与购物车信息写入数据库写入数据库
            OrderHeader oh = new OrderHeader()
            {
                Member = member,
                ContactName = form.ContactName,
                ContactAddress = form.ContactAddress,
                ContactPhoneNo = form.ContactPhoneNo,
                BuyOn = DateTime.Now,
                Memo = form.Memo,
                OrderDetailItems = new List<OrderDetail>()
            };
            int total_price = 0;
            foreach (var item in this.Carts)
            {
                var product = db.Products.Find(item.Product.Id);
                if (product == null) return RedirectToAction("Index", "Cart");

                total_price += item.Product.Price * item.Amount;
                oh.OrderDetailItems.Add(new OrderDetail() { Product = product, Price = product.Price, Amount = item.Amount });
            }
            oh.TotalPrince = total_price;

            db.Orders.Add(oh);
            db.SaveChanges();

            //订单完成后必须清空现有购物车的信息
            this.Carts.Clear();

            //订单完成后回到网站首页
            return Content("<script>alert('下单成功！快递小哥正在火速赶往你的身边。'); location.href='/Home/Index/'</script>");
        }
    }
}