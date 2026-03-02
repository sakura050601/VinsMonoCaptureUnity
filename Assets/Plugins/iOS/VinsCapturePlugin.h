#import <Foundation/Foundation.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef void (*VinsCameraFrameCallback)(long frameIndex, double timestampSeconds, const unsigned char *imageBytes, int imageLength, int width, int height);
typedef void (*VinsImuCallback)(double timestampSeconds, double x, double y, double z);
typedef void (*VinsStringCallback)(const char *jsonCString);

void VinsStartCameraCapture(VinsCameraFrameCallback frameCallback, VinsStringCallback intrinsicsCallback);
void VinsStopCameraCapture(void);
void VinsStartImuCapture(double updateIntervalSeconds, VinsImuCallback accelerometerCallback, VinsImuCallback gyroscopeCallback);
void VinsStopImuCapture(void);

#ifdef __cplusplus
}
#endif
