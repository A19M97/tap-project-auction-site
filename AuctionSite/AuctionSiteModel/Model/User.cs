using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TAP2018_19.AuctionSite.Interfaces;

namespace Mugnai.Model
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [MinLength(DomainConstraints.MinUserName), MaxLength(DomainConstraints.MaxUserName), Index("IX_UserUnique", 1, IsUnique = true), Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [MinLength(DomainConstraints.MinSiteName), MaxLength(DomainConstraints.MaxSiteName), Index("IX_UserUnique", 2, IsUnique = true), Required]
        public string SiteName { get; set; }

        [ForeignKey("SiteName")]
        public virtual Site Site { get; set; }

        public virtual string SessionId { get; set; }

        [ForeignKey("SessionId")]
        public virtual Session Session { get; set; }
    }
}