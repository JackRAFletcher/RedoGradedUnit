using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GradedUnitV2.POCO
{
    //this class holds the attributes for the customer
    public class Customer
    {
        [Required]
        public string CustomerID { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public string MobileTelephone { get; set; }
        public string HomeTelephone { get; set; }
    }
}