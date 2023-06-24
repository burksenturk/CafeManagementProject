﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeManagement.Core.Dtos
{
    public class CustomerCreateDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? IdentityNumber { get; set; }
        public DateTime BirthDate { get; set; }

        public int CompanyId { get; set; }
    }
}
