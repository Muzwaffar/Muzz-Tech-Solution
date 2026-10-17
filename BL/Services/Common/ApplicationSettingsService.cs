using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public  class ApplicationSettingsService
    {

        public ApplicationSettingsHandler getApplicationSettingsHandler()
        {
            ApplicationSettingsHandler aph = new ApplicationSettingsHandler();


            return aph;
        }

    }
}
