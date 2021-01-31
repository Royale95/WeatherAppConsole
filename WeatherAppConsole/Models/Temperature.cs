using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TempData.Models
{
    public class Temperature
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public double Temperatures { get; set; }
        public double Humidity { get; set; }

    }
}

