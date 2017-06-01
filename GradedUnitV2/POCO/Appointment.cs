using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GradedUnitV2.POCO
{
    //this class holds all attributes for the appointment
    public class Appointment
    {
        [Required]
        public string AppointmentID { get; set; }
        [Required]
        public String CustomerID { get; set; }
        [Required]
        public string ConsultantID { get; set; }
        public DateTime DateOfConsultation { get; set; }
    }
}