using DicomMeasurementApi.Models;
using Microsoft.Extensions.Configuration.UserSecrets;
using static DicomMeasurementApi.Models.MeasurementDto;

namespace DicomMeasurementApi.Services
{
    public interface IMeasurementServer
    {
        Task<ApiResponse<MeasurementResponse>> SaveMeasurementAsync(SaveMeasurementRequest saveRequest, string userId);
        Task<ApiResponse<List<MeasurementResponse>>> GetMeasurementsAsync(string fileId, string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid, int frameNumber, string userId);
        Task<ApiResponse<bool>> DeleteMeasurementAsync(int id, string userId);

    }
}
