using DicomMeasurementApi.Data;
using DicomMeasurementApi.Models;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using System;
using System.Text.Json;
using static DicomMeasurementApi.Models.MeasurementDto;

namespace DicomMeasurementApi.Services
{
    public class MeasurementServer : IMeasurementServer
    {
        private readonly ILogger<MeasurementServer> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly AsyncRetryPolicy _retryPolicy;

        public MeasurementServer(ILogger<MeasurementServer> logger, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;

            _retryPolicy = Policy
                .Handle<DbUpdateException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt-1)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"第 {retryCount} 次重试请求，等待 {timeSpan.TotalSeconds} 秒后重试");
                    });
        }

        public async Task<ApiResponse<MeasurementResponse>> SaveMeasurementAsync(SaveMeasurementRequest saveRequest,string userId)
        {
            if (string.IsNullOrEmpty(userId)||
                string.IsNullOrEmpty(saveRequest.FileId)||
                string.IsNullOrEmpty(saveRequest.StudyInstanceUid)||
                string.IsNullOrEmpty(saveRequest.SeriesInstanceUid))
            {
                return new ApiResponse<MeasurementResponse>
                {
                    Success = false,
                    Error = "参数错误或不完整"
                };
            }

            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try 
                {
                    var unifiedMeasurementJson = BuildUnifiedMeasurementJson(saveRequest);
                    var measurementDataJson = JsonSerializer.Serialize(unifiedMeasurementJson);

                    var exsitingMeasurement = await _dbContext.Measurements.FirstOrDefaultAsync(m =>
                        m.UserId == userId &&
                        m.FileId == saveRequest.FileId &&
                        m.StudyInstanceUid == saveRequest.StudyInstanceUid &&
                        m.SeriesInstanceUid == saveRequest.SeriesInstanceUid &&
                        m. SopInstanceUid == (saveRequest.SopInstanceUid ?? string.Empty) &&
                        m.FrameNumber == saveRequest.FrameNumber);

                    if (exsitingMeasurement is null)
                    {
                        var entity = new Measurement
                        {
                            UserId = userId,
                            FileId = saveRequest.FileId,
                            StudyInstanceUid = saveRequest.StudyInstanceUid,
                            SeriesInstanceUid = saveRequest.SeriesInstanceUid,
                            SopInstanceUid = saveRequest.SopInstanceUid ?? string.Empty,
                            FrameNumber = saveRequest.FrameNumber,
                            MeasurementType = saveRequest.MeasurementType,
                            Label = saveRequest.Label,
                            Description = saveRequest.Description,
                            Value = saveRequest.Value,
                            Color = saveRequest.Color,
                            Visible = saveRequest.Visible,
                            MeasurementData = measurementDataJson,
                            Unit = saveRequest.Unit
                        };

                        _dbContext.Measurements.Add(entity);
                        await _dbContext.SaveChangesAsync();

                        return new ApiResponse<MeasurementResponse>
                        {
                            Success = true,
                            Data = MapToDto(entity)
                        };
                    }

                    else
                    {
                        exsitingMeasurement.MeasurementType = saveRequest.MeasurementType;
                        exsitingMeasurement.Label = saveRequest.Label;
                        exsitingMeasurement.Description = saveRequest.Description;
                        exsitingMeasurement.Value = saveRequest.Value;
                        exsitingMeasurement.Color = saveRequest.Color;
                        exsitingMeasurement.Visible = saveRequest.Visible;
                        exsitingMeasurement.MeasurementData = measurementDataJson;
                        exsitingMeasurement.Unit = saveRequest.Unit;
                        exsitingMeasurement.UpdatedAt = DateTime.UtcNow;

                        await _dbContext.SaveChangesAsync();
                        return new ApiResponse<MeasurementResponse>
                        {
                            Success = true,
                            Data = MapToDto(exsitingMeasurement)
                        };
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,"给用户{userId}保存文件{fileId}时发生错误",userId,saveRequest.FileId);

                    return new ApiResponse<MeasurementResponse>
                    {
                        Success = false,
                        Error = "保存文件时发生错误"
                    };
                }
            });
        }

        public async Task<ApiResponse<List<MeasurementResponse>>> GetMeasurementsAsync(string fileId, string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid, int frameNumber, string userId)
        {
            if(string.IsNullOrEmpty(fileId)||
                string.IsNullOrEmpty(studyInstanceUid)||
                string.IsNullOrEmpty(seriesInstanceUid)||
                string.IsNullOrEmpty(userId))
            {
                return new ApiResponse<List<MeasurementResponse>>
                {
                    Success = false,
                    Error = "参数错误或不完整"
                };
            }

            return await _retryPolicy.ExecuteAsync(async()=>
            {
                try
                {
                    var query = _dbContext.Measurements.AsQueryable()
                        .Where(m => m.UserId == userId &&
                        m.FileId == fileId && 
                        m.StudyInstanceUid == studyInstanceUid && 
                        m.SeriesInstanceUid == seriesInstanceUid);

                    if (!string.IsNullOrEmpty(sopInstanceUid))
                    {
                        query = query.Where(m => m.SopInstanceUid == sopInstanceUid);
                    }

                    if (frameNumber > 0)
                    {
                        query = query.Where(m => m.FrameNumber == frameNumber);
                    }

                    var measurements=await query.OrderByDescending(m=>m.UpdatedAt).ToListAsync();
                    var result = measurements.Select(MapToDto).ToList();
                    return new ApiResponse<List<MeasurementResponse>>
                    {
                        Success = true,
                        Data = result
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "获取用户{userId}文件{fileId}的标注时发生错误", userId, fileId);
                    return new ApiResponse<List<MeasurementResponse>>
                    {
                        Success = false,
                        Error = "获取标注数据时发生错误"
                    };
                }
            });
        }

        public async Task<ApiResponse<bool>> DeleteMeasurementAsync(int id, string userId)
        {
            if ((id <= 0)|| string.IsNullOrEmpty(userId))
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Error = "参数错误或不完整"
                };
            }

            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var entity = await _dbContext.Measurements.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
                    if (entity is null)
                    {
                        return new ApiResponse<bool>
                        {
                            Success = false,
                            Error = "标注不存在或不属于当前用户"
                        };
                    }

                    _dbContext.Measurements.Remove(entity);
                    await _dbContext.SaveChangesAsync();

                    return new ApiResponse<bool>
                    {
                        Success = true,
                        Data = true
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "删除用户{userId}的标注{measurementId}时发生错误", userId, id);
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Error = "删除标注时发生错误"
                    };
                }
            });
        }

        //整合Points
        private static string BuildUnifiedMeasurementJson(SaveMeasurementRequest saveRequest)
        {
            Dictionary<string, object> result;
            try
            {
                if (!string.IsNullOrEmpty(saveRequest.MeasurementData))
                {
                    var parsed = JsonSerializer.Deserialize<Dictionary<string, object>>(saveRequest.MeasurementData);
                    result = parsed ?? new Dictionary<string, object>();
                }
                else
                {
                    result = new Dictionary<string, object>();
                } 
            }
            catch
            {
                result = new Dictionary<string, object>();
            }

            result["points"] = saveRequest.Points ?? new List<PointDto>();

            return JsonSerializer.Serialize(result);
        }

        //将数据模型转为DTO
        private MeasurementResponse MapToDto(Measurement m)
        {
            return new MeasurementResponse
            {
                Id = m.Id,
                UserId = m.UserId,
                FileId = m.FileId,
                StudyInstanceUid = m.StudyInstanceUid,
                SeriesInstanceUid = m.SeriesInstanceUid,
                SopInstanceUid = m.SopInstanceUid,
                FrameNumber = m.FrameNumber,
                MeasurementType = m.MeasurementType,
                Label = m.Label,
                Description = m.Description,
                Value = m.Value,
                Color = m.Color,
                Visible = m.Visible,
                MeasurementData = m.MeasurementData,
                Unit = m.Unit,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            };
        }
    }
}
