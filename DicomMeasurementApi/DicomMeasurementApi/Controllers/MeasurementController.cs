using DicomMeasurementApi.Models;
using DicomMeasurementApi.Services;
using Microsoft.AspNetCore.Mvc;
using static DicomMeasurementApi.Models.MeasurementDto;

namespace DicomMeasurementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeasurementController : ControllerBase
    {
        private readonly ILogger<MeasurementController> _logger;
        private readonly IMeasurementServer _measurementServer;
        public MeasurementController(ILogger<MeasurementController> logger,IMeasurementServer measurementServer)
        {
            _logger = logger;
            _measurementServer = measurementServer;
        }

        [HttpPost("save")]
        public async Task<ActionResult<ApiResponse<MeasurementResponse>>> Save([FromBody] SaveMeasurementRequest saveRequest,[FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new ApiResponse<MeasurementResponse> { Success = false, Error = "UserId为空" });
            }

            var result = await _measurementServer.SaveMeasurementAsync(saveRequest, userId);
            if(!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<ApiResponse<MeasurementResponse>>>> Get(
            [FromQuery] string fileId,
            [FromQuery] string studyInstanceUid, 
            [FromQuery] string seriesInstanceUid,
            [FromQuery] string sopInstanceUid,
            [FromQuery] int frameNumber,
            [FromQuery] string userId)
        {
            var result = await _measurementServer.GetMeasurementsAsync(fileId, studyInstanceUid, seriesInstanceUid, sopInstanceUid, frameNumber, userId);
            if(!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new ApiResponse<bool> { Success = false, Error = "UserId is required" });
            }
            var result = await _measurementServer.DeleteMeasurementAsync(id, userId);
            if(!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
