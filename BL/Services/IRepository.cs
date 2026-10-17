using BL.Constants;
using BL.Models.Common;
using DAL.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public interface IRepository<T>
    {

        BaseResponseDTO<List<T>> GetAll();
        BaseResponseDTO<T> GetById(string Id);

        BaseResponseDTO<bool> Save(BaseRequestDTO<T> dataToSave);

        BaseResponseDTO<bool> Update(BaseRequestDTO<T> dataToUpdate);

        BaseResponseDTO<bool> Delete(BaseRequestDTO<string> dataToDelId);
       // BaseResponseDTO<bool> Delete(BaseRequestDTO<T> dataToDel);

        //BaseResponseDTO<List<DropDownItem>> GetDropDownValues();

        BaseResponseDTO<List<DropDown>> GetAllDropDownValues();

    }
    public interface IRepositoryCus<T>
    {

        BaseResponseDTO<List<T>> GetAll();
        BaseResponseDTO<T> GetById(long Id);
        Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<T> dataToSave);
        Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<T> dataToUpdate);
        BaseResponseDTO<bool> Delete(BaseRequestDTO<long> dataToDelId);
        BaseResponseDTO<List<DropDown>> GetAllDropDownValues();

    }
}
