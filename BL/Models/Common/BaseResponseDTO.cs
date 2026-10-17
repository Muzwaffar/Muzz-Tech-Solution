using DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Common
{
    public  class BaseResponseDTO<T>
    {

        public T? Data { get; set; }
        public string AdditionalData { get; set; }
        public string ExtData { get; set; }
        public int TotalItems { get; set; }
        public Guid oneTimeToken { get; set; }
        public Guid RoleId { get; set; }
        public string? QryResult { get; set; }
        public string? ErrorMessage { get; set; }
        public AType AType { get; set; } = AType.None;
        public GeneralStatus GeneralStatus { get; set; } = GeneralStatus.Open;
        public long Id { get; set; } =0;
        public List<MAccessDTO> Access { get; set; } = new List<MAccessDTO>();


    }
    public class BaseResponseAccessDTO<T>
    {

        public T? Data { get; set; }
        public string AdditionalData { get; set; }
        public int TotalItems { get; set; }
        public Guid oneTimeToken { get; set; }
        public Guid RoleId { get; set; }
        public string? QryResult { get; set; }
        public string? ErrorMessage { get; set; }
        public List<MAccessDTO> Access { get; set; } = new List<MAccessDTO>();


    }
    public class MAccessDTO
    {
        public AccessOperationType AccessType { get; set; }
        public bool HasAccess { get; set; }

    }
    public class BaseAdditionalDataDTO<T, R> : BaseResponseDTO<T>
    {
        public R? AdditionalData { get; set; }

    }
}
