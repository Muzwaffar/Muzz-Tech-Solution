using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using DAL.Models.Administration;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Common;

namespace DAL.Models.Transaction
{
    public class NPF_TRN_Incident : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string IncidentNo { get; set; } = string.Empty;
        public string InitialIncidentType { get; set; } = string.Empty;
        public long? ParentIncidentId { get; set; } 
        public long? IncidentType { get; set; }
        public long? IncidentSubType { get; set; }
        public long? IncidentCode { get; set; }
        public string IncidentRemarks { get; set; } = string.Empty;
        public TransactionStatus Status { get; set; } = TransactionStatus.Draft;
        public StationStatus StationStatus { get; set; } = StationStatus.Open;
        public long? Priority { get; set; }
        public IncidentStatus IncidentStatus { get; set; } = IncidentStatus.Pending;
        public DateTime? DateTimeSubmitted {  get; set; }
        public DateTime? DateTimeAcknowledge {  get; set; }
        public DateTime? DateTimeIncidentCompleted {  get; set; }
        public DateTime? DateTimeIncidentAbort {  get; set; }
        public DateTime? DateTimeAssigned {  get; set; }
        public DateTime? DateTimeAccepted {  get; set; }
        public DateTime? DateTimeTurnOut {  get; set; }
        public DateTime? DateTimeOnsite {  get; set; }
        public DateTime? DateTimeUnderControl {  get; set; }
        public DateTime? DateTimeCompleted {  get; set; }
        public DateTime? DateTimeLeftSite {  get; set; }
        public DateTime? DateTimeReachedStation {  get; set; }
        public DateTime? Timer {  get; set; }
        public string IncidentReportRemarks { get; set; } = string.Empty;
    }
}
