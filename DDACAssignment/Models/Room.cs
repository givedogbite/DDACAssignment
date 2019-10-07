using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DDACAssignment.Models
{
    public class Room
    {
        [Key]
        public int room_id { get; set; }
        public string room_type { get; set; }
        public string room_availability { get; set; }
    }
}
