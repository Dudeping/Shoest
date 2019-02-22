using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Shopping.Controllers
{
    public class CartController : BaseController
    {
        // 添加商品项目到购物车，如果没有传入Amount参数则默认购买数量为1
        //因为知道要通过Ajax调用这个Action， 所以可以先标示[HttpPost]属性
        [HttpPost]
        public ActionResult AddToCart(int ProductId, int Amount = 1)
        {
            var product = db.Products.Find(ProductId);

            //验证产品是否存在
            if (product == null)
                return HttpNotFound();

            var existringCart = this.Carts.FirstOrDefault(p => p.Product.Id == ProductId);
            if (existringCart != null)
            {
                existringCart.Amount += Amount;
            }
            else
            {
                this.Carts.Add(new Cart() { Product = product, Amount = Amount });
            }
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.Created);
        }

        //显示当前购物车项目
        public ActionResult Index(int p=1)
        {
            return View(this.Carts);
        }

        //移除购物车项目
        //因为知道要通过Ajax调用这个Action， 所以可以先标示[HttpPost]属性
        [HttpPost]
        public ActionResult Remove(int ProductId)
        {
            var existringCart = this.Carts.FirstOrDefault(p => p.Product.Id == ProductId);
            if (existringCart != null)
            {
                this.Carts.Remove(existringCart);
            }
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        //更新购物车中特定项目的购买数量
        //因为知道要通过Ajax调用这个Action， 所以可以先标示[HttpPost]属性
        [HttpPost]
        public ActionResult UpdateAmount(List<Cart> Carts)
        {
            foreach (var item in Carts)
            {
                var existringCart = this.Carts.FirstOrDefault(p => p.Product.Id == item.Product.Id);

                if (existringCart != null)
                {
                    existringCart.Amount = item.Amount;
                }
            }
            return RedirectToAction("Index", "Cart");
        }

        protected override void HandleUnknownAction(string actionName)
        {
            Response.Redirect("http://www.baidu.com");
        }
    }
}