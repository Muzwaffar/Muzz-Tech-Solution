using BL.Models.Common;
using DAL.Common;
using DAL.Models.Common;
using DAL.Models.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Schedule
{
    public class ScheduleDTO
    {
        public long ServiceRequestId { get; set; }
        public string ServiceRequestCode { get; set; }
        public string CustomerName { get; set; }
        public DateTime Date { get; set; }
        public Status Status { get; set; }
        public List<NPF_SCH_Schedule>? data { get; set; } = new List<NPF_SCH_Schedule>();
    }
    public class ScheduleListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string AssignedTo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
    }
    public class StatusChange
    {
        public long Id { get; set; }
        public Status Status { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
    public class IncidentStatusChange
    {
        public long Id { get; set; }
        public IncidentStatus Status { get; set; }
        public float CenterPointLat { get; set; } = 0;
        public float CenterPointLong { get; set; } = 0;
        public string Type { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
    public class SaveFileRemarks
    {
        public long scheduleId { get; set; }
        public string str { get; set; } = string.Empty;
        public string fileName { get; set; } = string.Empty;
    }
    public class GetByDateDTO
    {
        public long Id { get; set; }
        public DateTime StartTime { get; set; } 
        public DateTime EndTime { get; set; } 
    }
    public class CalenderDTO
    {
        public long id { get; set; }
        public string title { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string color { get; set; }
        public string backgroundcolor { get; set; }
    }
    public class CalenderMobileDTO
    {
        public long id { get; set; }
        public string eventName { get; set; }
        public DateTime timeFrom { get; set; }
        public DateTime timeTo { get; set; }
        public string background { get; set; }
        public string clientName { get; set; }
        public int openItem { get; set; }
        public int completedItem { get; set; }
        public Status status { get; set; }
        public bool isAllDay { get; set; } = false;
    }
    public class DateRangeMobileDTO
    {
        public string userId { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }
    }
    public class AllTaskMobileDTO
    {
        public long scheduleId { get; set; }
        public string scheduleName { get; set; }//Title
        public string clientName { get; set; }
        public string zoneName { get; set; }
        public string time { get; set; }
        public IncidentStatus status { get; set; }
        public int numSchedule { get; set; }
    }
    public class CalenderViewDTO
    {
        public long Id { get; set; }
        public string ScheduleName { get; set; }
        public string ClientName { get; set; }
        public Status Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string DepartureAdd { get; set; }
        public string ArrivalAdd { get; set; }
        public List<string> Services { get; set; } = new List<string>();
        public List<string> Helpers { get; set; } = new List<string>();
        public List<string> Remarks { get; set; } = new List<string>();
    }
    public class CalendarGrid
    {
        public long AsssetId { get; set; }
        public string AssetName { get; set; }
        public string EquipementConsumable { get; set; }
        public int Quantity { get; set; }
    }
    public class FilesGrid
    {
        public long FileId { get; set; }
        public string FileName { get; set; }

    }
    public class GetFile
    {
        public string File { get; set; }
        public string FileName { get; set; }

    }
    public class HelpersGridDTO
    {
        public long ResourceId { get; set; }
        public string Name { get; set; }
        // public bool Status { get; set; }
    }
    public class HelpersDTO
    {
        public List<HelpersGridDTO> Helpers { get; set; } = new List<HelpersGridDTO>();
        public DropDown Dd { get; set; } = new DropDown();
    }
    public class AddEditSchduleDTO
    {
        public long SchduleId { get; set; }
        public string Form { get; set; }
        public long? FormPage { get; set; }
       // public long Items { get; set; }
        //public long? ServiceRquestId { get; set; }
        public string ScheduleName { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime? EndTime { get; set; }
        public long? Zone { get; set; }
        //public List<long> Services { get; set; } = new List<long>();
        public List<HelpersTableDTO> Helpers { get; set; } = new List<HelpersTableDTO>();
        //public List<string> Remarks { get; set; } = new List<string>();
        public string Remark { get; set; } = string.Empty;
        public bool IsMobile { get; set; } = false;
    }
	public class ItemListDTO
	{
		public long Id { get; set; }
		public string Name { get; set; }

	}
    public class HelpersTableDTO
    {
        public long ResourceId { get; set; }
        public string Name { get; set; }

    }

    public class ScheduleEqGrid
    {
        public long AsssetId { get; set; }
        public long? ServiceId { get; set; }
        public string AssetName { get; set; }
        public string EquipmentConsumable { get; set; }
        public int Quantity { get; set; }
    }

    public class ClientSchSerReqDTO
    {
        public long clientId { get; set; }
        public string clientName { get; set; }// 1 Title# TBA

        public long scheduleId { get; set; }//IncidentId
        public string scheduleName { get; set; }// 2 naming to be used#################
        public string serviceRequestCode { get; set; }// 3 Address
        public string additional { get; set; } = string.Empty;
        public int openItem { get; set; } = 0;
        public int completedItem { get; set; } = 0;
    }
    public class GetTaskDetailMobileDTO
    {
       public TaskHeader taskHeader { get; set; } = new TaskHeader();
       public Info info { get; set; } = new Info();
       public List<Details> details { get; set; } = new List<Details>();
       public List<Equipment> equipment { get; set; } = new List<Equipment>();
       public List<Remarks> remarks { get; set; } = new List<Remarks>();
       public List<Files> File { get; set; } = new List<Files>();
       public Route route { get; set; } = new Route();
    }
    public class Files
    {
        public string file { get; set; }
        public string fileName { get; set; }
        public long fileId { get; set; }
    }
    public class TaskHeader
    {
        public long id { get; set; }    
        public long? formId { get; set; }    
        public string taskTitle { get; set; }//MFRS
        public string client { get; set; } //IncidentType
        public string code { get; set; }//IncidenNo
        public string additional { get; set; }//IncidenNo
        public ScanStatus ScanStatus { get; set; }
        public IncidentStatus Status { get; set; }

    }
    public class Info
    {
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string contactPerson { get; set; }
        public string contactNumber { get; set; }
        public List<Helper> helper { get; set; } = new List<Helper>();
        public List<Agencies> agencies { get; set; } = new List<Agencies>();
        public List<Vehicle> vehicle { get; set; } = new List<Vehicle>();
        public List<Equipment> equipment { get; set; } = new List<Equipment>();
    }
    public class Agencies
    {
        public string agencyName { get; set; }
        public string agencyPhone { get; set; }
    }
    public class Helper
    {
        public string helper { get; set; }
        public long Id { get; set; }
        public HelperStatus Status { get; set; } = HelperStatus.Pending;
    }
    public class Helperstat
    {
        public long Id { get; set; }
        public HelperStatus Status { get; set; }
    }
    public class Resstat
    {
        public long Id { get; set; }
        public ResourceStatus Status { get; set; }
    }
    public class Details
    {
        public long id { get; set; }
        public string serviceName { get; set; }
        public Status? status { get; set; }
    }
    public class Equipment
    {
        public string title { get; set; }
        public string equipmentName { get; set; }
        public int quantity { get; set; }
    }
    public class Vehicle
    {
        public string vehicle { get; set; }
    }
    public class GetTaskDetailMobile
    {
        public long id { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
    }
    public class Remarks
    {
        public string remarks { get; set; }
    }
    public class Route
    {
        public List<ViaPoint> viaPoint { get; set; } = new List<ViaPoint>();
    }
    public class ViaPoint
    {
        public string viaPointName { get; set; }
        public DateTime viaPoinTime { get; set; }
        public bool isFirst { get; set; }
        public CenterPoint centerPoint { get; set; } = new CenterPoint();


    }
    public class CenterPoint
    {
        public float? lat { get; set; }
        public float? lon { get; set; }
    }


    public class ServiceListDTO
    {
        public long serviceRequestId { get; set; }
        public long scheduleId { get; set; }
    }
    public class ScheZone 
    {
        public string Name { get; set; }
        public long ScheduleId { get; set; }
        public string Type { get; set; }
        public float CenterPointLat { get; set; }
        public float CenterPointLong { get; set; }

    }
    public class SignatureMobile
    {
        public long srId { get; set; }
        public string document { get; set; } =string.Empty;
        public string signature { get; set; } = string.Empty;
        public Signed signed { get; set; }
    }
    public class QRDTO
    {
        public long schId { get; set; }
        public string code { get; set; }

    }
    public class CustomChecklist
    {
        public List<CheckList> checklist { get; set; } = new List<CheckList>();
        public bool isSave { get; set; }

    }
    public class CustomChecklistWithRemarks
    {
        public List<CheckListR> checklist { get; set; } = new List<CheckListR>();

        public bool isSave { get; set; }

    }
    public class SaveCustomChecklist
    {
        public string checklist { get; set; }
        public long scheduleId { get; set; }

    }
    public class CustomAccess
    {
        public string name { get; set; }
        public bool hasAccess { get; set; }

    }
    public class CheckList
    {
        public long id { get; set; }
        public string name { get; set; }
        public bool value { get; set; }
        public bool isDisable { get; set; }
        public bool hasRemarks { get; set; }
        public string remarks { get; set; }

    }
    public class CheckListR
    {
        public long id { get; set; }
        public string name { get; set; }
        public bool value { get; set; }
        public bool isDisable { get; set; }
        public bool hasRemarks { get; set; }
        public bool isText { get; set; }
        public string remarks { get; set; }

    }
    public class ChecklistRemarks
    {
        public long id { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public bool isDisable { get; set; }

    }
    
    public class UpdateCheckListItem
    {
        public long id { get; set; }//woelemId
        public bool value { get; set; }
        public string remarks { get; set; } = string.Empty ;

    }
    public class GetServicesDetails
    {
        public long serviceRequestId { get; set; }
        public List<long> serviceIds { get; set; }
    }
    public class JobRange
    {
        public DateTime From { get; set; } = DateTime.Now;
        public DateTime To { get; set; } = DateTime.Now;

    }

    //public class Values
    //{
    //    public long id { get; set; }
    //    public string text { get; set; }
    //    public bool selected { get; set; } = false;
    //    public bool disabled { get; set; } = false;
    //}
}

