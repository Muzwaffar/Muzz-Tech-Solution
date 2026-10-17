using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Common;
using DAL.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class VehicleService
    {
        private NPFContext context;
        private QueryResult queryResult;

        public VehicleService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<VehicleListDTO>> GetAll()
        {
            BaseResponseDTO<List<VehicleListDTO>> dto = new BaseResponseDTO<List<VehicleListDTO>>();
            List<VehicleListDTO> result = new List<VehicleListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Vehicle
                          where a.DeleteStatus == false
                          select new VehicleListDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              RegistrationNo = a.RegistrationNo,
                              Description = a.Description,
                              Category = context.NPF_SYS_LookUpValue.Where(x => x.Id == a.Category).FirstOrDefault().Name,
                              Abbreviation = context.NPF_SYS_LookUpValue.Where(x => x.Id == a.Abbreviation).FirstOrDefault().Name,
                              ParentVehicle = context.NPF_SYS_Vehicle.Where(x => x.Id == a.ParentVehicle).FirstOrDefault().RegistrationNo,
                              Make = context.NPF_SYS_AssetMake.Where(x => x.Id == a.Make).FirstOrDefault().Name,
                              Model = context.NPF_SYS_AssetModel.Where(x => x.Id == a.Model).FirstOrDefault().Name,
                              Manufacturer = a.Manufacturer,
                              ManufacturingDate = a.ManufacturingDate,
                              SerialNo = a.SerialNo,
                              ExternalReferenceCode = a.ExternalReferenceCode,
                              StationName = context.NPF_SYS_Station.Where(x => x.Id == a.Station).FirstOrDefault().Name,

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

        public BaseResponseDTO<List<DropDown>> GetAllDropDownValues()
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                BaseRequestDTO<string> dt = new BaseRequestDTO<string>();
                string[] values = new string[] { "Vehicle Category", "Vehicle Abbreviation" };
                List<NPF_SYS_LookUpType> Ltl = context.NPF_SYS_LookUpType.Where(x => values.Contains(x.Name) && x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_LookUpType lt in Ltl)
                {
                    Dd = new DropDown();
                    Dd.title = lt.Name;
                    List<NPF_SYS_LookUpValue> Lvl = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Id == lt.Id && x.DeleteStatus == false).ToList();
                    Dd.items = new List<DropDownItem>();
                    foreach (NPF_SYS_LookUpValue lv in Lvl)
                    {
                        DropDownItem Ddi = new DropDownItem();
                        Ddi.Id = lv.Id;
                        Ddi.text = lv.Name;
                        Dd.items.Add(Ddi);
                    }

                    Ddl.Add(Dd);
                }

                List<NPF_SYS_Vehicle> DPT = context.NPF_SYS_Vehicle.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Parent Vehicle";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_Vehicle D in DPT)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = D.Id;
                    Ddi.text = D.RegistrationNo;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);


                List<NPF_SYS_AssetMake> AssetMake = context.NPF_SYS_AssetMake.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Make";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_AssetMake D in AssetMake)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = D.Id;
                    Ddi.text = D.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                List<NPF_SYS_AssetModel> AssetModel = context.NPF_SYS_AssetModel.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Model";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_AssetModel D in AssetModel)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = D.Id;
                    Ddi.text = D.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                List<NPF_SYS_Station> Station = context.NPF_SYS_Station.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Station";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_Station D in Station)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = D.Id;
                    Ddi.text = D.Name;
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

        public BaseResponseDTO<VehicleCRUDDTO> GetById(long Id)
        {
            BaseResponseDTO<VehicleCRUDDTO> dto = new BaseResponseDTO<VehicleCRUDDTO>();
            VehicleCRUDDTO result = new VehicleCRUDDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Vehicle
                          where a.DeleteStatus == false && a.Id == Id
                          select new VehicleCRUDDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              RegistrationNo = a.RegistrationNo,
                              Description = a.Description,
                              Station = (long)a.Station,
                              Category = a.Category,
                              Abbreviation = a.Abbreviation,
                              ParentVehicle = a.ParentVehicle,
                              Make = a.Make,
                              Model = a.Model,
                              Manufacturer = a.Manufacturer,
                              ManufacturingDate = a.ManufacturingDate,
                              SerialNo = a.SerialNo,
                              ExternalReferenceCode = a.ExternalReferenceCode,
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<VehicleCRUDDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.NPF_SYS_Vehicle.Any(x => x.RegistrationNo.ToLower() == dataToSave.Data.RegistrationNo.ToLower() && x.DeleteStatus != true))
                {
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_Vehicle");
                    NPF_SYS_Vehicle save = new NPF_SYS_Vehicle()
                    {
                        Code = BaseDtoS.Data,
                        RegistrationNo = dataToSave.Data.RegistrationNo,
                        Description = dataToSave.Data.Description,
                        Station = dataToSave.Data.Station,
                        Category = dataToSave.Data.Category,
                        Abbreviation = dataToSave.Data.Abbreviation,
                        ParentVehicle = dataToSave.Data.ParentVehicle,
                        Make = dataToSave.Data.Make,
                        Model = dataToSave.Data.Model,
                        Manufacturer = dataToSave.Data.Manufacturer,
                        ManufacturingDate = dataToSave.Data.ManufacturingDate,
                        SerialNo = dataToSave.Data.SerialNo,
                        ExternalReferenceCode = dataToSave.Data.ExternalReferenceCode,
                    };
                    context.NPF_SYS_Vehicle.Add(save);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Vehicle Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<VehicleCRUDDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (context.NPF_SYS_Vehicle.Any(x => x.RegistrationNo.ToLower() == dataToUpdate.Data.RegistrationNo.ToLower()
               && x.DeleteStatus != true && x.Id != dataToUpdate.Data.Id))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Vehicle Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }

                NPF_SYS_Vehicle Update = context.NPF_SYS_Vehicle.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();

                Update.RegistrationNo = dataToUpdate.Data.RegistrationNo;
                Update.Description = dataToUpdate.Data.Description;
                Update.Station = dataToUpdate.Data.Station;
                Update.Category = dataToUpdate.Data.Category;
                Update.Abbreviation = dataToUpdate.Data.Abbreviation;
                Update.ParentVehicle = dataToUpdate.Data.ParentVehicle;
                Update.Make = dataToUpdate.Data.Make;
                Update.Model = dataToUpdate.Data.Model;
                Update.Manufacturer = dataToUpdate.Data.Manufacturer;
                Update.ManufacturingDate = dataToUpdate.Data.ManufacturingDate;
                Update.SerialNo = dataToUpdate.Data.SerialNo;
                Update.ExternalReferenceCode = dataToUpdate.Data.ExternalReferenceCode;

                context.NPF_SYS_Vehicle.Update(Update);
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
                NPF_SYS_Vehicle upd = context.NPF_SYS_Vehicle.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_Vehicle.Update(upd);
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

        public BaseResponseDTO<List<DropDown>> GetAllDropdownValuesModel(long Id)
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                //UOM
                Dd = new DropDown();
                Dd.title = "Model";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_AssetModel> uom = context.NPF_SYS_AssetModel.Where(x => x.AssetMake.Id == Id && x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_AssetModel lt in uom)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.Name;
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
