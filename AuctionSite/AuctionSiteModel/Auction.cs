using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mugnai
{
    public class Auction
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime EndsOn { get; set; }
        public string SiteName { get; set; }
        [ForeignKey("SiteName")]
        public Site Site { get; set; }
    }
}