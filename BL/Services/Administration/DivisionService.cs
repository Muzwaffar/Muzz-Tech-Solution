using BL.Constants;
using BL.Models.Access;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Common;
using DAL.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class DivisionService
    {
        private NPFContext context;
        private QueryResult queryResult;
        private MatrixService Mservice;
        public UserMatrixDTO UM = new UserMatrixDTO();

        public DivisionService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            Mservice = new MatrixService();
            UM = Mservice.sGetMatrixUserByUserID();
        }

        public BaseResponseDTO<List<DivisionListDTO>> GetAll()
        {
            BaseResponseDTO<List<DivisionListDTO>> dto = new BaseResponseDTO<List<DivisionListDTO>>();
            List<DivisionListDTO> result = new List<DivisionListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Division
                          where a.DeleteStatus == false
                          select new DivisionListDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
                              Department = a.Department.Name,

                          }).ToList();
                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = result;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }

            return dto;

        }

        public BaseResponseDTO<DivisionCRUDDTO> GetById(long Id)
        {
            BaseResponseDTO<DivisionCRUDDTO> dto = new BaseResponseDTO<DivisionCRUDDTO>();
            DivisionCRUDDTO result = new DivisionCRUDDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Division
                          where a.Id == Id
                          select new DivisionCRUDDTO()
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Description = a.Description,
                              Code = a.Code,
                              DeptId = a.Department.Id,

                          }).FirstOrDefault();
                if (result == null)
                {
                    dto.Data = result;
                    dto.QryResult = queryResult.FAILED;
                }
                else
                {
                    dto.Data = result;
                    dto.QryResult = queryResult.SUCEEDED;
                }
            }
            catch (Exception e)
            {
                dto.Data = result;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }


            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<DivisionCRUDDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                var xx = context.NPF_SYS_Division.Where(x => x.Name.ToLower() == dataToSave.Data.Name.ToLower() && x.Department.Id == dataToSave.Data.DeptId && x.DeleteStatus != true).ToList();
                if (!context.NPF_SYS_Division.Any(x => x.Name.ToLower() == dataToSave.Data.Name.ToLower() && x.Department.Id == dataToSave.Data.DeptId && x.DeleteStatus != true))
                {
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_Division");
                    NPF_SYS_Division dt = new NPF_SYS_Division()
                    {
                        Code = BaseDtoS.Data,
                        Name = dataToSave.Data.Name,
                        Description = dataToSave.Data.Description,
                        Department = context.NPF_SYS_Department.Where(x => x.Id == dataToSave.Data.DeptId).FirstOrDefault()
                    };
                    context.NPF_SYS_Division.Add(dt);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Division Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                }
            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Adding Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<DivisionCRUDDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (context.NPF_SYS_Division.Any(x => x.Name.ToLower() == dataToUpdate.Data.Name.ToLower()
                && x.DeleteStatus != true && x.Id != dataToUpdate.Data.Id && x.Department.Id != dataToUpdate.Data.DeptId))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Division Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                NPF_SYS_Division DIV = context.NPF_SYS_Division.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                DIV.Name = dataToUpdate.Data.Name;
                DIV.Description = dataToUpdate.Data.Description;
                DIV.Department = context.NPF_SYS_Department.Where(x => x.Id == dataToUpdate.Data.DeptId).FirstOrDefault();
                context.NPF_SYS_Division.Update(DIV);
                context.SaveChanges();
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Updating Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }

        public BaseResponseDTO<bool> Delete(BaseRequestDTO<long> dataToDelId)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_SYS_Division upd = context.NPF_SYS_Division.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_Division.Update(upd);
                context.SaveChanges();
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Deleting Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }

        public BaseResponseDTO<List<DropDown>> GetAllDropDownValues()
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                BaseRequestDTO<string> dt = new BaseRequestDTO<string>();

                List<NPF_SYS_Department> RT = context.NPF_SYS_Department.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Department";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_Department R in RT)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = R.Id;
                    Ddi.text = R.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                dto.Data = Ddl;
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = Ddl;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }
            return dto;
        }
    }
}

