using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Administration;
using DAL.Models;
using DAL.Models.Administration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Controllers.Administration;

namespace API.Controllers.Administration
{

    [Route("api/[controller]")]
    [ApiController]
    public class LookupTypeController : ControllerBase
    {
        public LookUpTypeService service;
        public LookupTypeController()
        {
            service = new LookUpTypeService();
        }
     
        [HttpPost("getLookupTypeList")]
        public async Task<ActionResult<BaseResponseDTO<List<NPF_SYS_LookUpType>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<NPF_SYS_LookUpType>> List = new BaseResponseDTO<List<NPF_SYS_LookUpType>>();
                List = service.GetAll();
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost("saveLookupType")]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<NPF_SYS_LookUpType> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                if (dto.Data.Id == 0)
                {
                    BaseResponseDTO = await service.SaveAsync(dto);
                }
                else
                {
                    BaseResponseDTO = await service.UpdateAsync(dto);
                }

                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost("deleteLookupType")]
        public async Task<ActionResult<BaseResponseDTO<bool>>> Delete([FromBody] BaseRequestDTO<int> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = service.Delete(dto);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost("getDropdown")]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetAllDropDownValues()
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = service.GetAllDropDownValues();
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

    }
}
