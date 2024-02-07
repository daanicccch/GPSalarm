﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GPSclocker.Models
{
    public class GpsItem
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; } 
        public double Longitude { get; set; }
        public bool IsEnabled { get; set; }
    }
}
