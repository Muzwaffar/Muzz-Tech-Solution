using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Common
{
    public class Databases
    {
        public static readonly string NPF = "NPF";

    }
    public enum Status
    {
        Draft = 1,
        Schedule = 2,
        Completed = 3,
        Accepted = 4,
        Rejected =5,
        Started =6,
        Stopped = 7,
        Cancelled = 8,
        Unschedule = 9,
        Inprogress =10,
        Submitted = 11,
        SubmittedLvl2 = 12,
        DrafLvl2 = 13,
        DrafLvl3 = 14,
        DrafLvl4 = 15,
        SubmittedLvl3 = 16,
        Rework = 17,
        Open = 18,

    }
    public enum IncidentStatus
    {
        Pending=10,
        Assigned = 1,
        Accepted = 2,
        TurnOut = 3,
        Onsite = 4,
        UnderControl = 8,
        Completed = 5,
        LeftSite = 6,
        ReachedStation = 7,
        Leavesite=20,
    }
    public enum TransactionStatus
    {
        Draft = 1,
        Open = 2,
        Submitted =3,
        Completed = 4,
        Abort =5,

    }
    public enum StationStatus
    {
        Pending = 1,
        Acknowledged = 2,
        Open = 3,
        NotAcknowledge = 4,

    }
    public enum GeneralStatus
    {
        Open = 1,
        Closed = 2,
        Inprogress = 3,
        Completed = 4,
    }
    public enum MRequestType
    {
        Asset = 1,
        Agency = 2,
        Resource = 3,
        Equipment = 4,
    }
    public enum CaseStatus
    {
        Draft = 1,
        Schedule = 2,
        Completed = 3,
        Accepted = 4,
        Rejected = 5,
        Started = 6,
        Stopped = 7,
        Cancelled = 8,
        Unschedule = 9,
        Inprogress = 10,
        Submitted = 11,//
        Approved = 12,//
        Declined = 13,//
        AwaitingApproval = 14,//
    }
    public enum RequestType
    {
        In = 1,
        Out = 2,
        Transfer = 3,
        Ajustment =4,
    }
    public enum AttributeType
    {
        POWC = 1,
        POWLS = 2,
        LRC = 3,
        LRLS = 4,
        TMC = 5,
        TMLS = 6,
        RPC = 7,
        FRMP=8,
        RPLS = 9,
        RPF = 10,
        none=11,
        livestockevent = 12,
        cropevent = 13,
    }
    
    public enum WFType
    {
        Project = 1,
        Case = 2,
        Process = 3,
    }
    public enum AType
    {
        Approval = 1,
        Activity = 2,
        Intergration =3,
        None=4,
    }
    public enum RModule
    {
        Approval = 1,
        Activity = 2,
        Intergration = 3,
    }
    public enum NotifType
    {
        Push = 1,
        Email = 2,
        Sms = 3,
    }
    public enum TargetType
    {
        API = 1,
        Fucntion = 2,
        Redirect = 3,
    }
    public enum Priority
    {
        Low = 1,
        Medium = 2,
        High = 3,
    }
    public enum MessageType
    {
        Status = 1,
        Assigned = 2,
        UnAssigned = 3,
        Changes =4,
        Pending =5,
        Ack =6,
        Txt =8,
        CRR =9,
        REQ =10,
    }
    public enum HelperStatus
    {
        Pending = 1,
        Acknowledged = 2,
        Decline = 3,
    }
    public enum ResourceStatus
    {
        NoShow = 1,
        Show = 2,
    }
    public enum OperationType
    {
        INSERT = 1,
        UPDATE = 2,
        DELETE = 3,
        ALL = 4,
    }
    public enum Signed
    {
        YES = 1,
        NO = 2,
    }
    public enum ScanStatus
    {
        DEFAULT = 1,
        VALID = 2,
        INVALID =3,
        ERROR=4
    }

    public enum AccessOperationType
    {
        Create = 1,
        Read =2,
        Update = 3,
        Delete = 4,
        Approve =5,
        Upload =6,
        Export =7,

    }
    public enum AccessLevel
    {
        Self = 1,
        Matrix = 2,
        Custom = 3,
    }
    public enum ParentModule
    {
        Research = 1,
        Extension = 2,
    }
    public enum ResearchType
    {
        Crop = 1,
        LiveStock = 2,
        Both = 3,
        Biofarming = 4,
        Fodder = 5
    }
    public enum ApprovalStatus
    {
        Approved = 1,
        Declined = 2,
        AwaitingApproval = 3,
        Draft = 4,
        Cancelled = 5,
        Saved = 6,
        None = 7,
    }
    public enum RowStructType
    {
        Role = 1,
        User = 2,
    }
    public enum ModuleName
    {
        ResearchCropPOW = 1,
        ResearchLiveStockPOW = 2,
        ResearchCropLR = 3,
        ResearchLiveStockLR = 4,
        ResearchCropTM = 5,
        ResearchLiveStockTM = 6,
    }
    public enum ResearchSubModule
    {
        POW = 1,
        LR = 2,
        TM = 3,
        RP = 4,
        RPL = 5,
        RU = 6,
        RPF = 7,
        Form = 8
    }
    public enum ResourceType
    {
        Officer = 1,
        ExternalCollaborator = 2,
        InternalCollaborator = 3,
        Farmer = 4,
        Breeder = 5,
    }
    public enum PageName
    {
        MyActivities = 1,
        Incident=2,
    }

    public enum ActivityElementType
    {
        Text = 1,
        Number = 2,
        Dropdown = 3
    }

    public enum ActivityElementSource
    {
        SchemeName = 1,
        SchemeCode = 2,
        Section = 3,
        AnimalStock = 4,
        CropStock = 5,
        FieldProduceBook = 6,
        StationLandRequirement = 7,
    }

    public enum Categorisation
    {
        GroupType = 1,
        Category = 2,
        Species = 3,
        Variety = 4
    }

    public enum AnimalProducts
    {
        Milk = 1,
        Colostrum = 2,
        Others = 3,
        Crop = 4,
        Livestock = 5
    }

    public enum ActivityModule
    {
        General = 1,
        Station = 2
    }
	public enum ScheduleForm
	{
		Extension = 1,
		ResearchandStationManagement = 2
	}
    public enum AssignmentType
    {
        Resource = 1,
        Location = 2,
        Vehicle = 3
    }
}
