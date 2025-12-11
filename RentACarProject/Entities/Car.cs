using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentACarProject.Entities
{
    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; }      
        public string Model { get; set; }      
        public int Year { get; set; }          
        public decimal DailyPrice { get; set; } 
        public string Status { get; set; }
        public string ImagePath { get; set; }
    }
}
