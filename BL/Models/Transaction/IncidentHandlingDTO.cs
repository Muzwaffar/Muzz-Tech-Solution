using DAL.Common;
using DAL.Models.Transaction;
using Microsoft.AspNetCore.Http;
using System.Transactions;

namespace BL.Models.Transaction
{
    public class IncidentHandlingListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string IncidentNo { get; set; }
        public string Station { get; set; }
        public string ParentIncident { get; set; }
        public string IncidentType { get; set; }
        public string IncidentSubType { get; set; }
        public string IncidentCode { get; set; }
        public string NatureOfIncident { get; set; }
        public string Location { get; set; }
        public string DateSubmitted { get; set; }
        public string DateCreated { get; set; }
        public string Status { get; set; }
        public string CrewStatus { get; set; }
        public string CallInStatus { get; set; }
    }
    public class IncidentHandlingCallerListDTO
    {
        public long Id { get; set; }
        public string IncidentNo { get; set; }
        public string IncidentType { get; set; }
        public string DateCreated { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
    }
    public class AbortCompletedDTO
    {
        public long IncidentId { get; set; }
        public string Reason { get; set; }
        public DAL.Common.TransactionStatus Status { get; set; }
    }
    public class IncidentHandlingDTO
    {
        public NPF_TRN_Incident Incident { get; set; } 
        public NPF_TRN_IncidentCaller IncidentCaller { get; set; }
        public NPF_TRN_IncidentLocation IncidentLocation { get; set; }
        public List<NPF_TRN_IncidentVehicle> IncidentVehicle { get; set; }
        public List<NPF_TRN_IncidentEquipment> IncidentEquipment { get; set; } 
        public List<NPF_TRN_IncidentDamage> IncidentDamage { get; set; }
        public NPF_TRN_IncidentFatality IncidentFatality { get; set; }
        public NPF_TRN_IncidentStation IncidentStation { get; set; }
        public NPF_TRN_IncidentCasualty IncidentCasualty { get; set; }
        public List<NPF_TRN_IncidentExternalAgency> IncidentExternalAgency { get; set; }
        public List<NPF_TRN_IncidentOtherStation> IncidentOtherStation { get; set; } 
        public List<NPF_TRN_IncidentResource> IncidentResource { get; set; } 
        public List<NPF_TRN_IncidentCasualtyDetails> IncidentCasualtyDetails { get; set; } 
        public List<NPF_TRN_IncidentFatalityDetails> IncidentFatalityDetails { get; set; } 
        public string SelectedIncidentSubType { get; set; } = string.Empty;
        public string SelectedIncidentCode { get; set; } = string.Empty;
        public string Update { get; set; } = string.Empty;
        public DAL.Common.TransactionStatus Status { get; set; } = DAL.Common.TransactionStatus.Open;
        public long Id { get; set; } = 0;
        public bool HasAckowledge = false;
    }
    public class TableDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class DropdownParameterDTO
    {
        public long? Parameter1 { get; set; }
        public long? Parameter2 { get; set; }
    }
    public class IncidentHandlingGetDetailsDTO
    {
        public List<IncidentVehicle> IncidentVehicle { get; set; } = new List<IncidentVehicle>();
        public List<IncidentEquipment> IncidentEquipment { get; set; } = new List<IncidentEquipment>();
        public List<IncidentDamage> IncidentDamage { get; set; } = new List<IncidentDamage>();
        public List<IncidentExternalAgency> IncidentExternalAgency { get; set; } = new List<IncidentExternalAgency>();
        public List<IncidentResource> IncidentResource { get; set; } = new List<IncidentResource>();
        public List<IncidentOtherStation> IncidentOtherStation { get; set; } = new List<IncidentOtherStation>();
        public List<IncidentCasualtyDetails> IncidentCasualtyDetails { get; set; } = new List<IncidentCasualtyDetails>();
        public List<IncidentFatalityDetails> IncidentFatalityDetails { get; set; } = new List<IncidentFatalityDetails>();
    }
    public class IncidentVehicle
    {
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public long VehicleId { get; set; }
        public string Name { get; set; }
        public long? StationId { get; set; } = 0;
        public string? StationName { get; set; } = string.Empty;
    }
    public class IncidentEquipment
    {
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public long EquipmentId { get; set; }
        public string Name { get; set; }
        public long? StationId { get; set; } = 0;
        public string? StationName { get; set; } = string.Empty;
    }
    public class IncidentDamage
    {
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public string Details { get; set; } = string.Empty;
        public float? EstimatedValue { get; set; }
        public float? Cost { get; set; }
        public float? Salvage { get; set; }
    }
    public class IncidentExternalAgency
    {
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public long ExternalAgencyId { get; set; }
        public string Remarks { get; set; }
        public string ContactNumber { get; set; }
        public string Name { get; set; }
    }
    public class IncidentResource
    {
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public long ResourceId { get; set; }
        public string Name { get; set; }
        public bool IsCrewCommander { get; set; }
        public int? Order { get; set; }
        public long? StationId { get; set; } = 0;
        public string? StationName { get; set; } = string.Empty;
    }
    public class IncidentOtherStation
    {
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public long OtherStationId { get; set; }
        public string Name { get; set; }
    }
    public class IncidentCasualtyDetails
    {
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Age { get; set; } = 0;
        public string Remarks { get; set; } = string.Empty;
        public long? Gender { get; set; }
        public long? CasualtyType { get; set; }
        public string GenderName { get; set; }
        public string CasualtyTypeName { get; set; }
    }
    public class IncidentFatalityDetails
    {
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Age { get; set; } = 0;
        public string Remarks { get; set; } = string.Empty;
        public long? Gender { get; set; }
        public long? FatalityType { get; set; }
        public string GenderName { get; set; }
        public string FatalityTypeName { get; set; }
    }
    public class CRRequestListDTO
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string Datetime { get; set; }
        public bool Completed { get; set; } = false;
    }
    public class UpdateCRRequestDTO
    {
        public long Id { get; set; }
        public bool Completed { get; set; }
    }
    public class IncidentAssignmentDTO
    {
        public long Id { get; set; }
        public long IncidentId { get; set; }
        public long ResourceId { get; set; }
        public bool IsMain { get; set; } = false;
    }
    public class IncidentFilesDTO
    {
        public long Id { get; set; }//Id of data in DbTable
        public long IncidentId { get; set; }
        public string FileName { get; set; }
        public IFormFile File { get; set; }
    }
    public class IncidentFilesDownloadDTO
    {
        public string File { get; set; }
        public string Filename { get; set; }
    }

    public class IncidentGetRefreshedListDTO
    {
        public List<IncidentFilesListDTO> FilesList { get; set; } = new List<IncidentFilesListDTO>();
    }

    public class IncidentFilesListDTO
    {

        public long Id { get; set; }
        public string FileName { get; set; }
    }

    public class IncidentPDASearchDTO
    {

        public long StationId { get; set; }
        public long NatureofIncidentId { get; set; }
        public long IncidentTypeId { get; set; }
    }

    public class IncidentPDADTO
    {

        public string Equipment { get; set; } = string.Empty;
        public string Vehicle { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
    }

}
