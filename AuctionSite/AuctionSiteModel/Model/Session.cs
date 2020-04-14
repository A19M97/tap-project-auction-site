using System;
using System.ComponentModel.DataAnnotations;

namespace Mugnai.Model
{
    public class Session
    {
        public string Id { get; set; }

        [Required]
        public DateTime ValidUntil { get; set; }

    }
}