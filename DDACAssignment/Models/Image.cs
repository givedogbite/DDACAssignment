using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DDACAssignment.Models
{
    public class Image
    {
        [Key]
        public string image_id { get; set; }
        public string image_name { get; set; }
        public string room_image { get; set; }
    }
}
