export class ApiTypes {
    static POST = "POST";
    static GET = "GET";
    static PUT = "PUT";
    static DELETE = "DELETE";
}
export class LocalStorageValues {
    static USERTOKEN = "userToken";
    static ONETIMETOKEN = "oneTimeToken";
    static USERNAME = "userName";
    static ROLEID = "phoneNumber";
    static EMAIL = "email";
    static LOGOUT = "logout";
    static MODULEID = "ModuleId";
    static FULLUSERNAME = "FullUserName";
}

export class StatusValues {
    static Draft = 1;
    static Schedule = 2;
    static Completed = 3;
    static Accepted = 4;
    static Rejected = 5;
    static Started = 6;
    static Stopped = 7;
    static Cancelled = 8;
    static Unschedule = 9;
    static Inprogress = 10;
    static Submitted = 11;
    static SubmittedLvl2 = 12;
    static DrafLvl2 = 13;
    static DrafLvl3 = 14;
    static DrafLvl4 = 15;
    static SubmittedLvl3 = 16;
}

export class ScreenMode {
    static VIEW = 1;
    static EDIT = 2;
}

export class ResearchTypeEnum {
    static Crop = 1;
    static LiveStock = 2;
    static Both = 3;
}

export class ResearchSubModuleEnum {
    static POW = 1;
    static LR = 2;
    static TM = 3;
    static RP = 4;
    static RPL = 5;
    static RU = 6;
    static RPF = 7;
    static Form = 8;
}

export class SaveStatus {
    static Draft = 1;
    static Save = 2;
}

export class ParentModule {
    static Research = 1;
    static Extension = 2;
}

export class AccessOperationType {
    static Create = 1;
    static Read = 2;
    static Update = 3;
    static Delete = 4;
    static Approve = 5;
    static Upload = 6;
    static Export = 7;
}

export class ApprovalStatus {

    static Approved = 1;
    static Declined = 2;
    static AwaitingApproval = 3;
    static Draft = 4;
    static Cancelled = 5;
    static Saved = 6;
}
export class AttachmentType {

    static Pictures = 1;
    static Documents = 2;
    static All = 3;
}

export class StationLivestockEvent {
    static Bought = 1;
    static Kidding = 2;
    static Weaning = 3;
    static Death = 4;
}

export class CaseStatus {
    static Draft = 1;
    static Schedule = 2;
    static Completed = 3;
    static Accepted = 4;
    static Rejected = 5;
    static Started = 6;
    static Stopped = 7;
    static Cancelled = 8;
    static Unschedule = 9;
    static Inprogress = 10;
    static Submitted = 11;
    static Approved = 12;
    static Declined = 13;
    static AwaitingApproval = 14;
}
export class ActivityElementType {
    static Text = 1;
    static Number = 2;
    static Dropdown = 3;
}  

export class ActivityElementSource {
    static SchemeName = 1;
    static SchemeCode = 2;
    static Section = 3;
    static AnimalStock = 4;
    static CropStock = 5;
    static FieldProduceBook = 6;
    static StationLandRequirement = 7;
}

export class ActivityModule {
    static General = 1;
    static Station = 2;

}

export class TransactionType {
    static Add = 1;
    static Delete = 2;
}

export class TransactionStatus {
    static Draft = 1;
    static Open = 2;
    static Submitted = 3;
    static Completed = 4;
    static Abort = 5;

}

export class AssignmentType {
    static Resource = 1;
    static Location = 2;
    static Vehicle = 3;
}