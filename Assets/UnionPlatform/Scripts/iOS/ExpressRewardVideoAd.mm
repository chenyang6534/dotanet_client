//------------------------------------------------------------------------------
// Copyright (c) 2018-2019 Beijing Bytedance Technology Co., Ltd.
// All Right Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//------------------------------------------------------------------------------

#import <BUAdSDK/BUAdSDK.h>
#import "UnityAppController.h"
#import "BUToUnityAdManager.h"

static const char* AutonomousStringCopy(const char* string);

static const char* AutonomousStringCopy(const char* string)
{
    if (string == NULL) {
        return NULL;
    }
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

// IRewardVideoAdListener callbacks.
typedef void(*RewardVideoAd_OnError)(int code, const char* message, int context);
typedef void(*RewardVideoAd_OnRewardVideoAdLoad)(void* rewardVideoAd, int context);
typedef void(*RewardVideoAd_OnRewardVideoCached)(int context);

// IRewardAdInteractionListener callbacks.
typedef void(*RewardVideoAd_OnAdShow)(int context);
typedef void(*RewardVideoAd_OnAdVideoBarClick)(int context);
typedef void(*RewardVideoAd_OnAdClose)(int context);
typedef void(*RewardVideoAd_OnVideoComplete)(int context);
typedef void(*RewardVideoAd_OnVideoError)(int context);
typedef void(*RewardVideoAd_OnRewardVerify)(
    bool rewardVerify, int rewardAmount, const char* rewardName, int context);

// The BURewardedVideoAdDelegate implement.
@interface ExpressRewardVideoAd : NSObject
@end

@interface ExpressRewardVideoAd () <BUNativeExpressRewardedVideoAdDelegate>
@property (nonatomic, strong) BUNativeExpressRewardedVideoAd *expressRewardedVideoAd;

@property (nonatomic, assign) int loadContext;
@property (nonatomic, assign) RewardVideoAd_OnError onError;
@property (nonatomic, assign) RewardVideoAd_OnRewardVideoAdLoad onRewardVideoAdLoad;
@property (nonatomic, assign) RewardVideoAd_OnRewardVideoCached onRewardVideoCached;

@property (nonatomic, assign) int interactionContext;
@property (nonatomic, assign) RewardVideoAd_OnAdShow onAdShow;
@property (nonatomic, assign) RewardVideoAd_OnAdVideoBarClick onAdVideoBarClick;
@property (nonatomic, assign) RewardVideoAd_OnAdClose onAdClose;
@property (nonatomic, assign) RewardVideoAd_OnVideoComplete onVideoComplete;
@property (nonatomic, assign) RewardVideoAd_OnVideoError onVideoError;
@property (nonatomic, assign) RewardVideoAd_OnRewardVerify onRewardVerify;
@end

@implementation ExpressRewardVideoAd

+ (ExpressRewardVideoAd *)sharedInstance {
    
    static ExpressRewardVideoAd *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[self alloc] init];
    });
    return sharedInstance;
}


#pragma mark - BUNativeExpressRewardedVideoAdDelegate
- (void)nativeExpressRewardedVideoAdDidLoad:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
    
    // 渲染成功才算成功,可以展示
    self.onRewardVideoAdLoad((__bridge void*)self, self.loadContext);
}

- (void)nativeExpressRewardedVideoAd:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd didFailWithError:(NSError *_Nullable)error {
    self.onError((int)error.code, AutonomousStringCopy([[error localizedDescription] UTF8String]), self.loadContext);
}

- (void)nativeExpressRewardedVideoAdDidDownLoadVideo:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
    self.onRewardVideoCached(self.loadContext);
}

- (void)nativeExpressRewardedVideoAdViewRenderSuccess:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
    
}

- (void)nativeExpressRewardedVideoAdViewRenderFail:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd error:(NSError *_Nullable)error {
    self.onError((int)error.code, AutonomousStringCopy([[error localizedDescription] UTF8String]), self.loadContext);
}

- (void)nativeExpressRewardedVideoAdWillVisible:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
    self.onAdShow(self.interactionContext);
}

- (void)nativeExpressRewardedVideoAdDidVisible:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
}

- (void)nativeExpressRewardedVideoAdWillClose:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
}

- (void)nativeExpressRewardedVideoAdDidClose:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
    self.onAdClose(self.interactionContext);
}

- (void)nativeExpressRewardedVideoAdDidClick:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
    self.onAdVideoBarClick(self.interactionContext);
}

- (void)nativeExpressRewardedVideoAdDidClickSkip:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
}

- (void)nativeExpressRewardedVideoAdDidPlayFinish:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd didFailWithError:(NSError *_Nullable)error {
    if (error) {
        self.onVideoError(self.interactionContext);
    } else {
        self.onVideoComplete(self.interactionContext);
    }
}

- (void)nativeExpressRewardedVideoAdServerRewardDidSucceed:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd verify:(BOOL)verify {
    self.onRewardVerify(true, 0, AutonomousStringCopy(""), self.interactionContext);
}

- (void)nativeExpressRewardedVideoAdServerRewardDidFail:(BUNativeExpressRewardedVideoAd *)rewardedVideoAd {
    self.onRewardVerify(false, 0, AutonomousStringCopy(""), self.interactionContext);
}



@end

#if defined (__cplusplus)
extern "C" {
#endif

void UnionPlatform_ExpressRewardVideoAd_Load(
    const char* slotID,
    const char* userID,
                                             const char* rewardName,
                                             int rewardAmount,
                                             const char* mediaExtxa,
    RewardVideoAd_OnError onError,
    RewardVideoAd_OnRewardVideoAdLoad onRewardVideoAdLoad,
    RewardVideoAd_OnRewardVideoCached onRewardVideoCached,
    int context) {
    
    BURewardedVideoModel *model = [[BURewardedVideoModel alloc] init];
    
    NSString *userIDStr = [[NSString alloc] initWithUTF8String:userID];
    if (userIDStr.length > 0) {
        model.userId = userIDStr;
    }
    
    NSString *rewardNameStr = [[NSString alloc] initWithUTF8String:rewardName];
    if (rewardNameStr.length > 0) {
        model.rewardName = rewardNameStr;
    }
    
    NSString *extraStr = [[NSString alloc] initWithUTF8String:mediaExtxa];
    if (extraStr.length > 0) {
        model.extra = extraStr;
    }
    
    if (rewardAmount > 0) {
        model.rewardAmount = (NSInteger)rewardAmount;
    }
    
    BUNativeExpressRewardedVideoAd* expressRewardedVideoAd = [[BUNativeExpressRewardedVideoAd alloc] initWithSlotID:[[NSString alloc] initWithUTF8String:slotID] rewardedVideoModel:model];
    ExpressRewardVideoAd* instance = [ExpressRewardVideoAd sharedInstance];//[[ExpressRewardVideoAd alloc] init];
    instance.expressRewardedVideoAd = expressRewardedVideoAd;
    instance.onError = onError;
    instance.onRewardVideoAdLoad = onRewardVideoAdLoad;
    instance.onRewardVideoCached = onRewardVideoCached;
    instance.loadContext = context;
    expressRewardedVideoAd.delegate = instance;
    [expressRewardedVideoAd loadAdData];
    
    // 强持有，是引用加+1
    [[BUToUnityAdManager sharedInstance] addAdManager:instance];

    (__bridge_retained void*)instance;
}

void UnionPlatform_ExpressRewardVideoAd_SetInteractionListener(
    void* rewardedVideoAdPtr,
    RewardVideoAd_OnAdShow onAdShow,
    RewardVideoAd_OnAdVideoBarClick onAdVideoBarClick,
    RewardVideoAd_OnAdClose onAdClose,
    RewardVideoAd_OnVideoComplete onVideoComplete,
    RewardVideoAd_OnVideoError onVideoError,
    RewardVideoAd_OnRewardVerify onRewardVerify,
    int context) {
    ExpressRewardVideoAd* rewardedVideoAd = [ExpressRewardVideoAd sharedInstance]; //(__bridge ExpressRewardVideoAd*)rewardedVideoAdPtr;
    rewardedVideoAd.onAdShow = onAdShow;
    rewardedVideoAd.onAdVideoBarClick = onAdVideoBarClick;
    rewardedVideoAd.onAdClose = onAdClose;
    rewardedVideoAd.onVideoComplete = onVideoComplete;
    rewardedVideoAd.onVideoError = onVideoError;
    rewardedVideoAd.onRewardVerify = onRewardVerify;
    rewardedVideoAd.interactionContext = context;
}

void UnionPlatform_ExpressRewardVideoAd_ShowRewardVideoAd(void* rewardedVideoAdPtr) {
    ExpressRewardVideoAd* rewardedVideoAd = [ExpressRewardVideoAd sharedInstance]; //(__bridge ExpressRewardVideoAd*)rewardedVideoAdPtr;
    [rewardedVideoAd.expressRewardedVideoAd showAdFromRootViewController:GetAppController().rootViewController];
}

void UnionPlatform_ExpressRewardVideoAd_Dispose(void* rewardedVideoAdPtr) {
    
    ExpressRewardVideoAd *rewardedVideoAd = (__bridge_transfer ExpressRewardVideoAd*)rewardedVideoAdPtr;
    [[BUToUnityAdManager sharedInstance] deleteAdManager:rewardedVideoAd];
}

#if defined (__cplusplus)
}
#endif
