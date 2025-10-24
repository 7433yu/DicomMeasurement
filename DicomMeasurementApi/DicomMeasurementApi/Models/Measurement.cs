using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static DicomMeasurementApi.Models.MeasurementDto;

namespace DicomMeasurementApi.Models
{
    public class Measurement
    {
        [Key]
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

        [NotMapped]
        public List<PointDto> Points { get; set; } = new List<PointDto>();

        [StringLength(16)]
        public string Unit { get; set; } = "mm";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum MeasurementType
    {
        Length,
        Angle,               // 角度测量
        Area,                // 面积测量（多边形/ROI）
        EllipticalRoi,
        RectangleRoi,
        Bidirectional,       // 双向测量
        Point,               // 单点标注/定位
        Polyline,            // 多段线测量/标注
        ArrowAnnotate,       // 箭头标注
        FreehandRoi,
        TextAnnotate,        // 文字标注
        CircleRoi,
        CobbAngle,
        Probe,               // 探针测量（像素值）
    }
}