using BL.Models.Common;
using BL.Models.Incident;

namespace BL.Models.Incident
{
    public class IncidentDTO
    {
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Type { get; set; }
        public string SubType { get; set; }
        public string IncidentCode { get; set; }
        public string IncidentNo { get; set; }
        public string Location { get; set; }
        public string Coordinates { get; set; }
        public string NatureOfIncident { get; set; }
        public string Station { get; set; }
        public string Timelapse { get; set; }
    }
    public class Getparams
    {
        public string FleetDataURL { get; set; }
        public string FleetLoginPayload { get; set; }
        public string FleetMapPayload { get; set; }

    }
    public class IncidentStationDTO
    {
        public string Station { get; set; }
        public string NatureOfIncident { get; set; }

    }
}
