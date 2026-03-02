#!/usr/bin/env python3
import csv
import json
import re
from pathlib import Path
from datetime import datetime, timezone

ROOT = Path(__file__).resolve().parents[1]
sample_root = ROOT / "SampleOutput"
session_dir = sample_root / f"{datetime.now(timezone.utc).strftime('%Y%m%d_%H%M%S')}_python_validation_session"
images_dir = session_dir / "images"
logs_dir = session_dir / "logs"
images_dir.mkdir(parents=True, exist_ok=True)
logs_dir.mkdir(parents=True, exist_ok=True)

frames = []
for idx in range(10):
    name = f"frame_{idx:06d}.jpg"
    (images_dir / name).write_bytes(f"mock_frame_{idx}".encode())
    frames.append([idx, name, 1000 + idx * 0.033333, 1280, 720, "jpeg"])

with (session_dir / "frame_index_to_timestamp.csv").open("w", newline="") as f:
    w = csv.writer(f)
    w.writerow(["frame_index","file_name","timestamp_seconds","image_width","image_height","pixel_format"])
    w.writerows(frames)

for filename, unit in [("imu_accel.csv", "m/s^2"), ("imu_gyro.csv", "rad/s")]:
    with (session_dir / filename).open("w", newline="") as f:
        w = csv.writer(f)
        w.writerow(["timestamp_seconds","x","y","z","unit","coordinate_frame"])
        for i in range(100):
            w.writerow([1000 + i * 0.005, 0.1, 0.2, 0.3, unit, "device"])

metadata = {
    "SessionName": "python_validation_session",
    "SessionIdentifier": session_dir.name,
    "CaptureStartIso8601": datetime.now(timezone.utc).isoformat(),
    "CaptureEndIso8601": datetime.now(timezone.utc).isoformat(),
    "CapturedFrameCount": 10,
    "CapturedAccelerometerSampleCount": 100,
    "CapturedGyroscopeSampleCount": 100,
}
(session_dir / "session_metadata.json").write_text(json.dumps(metadata, indent=2))
(session_dir / "camera_intrinsics.json").write_text(json.dumps({"IntrinsicsAvailable": False, "Source": "validation"}, indent=2))
(logs_dir / "logs.txt").write_text("Validation export generated.\n")

# basic monotonic checks
frame_ts = [row[2] for row in frames]
assert all(frame_ts[i] < frame_ts[i+1] for i in range(len(frame_ts)-1))

# bridge parity check
csharp = (ROOT / "Assets/Scripts/Native/IosCaptureBridge.cs").read_text()
plugin_h = (ROOT / "Assets/Plugins/iOS/VinsCapturePlugin.h").read_text()
externs = set(re.findall(r"extern void (Vins\w+)\(", csharp))
exports = set(re.findall(r"void (Vins\w+)\(", plugin_h))
missing = externs - exports
if missing:
    raise SystemExit(f"Bridge mismatch, missing exports: {missing}")

print(f"VALIDATION_OK {session_dir}")
