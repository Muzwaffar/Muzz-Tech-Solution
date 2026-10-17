using BL.Models.Common;
using DAL.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public  class ContextService
    {
        public NPFContext getNPFContext()
        {
            NPFContext wkcontext = new NPFContext();

            return wkcontext;
        }

    }
}
