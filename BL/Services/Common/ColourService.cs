using BL.Models.Common;
using DAL.Common;
using DAL.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public class ColourService
    {
        public async Task<string> GetColoour(Status srs)
        {
            string Colour = "";
            try
            {
                switch (srs)
                {
                    case Status.Draft:
                        Colour = "#b8b8b8";
                        break;
                    case Status.Schedule:
                         Colour = "#000596";
                        break;
                    case Status.Completed:
                         Colour = "#00850b";
                        break;
                    case Status.Accepted:
                         Colour = "#91037c";
                        break;
                    case Status.Rejected:
                        Colour = "#960505";
                        break;
                    case Status.Started:
                         Colour = "#bda404";
                        break;
                    case Status.Stopped:
                         Colour = "#ad4902";
                        break;
                    case Status.Cancelled:
                         Colour = "#960505";
                        break;
                    case Status.Unschedule:
                         Colour = "#141414";
                        break;

                }

               
            }
            catch (Exception ex)
            {
                Colour = "#05f5d5";
            }

            return Colour;

        }
        public string GetColour(Status srs)
        {
            string Colour = "";
            try
            {
                switch (srs)
                {
                    case Status.Draft:
                        Colour = "#b8b8b8";
                        break;
                    case Status.Schedule:
                        Colour = "#000596";
                        break;
                    case Status.Completed:
                        Colour = "#00850b";
                        break;
                    case Status.Accepted:
                        Colour = "#91037c";
                        break;
                    case Status.Rejected:
                        Colour = "#960505";
                        break;
                    case Status.Started:
                        Colour = "#bda404";
                        break;
                    case Status.Stopped:
                        Colour = "#ad4902";
                        break;
                    case Status.Cancelled:
                        Colour = "#960505";
                        break;
                    case Status.Unschedule:
                        Colour = "#141414";
                        break;

                }


            }
            catch (Exception ex)
            {
                Colour = "#05f5d5";
            }

            return Colour;

        }
        //Draft = "#b8b8b8"

        //Schedule = "#000596",

        //Completed = "#00850b",

        //Accepted = "#91037c",

        //Rejected = "#960505",

        //Started = "#bda404",

        //Stopped = "#ad4902",

        //Cancelled = "#960505",

        //Unschedule = "#141414",

    }
}
