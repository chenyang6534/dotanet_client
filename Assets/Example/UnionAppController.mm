//------------------------------------------------------------------------------
// Copyright (c) 2018-2019 Beijing Bytedance Technology Co., Ltd.
// All Right Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//------------------------------------------------------------------------------

#import "UnityAppController.h"
#import <BUAdSDK/BUAdSDKManager.h>
#import <BUAdSDK/BUSplashAdView.h>

static NSString * const MopubADUnitID = @"e1cbce0838a142ec9bc2ee48123fd470";

@interface UnionAppController : UnityAppController <BUSplashAdDelegate>
@property (nonatomic, assign) CFTimeInterval startTime;
@end

IMPL_APP_CONTROLLER_SUBCLASS (UnionAppController)

@implementation UnionAppController

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    // Override point for customization after application launch.
    
    [super application:application didFinishLaunchingWithOptions:launchOptions];

    [BUAdSDKManager setAppID: @"5000546"];
    [BUAdSDKManager setIsPaidApp:NO];
#if DEBUG
    //是否打开log信息，默认没有
    [BUAdSDKManager setLoglevel:BUAdSDKLogLevelDebug];
#endif

    return YES;
}

@end
