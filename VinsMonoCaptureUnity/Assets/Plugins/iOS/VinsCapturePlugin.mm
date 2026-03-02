#import "VinsCapturePlugin.h"
#import <AVFoundation/AVFoundation.h>
#import <CoreMotion/CoreMotion.h>
#import <UIKit/UIKit.h>

@interface VinsCameraCaptureService : NSObject<AVCaptureVideoDataOutputSampleBufferDelegate>
@property(nonatomic, strong) AVCaptureSession *captureSession;
@property(nonatomic, strong) dispatch_queue_t captureQueue;
@property(nonatomic, assign) long frameIndex;
@property(nonatomic, assign) VinsCameraFrameCallback frameCallback;
@property(nonatomic, assign) VinsStringCallback intrinsicsCallback;
@end

@implementation VinsCameraCaptureService

- (instancetype)initWithFrameCallback:(VinsCameraFrameCallback)frameCallback intrinsicsCallback:(VinsStringCallback)intrinsicsCallback {
    self = [super init];
    if (self) {
        _frameCallback = frameCallback;
        _intrinsicsCallback = intrinsicsCallback;
        _frameIndex = 0;
        _captureQueue = dispatch_queue_create("com.vins.capture.camera", DISPATCH_QUEUE_SERIAL);
    }
    return self;
}

- (void)start {
    self.captureSession = [[AVCaptureSession alloc] init];
    self.captureSession.sessionPreset = AVCaptureSessionPreset1280x720;

    AVCaptureDevice *camera = [AVCaptureDevice defaultDeviceWithMediaType:AVMediaTypeVideo];
    if (!camera) { return; }

    NSError *error = nil;
    AVCaptureDeviceInput *input = [AVCaptureDeviceInput deviceInputWithDevice:camera error:&error];
    if (error || !input || ![self.captureSession canAddInput:input]) { return; }
    [self.captureSession addInput:input];

    AVCaptureVideoDataOutput *output = [[AVCaptureVideoDataOutput alloc] init];
    output.videoSettings = @{(NSString *)kCVPixelBufferPixelFormatTypeKey: @(kCVPixelFormatType_32BGRA)};
    output.alwaysDiscardsLateVideoFrames = YES;
    [output setSampleBufferDelegate:self queue:self.captureQueue];

    if ([self.captureSession canAddOutput:output]) {
        [self.captureSession addOutput:output];
    }

    AVCaptureConnection *connection = [output connectionWithMediaType:AVMediaTypeVideo];
    if (connection.isVideoOrientationSupported) {
        connection.videoOrientation = AVCaptureVideoOrientationPortrait;
    }

    if (@available(iOS 13.0, *)) {
        AVCaptureDeviceFormat *format = camera.activeFormat;
        CMVideoDimensions dimensions = CMVideoFormatDescriptionGetDimensions(format.formatDescription);
        NSDictionary *intrinsics = @{
            @"ImageWidth": @(dimensions.width),
            @"ImageHeight": @(dimensions.height),
            @"IntrinsicsAvailable": @([camera.activeFormat isVideoBinned] ? NO : YES),
            @"Source": @"AVFoundation activeFormat"
        };
        if (self.intrinsicsCallback) {
            NSData *jsonData = [NSJSONSerialization dataWithJSONObject:intrinsics options:0 error:nil];
            NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
            self.intrinsicsCallback(jsonString.UTF8String);
        }
    }

    [self.captureSession startRunning];
}

- (void)stop {
    [self.captureSession stopRunning];
    self.captureSession = nil;
}

- (void)captureOutput:(AVCaptureOutput *)output didOutputSampleBuffer:(CMSampleBufferRef)sampleBuffer fromConnection:(AVCaptureConnection *)connection {
    (void)output;
    (void)connection;
    if (!self.frameCallback) { return; }

    CVImageBufferRef imageBuffer = CMSampleBufferGetImageBuffer(sampleBuffer);
    if (!imageBuffer) { return; }

    CVPixelBufferLockBaseAddress(imageBuffer, kCVPixelBufferLock_ReadOnly);
    CIImage *ciImage = [CIImage imageWithCVPixelBuffer:imageBuffer];
    CIContext *context = [CIContext contextWithOptions:nil];
    CGImageRef cgImage = [context createCGImage:ciImage fromRect:CGRectMake(0, 0, CVPixelBufferGetWidth(imageBuffer), CVPixelBufferGetHeight(imageBuffer))];
    UIImage *uiImage = [UIImage imageWithCGImage:cgImage];
    NSData *jpegData = UIImageJPEGRepresentation(uiImage, 0.8);

    CMTime presentationTime = CMSampleBufferGetPresentationTimeStamp(sampleBuffer);
    double timestampSeconds = CMTimeGetSeconds(presentationTime);

    self.frameCallback(self.frameIndex, timestampSeconds, (const unsigned char *)jpegData.bytes, (int)jpegData.length, (int)CVPixelBufferGetWidth(imageBuffer), (int)CVPixelBufferGetHeight(imageBuffer));
    self.frameIndex += 1;

    CGImageRelease(cgImage);
    CVPixelBufferUnlockBaseAddress(imageBuffer, kCVPixelBufferLock_ReadOnly);
}

@end

static VinsCameraCaptureService *cameraService = nil;
static CMMotionManager *motionManager = nil;

void VinsStartCameraCapture(VinsCameraFrameCallback frameCallback, VinsStringCallback intrinsicsCallback) {
    AVAuthorizationStatus authStatus = [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo];
    if (authStatus == AVAuthorizationStatusDenied || authStatus == AVAuthorizationStatusRestricted) {
        return;
    }

    cameraService = [[VinsCameraCaptureService alloc] initWithFrameCallback:frameCallback intrinsicsCallback:intrinsicsCallback];
    [cameraService start];
}

void VinsStopCameraCapture(void) {
    [cameraService stop];
    cameraService = nil;
}

void VinsStartImuCapture(double updateIntervalSeconds, VinsImuCallback accelerometerCallback, VinsImuCallback gyroscopeCallback) {
    motionManager = [[CMMotionManager alloc] init];
    motionManager.accelerometerUpdateInterval = updateIntervalSeconds;
    motionManager.gyroUpdateInterval = updateIntervalSeconds;

    NSOperationQueue *queue = [[NSOperationQueue alloc] init];

    if (motionManager.isAccelerometerAvailable && accelerometerCallback != nil) {
        [motionManager startAccelerometerUpdatesToQueue:queue withHandler:^(CMAccelerometerData * _Nullable data, NSError * _Nullable error) {
            if (data != nil && error == nil) {
                accelerometerCallback(data.timestamp, data.acceleration.x * 9.80665, data.acceleration.y * 9.80665, data.acceleration.z * 9.80665);
            }
        }];
    }

    if (motionManager.isGyroAvailable && gyroscopeCallback != nil) {
        [motionManager startGyroUpdatesToQueue:queue withHandler:^(CMGyroData * _Nullable data, NSError * _Nullable error) {
            if (data != nil && error == nil) {
                gyroscopeCallback(data.timestamp, data.rotationRate.x, data.rotationRate.y, data.rotationRate.z);
            }
        }];
    }
}

void VinsStopImuCapture(void) {
    if (motionManager != nil) {
        [motionManager stopAccelerometerUpdates];
        [motionManager stopGyroUpdates];
        motionManager = nil;
    }
}
