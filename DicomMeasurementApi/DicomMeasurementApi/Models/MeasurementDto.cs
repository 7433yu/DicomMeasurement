using System.ComponentModel.DataAnnotations;

namespace DicomMeasurementApi.Models
{
    public class MeasurementDto
    {
        public class SaveMeasurementRequest
        {
            public int Id { get; set; }

            [Required]
            [StringLength(64)]
            public string UserId { get; set; } = string.Empty;

            [Required]
            [StringLength(128)]
            public string FileId { get; set; } = string.Empty;

            [Required]
            [StringLength(128)]
            public string StudyInstanceUid { get; set; } = string.Empty;

            [Required]
            [StringLength(128)]
            public string SeriesInstanceUid { get; set; } = string.Empty;

            [StringLength(128)]
            public string SopInstanceUid { get; set; } = string.Empty;

            public int FrameNumber { get; set; } = 1;

            [Required]
            public MeasurementType MeasurementType { get; set; } = MeasurementType.Length;

            [StringLength(256)]
            public string Label { get; set; } = string.Empty;

            [StringLength(512)]
            public string Description { get; set; } = string.Empty;

            public double? Value { get; set; }
            public string Color { get; set; } = "#FF0000";
            public bool Visible { get; set; } = true;

            public string MeasurementData { get; set; } = "{}";

            public List<PointDto> Points { get; set; } = new List<PointDto>();

            [StringLength(16)]
            public string Unit { get; set; } = "mm";
        }

        public class MeasurementResponse
        {
            public int Id { get; set; }
            public string UserId { get; set; } = string.Empty;
            public string FileId { get; set; } = string.Empty;
            public string StudyInstanceUid { get; set; } = string.Empty;
            public string SeriesInstanceUid { get; set; } = string.Empty;
            public string SopInstanceUid { get; set; } = string.Empty;
            public int FrameNumber { get; set; }
            public MeasurementType MeasurementType { get; set; }
            public string Label { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public double? Value { get; set; }
            public string Color { get; set; } = "#FF0000";
            public bool Visible { get; set; } = true;
            public string MeasurementData { get; set; } = "{}";
            public List<PointDto> Points { get; set; } = new List<PointDto>();
            public string Unit { get; set; } = "mm";
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        public class DeleteMeasurementRequest
        {
            [Required]
            public int Id { get; set; }
        }

        public class PointDto
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
        }
    }
}