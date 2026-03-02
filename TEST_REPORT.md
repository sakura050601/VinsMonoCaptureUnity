# TEST_REPORT

## Environment
- Host: macOS (Darwin arm64)
- Unity Editor: not available in this execution environment
- iOS device runtime: not available in this execution environment

## Executed Checks
1. Host-side dataset and export validation:
   - Command: `python3 Tools/run_validation.py`
   - Checks performed:
     - Session directory creation
     - Image file generation
     - CSV generation (`frame_index_to_timestamp.csv`, `imu_accel.csv`, `imu_gyro.csv`)
     - JSON generation (`session_metadata.json`, `camera_intrinsics.json`)
     - Timestamp monotonicity assertion
     - Bridge symbol parity check between C# extern declarations and iOS plugin exports
2. Native iOS plugin syntax check:
   - Command: `xcrun --sdk iphoneos clang -fobjc-arc -fsyntax-only -x objective-c++ Assets/Plugins/iOS/VinsCapturePlugin.mm`
   - Result: success with 3 deprecation warnings (orientation API on iOS 17+ SDK).
3. Attempted .NET C# validator run:
   - Command: `dotnet run` in `Tools/ValidationRunner`
   - Result: failed (`dotnet` not installed in environment).

## Results
- Host-side export pipeline: PASS
- Sample output generated: PASS
- iOS plugin syntax check (iphoneos SDK): PASS (warnings only)
- Unity compile-time open/project test: NOT EXECUTED (Unity unavailable)
- iOS on-device camera/IMU real capture: NOT EXECUTED (device unavailable)

## Remaining Device-only Validation
- Verify AVFoundation frame callback throughput and encoded image persistence.
- Verify CoreMotion accelerometer/gyro callback timing under sustained capture.
- Verify intrinsics extraction values on target device.
- Verify long-run synchronization drift characteristics.
