using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ImSuperSir.Security.Security
{
    public class SecurityContex : IdentityDbContext
    {
        public SecurityContex(DbContextOptions<SecurityContex> options) 
            : base(options)
        {
            
        }
    }
}