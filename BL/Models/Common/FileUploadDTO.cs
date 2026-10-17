using DAL.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Common
{
    public class FileUploadDTO
    {

        public long RecordId { get; set; }//Id of data in DbTable
        public ParentModule ParentModule { get; set; }// Research/Extension.....
        public ResearchType Module { get; set; }//Crop/Livestock.....
        public ResearchSubModule SubModule { get; set; }//POW/LandRequirement
        public List<FilesDTO> FilesList { get; set; } = new List<FilesDTO>();


    }
    public class FileUploadReworkDTO
    {

        public long RecordId { get; set; }//Id of data in DbTable
        public ParentModule ParentModule { get; set; }// Research/Extension.....
        public ResearchType Module { get; set; }//Crop/Livestock.....
        public ResearchSubModule SubModule { get; set; }//POW/LandRequirement
        public List<FilesReworkDTO> FilesList { get; set; } = new List<FilesReworkDTO>();


    }
    public class FilesReworkDTO
    {
        public long Id { get; set; }//Id of data in DbTable
        public int TableId { get; set; }
        public string Keywords { get; set; } // Delimeter ;
        public string File { get; set; }
        public string FileName { get; set; }
    }
    public class FilesDTO
    {
        public long Id { get; set; }//Id of data in DbTable
        public int TableId { get; set; }
        public string Keywords { get; set; } // Delimeter ;
        public IFormFile File { get; set; }
    }

    public class SingleFilesDTO
    {
        public IFormFile File { get; set; }
    }

    public class GetAttachmentDTO
    {
        public long RecordId { get; set; }//Id of data in DbTable
        public ParentModule ParentModule { get; set; }// Research/Extension.....
        public ResearchType Module { get; set; }//Crop/Livestock.....
        public ResearchSubModule SubModule { get; set; }//POW/LandRequirement
    }
    public class AttachmentListDTO
    {

        public long Id { get; set; }//FileIndexId
        public long RecordId { get; set; }//Id of data in DbTable
        public int TableId { get; set; }
        public string FileName { get; set; }
        public string Keywords { get; set; }
    }
    public class AttachmentDTO
    {
        public string File { get; set; }
        public string Filename { get; set; }
    }
}
