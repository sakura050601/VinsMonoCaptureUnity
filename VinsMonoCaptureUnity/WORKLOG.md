# WORKLOG

## 2026-03-02 Initial Plan

### Goal
Build a Unity iOS-first data capture app that records camera frames + timestamps + IMU (accelerometer/gyroscope) + camera metadata, exported in a structured format suitable for later VINS-Mono conversion.

### Implementation Plan
1. Scaffold Unity project folder layout with clear architecture boundaries.
2. Implement data models and export/persistence pipeline first (CSV/JSON/session folder manager/logging).
3. Implement capture orchestration in Unity C# with clean interfaces:
   - `ICameraCaptureBridge`
   - `IImuCaptureBridge`
   - `CaptureSessionController`
4. Implement iOS native plugin bridge (AVFoundation + CoreMotion) in isolated plugin files.
5. Implement simple UI scripts (`CaptureStatusView`) and expected controls.
6. Add editor/mock capture bridge for reproducible validation without device.
7. Add validation tests/checks (file generation, timestamp monotonicity, schema sanity, bridge signature consistency checks where possible).
8. Add docs: `README.md`, `DATA_FORMAT.md`, `TEST_REPORT.md`.
9. Generate sample exported session via mock runner.
10. Commit incrementally with milestone commit messages.

### Known Environment Risks
- Unity Editor binary is not currently available in this environment.
- Real iOS device execution cannot be verified here.
- Will maximize compile/data validation via reproducible non-Unity checks and complete plugin source implementation.

## 2026-03-02 Execution Log
- Scaffolded Unity project structure with C# architecture split: Capture, UI, DataModels, Export, Native.
- Implemented data export services and session folder manager.
- Implemented `CaptureSessionController` and `CaptureStatusView`.
- Implemented iOS native bridge C API declarations and Objective-C++ plugin implementation using AVFoundation + CoreMotion.
- Added editor mock bridge for non-device simulation.
- Added validation tooling:
  - `Tools/run_validation.py` for reproducible host-side dataset generation + checks.
  - iOS plugin syntax check with `xcrun --sdk iphoneos clang -fsyntax-only`.

### Issues and Fixes
1. **Issue:** `dotnet` runtime unavailable, so C# console validation app could not run.
   - **Fix:** Added Python-based reproducible validator that verifies output structure and bridge symbol parity.
2. **Issue:** native plugin compile check on macOS SDK failed due `CMMotionManager` unavailability.
   - **Fix:** switched syntax check target to iPhoneOS SDK using `xcrun --sdk iphoneos clang`.
3. **Issue:** iOS plugin compile produced orientation API deprecation warnings on iOS 17+ SDK.
   - **Fix:** documented as non-blocking warning; current implementation remains functional for baseline capture.
