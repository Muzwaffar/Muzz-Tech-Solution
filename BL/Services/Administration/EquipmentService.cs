using BL.Constants;
using BL.Models.Access;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Common;
using DAL.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using DAL.Models.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class EquipmentService
    {
        private NPFContext context;
        private QueryResult queryResult;

        public EquipmentService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }
        public async Task<BaseResponseDTO<List<EquipmentListDTO>>> GetAllEquipmentItems()
        {
            BaseResponseDTO<List<EquipmentListDTO>> dto = new BaseResponseDTO<List<EquipmentListDTO>>();
            List<EquipmentListDTO> result = new List<EquipmentListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                List<EquipmentListDTO> equipmentData = (from a in context.NPF_SYS_Equipment
                          where a.DeleteStatus == false
                          select new EquipmentListDTO
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Code = a.Code,
                              Description = a.Description,
                              Make = context.NPF_SYS_EquipmentMake.Where(x => x.Id == a.Make).FirstOrDefault().Name,
                              Model = context.NPF_SYS_EquipmentModel.Where(x => x.Id == a.Model).FirstOrDefault().Name,
                              Category = context.NPF_SYS_LookUpValue.Where(x => x.Id == a.Category).FirstOrDefault().Name,
                              Abbreviation = context.NPF_SYS_LookUpValue.Where(x => x.Id == a.Abbreviation).FirstOrDefault().Name,
                              AssignmentType = a.AssignmentType.ToString(),
                              AssignmentTypeId = a.AssignmentType,
                              AssignmentName = string.Empty,
                              AssignmentNameId = a.AssignmentId
                          }).ToList();

                // Map the fetched data to the EquipmentListDTO and set the AssignmentName
                result = equipmentData.Select(a => new EquipmentListDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Code = a.Code,
                    Description = a.Description,
                    Make = a.Make,
                    Model = a.Model,
                    Category = a.Category,
                    Abbreviation = a.Abbreviation,
                    AssignmentType = a.AssignmentType.ToString(),
                    AssignmentName = getAssignmentName(a.AssignmentTypeId,a.AssignmentNameId)
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

        public string getAssignmentName(AssignmentType? AssignmentType, long? AssignmentId)
        {
            string result = string.Empty;
            try
            {
                if(AssignmentType != null && AssignmentId != null)
                {
                    if (AssignmentType == DAL.Common.AssignmentType.Resource)
                    {
                        NPF_SYS_Resource res = context.NPF_SYS_Resource.Where(x => x.Id == AssignmentId).FirstOrDefault();
                        if (res != null)
                        {
                            result = res.ResourceType.Name + " - " + res.NAA.FullName;
                        }
                    } else if (AssignmentType == DAL.Common.AssignmentType.Vehicle)
                    {
                        NPF_SYS_Vehicle veh = context.NPF_SYS_Vehicle.Where(x => x.Id == AssignmentId).FirstOrDefault();
                        if (veh != null)
                        {
                            result = veh.RegistrationNo;
                        }
                    }
                    else if (AssignmentType == DAL.Common.AssignmentType.Location)
                    {
                        NPF_SYS_Station sta = context.NPF_SYS_Station.Where(x => x.Id == AssignmentId).FirstOrDefault();
                        if (sta != null)
                        {
                            result = sta.Name;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public BaseResponseDTO<List<DropDown>> GetAllDropDownValues()
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                //Look Up Type
                string[] values = new string[] { "Equipment Category", "Equipment Abbreviation" };
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

                //Make
                Dd = new DropDown();
                Dd.title = "Make";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_EquipmentMake> make = context.NPF_SYS_EquipmentMake.Where(x => x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_EquipmentMake lt in make)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //Model
                Dd = new DropDown();
                Dd.title = "Model";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_EquipmentModel> Model = context.NPF_SYS_EquipmentModel.Where(x => x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_EquipmentModel lt in Model)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //Parent Equipment
                Dd = new DropDown();
                Dd.title = "Parent Equipment";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_Equipment> parent = context.NPF_SYS_Equipment.Where(x => x.DeleteStatus == false && x.ParentEquipment == null).ToList();
                foreach (NPF_SYS_Equipment lt in parent)
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
                List<NPF_SYS_EquipmentModel> uom = context.NPF_SYS_EquipmentModel.Where(x => x.EquipmentMake.Id == Id && x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_EquipmentModel lt in uom)
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

        public async Task<BaseResponseDTO<List<DropDown>>> GetAllDropdownValuesDependent(AssignmentType AssignmentType)
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                switch (AssignmentType)
                {
                    case AssignmentType.Resource:
                        Dd = new DropDown();
                        Dd.title = "Dropdown";
                        Dd.items = new List<DropDownItem>();
                        List<NPF_SYS_Resource> Resource = context.NPF_SYS_Resource.Where(x => x.DeleteStatus == false).ToList();
                        foreach (NPF_SYS_Resource st in Resource)
                        {
                            DropDownItem Ddi = new DropDownItem();
                            Ddi.Id = st.Id;
                            Ddi.text = st.ResourceType.Name + " - " + st.NAA.FullName;
                            Dd.items.Add(Ddi);
                        }
                        Ddl.Add(Dd);
                        break;
                    case AssignmentType.Location:
                        Dd = new DropDown();
                        Dd.title = "Dropdown";
                        Dd.items = new List<DropDownItem>();
                        List<NPF_SYS_Station> Location = context.NPF_SYS_Station.Where(x => x.DeleteStatus == false).ToList();
                        foreach (NPF_SYS_Station st in Location)
                        {
                            DropDownItem Ddi = new DropDownItem();
                            Ddi.Id = st.Id;
                            Ddi.text = st.Name;
                            Dd.items.Add(Ddi);
                        }
                        Ddl.Add(Dd);
                        break;
                    case AssignmentType.Vehicle:
                        Dd = new DropDown();
                        Dd.title = "Dropdown";
                        Dd.items = new List<DropDownItem>();
                        List<NPF_SYS_Vehicle> Vehicle = context.NPF_SYS_Vehicle.Where(x => x.DeleteStatus == false).ToList();
                        foreach (NPF_SYS_Vehicle st in Vehicle)
                        {
                            DropDownItem Ddi = new DropDownItem();
                            Ddi.Id = st.Id;
                            Ddi.text = st.RegistrationNo;
                            Dd.items.Add(Ddi);
                        }
                        Ddl.Add(Dd);
                        break;
                    default:
                        break;
                }



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

        public BaseResponseDTO<EquipmentDTO> GetById(long? Id)
        {
            BaseResponseDTO<EquipmentDTO> dto = new BaseResponseDTO<EquipmentDTO>();
            EquipmentDTO result = new EquipmentDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Equipment
                          where a.DeleteStatus == false && a.Id == Id
                          select new EquipmentDTO
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Code = a.Code,
                              Description = a.Description,
                              AssignmentType = a.AssignmentType,
                              AssignmentId = a.AssignmentId,
                              Category = a.Category,
                              Abbreviation = a.Abbreviation,
                              Make = a.Make,
                              Model = a.Model,
                              ParentEquipment = a.ParentEquipment,
                              Manufacturer = a.Manufacturer,
                              ManufacturingDate = a.ManufacturingDate,
                              SerialNo = a.SerialNo,
                              ExternalReferenceCode = a.ExternalReferenceCode,
                          }).FirstOrDefault();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception e)
            {
                dto.Data = result;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }


            return dto;
        }
        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<EquipmentDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
                BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_Equipment");
                NPF_SYS_Equipment equipment = new NPF_SYS_Equipment()
                {
                    Code = BaseDtoS.Data,
                    Name = dataToSave.Data.Name,
                    Description = dataToSave.Data.Description,
                    AssignmentType = dataToSave.Data.AssignmentType,
                    AssignmentId = dataToSave.Data.AssignmentId,
                    Category = dataToSave.Data.Category,
                    Abbreviation = dataToSave.Data.Abbreviation,
                    Make = dataToSave.Data.Make,
                    Model = dataToSave.Data.Model,
                    Manufacturer = dataToSave.Data.Manufacturer,
                    ManufacturingDate = dataToSave.Data.ManufacturingDate,
                    SerialNo = dataToSave.Data.SerialNo,
                    ExternalReferenceCode = dataToSave.Data.ExternalReferenceCode,
                    ParentEquipment = dataToSave.Data.ParentEquipment == 0 ? null : dataToSave.Data.ParentEquipment,
                };

                context.NPF_SYS_Equipment.Add(equipment);
                context.SaveChanges();

                BaseDto.Data = true;
                BaseDto.Id = equipment.Id;
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Adding Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<EquipmentDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_SYS_Equipment equipment = context.NPF_SYS_Equipment.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                equipment.Name = dataToUpdate.Data.Name;
                equipment.Description = dataToUpdate.Data.Description;
                equipment.AssignmentType = dataToUpdate.Data.AssignmentType;
                equipment.AssignmentId = dataToUpdate.Data.AssignmentId;
                equipment.Category = dataToUpdate.Data.Category;
                equipment.Abbreviation = dataToUpdate.Data.Abbreviation;
                equipment.Make = dataToUpdate.Data.Make;
                equipment.Model = dataToUpdate.Data.Model;
                equipment.Manufacturer = dataToUpdate.Data.Manufacturer;
                equipment.ManufacturingDate = dataToUpdate.Data.ManufacturingDate;
                equipment.SerialNo = dataToUpdate.Data.SerialNo;
                equipment.ExternalReferenceCode = dataToUpdate.Data.ExternalReferenceCode;
                equipment.ParentEquipment = dataToUpdate.Data.ParentEquipment == 0 ? null : dataToUpdate.Data.ParentEquipment;

                context.NPF_SYS_Equipment.Update(equipment);
                context.SaveChanges();

                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Adding Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<bool> Delete(long dataToDelId)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_SYS_Equipment Del = context.NPF_SYS_Equipment.Where(x => x.Id == dataToDelId).FirstOrDefault();
                if (Del != null)
                {
                    Del.DeleteStatus = true;
                    context.NPF_SYS_Equipment.Update(Del);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {

                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Cannot Delete Record. Record Currently In Use";
                    BaseDto.QryResult = queryResult.FAILED;
                }


            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Deleting Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }

        #region Equipment Assignment
        public BaseResponseDTO<List<EquipmentListAssignmentDTO>> GetEquipmentByAssignment(EquipemntAssignmentDTO data)
        {
            BaseResponseDTO<List<EquipmentListAssignmentDTO>> BaseDto = new BaseResponseDTO<List<EquipmentListAssignmentDTO>>();
            List<EquipmentListAssignmentDTO> equipmentList = new List<EquipmentListAssignmentDTO>();
            try
            {
                List<NPF_SYS_Equipment> equipments = context.NPF_SYS_Equipment.Where(x => x.AssignmentType == data.AssignmentType && x.AssignmentId == data.Id && x.DeleteStatus == false).ToList();

                foreach (NPF_SYS_Equipment item in equipments)
                {
                    EquipmentListAssignmentDTO equ = new EquipmentListAssignmentDTO();
                    equ.Id = item.Id;
                    equ.Name = item.Name;
                    equ.StationId = 0;
                    equ.StationName = string.Empty;
                    if (item.AssignmentType == AssignmentType.Resource) 
                    {
                        NPF_SYS_Resource res = context.NPF_SYS_Resource.Where(x => x.Id == item.AssignmentId).FirstOrDefault();
                        if (res != null)
                        {
                            equ.StationId = res.StationUnitName.Id;
                            equ.StationName = res.StationUnitName.Name;
                        }
                    }
                    else if (item.AssignmentType == AssignmentType.Location)
                    {
                        NPF_SYS_Station sta = context.NPF_SYS_Station.Where(x => x.Id == item.AssignmentId).FirstOrDefault();
                        if (sta != null)
                        {
                            equ.StationId = sta.Id;
                            equ.StationName = sta.Name;
                        }
                    }
                    else if(item.AssignmentType == AssignmentType.Vehicle)
                    {
                        NPF_SYS_Vehicle veh = context.NPF_SYS_Vehicle.Where(x => x.Id == item.AssignmentId).FirstOrDefault();
                        if (veh != null)
                        {
                            if(veh.Station != 0 || veh.Station != null)
                            {
                                NPF_SYS_Station station = context.NPF_SYS_Station.Where(x => x.Id == veh.Station).FirstOrDefault();
                                equ.StationId = station.Id;
                                equ.StationName = station.Name;
                            }
                            
                        }
                    }
                    equipmentList.Add(equ);
                }

                BaseDto.Data = equipmentList;
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception)
            {
                BaseDto.Data = equipmentList;
                BaseDto.AdditionalData = "Error fetching Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        #endregion
    }

}
