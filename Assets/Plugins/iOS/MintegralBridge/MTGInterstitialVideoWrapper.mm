//
//  MTGInterstitialVideoWrapper.mm
// 
//  Copyright © 2016年 Mintegral. All rights reserved.
//


#import "MTGInterstitialVideoWrapper.h"
#import <MTGSDK/MTGSDK.h>
#import <MTGSDKInterstitialVideo/MTGInterstitialVideoAdManager.h>
#import <MTGSDKInterstitialVideo/MTGInterstitialVideoAd.h>

#import "UnityInterface.h"
#import "MTGUnityUtility.h"

@interface MTGAdPlayVideo : NSObject
@property (nonatomic, copy  ) NSString  *isComplete;
@property (nonatomic, assign) NSString *rewardAlertStatus;
@end

@implementation MTGAdPlayVideo
@end

@interface MTGInterstitialVideoWrapper()<MTGInterstitialVideoDelegate>
@end

@implementation MTGInterstitialVideoWrapper

/**
 *  Called when the ad is loaded , but not ready to be displayed,need to wait load video
 completely
 */
- (void) onInterstitialAdLoadSuccess:(MTGInterstitialVideoAdManager *_Nonnull)adManager
{
    UnitySendMessage("MintegralManager","onInterstitialVideoLoadSuccess","");
}
/**
 *  Sent when the ad is successfully load , and is ready to be displayed
 */
- (void)onInterstitialVideoLoadSuccess:(MTGInterstitialVideoAdManager *_Nonnull)adManager
{
    
    UnitySendMessage("MintegralManager","onInterstitialVideoLoaded","");
}

/**
 *  Sent when there was an error loading the ad.
 *
 *  @param error An NSError object with information about the failure.
 */
- (void)onInterstitialVideoLoadFail:(nonnull NSError *)error adManager:(MTGInterstitialVideoAdManager *_Nonnull)adManager
{
    
    if (!error) {
        UnitySendMessage("MintegralManager","onInterstitialVideoFailed","");
    }else{
        const char * param = [error.localizedDescription UTF8String];
        UnitySendMessage("MintegralManager","onInterstitialVideoFailed",param);
    }
}

/**
 *  Sent when the Interstitial success to open
 */
- (void)onInterstitialVideoShowSuccess:(MTGInterstitialVideoAdManager *_Nonnull)adManager
{
    
    UnitySendMessage("MintegralManager","onInterstitialVideoShown","");
}

/**
 *  Sent when the Interstitial failed to open for some reason
 *
 *  @param error An NSError object with information about the failure.
 */
- (void)onInterstitialVideoShowFail:(nonnull NSError *)error adManager:(MTGInterstitialVideoAdManager *_Nonnull)adManager
{
    
    if (!error) {
        UnitySendMessage("MintegralManager","onInterstitialVideoShownFailed","");
    }else{
        const char * param = [error.localizedDescription UTF8String];
        UnitySendMessage("MintegralManager","onInterstitialVideoShownFailed",param);
    }
}

/**
 *  Sent when the Interstitial has been clesed from being open, and control will return to your app
 */
- (void)onInterstitialVideoAdDismissedWithConverted:(BOOL)converted adManager:(MTGInterstitialVideoAdManager *_Nonnull)adManager
{

    UnitySendMessage("MintegralManager","onInterstitialVideoDismissed","");
}

/**
 *  Sent after the Interstitial has been clicked by a user.
 */
- (void)onInterstitialVideoAdClick:(MTGInterstitialVideoAdManager *_Nonnull)adManager{

    UnitySendMessage("MintegralManager","onInterstitialVideoClicked","");
}

/**
 *  Called only when the ad has a video content, and called when the video play completed
 */
- (void) onInterstitialVideoPlayCompleted:(MTGInterstitialVideoAdManager *_Nonnull)adManager{
    UnitySendMessage("MintegralManager","onInterstitialVideoPlayCompleted","");
}

/**
 *  Called only when the ad has a endcard content, and called when the endcard show
 */
- (void) onInterstitialVideoEndCardShowSuccess:(MTGInterstitialVideoAdManager *_Nonnull)adManager{
    UnitySendMessage("MintegralManager","onInterstitialVideoEndCardShowSuccess","");
}

/**
 *  Called when the ad  did closed;
 *
 *  @param adManager - the unitId string of the Ad clicked.
 */
- (void) onInterstitialVideoAdDidClosed:(MTGInterstitialVideoAdManager *_Nonnull)adManager{
    //No development for the time being
}

/**
    *  If iv reward is set, you will receive this callback
    *  @param achieved  Whether the video played to required rate
    * @param alertWindowStatus MTGIVAlertWindowStatus
    */

- (void)onInterstitialVideoAdPlayVideo:(BOOL)achieved alertWindowStatus:(MTGIVAlertWindowStatus)alertWindowStatus adManager:(MTGInterstitialVideoAdManager *_Nonnull)adManager{
    MTGAdPlayVideo *model = nil;
    model = [[MTGAdPlayVideo alloc] init];
    
    model.isComplete = @"0";
    if (achieved) {
        model.isComplete = @"1";
    }

    if(alertWindowStatus == MTGIVAlertNotShown){
        model.rewardAlertStatus = @"1";
    }else if (alertWindowStatus == MTGIVAlertChooseContinue){
        model.rewardAlertStatus = @"2";
    }else if (alertWindowStatus == MTGIVAlertChooseCancel){
        model.rewardAlertStatus = @"3";
    }
    

    NSString *rewardInfoStr = [MTGUnityUtility convertJsonFromObject:model withClassName:NSStringFromClass([MTGAdPlayVideo class])];
    if (!rewardInfoStr) {
        UnitySendMessage("MintegralManager","onInterstitialVideoAdPlayVideo","");
    }else{
        const char *jsonString = [rewardInfoStr UTF8String];
        UnitySendMessage("MintegralManager","onInterstitialVideoAdPlayVideo",jsonString);
    }

    
}


@end



static MTGInterstitialVideoWrapper* delegateObject = nil;

extern "C" {

    void *initInterstitialVideo (const char* unitId);
    void preloadInterstitialVideo (void *instance);
    void showInterstitialVideo (void *instance);
    bool isIVReady(void *instance,const char* unitId);
    void setIVRewardModeTime(void *instance,int ivRewardMode,int playTime);
    void setIVRewardModeRate(void *instance,int ivRewardMode,float playRate);
    void setRewardAlertDialogText(void *instance,const char* title,const char* content,const char* confirmText,const char* cancelText);

}



void *initInterstitialVideo (const char* unitId)
{
    NSString *unitIdStr = [NSString stringWithUTF8String:unitId];
    MTGInterstitialVideoAdManager *manager = nil;
    
    if (delegateObject == nil){
        delegateObject = [[MTGInterstitialVideoWrapper  alloc] init];
    }
    
    manager = [[MTGInterstitialVideoAdManager alloc] initWithUnitID:unitIdStr delegate:delegateObject];

    id instance = manager;
    return (__bridge_retained void*)instance;
    
}


void preloadInterstitialVideo (void *instance){
    
    MTGInterstitialVideoAdManager *adManager = (__bridge MTGInterstitialVideoAdManager *)instance;
    [adManager loadAd];
}

void showInterstitialVideo (void *instance){
    
    MTGInterstitialVideoAdManager *adManager = (__bridge MTGInterstitialVideoAdManager *)instance;
    [adManager showFromViewController:UnityGetGLViewController() ];

}

bool isIVReady (void *instance,const char* unitId){
     NSString *unitIdStr = [NSString stringWithUTF8String:unitId];
    MTGInterstitialVideoAdManager *adManager = (__bridge MTGInterstitialVideoAdManager *)instance;
    return [adManager isVideoReadyToPlay:unitIdStr];

}

void setIVRewardModeTime(void *instance,int ivRewardMode,int time){
    MTGInterstitialVideoAdManager *adManager = (__bridge MTGInterstitialVideoAdManager *)instance;
    if(ivRewardMode == 0){
        [adManager setIVRewardMode:MTGIVRewardPlayMode playTime:time];
    }else {
        [adManager setIVRewardMode:MTGIVRewardCloseMode playTime:time];
    }
}

void setIVRewardModeRate(void *instance,int ivRewardMode,float rate){
    MTGInterstitialVideoAdManager *adManager = (__bridge MTGInterstitialVideoAdManager *)instance;
    if(ivRewardMode == 0){
        [adManager setIVRewardMode:MTGIVRewardPlayMode playRate:rate];
    }else {
        [adManager setIVRewardMode:MTGIVRewardCloseMode playRate:rate];
    }
}

void setRewardAlertDialogText(void *instance,const char* title,const char* content,const char* confirmText,const char* cancelText){
    MTGInterstitialVideoAdManager *adManager = (__bridge MTGInterstitialVideoAdManager *)instance;

    NSString *titleStr = [NSString stringWithUTF8String:title];
    NSString *contentStr = [NSString stringWithUTF8String:content];
    NSString *confirmTextStr = [NSString stringWithUTF8String:confirmText];
    NSString *cancelTextStr = [NSString stringWithUTF8String:cancelText];
    [adManager setAlertWithTitle:titleStr content:contentStr confirmText:confirmTextStr cancelText:cancelTextStr];
}



