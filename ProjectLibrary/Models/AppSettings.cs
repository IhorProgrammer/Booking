﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLibrary.Models
{
    public class AppSettings
    {
        public string? Secret { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }

        public string? EmailFrom { get; set; }
        public string? EmailFromPassword { get; set; }
        public string? FromName { get; set; }


    }
}
