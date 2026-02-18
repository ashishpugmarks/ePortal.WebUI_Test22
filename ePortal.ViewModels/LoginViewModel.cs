using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ePortal.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;


namespace ePortal.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Login Id")]
        public string Loginid { get; set; }

        public List<ContentViewModel> ContentList { get; set; }
        public List<ContentViewModel> NewsListUpperSec { get; set; }
        public List<ContentViewModel> NewsListBottomSec { get; set; }

        public string selectedusertype { get; set; }

        public List<SelectListItem> usertype { get; set; }

       [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ForgotECode")]
        public string ForgotEcode { get; set; }

        [Required(ErrorMessage = "Field can't be empty")]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        [Display(Name = "ForgotEmailid")]
        public string ForgotEmailid { get; set; }
        public string APIaddress { get; set; }

    }

    public class TokenRequest
    {
        public string ExpiredToken { get; set; }
        public string RefreshToken { get; set; }
    }

    [Table("REFRESHTOKEN")]
    public class RefreshToken
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("TOKEN")]
        [Required]
        [StringLength(255)]
        public string Token { get; set; }

        [Column("EXPIRATION")]
        [Required]
        public DateTime Expiration { get; set; }
    }

    [Table("JWTUSERLOG")]
    public class JwtUserLog
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("USERNAME")]
        [Required]
        [StringLength(255)]
        public string UserName { get; set; }

        [Column("REFRESHTOKENID")]
        public int? RefreshTokenId { get; set; }

        [ForeignKey("RefreshTokenId")]
        public virtual RefreshToken RefreshToken { get; set; }
    }
}