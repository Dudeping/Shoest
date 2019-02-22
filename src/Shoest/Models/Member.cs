using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Shopping.Models
{
    [DisplayName("会员信息")]
    [DisplayColumn("Name")]
    public class Member
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("会员账号")]
        [Required(ErrorMessage = "请输入Email地址")]
        [Description("我们直接以Email当成会员的登录账号")]
        [MaxLength(250, ErrorMessage = "Email地址长度无法超过250个字符")]
        [DataType(DataType.EmailAddress)]
        [Remote("CheckDup", "Member", HttpMethod ="POST", ErrorMessage ="您输入的Email已经有人注册过了！")]
        public string Email { get; set; }

        [DisplayName("会员密码")]
        [Required(ErrorMessage = "请输入会员密码")]
        [MaxLength(40, ErrorMessage = "密码不得超过40个字符")]
        //1为数字1
        [Description("密码将以SHA1进行哈希运算，通过SHA1哈希运算后的结果转为HEX表示法的字符串长度皆为40个字符")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("中文姓名")]
        [Required(ErrorMessage = "请输入中文姓名")]
        [MaxLength(5, ErrorMessage = "中文姓名不可超过5个字")]
        [Description("暂不考虑有外国人用英文注册会员的情况")]
        public string Name { get; set; }

        [DisplayName("网络昵称")]
        [Required(ErrorMessage = "请输入网络昵称")]
        [MaxLength(10, ErrorMessage = "网络昵称请勿超过10个字")]
        public string Nickname { get; set; }

        [DisplayName("会员注册时间")]
        public DateTime RegisterOn { get; set; }

        [DisplayName("会员启用码")]
        [MaxLength(36)]
        [Description("当AuthCode等于NULL则代表此会员已经通过Email有效验证")]
        public string AuthCode { get; set; }

        public virtual ICollection<OrderHeader> Orders { get; set; }
    }
}
