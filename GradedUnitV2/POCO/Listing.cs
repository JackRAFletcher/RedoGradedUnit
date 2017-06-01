using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GradedUnitV2.POCO
{
    //Class which stores the basic data model of the properties class in the data model. Due to a clash of the name properties I have re-named this to listings
    public class Listing
    {
        //get and set all the listing attributes - PropertyNumber(KEY) StreetAddress Town and ClientID(Foreign key from client table)
        [Required]
        public int PropertyNumber { get; set; }
        public string StreetAddress { get; set; }
        public string Town { get; set; }
        public string ClientID { get; set; }
        [Required]
        public virtual Owner Owner { get; set; }
    }
}