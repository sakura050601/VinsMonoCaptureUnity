using System.Collections.Generic;
using System.Globalization;
using System.IO;
using VinsMonoCapture.DataModels;

namespace VinsMonoCapture.Export
{
    public class CsvWriterService
    {
        public void WriteFrameTimestampCsv(string filePath, IReadOnlyList<ImageFrameRecord> frameRecords)
        {
            using var writer = new StreamWriter(filePath, false);
            writer.WriteLine("frame_index,file_name,timestamp_seconds,image_width,image_height,pixel_format");
            foreach (var frameRecord in frameRecords)
            {
                writer.WriteLine($"{frameRecord.FrameIndex},{frameRecord.FileName},{frameRecord.TimestampSeconds.ToString("F9", CultureInfo.InvariantCulture)},{frameRecord.ImageWidth},{frameRecord.ImageHeight},{frameRecord.PixelFormat}");
            }
        }

        public void WriteImuCsv(string filePath, IReadOnlyList<ImuSampleRecord> imuSamples)
        {
            using var writer = new StreamWriter(filePath, false);
            writer.WriteLine("timestamp_seconds,x,y,z,unit,coordinate_frame");
            foreach (var sample in imuSamples)
            {
                writer.WriteLine($"{sample.TimestampSeconds.ToString("F9", CultureInfo.InvariantCulture)},{sample.X.ToString("F9", CultureInfo.InvariantCulture)},{sample.Y.ToString("F9", CultureInfo.InvariantCulture)},{sample.Z.ToString("F9", CultureInfo.InvariantCulture)},{sample.Unit},{sample.CoordinateFrame}");
            }
        }
    }
}
