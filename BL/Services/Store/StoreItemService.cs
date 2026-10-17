using BL.Constants;
using BL.Models.Access;
using BL.Models.Common;
using BL.Models.Store;
using BL.Services.Common;
using DAL.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using DAL.Models.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BL.Services.Store
{
    public class StoreItemService
    {
        private NPFContext context;
        private QueryResult queryResult;
        private MatrixService Mservice;
        public UserMatrixDTO UM = new UserMatrixDTO();

        public StoreItemService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            Mservice = new MatrixService();
            UM = Mservice.sGetMatrixUserByUserID();
        }
        public BaseResponseDTO<List<StoreItemListDTO>> GetAllStoreItems()
        {
            BaseResponseDTO<List<StoreItemListDTO>> dto = new BaseResponseDTO<List<StoreItemListDTO>>();
            List<StoreItemListDTO> result = new List<StoreItemListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_STR_StoreItem
                          where a.DeleteStatus == false
                          select new StoreItemListDTO
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Code = a.Code,
                              Description = a.Description,
                              IsConsumable = a.IsConsumable == true ? "Yes" : "No",
                              UomDetails  = context.NPF_SYS_UomDetails.Where(x => x.Id == a.UomDetails).FirstOrDefault().Name
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
                //UOM
                Dd = new DropDown();
                Dd.title = "UOM";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_UomHeader> uom = context.NPF_SYS_UomHeader.Where(x => x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_UomHeader lt in uom)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //Animal Product Type
                Dd = new DropDown();
                Dd.title = "Type";
                Dd.items = new List<DropDownItem>();
                Dd.items = Enum.GetValues(typeof(AnimalProducts)).Cast<AnimalProducts>()
                            .Select(t => new DropDownItem
                            {
                                Id = ((long)t),
                                text = t.ToString()
                            }).ToList();

                Ddl.Add(Dd);

                //Parent Item
                Dd = new DropDown();
                Dd.title = "Parent";
                Dd.items = new List<DropDownItem>();
                List<NPF_STR_StoreItem> parent = context.NPF_STR_StoreItem.Where(x => x.DeleteStatus == false && x.ParentItem == null).ToList();
                foreach (NPF_STR_StoreItem lt in parent)
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
        public BaseResponseDTO<List<DropDown>> GetAllDropdownValuesUOM(long Id)
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                //UOM
                Dd = new DropDown();
                Dd.title = "UOMDetails";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_UomDetails> uom = context.NPF_SYS_UomDetails.Where(x => x.UomHeader.Id == Id && x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_UomDetails lt in uom)
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
        public BaseResponseDTO<StoreItemDTO> GetById(long? Id)
        {
            BaseResponseDTO<StoreItemDTO> dto = new BaseResponseDTO<StoreItemDTO>();
            StoreItemDTO result = new StoreItemDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_STR_StoreItem
                          join b in context.NPF_SYS_UomDetails on a.UomDetails equals b.Id
                          where a.DeleteStatus == false && a.Id == Id
                          select new StoreItemDTO
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Code = a.Code,
                              Description = a.Description,
                              IsConsumable = a.IsConsumable,
                              UOM = b.UomHeader.Id,
                              UomDetails = a.UomDetails,
                              AnimalProductType = (long)a.AnimalProductType,
                              ParentItem = a.ParentItem,
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
        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<StoreItemDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
                BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_STR_StoreItem");
                NPF_STR_StoreItem storeItem = new NPF_STR_StoreItem()
                {
                    Code = BaseDtoS.Data,
                    Name = dataToSave.Data.Name,
                    Description = dataToSave.Data.Description,
                    UomDetails = dataToSave.Data.UomDetails,
                    IsConsumable = dataToSave.Data.IsConsumable,
                    AnimalProductType = (AnimalProducts)dataToSave.Data.AnimalProductType,
                    ParentItem = dataToSave.Data.ParentItem == 0 ? null : dataToSave.Data.ParentItem,
                };

                context.NPF_STR_StoreItem.Add(storeItem);
                context.SaveChanges();

                BaseDto.Data = true;
                BaseDto.Id = storeItem.Id;
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
        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<StoreItemDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_STR_StoreItem storeItem = context.NPF_STR_StoreItem.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                storeItem.Name = dataToUpdate.Data.Name;
                storeItem.Description = dataToUpdate.Data.Description;
                storeItem.UomDetails = dataToUpdate.Data.UomDetails;
                storeItem.IsConsumable = dataToUpdate.Data.IsConsumable;
                storeItem.AnimalProductType = (AnimalProducts)dataToUpdate.Data.AnimalProductType;
                storeItem.ParentItem = dataToUpdate.Data.ParentItem == 0 ? null : dataToUpdate.Data.ParentItem;

                context.NPF_STR_StoreItem.Update(storeItem);
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
                NPF_STR_StoreItem storeToDel = context.NPF_STR_StoreItem.Where(x => x.Id == dataToDelId).FirstOrDefault();
                if (storeToDel != null)
                {
                    storeToDel.DeleteStatus = true;
                    context.NPF_STR_StoreItem.Update(storeToDel);
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
    }
}
