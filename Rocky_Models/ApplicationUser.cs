﻿using Microsoft.AspNetCore.Identity;

namespace Rock_Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

    }
}
