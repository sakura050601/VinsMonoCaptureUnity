# DATA_FORMAT

Each session exports to:

```
<session_identifier>/
  images/
    frame_000000.jpg
    ...
  frame_index_to_timestamp.csv
  imu_accel.csv
  imu_gyro.csv
  camera_intrinsics.json
  session_metadata.json
  logs/
    logs.txt
```

## frame_index_to_timestamp.csv
Columns:
- `frame_index`
- `file_name`
- `timestamp_seconds`
- `image_width`
- `image_height`
- `pixel_format`

## imu_accel.csv / imu_gyro.csv
Columns:
- `timestamp_seconds`
- `x`
- `y`
- `z`
- `unit`
- `coordinate_frame`

Units:
- Accelerometer: `m/s^2`
- Gyroscope: `rad/s`

## session_metadata.json
Contains:
- Session identity and time range
- Timestamp source assumptions
- Device metadata
- Sample counters
- Camera intrinsics block

## Timestamp Convention
- Camera target source: AVFoundation sample buffer presentation timestamp.
- IMU target source: CoreMotion timestamps (monotonic time domain).
- Conversion/alignment for VINS-Mono should preserve monotonic increasing timeline, with explicit offset estimation between streams if required.

## Coordinate / Conversion Notes
- IMU frame currently documented as device frame from CoreMotion callbacks.
- Additional conversion for VINS-Mono is expected (axis convention confirmation, calibration alignment, and final bag/text format transform).
