# VinsMonoCaptureUnity

Unity iOS-first capture project for collecting camera + IMU data on iPhone/iPad for later VINS-Mono conversion.

## Implemented Scope
- Unity C# capture/session architecture with clear separation:
  - `CaptureSessionController`
  - `CaptureStatusView`
  - `FileExportService`
  - `SessionMetadataModel`
  - `ImageFrameRecord`
  - `ImuSampleRecord`
  - Native bridge interfaces in `Assets/Scripts/Native`
- iOS native plugin scaffold in `Assets/Plugins/iOS` using AVFoundation/CoreMotion entry points.
- Export format with session folder + CSV + JSON + logs.
- Reproducible validation runner (`Tools/ValidationRunner`) that generates sample output and verifies export pipeline in this environment.

## Project Structure
- `Assets/Scripts/Capture` - capture orchestration
- `Assets/Scripts/UI` - basic UI wiring
- `Assets/Scripts/DataModels` - serializable models
- `Assets/Scripts/Export` - CSV/JSON/session path export services
- `Assets/Scripts/Native` - bridge interfaces + iOS + editor mock implementation
- `Assets/Plugins/iOS` - Objective-C++ plugin bridge
- `Tools/ValidationRunner` - non-Unity reproducible validation app
- `SampleOutput` - generated mock capture session artifacts

## Running Validation (Host)
```bash
cd Tools/ValidationRunner
dotnet run
```
This creates one sample exported session under `SampleOutput/`.

## Unity Setup Notes
1. Open project in Unity 2022.3 LTS.
2. Use menu: `VinsMono -> Create Default Capture Scene`.
3. Open `Assets/Scenes/CaptureScene.unity`.
4. For editor simulation, keep `Use Editor Mock Bridge` enabled on `CaptureSessionController`.
5. For iOS device, disable mock bridge and build for iOS.

## iOS Permissions
Add camera and motion usage descriptions in iOS Player settings / generated Info.plist:
- `NSCameraUsageDescription`
- `NSMotionUsageDescription`

## Environment Limitation Disclosure
- Unity Editor and real iOS hardware execution were **not available in this environment**.
- Therefore, real on-device AVFoundation frame callbacks and CoreMotion runtime verification are not claimed here.
- Export/data pipeline and file outputs were validated through reproducible host-side runner.
