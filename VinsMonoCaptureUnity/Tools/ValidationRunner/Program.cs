using VinsMonoCapture.DataModels;
using VinsMonoCapture.Export;

var outputRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "SampleOutput"));
Directory.CreateDirectory(outputRoot);

var folderManager = new SessionFolderManager();
var sessionPaths = folderManager.CreateSessionFolders(outputRoot, "mock_validation_session");

var frames = Enumerable.Range(0, 10).Select(index => new ImageFrameRecord
{
    FrameIndex = index,
    FileName = $"frame_{index:D6}.jpg",
    TimestampSeconds = 1000.0 + index * 0.033333,
    ImageWidth = 1280,
    ImageHeight = 720,
    PixelFormat = "jpeg"
}).ToList();

foreach (var frame in frames)
{
    File.WriteAllBytes(Path.Combine(sessionPaths.ImageDirectoryPath, frame.FileName), System.Text.Encoding.ASCII.GetBytes($"mock_image_{frame.FrameIndex}"));
}

var accelerometer = Enumerable.Range(0, 100).Select(i => new ImuSampleRecord
{
    TimestampSeconds = 1000.0 + i * 0.005,
    X = Math.Sin(i * 0.1),
    Y = 0.0,
    Z = 9.81,
    Unit = "m/s^2",
    CoordinateFrame = "device"
}).ToList();

var gyroscope = Enumerable.Range(0, 100).Select(i => new ImuSampleRecord
{
    TimestampSeconds = 1000.0 + i * 0.005,
    X = 0.01,
    Y = Math.Cos(i * 0.1),
    Z = 0.02,
    Unit = "rad/s",
    CoordinateFrame = "device"
}).ToList();

var metadata = new SessionMetadataModel
{
    SessionName = "mock_validation_session",
    SessionIdentifier = sessionPaths.SessionIdentifier,
    CaptureStartIso8601 = DateTime.UtcNow.AddSeconds(-3).ToString("O"),
    CaptureEndIso8601 = DateTime.UtcNow.ToString("O"),
    DeviceModel = "ValidationHost",
    OperatingSystem = Environment.OSVersion.ToString(),
    CapturedFrameCount = frames.Count,
    CapturedAccelerometerSampleCount = accelerometer.Count,
    CapturedGyroscopeSampleCount = gyroscope.Count,
    CameraIntrinsics = new CameraIntrinsicsModel
    {
        IntrinsicsAvailable = true,
        FocalLengthX = 900,
        FocalLengthY = 900,
        PrincipalPointX = 640,
        PrincipalPointY = 360,
        ImageWidth = 1280,
        ImageHeight = 720,
        DistortionModel = "unknown",
        Source = "mock"
    }
};

var exportService = new FileExportService(new CsvWriterService(), new JsonWriterService());
exportService.ExportSession(sessionPaths, metadata, frames, accelerometer, gyroscope, "missing_intrinsics.json", "ValidationRunner completed export.");

Console.WriteLine(sessionPaths.SessionDirectoryPath);
