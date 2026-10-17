using BL.Constants;
using BL.Models.Access;
using BL.Models.Common;
using BL.Models.Store;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models.Administration;
using DAL.Models.Store;
using DAL.Models;
using NPOI.SS.Formula.Functions;

namespace BL.Services.Store
{
    public class StoreTransactionService
    {
        private NPFContext context;
        private QueryResult queryResult;
        private MatrixService Mservice;
        public UserMatrixDTO UM = new UserMatrixDTO();

        public StoreTransactionService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            Mservice = new MatrixService();
            UM = Mservice.sGetMatrixUserByUserID();
        }

        public BaseResponseDTO<List<StockRequestListDTO>> GetStockRequestList()
        {
            BaseResponseDTO<List<StockRequestListDTO>> dto = new BaseResponseDTO<List<StockRequestListDTO>>();
            List<StockRequestListDTO> result = new List<StockRequestListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_STR_StockRequest
                          where a.DeleteStatus == false
                          select new StockRequestListDTO
                          {
                              Id = a.Id,
                              Name = context.NPF_STR_StoreItem.Where(x => x.Id == a.StoreItemId).FirstOrDefault().Name,
                              RequestDate = a.RequestDate.ToString("dd/MM/yyyy"),
                              ItemSource = context.NPF_STR_Store.Where(p => p.Id == a.ItemSource).FirstOrDefault().Name,
                              Quantity = a.Quantity,
                              ApprovalStatus = a.ApprovalStatus,
                              RequestBy = (from c in context.NPF_SYS_User
                              join d in context.Users on c.AId equals d.Id
                                           where c.Id == a.RequestBy && c.DeleteStatus == false select d.FullName).FirstOrDefault()
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

        public BaseResponseDTO<List<StoreTransactionListDTO>> GetStoreTransactionList()
        {
            BaseResponseDTO<List<StoreTransactionListDTO>> dto = new BaseResponseDTO<List<StoreTransactionListDTO>>();
            List<StoreTransactionListDTO> result = new List<StoreTransactionListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_STR_StoreTransaction
                          where a.DeleteStatus == false && a.StoreItem.AnimalProductType == AnimalProducts.Others
                          select new StoreTransactionListDTO
                          {
                              Id = a.Id,
                              Name = a.StoreItem.Name,
                              StoreLocation = a.Store.Name,
                              Quantity = a.Quantity,
                              Date = a.CreatedDate.ToString("dd/MM/yyyy"),
                              Type = a.RequestType.ToString()
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

        public BaseResponseDTO<List<StoreItemTableDTO>> GetStoreItemList()
        {
            BaseResponseDTO<List<StoreItemTableDTO>> dto = new BaseResponseDTO<List<StoreItemTableDTO>>();
            List<StoreItemTableDTO> result = new List<StoreItemTableDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_STR_StoreItem
                          where a.DeleteStatus == false && a.AnimalProductType == AnimalProducts.Others
                          select new StoreItemTableDTO
                          {
                              Id = a.Id,
                              Name = a.Name
                          }).ToList();
                foreach(StoreItemTableDTO item in result)
                {
                    var storeIn = context.NPF_STR_StoreTransaction.Where(x => x.StoreItem.Id == item.Id && x.RequestType == RequestType.In).Sum(a => a.Quantity);
                    var storeOut = context.NPF_STR_StoreTransaction.Where(x => x.StoreItem.Id == item.Id && x.RequestType == RequestType.Out).Sum(a => a.Quantity);
                    item.Quantity = storeIn - storeOut;
                }
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
        public async Task<BaseResponseDTO<List<DropDown>>> GetAllDropDownValues()
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                Dd = new DropDown();
                Dd.title = "StoreLocation";
                Dd.items = new List<DropDownItem>();
                List<NPF_STR_Store> station = context.NPF_STR_Store.Where(x => x.DeleteStatus == false).ToList();
                foreach (NPF_STR_Store st in station)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = st.Id;
                    Ddi.text =st.Station.Name + " - " + st.Name;
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<StoreTransactionDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
                BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_STR_StoreTransaction");
                NPF_STR_StoreTransaction store = new NPF_STR_StoreTransaction()
                {
                    Code = BaseDtoS.Data,
                    StoreItem = context.NPF_STR_StoreItem.Where(x => x.Id == dataToSave.Data.StoreId).FirstOrDefault(),
                    Store = context.NPF_STR_Store.Where(x => x.Id == dataToSave.Data.StoreLocation).FirstOrDefault(),
                    Quantity = dataToSave.Data.Quantity,
                    DateReceived = dataToSave.Data.DateRecieved,
                    AbsoluteValue = 0,
                    CostPrice = dataToSave.Data.CostPrice,
                    SellPrice = 0,
                    RequestType = RequestType.In,
                    UomDetails = context.NPF_STR_StoreItem.Where(x => x.Id == dataToSave.Data.StoreId).FirstOrDefault().UomDetails,
                    BatchNo = dataToSave.Data.BatchNo,
                    ExpiryDate = dataToSave.Data.ExpiryDate,
                    FromBatchNo = string.Empty,
                    Identifier = string.Empty,
                    PONumber = dataToSave.Data.PONo,
                    Remarks = dataToSave.Data.Remarks
                };

                context.NPF_STR_StoreTransaction.Add(store);
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

        public async Task<BaseResponseDTO<bool>> AproveDecline(BaseRequestDTO<StockRequestADDTO> dto)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (dto.Data.ApprovalStatus == ApprovalStatus.Declined)
                {
                    NPF_STR_StockRequest upd = context.NPF_STR_StockRequest.Where(x => x.Id == dto.Data.Id).FirstOrDefault();
                    upd.ApprovalStatus = ApprovalStatus.Declined;
                    context.NPF_STR_StockRequest.Update(upd);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    NPF_STR_StockRequest upd = context.NPF_STR_StockRequest.Where(x => x.Id == dto.Data.Id).FirstOrDefault();
                    upd.ApprovalStatus = ApprovalStatus.Approved;
                    context.NPF_STR_StockRequest.Update(upd);
                    context.SaveChanges();

                    BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
                    BaseDtoS = await new GenerateCode().GCodeMain(dto.userToken, "NPF_STR_StoreTransaction");
                    NPF_STR_StoreTransaction store = new NPF_STR_StoreTransaction()
                    {
                        Code = BaseDtoS.Data,
                        StoreItem = context.NPF_STR_StoreItem.Where(x => x.Id == upd.StoreItemId).FirstOrDefault(),
                        Store = context.NPF_STR_Store.Where(x => x.Id == upd.ItemSource).FirstOrDefault(),
                        Quantity = upd.Quantity,
                        DateReceived = DateTime.Now,
                        AbsoluteValue = 0,
                        CostPrice = 0,
                        SellPrice = 0,
                        RequestType = RequestType.Out,
                        UomDetails = context.NPF_STR_StoreItem.Where(x => x.Id == upd.StoreItemId).FirstOrDefault().UomDetails,
                        BatchNo = string.Empty,
                        ExpiryDate = null,
                        FromBatchNo = string.Empty,
                        Identifier = string.Empty,
                        PONumber = string.Empty,
                        Remarks = string.Empty
                    };
                    context.NPF_STR_StoreTransaction.Add(store);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
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
    }
}
