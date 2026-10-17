using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class PDAService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public PDAService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<PDADTO>> GetAll()
        {
            BaseResponseDTO<List<PDADTO>> dto = new BaseResponseDTO<List<PDADTO>>();
            List<PDADTO> result = new List<PDADTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_PDA
                          where a.DeleteStatus == false
                          select new PDADTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              StationTypeId = a.StationType.Id,
                              StationTypeName = a.StationType.Name,
                              NatureOfIncidentId = a.NatureOfIncident.Id,
                              NatureOfIncidentName = a.NatureOfIncident.Name,
                              IncidentTypeId = a.IncidentType == null ? 0 : a.IncidentType.Id,
                              IncidentTypeName = a.IncidentType == null ? string.Empty : a.IncidentType.Name,

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

        public BaseResponseDTO<PDADTO> GetById(long? Id)
        {
            BaseResponseDTO<PDADTO> dto = new BaseResponseDTO<PDADTO>();
            PDADTO result = new PDADTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_PDA
                          where a.DeleteStatus == false && a.Id == Id
                          select new PDADTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              StationTypeId = a.StationType.Id,
                              NatureOfIncidentId = a.NatureOfIncident.Id,
                              IncidentTypeId = a.IncidentType == null ? 0 : a.IncidentType.Id
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


        public BaseResponseDTO<PDADTO> getTableDetails(long Id)
        {

            BaseResponseDTO<PDADTO> dto = new BaseResponseDTO<PDADTO>();
            List<PDAVehicleMappingDTO> vehicleMappingList = new List<PDAVehicleMappingDTO>();
            List<PDAEquipmentMappingDTO> equipmentMappingList = new List<PDAEquipmentMappingDTO>();
            List<PDAResourceMappingDTO> resourceMappingList = new List<PDAResourceMappingDTO>();
            dto.Data = new PDADTO();
            string errorMsg = "No Data Found";
            try
            {
                vehicleMappingList = (from  vehicleMapping in context.NPF_SYS_PDAVehicleMapping
                                      where vehicleMapping.Fk_PDA == Id && vehicleMapping.DeleteStatus == false
                                      select new PDAVehicleMappingDTO
                                      {
                                          Id = vehicleMapping.Id,
                                          VehicleCategory = context.NPF_SYS_LookUpValue.Where(x => x.Id == vehicleMapping.VehicleCategory.Id).FirstOrDefault(),
                                          VehicleQuantity = vehicleMapping.VehicleQuantity,
                                      }).ToList();

                equipmentMappingList = (from equipmentMapping in context.NPF_SYS_PDAEquipmentMapping
                                      where equipmentMapping.Fk_PDA == Id && equipmentMapping.DeleteStatus == false
                                      select new PDAEquipmentMappingDTO
                                      {
                                          Id = equipmentMapping.Id,
                                          EquipmentCategory = context.NPF_SYS_LookUpValue.Where(x => x.Id == equipmentMapping.EquipmentCategory.Id).FirstOrDefault(),
                                          EquipmentQuantity = equipmentMapping.EquipmentQuantity,
                                      }).ToList();

                resourceMappingList = (from resourceMapping in context.NPF_SYS_PDAResourceMapping
                                        where resourceMapping.Fk_PDA == Id && resourceMapping.DeleteStatus == false
                                        select new PDAResourceMappingDTO
                                        {
                                            Id = resourceMapping.Id,
                                            Name = context.NPF_SYS_Rank.Where(x => x.Id == resourceMapping.Rank.Id).FirstOrDefault(),
                                            RankQuantity = resourceMapping.RankQuantity,
                                        }).ToList();

                dto.Data.PDAVehicleMappingDTO = vehicleMappingList;
                dto.Data.PDAEquipmentMappingDTO = equipmentMappingList;
                dto.Data.PDAResourceMappingDTO = resourceMappingList;

                dto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception e)
            {
                dto.Data = new PDADTO();
                dto.AdditionalData = errorMsg;
                dto.QryResult = new QueryResult().FAILED;
            }

            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<PDADTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {

                BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_PDA");
                NPF_SYS_StationType st = context.NPF_SYS_StationType.Where(x => x.Id == dataToSave.Data.StationTypeId).FirstOrDefault();
                context.Update(st);

                NPF_SYS_NatureOfIncident noi = context.NPF_SYS_NatureOfIncident.Where(x => x.Id == dataToSave.Data.NatureOfIncidentId).FirstOrDefault();
                context.Update(noi);

                NPF_SYS_IncidentType inct = context.NPF_SYS_IncidentType.Where(x => x.Id == dataToSave.Data.IncidentTypeId).FirstOrDefault();
                context.Update(inct);

                NPF_SYS_PDA ZN = new NPF_SYS_PDA()
                {
                    Code = BaseDtoS.Data,
                    StationType = st,
                    NatureOfIncident = noi,
                    IncidentType = inct
                };
                context.NPF_SYS_PDA.Add(ZN);
                context.SaveChanges();

                if (dataToSave.Data.PDAVehicleMappingDTO.Count > 0)
                {
                    foreach (PDAVehicleMappingDTO pdaveh in dataToSave.Data.PDAVehicleMappingDTO)
                    {
                        NPF_SYS_LookUpValue vec = context.NPF_SYS_LookUpValue.Where(x=> x.Id == pdaveh.VehicleCategory.Id).FirstOrDefault();
                        NPF_SYS_PDAVehicleMapping veh = new NPF_SYS_PDAVehicleMapping();
                        veh.VehicleCategory = vec;
                        veh.VehicleQuantity = pdaveh.VehicleQuantity;
                        veh.Fk_PDA = ZN.Id;
                        context.NPF_SYS_PDAVehicleMapping.Add(veh);
                        context.SaveChanges();
                    }

                }

                if (dataToSave.Data.PDAEquipmentMappingDTO.Count > 0)
                {
                    foreach (PDAEquipmentMappingDTO pdaEdip in dataToSave.Data.PDAEquipmentMappingDTO)
                    {
                        NPF_SYS_LookUpValue equ = context.NPF_SYS_LookUpValue.Where(x => x.Id == pdaEdip.EquipmentCategory.Id).FirstOrDefault();
                        NPF_SYS_PDAEquipmentMapping euip = new NPF_SYS_PDAEquipmentMapping();
                        euip.EquipmentCategory = equ;
                        euip.EquipmentQuantity = pdaEdip.EquipmentQuantity;
                        euip.Fk_PDA = ZN.Id;
                        context.NPF_SYS_PDAEquipmentMapping.Add(euip);
                        context.SaveChanges();
                    }

                }
                if (dataToSave.Data.PDAResourceMappingDTO.Count > 0)
                {
                    foreach (PDAResourceMappingDTO pdaRes in dataToSave.Data.PDAResourceMappingDTO)
                    {
                        NPF_SYS_Rank equ = context.NPF_SYS_Rank.Where(x => x.Id == pdaRes.Name.Id).FirstOrDefault();
                        NPF_SYS_PDAResourceMapping euip = new NPF_SYS_PDAResourceMapping();
                        euip.Rank = equ;
                        euip.RankQuantity = pdaRes.RankQuantity;
                        euip.Fk_PDA = ZN.Id;
                        context.NPF_SYS_PDAResourceMapping.Add(euip);
                        context.SaveChanges();
                    }

                }


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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<PDADTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                    NPF_SYS_StationType st = context.NPF_SYS_StationType.Where(x => x.Id == dataToUpdate.Data.StationTypeId).FirstOrDefault();
                    context.Update(st);

                    NPF_SYS_NatureOfIncident noi = context.NPF_SYS_NatureOfIncident.Where(x => x.Id == dataToUpdate.Data.NatureOfIncidentId).FirstOrDefault();
                    context.Update(noi);

                    NPF_SYS_IncidentType inct = context.NPF_SYS_IncidentType.Where(x => x.Id == dataToUpdate.Data.IncidentTypeId).FirstOrDefault();
                    context.Update(inct);

                    NPF_SYS_PDA pda = context.NPF_SYS_PDA.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                    pda.StationType = st;
                    pda.NatureOfIncident = noi;
                pda.IncidentType = inct;
                    context.NPF_SYS_PDA.Update(pda);
                    context.SaveChanges();

                    List<NPF_SYS_PDAVehicleMapping> vmList = context.NPF_SYS_PDAVehicleMapping.Where(x => x.Fk_PDA == dataToUpdate.Data.Id && x.DeleteStatus == false).ToList();

                    foreach (PDAVehicleMappingDTO vm in dataToUpdate.Data.PDAVehicleMappingDTO)
                    {
                        if (vm.Id == 0)
                        {
                        NPF_SYS_LookUpValue vec = context.NPF_SYS_LookUpValue.Where(x => x.Id == vm.VehicleCategory.Id).FirstOrDefault();
                        NPF_SYS_PDAVehicleMapping veh = new NPF_SYS_PDAVehicleMapping();
                            veh.VehicleCategory = vec;
                            veh.VehicleQuantity = vm.VehicleQuantity;
                            veh.Fk_PDA = dataToUpdate.Data.Id;
                            context.NPF_SYS_PDAVehicleMapping.Add(veh);
                            context.SaveChanges();
                        }
                        else
                        {
                            NPF_SYS_PDAVehicleMapping ve = context.NPF_SYS_PDAVehicleMapping.Where(x => x.Id == vm.Id).FirstOrDefault();
                            if(ve != null)
                            {
                                vmList.Remove(ve);
                            }

                        }
                    }
                    foreach(NPF_SYS_PDAVehicleMapping lp in vmList)
                    {
                        lp.DeleteStatus = true;
                        context.NPF_SYS_PDAVehicleMapping.Update(lp);
                        context.SaveChanges();
                    }

                    List<NPF_SYS_PDAEquipmentMapping> emList = context.NPF_SYS_PDAEquipmentMapping.Where(x => x.Fk_PDA == dataToUpdate.Data.Id && x.DeleteStatus == false).ToList();

                    foreach (PDAEquipmentMappingDTO eq in dataToUpdate.Data.PDAEquipmentMappingDTO)
                    {
                        if (eq.Id == 0)
                        {
                        NPF_SYS_LookUpValue equ = context.NPF_SYS_LookUpValue.Where(x => x.Id == eq.EquipmentCategory.Id).FirstOrDefault();
                        NPF_SYS_PDAEquipmentMapping equi = new NPF_SYS_PDAEquipmentMapping();
                            equi.Id = eq.Id;
                            equi.EquipmentCategory = equ;
                            equi.EquipmentQuantity = eq.EquipmentQuantity;
                            equi.Fk_PDA = dataToUpdate.Data.Id;
                            context.NPF_SYS_PDAEquipmentMapping.Add(equi);
                            context.SaveChanges();
                        }
                        else
                        {
                            NPF_SYS_PDAEquipmentMapping ep = context.NPF_SYS_PDAEquipmentMapping.Where(x => x.Id == eq.Id).FirstOrDefault();
                            if (ep != null)
                            {
                                emList.Remove(ep);
                            }

                        }
                    }
                    foreach (NPF_SYS_PDAEquipmentMapping le in emList)
                    {
                        le.DeleteStatus = true;
                        context.NPF_SYS_PDAEquipmentMapping.Update(le);
                        context.SaveChanges();
                    }

                    List<NPF_SYS_PDAResourceMapping> reList = context.NPF_SYS_PDAResourceMapping.Where(x => x.Fk_PDA == dataToUpdate.Data.Id && x.DeleteStatus == false).ToList();

                    foreach (PDAResourceMappingDTO res in dataToUpdate.Data.PDAResourceMappingDTO)
                    {
                        if (res.Id == 0)
                        {
                        NPF_SYS_Rank equ = context.NPF_SYS_Rank.Where(x => x.Id == res.Name.Id).FirstOrDefault();
                        NPF_SYS_PDAResourceMapping re = new NPF_SYS_PDAResourceMapping();
                            re.Id = res.Id;
                            re.Rank = equ;
                            re.RankQuantity = res.RankQuantity;
                            re.Fk_PDA = dataToUpdate.Data.Id;
                            context.NPF_SYS_PDAResourceMapping.Add(re);
                            context.SaveChanges();
                        }
                        else
                        {
                            NPF_SYS_PDAResourceMapping re = context.NPF_SYS_PDAResourceMapping.Where(x => x.Id == res.Id).FirstOrDefault();
                            if (re != null)
                            {
                                reList.Remove(re);
                            }

                        }
                    }
                    foreach (NPF_SYS_PDAResourceMapping r in reList)
                    {
                        r.DeleteStatus = true;
                        context.NPF_SYS_PDAResourceMapping.Update(r);
                        context.SaveChanges();
                    }

                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                
            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Updating Record";
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
                //Get StationType Dropdown
                List<NPF_SYS_StationType> ST = context.NPF_SYS_StationType.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "StationType";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_StationType te in ST)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = te.Id;
                    Ddi.text = te.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //Get Incicent Dropdown
                List<NPF_SYS_NatureOfIncident> IT = context.NPF_SYS_NatureOfIncident.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "NatureOfIncident";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_NatureOfIncident te in IT)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = te.Id;
                    Ddi.text = te.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //Get Incicent Type Dropdown
                List<NPF_SYS_IncidentType> IncidentType = context.NPF_SYS_IncidentType.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "IncidentType";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_IncidentType te in IncidentType)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = te.Id;
                    Ddi.text = te.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //Get Vehicle category Dropdown
                NPF_SYS_LookUpType lkVeh = context.NPF_SYS_LookUpType.Where(x => x.Name == "Vehicle Category" && x.DeleteStatus == false).FirstOrDefault();
                List<NPF_SYS_LookUpValue> vehCategory = context.NPF_SYS_LookUpValue.Where(x => x.DeleteStatus == false && x.LookUpType.Id == lkVeh.Id).ToList();
                Dd = new DropDown();
                Dd.title = "VehicleCat";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_LookUpValue s in vehCategory)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = s.Id;
                    Ddi.text = s.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //Get Equipment category Dropdown
                NPF_SYS_LookUpType lkEquip = context.NPF_SYS_LookUpType.Where(x => x.Name == "Equipment Category" && x.DeleteStatus == false).FirstOrDefault();
                List<NPF_SYS_LookUpValue> equipCategory = context.NPF_SYS_LookUpValue.Where(x => x.DeleteStatus == false && x.LookUpType.Id == lkEquip.Id).ToList();
                Dd = new DropDown();
                Dd.title = "EquipmentCat";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_LookUpValue s in equipCategory)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = s.Id;
                    Ddi.text = s.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);
                //Get Resource Dropdown
                List<NPF_SYS_Rank> Rel = context.NPF_SYS_Rank.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Rank";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_Rank re in Rel)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = re.Id;
                    Ddi.text = re.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);
                dto.Data = Ddl;
                dto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = Ddl;
                dto.AdditionalData = errorMsg;
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }

        public BaseResponseDTO<bool> Delete(long dataToDelId)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_SYS_PDA upd = context.NPF_SYS_PDA.Where(x => x.Id == dataToDelId).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_PDA.Update(upd);
                context.SaveChanges();

                List<NPF_SYS_PDAVehicleMapping> veh = context.NPF_SYS_PDAVehicleMapping.Where(x => x.Fk_PDA ==  dataToDelId).ToList();
                foreach(NPF_SYS_PDAVehicleMapping ve in veh)
                {
                    ve.DeleteStatus = true;
                    context.NPF_SYS_PDAVehicleMapping.Update(ve);
                    context.SaveChanges();
                }

                List<NPF_SYS_PDAEquipmentMapping> equip = context.NPF_SYS_PDAEquipmentMapping.Where(x => x.Fk_PDA == dataToDelId).ToList();
                foreach (NPF_SYS_PDAEquipmentMapping equi in equip)
                {
                    equi.DeleteStatus = true;
                    context.NPF_SYS_PDAEquipmentMapping.Update(equi);
                    context.SaveChanges();
                }

                List<NPF_SYS_PDAResourceMapping> resource = context.NPF_SYS_PDAResourceMapping.Where(x => x.Fk_PDA == dataToDelId).ToList();
                foreach (NPF_SYS_PDAResourceMapping res in resource)
                {
                    res.DeleteStatus = true;
                    context.NPF_SYS_PDAResourceMapping.Update(res);
                    context.SaveChanges();
                }


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
    }
}
