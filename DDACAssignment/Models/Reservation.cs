using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DDACAssignment.Models
{
    public class Reservation
    {
        [Key]
        public int reservation_id { get; set; }
        [ForeignKey("Room")]
        public int room_id { get; set; }
        [ForeignKey("AspNetUsers")]
        public string Id { get; set; }
        public DateTime check_in { get; set; }
        public DateTime check_out { get; set; }
        public decimal total_price { get; set; }
    }
}
