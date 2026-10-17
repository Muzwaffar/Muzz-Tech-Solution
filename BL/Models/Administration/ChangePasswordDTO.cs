using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class ChangePasswordDTO
    {
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }

    }

}
