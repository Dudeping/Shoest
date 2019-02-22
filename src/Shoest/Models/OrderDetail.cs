using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.Models
{
    [DisplayName("订单明细")]
    [DisplayColumn("Name")]
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("订单主文件")]
        [Required]
        public virtual OrderHeader OrderHeader { get; set; }

        [DisplayName("订购商品")]
        [Required]
        public Product Product { get; set; }

        [DisplayName("商品售价")]
        [Required(ErrorMessage = "请输入商品售价")]
        [Range(1, 99999, ErrorMessage = "商品售价必须在1-99999之间")]
        [DataType(DataType.Currency)]
        [Description("因为订单金额可能会受到递送方式或优惠折扣等方式异动价格，因此必须保留购买当下算出来的订单金额")]
        public int Price { get; set; }

        [DisplayName("选购数量")]
        [Required]
        public int Amount { get; set; }
    }
}
