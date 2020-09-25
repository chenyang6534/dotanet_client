//------------------------------------------------------------------------------
// Copyright (c) 2018-2019 Beijing Bytedance Technology Co., Ltd.
// All Right Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//------------------------------------------------------------------------------

#import <BUAdSDK/BURewardedVideoAd.h>
#import <BUAdSDK/BURewardedVideoModel.h>
#import "UnityAppController.h"
#import "BUToUnityAdManager.h"

extern const char* AutonomousStringCopy(const char* string);

const char* AutonomousStringCopy(const char* string)
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
typedef void(*ExpressRewardVideoAd_OnRewardVerify)(
    bool rewardVerify, int rewardAmount, const char* rewardName, int context);

// The BURewardedVideoAdDelegate implement.
@interface RewardVideoAd : NSObject
@end

@interface RewardVideoAd () <BURewardedVideoAdDelegate>
@property (nonatomic, strong) BURewardedVideoAd *rewardedVideoAd;

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
@property (nonatomic, assign) ExpressRewardVideoAd_OnRewardVerify onRewardVerify;
@end

@implementation RewardVideoAd

+ (RewardVideoAd *)sharedInstance {
    
    static RewardVideoAd *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[self alloc] init];
    });
    return sharedInstance;
}


- (void)rewardedVideoAdDidLoad:(BURewardedVideoAd *)rewardedVideoAd {
    self.onRewardVideoAdLoad((__bridge void*)self, self.loadContext);
}

- (void)rewardedVideoAdVideoDidLoad:(BURewardedVideoAd *)rewardedVideoAd {
    self.onRewardVideoCached(self.loadContext);
}

- (void)rewardedVideoAdWillVisible:(BURewardedVideoAd *)rewardedVideoAd {
    self.onAdShow(self.interactionContext);
}

- (void)rewardedVideoAdDidClose:(BURewardedVideoAd *)rewardedVideoAd {
    self.onAdClose(self.interactionContext);
}

- (void)rewardedVideoAdDidClick:(BURewardedVideoAd *)rewardedVideoAd {
    self.onAdVideoBarClick(self.interactionContext);
}

- (void)rewardedVideoAd:(BURewardedVideoAd *)rewardedVideoAd didFailWithError:(NSError *)error {
    self.onError((int)error.code, AutonomousStringCopy([[error localizedDescription] UTF8String]), self.loadContext);
}

- (void)rewardedVideoAdDidPlayFinish:(BURewardedVideoAd *)rewardedVideoAd didFailWithError:(NSError *)error {
    self.onVideoComplete(self.interactionContext);
}

- (void)rewardedVideoAdServerRewardDidFail:(BURewardedVideoAd *)rewardedVideoAd {
}

- (void)rewardedVideoAdServerRewardDidSucceed:(BURewardedVideoAd *)rewardedVideoAd verify:(BOOL)verify{
    self.onRewardVerify(true, 0, AutonomousStringCopy(""), self.interactionContext);
}
@end

#if defined (__cplusplus)
extern "C" {
#endif

void UnionPlatform_RewardVideoAd_Load(
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
    
    BURewardedVideoAd* rewardedVideoAd = [[BURewardedVideoAd alloc] initWithSlotID:[[NSString alloc] initWithUTF8String:slotID] rewardedVideoModel:model];
    
    RewardVideoAd* instance = [RewardVideoAd sharedInstance]; //[[RewardVideoAd alloc] init];
    instance.rewardedVideoAd = rewardedVideoAd;
    instance.onError = onError;
    instance.onRewardVideoAdLoad = onRewardVideoAdLoad;
    instance.onRewardVideoCached = onRewardVideoCached;
    instance.loadContext = context;
    rewardedVideoAd.delegate = instance;
    [rewardedVideoAd loadAdData];

    // 强持有，是引用加+1
    [[BUToUnityAdManager sharedInstance] addAdManager:instance];
    
    (__bridge_retained void*)instance;
}

void UnionPlatform_RewardVideoAd_SetInteractionListener(
    void* rewardedVideoAdPtr,
    RewardVideoAd_OnAdShow onAdShow,
    RewardVideoAd_OnAdVideoBarClick onAdVideoBarClick,
    RewardVideoAd_OnAdClose onAdClose,
    RewardVideoAd_OnVideoComplete onVideoComplete,
    RewardVideoAd_OnVideoError onVideoError,
    ExpressRewardVideoAd_OnRewardVerify onRewardVerify,
    int context) {
    RewardVideoAd* rewardedVideoAd = [RewardVideoAd sharedInstance]; //(__bridge RewardVideoAd*)rewardedVideoAdPtr;
    rewardedVideoAd.onAdShow = onAdShow;
    rewardedVideoAd.onAdVideoBarClick = onAdVideoBarClick;
    rewardedVideoAd.onAdClose = onAdClose;
    rewardedVideoAd.onVideoComplete = onVideoComplete;
    rewardedVideoAd.onVideoError = onVideoError;
    rewardedVideoAd.onRewardVerify = onRewardVerify;
    rewardedVideoAd.interactionContext = context;
}

void UnionPlatform_RewardVideoAd_ShowRewardVideoAd(void* rewardedVideoAdPtr) {
    RewardVideoAd* rewardedVideoAd = [RewardVideoAd sharedInstance]; //(__bridge RewardVideoAd*)rewardedVideoAdPtr;
    
    [rewardedVideoAd.rewardedVideoAd showAdFromRootViewController:GetAppController().rootViewController];
}

void UnionPlatform_RewardVideoAd_Dispose(void* rewardedVideoAdPtr) {
    
    RewardVideoAd *rewardedVideoAd = (__bridge_transfer RewardVideoAd*)rewardedVideoAdPtr;
    [[BUToUnityAdManager sharedInstance] deleteAdManager:rewardedVideoAd];
}

#if defined (__cplusplus)
}
#endif
