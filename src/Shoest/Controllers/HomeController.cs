using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Shopping.Controllers
{
    public class HomeController : BaseController
    {
        // 首页--商品类别列表
        public ActionResult Index(int p=1)
        {
            var productcategories = db.ProductCategories;
            //插入数据
            if (productcategories.Count() == 0)
            {
                List<ProductCategory> entity = new List<ProductCategory>();
                entity.Add(new ProductCategory { Name = "生活用品" });
                entity.Add(new ProductCategory { Name = "学习用品" });
                entity.Add(new ProductCategory { Name = "数码产品" });
                entity.Add(new ProductCategory { Name = "床上用品" });
                foreach (var item in entity)
                {
                    db.ProductCategories.Add(item);
                    db.SaveChanges();
                }

                foreach (var item in productcategories.ToList())
                {
                    for (int i = 0; i < 100; i++)
                    {
                        var name = item.Name + "类别下的商品" + i;
                        if (db.Products.Where(m => m.Name == name).Count() == 0)
                        {
                            Product product = new Product();
                            product.Name = item.Name + "类别下的商品" + i;
                            product.Color = System.Drawing.Color.Red;
                            product.ProductCategory = item;
                            product.Price = 99;
                            product.Description = "N/A";
                            product.PublishOn = DateTime.Now;

                            db.Products.Add(product);
                            db.SaveChanges();
                        }
                    }
                }
            }
            var data = db.ProductCategories.ToList();
            var pageDate = data.ToPagedList(pageNumber: p, pageSize: 10);
            return View(pageDate);
        }

        //商品列表
        public ActionResult ProductList(int id=1, int p = 1)
        {
            var productCategory = db.ProductCategories.Find(id);
            if(productCategory != null)
            {
                var data = productCategory.Products.ToList();
                var pageDate = data.ToPagedList(pageNumber: p, pageSize: 10);
                return View(pageDate);
            }
            else
            {
                return HttpNotFound();
            }
        }

        //商品明细
        public ActionResult ProductDetail(int id = 1)
        {
            return View(db.Products.Find(id));
        }
    }
}