using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GradedUnitV2.POCO
{
    //this class holds all the attributes for the viewing
    public class Viewing
    {
        public Viewing()
        {
            this.Date = DateTime.Now;
        }
            
        [Required]
        public int PropertyNumber { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public string AppointmentID { get; set; }




    }
}