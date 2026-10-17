using BL.Constants;
using BL.Models.Common;
using BL.Models.Dashboard;
using DAL.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using DAL.Models.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public class FileuploadService
    {
        private NPFContext context;
        private QueryResult queryResult;

        public FileuploadService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }
        public async Task<BaseResponseDTO<bool>> SaveFileRework(FileUploadReworkDTO data)
        {
            BaseResponseDTO<bool> dto = new BaseResponseDTO<bool>();
            Files f = new Files();
            ParentModule pm;
            ResearchType Module;
            ResearchSubModule SubModule;

            string errorMsg = string.Empty;
            try
            {
                List<long> IDS = new List<long>();
                foreach (FilesReworkDTO dt in data.FilesList)
                {
                    if (dt.Id == 0)
                    {

                        f = new Files()
                        {
                            File = dt.File
                        };
                        context.Files.Add(f);
                        context.SaveChanges();
                        string ParentPath = data.ParentModule.ToString();
                        string ModulePath = data.Module.ToString();
                        string SubModulePath = data.SubModule.ToString();
                        string relativePath = "\\" + ParentPath + "\\" + ModulePath + "\\" + SubModulePath + "\\" + data.RecordId.ToString();
                        NPF_SYS_GlobalParam Gp = context.NPF_SYS_GlobalParam.Where(x => x.Name == "BasePath").FirstOrDefault();
                        string BasePath = Gp.Value;
                        string targetDirectory = BasePath + relativePath;

                        string fname = System.IO.Path.GetFileNameWithoutExtension(dt.FileName);
                        string fext = System.IO.Path.GetExtension(dt.FileName);
                        FileIndex Fi = new FileIndex()
                        {
                            FileId = f.Id,
                            FileName = fname,
                            FileExtension = fext,
                            Path = targetDirectory,
                            ConcatKeywords = dt.Keywords
                        };
                        context.FileIndex.Add(Fi);
                        context.SaveChanges();
                        IDS.Add(Fi.Id);
                        FileProperties fp = new FileProperties()
                        {
                            FileIndexId = Fi.Id,
                            RecordId = data.RecordId,
                            ParentModule = data.ParentModule,
                            Module = data.Module,
                            SubModule = data.SubModule,
                            TableId = dt.TableId
                        };
                        context.FileProperties.Add(fp);
                        context.SaveChanges();
                        string[] splt = dt.Keywords.Split(';');
                        foreach (string key in splt)
                        {
                            FileTags ft = new FileTags()
                            {
                                FileIndexId = Fi.Id,
                                Tag = key
                            };
                            context.FileTags.Add(ft);
                            context.SaveChanges();
                        }
                    }

                }
                dto.Data = true;
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = false;
                dto.QryResult = queryResult.FAILED;
            }

            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveFile(FileUploadDTO data)
        {
            BaseResponseDTO<bool> dto = new BaseResponseDTO<bool>();
            Files f = new Files();
            ParentModule pm;
            ResearchType Module;
            ResearchSubModule SubModule;

            string errorMsg = string.Empty;
            try
            {
                List<long> IDS = new List<long>();
                foreach (FilesDTO dt in data.FilesList)
                {
                    if (dt.Id == 0)
                    {
                        IFormFile source = dt.File;
                        using (var ms = new MemoryStream())
                        {
                            source.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            string BaseFile = Convert.ToBase64String(fileBytes);
                            f = new Files()
                            {
                                File = BaseFile
                            };
                            context.Files.Add(f);
                            context.SaveChanges();
                        }
                        string filename = System.Net.Http.Headers.ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');

                        if (filename.Contains("\\"))
                        {
                            filename = filename.Substring(filename.LastIndexOf("\\") + 1);
                        }

                        string ParentPath = data.ParentModule.ToString();
                        string ModulePath = data.Module.ToString();
                        string SubModulePath = data.SubModule.ToString();
                        string relativePath = "\\" + ParentPath + "\\" + ModulePath + "\\" + SubModulePath + "\\" + data.RecordId.ToString();
                        NPF_SYS_GlobalParam Gp = context.NPF_SYS_GlobalParam.Where(x => x.Name == "BasePath").FirstOrDefault();
                        string BasePath = Gp.Value;
                        string targetDirectory = BasePath + relativePath;
                        if (!System.IO.Directory.Exists(targetDirectory))
                        {
                            System.IO.Directory.CreateDirectory(targetDirectory);
                        }


                        using (System.IO.FileStream output = System.IO.File.Create(System.IO.Path.Combine(targetDirectory, filename)))
                        {
                            await source.CopyToAsync(output);
                        }
                        string fname = System.IO.Path.GetFileNameWithoutExtension(dt.File.FileName);
                        string fext = System.IO.Path.GetExtension(dt.File.FileName);
                        FileIndex Fi = new FileIndex()
                        {
                            FileId = f.Id,
                            FileName = fname,
                            FileExtension = fext,
                            Path = targetDirectory,
                            ConcatKeywords = dt.Keywords
                        };
                        context.FileIndex.Add(Fi);
                        context.SaveChanges();
                        IDS.Add(Fi.Id);
                        FileProperties fp = new FileProperties()
                        {
                            FileIndexId = Fi.Id,
                            RecordId = data.RecordId,
                            ParentModule = data.ParentModule,
                            Module = data.Module,
                            SubModule = data.SubModule,
                            TableId = dt.TableId
                        };
                        context.FileProperties.Add(fp);
                        context.SaveChanges();
                        string[] splt = dt.Keywords.Split(';');
                        foreach (string key in splt)
                        {
                            FileTags ft = new FileTags()
                            {
                                FileIndexId = Fi.Id,
                                Tag = key
                            };
                            context.FileTags.Add(ft);
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        IDS.Add(dt.Id);
                    }
                }
                var fil = (from a in context.FileProperties.Where(x => !IDS.Contains(x.FileIndexId))
                           where a.ParentModule == data.ParentModule && a.Module == data.Module
                           && a.SubModule == data.SubModule && a.RecordId == data.RecordId
                           select new
                           {
                               IndexId = a.FileIndexId,
                               PropertiesId = a.Id
                           }).ToList();
                if (fil.Count > 0)
                {
                    foreach (var fi in fil)
                    {
                        FileIndex filei = context.FileIndex.Where(x => x.Id == fi.IndexId).FirstOrDefault();
                        filei.DeleteStatus = true;
                        context.FileIndex.Update(filei);
                        context.SaveChanges();

                        FileProperties fp = context.FileProperties.Where(x => x.Id == fi.PropertiesId).FirstOrDefault();
                        fp.DeleteStatus = true;
                        context.FileProperties.Update(fp);
                        context.SaveChanges();
                    }
                }
                dto.Data = true;
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = false;
                dto.QryResult = queryResult.FAILED;
            }

            return dto;
        }
        public async Task<BaseResponseDTO<bool>> SaveSingleFile(SingleFilesDTO data)
        {
            BaseResponseDTO<bool> dto = new BaseResponseDTO<bool>();
            Files f = new Files();
            try
            {
                IFormFile source = data.File;
                string filename = System.Net.Http.Headers.ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');

                if (filename.Contains("\\"))
                {
                    filename = filename.Substring(filename.LastIndexOf("\\") + 1);
                }
                string targetDirectory = "Template\\";
                if (!System.IO.Directory.Exists(targetDirectory))
                {
                    System.IO.Directory.CreateDirectory(targetDirectory);
                }


                using (System.IO.FileStream output = System.IO.File.Create(System.IO.Path.Combine(targetDirectory, filename)))
                {
                    await source.CopyToAsync(output);
                }
                        
                dto.Data = true;
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = false;
                dto.QryResult = queryResult.FAILED;
            }

            return dto;
        }

        public async Task<BaseResponseDTO<List<AttachmentListDTO>>> GetAttachementList(GetAttachmentDTO dt)
        {
            BaseResponseDTO<List<AttachmentListDTO>> dto = new BaseResponseDTO<List<AttachmentListDTO>>();
            dto.Data = new List<AttachmentListDTO>();
            try
            {
                dto.Data = (from a in context.FileProperties
                            join b in context.FileIndex on a.FileIndexId equals b.Id
                            where b.DeleteStatus == false && a.ParentModule == dt.ParentModule && a.Module == dt.Module
                            && a.SubModule == dt.SubModule && a.RecordId == dt.RecordId && a.DeleteStatus == false
                            select new AttachmentListDTO
                            {
                                Id = a.FileIndexId,
                                RecordId = a.RecordId,
                                TableId = a.TableId,
                                FileName = b.FileName + b.FileExtension,
                                Keywords = b.ConcatKeywords
                            }).ToList();
            }
            catch (Exception ex)
            {
                dto.Data = new List<AttachmentListDTO>();
                dto.QryResult = queryResult.FAILED;
            }
            return dto;
        }
        public async Task<BaseResponseDTO<AttachmentDTO>> DownloadFile(long Id)
        {
            BaseResponseDTO<AttachmentDTO> dto = new BaseResponseDTO<AttachmentDTO>();
            dto.Data = new AttachmentDTO();
            try
            {
                dto.Data = (from a in context.FileIndex
                            join b in context.Files on a.FileId equals b.Id
                            where a.Id == Id && b.DeleteStatus == false && a.DeleteStatus == false
                            select new AttachmentDTO
                            {
                                File = b.File,
                                Filename = a.FileName + a.FileExtension
                            }).FirstOrDefault();

                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = new AttachmentDTO();
                dto.QryResult = queryResult.FAILED;
            }
            return dto;
        }
    }
}
