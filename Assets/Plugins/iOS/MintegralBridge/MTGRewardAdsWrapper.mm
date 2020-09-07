//
//  MTGRewardAdsWrapper.mm
//
//  Copyright © 2016年 Mintegral. All rights reserved.
//


#import "MTGRewardAdsWrapper.h"
#import <MTGSDK/MTGSDK.h>
#import <MTGSDKReward/MTGRewardAdManager.h>
#import "UnityInterface.h"

@interface MTGRewardAdModel : NSObject
@property (nonatomic, copy  ) NSString  *rewardName;
@property (nonatomic, copy  ) NSString  *converted;
@property (nonatomic, assign) NSInteger rewardAmount;
@end

@implementation MTGRewardAdModel
@end


    
@interface MTGRewardAdsWrapper ()<MTGRewardAdLoadDelegate,MTGRewardAdShowDelegate>
@end

@implementation MTGRewardAdsWrapper

/**
 *  Called when the ad is loaded , but not ready to be displayed,need to wait load video
 completely
 *  @param unitId - the unitId string of the Ad that was loaded.
 */
- (void)onAdLoadSuccess:(nullable NSString *)unitId{
    const char * param = [unitId UTF8String];
    UnitySendMessage("MintegralManager","onRewardedVideoLoadSuccess",param);
}
/**
 *  Called when the ad is successfully load , and is ready to be displayed
 *
 *  @param unitId - the unitId string of the Ad that was loaded.
 */
- (void)onVideoAdLoadSuccess:(nullable NSString *)unitId{
    if (!unitId) {
        UnitySendMessage("MintegralManager","onRewardedVideoLoaded","");
    }else{
        const char * param = [unitId UTF8String];
        UnitySendMessage("MintegralManager","onRewardedVideoLoaded",param);
    }
}

/**
 *  Called when there was an error loading the ad.
 *
 *  @param unitId      - the unitId string of the Ad that failed to load.
 *  @param error       - error object that describes the exact error encountered when loading the ad.
 */
- (void)onVideoAdLoadFailed:(nullable NSString *)unitId error:(nonnull NSError *)error{

    if (!error) {
        UnitySendMessage("MintegralManager","onRewardedVideoFailed","");
    }else{
        const char * param = [error.localizedDescription UTF8String];
        UnitySendMessage("MintegralManager","onRewardedVideoFailed",param);
    }
}


/**
 *  Called when the ad display success
 *
 *  @param unitId - the unitId string of the Ad that display success.
 */
- (void)onVideoAdShowSuccess:(nullable NSString *)unitId{
    
    if (!unitId) {
        UnitySendMessage("MintegralManager","onRewardedVideoShown","");
    }else{
        const char * param = [unitId UTF8String];
        UnitySendMessage("MintegralManager","onRewardedVideoShown",param);
    }
}

/**
 *  Called when the ad failed to display for some reason
 *
 *  @param unitId      - the unitId string of the Ad that failed to be displayed.
 *  @param error       - error object that describes the exact error encountered when showing the ad.
 */
- (void)onVideoAdShowFailed:(nullable NSString *)unitId withError:(nonnull NSError *)error{

    if (!error) {
        UnitySendMessage("MintegralManager","onRewardedVideoShownFailed","");
    }else{
        const char * param = [error.localizedDescription UTF8String];
        UnitySendMessage("MintegralManager","onRewardedVideoShownFailed",param);
    }
}

/**
 *  Called when the ad is clicked
 *
 *  @param unitId - the unitId string of the Ad clicked.
 */
- (void)onVideoAdClicked:(nullable NSString *)unitId{
    if (!unitId) {
        UnitySendMessage("MintegralManager","onRewardedVideoClicked","");
    }else{
        const char * param = [unitId UTF8String];
        UnitySendMessage("MintegralManager","onRewardedVideoClicked",param);
    }
}

/**
 *  Called when the ad has been dismissed from being displayed, and control will return to your app
 *
 *  @param unitId      - the unitId string of the Ad that has been dismissed
 *  @param converted   - BOOL describing whether the ad has converted
 *  @param rewardInfo  - the rewardInfo object containing an array of reward objects that should be given to your user.
 */
- (void)onVideoAdDismissed:(nullable NSString *)unitId withConverted:(BOOL)converted withRewardInfo:(nullable MTGRewardAdInfo *)rewardInfo{
 
    MTGRewardAdModel *model = nil;
    model = [[MTGRewardAdModel alloc] init];
    
    model.converted = @"0";
    if (converted && rewardInfo) {
        model.converted = @"1";
        model.rewardName = rewardInfo.rewardName;
        model.rewardAmount = rewardInfo.rewardAmount;
    }

    NSString *rewardInfoStr = [MTGUnityUtility convertJsonFromObject:model withClassName:NSStringFromClass([MTGRewardAdModel class])];
    if (!rewardInfoStr) {
        UnitySendMessage("MintegralManager","onRewardedVideoClosed","");
    }else{
        const char *jsonString = [rewardInfoStr UTF8String];
        UnitySendMessage("MintegralManager","onRewardedVideoClosed",jsonString);
    }
}

/**
 *  Called only when the ad has a video content, and called when the video play completed.
 *  @param unitId - the unitId string of the Ad that video play completed.
 */
- (void) onVideoPlayCompleted:(nullable NSString *)unitId{
    const char * param = [unitId UTF8String];
    UnitySendMessage("MintegralManager","onRewardedVideoPlayCompleted",param);
}

/**
 *  Called only when the ad has a endcard content, and called when the endcard show.
 *  @param unitId - the unitId string of the Ad that endcard show.
 */
- (void) onVideoEndCardShowSuccess:(nullable NSString *)unitId{
    const char * param = [unitId UTF8String];
    UnitySendMessage("MintegralManager","onRewardedVideoEndCardShowSuccess",param);
}

/**
 *  Called when the ad  did closed;
 *
 *  @param unitId - the unitId string of the Ad clicked.
 */
- (void)onVideoAdDidClosed:(nullable NSString *)unitId{
    //No development for the time being
}

@end


static MTGRewardAdsWrapper* delegateObject = nil;

extern "C" {

    void loadRewardView (const char*  unitId);
    bool isReady (const char*  unitId);
    void showRewardView (const char*  unitId, const char*  rewardId,const char*  userId);
    void cleanRewardView ();
    void setAlertDialogWithTitle(const char*  unitId,const char* title,const char* content,const char* confirmText,const char* cancelText);
}



void loadRewardView (const char*  unitId){
    NSString *unitIdStr = [NSString stringWithUTF8String:unitId];
    
    if (delegateObject == nil){
        
        delegateObject = [[MTGRewardAdsWrapper  alloc] init];
    }

    [[MTGRewardAdManager sharedInstance] loadVideo:unitIdStr delegate:delegateObject];
}

bool isReady (const char*  unitId){
    NSString *unitIdStr = [NSString stringWithUTF8String:unitId];
    return [[MTGRewardAdManager sharedInstance] isVideoReadyToPlay:unitIdStr];
}

void showRewardView (const char*  unitId, const char*  rewardId,const char*  userId){
    NSString *unitIdStr = [NSString stringWithUTF8String:unitId];
    NSString *rewardIdStr = [NSString stringWithUTF8String:rewardId];
    NSString *userIdStr = [NSString stringWithUTF8String:userId];

    if (delegateObject == nil){
        
        delegateObject = [[MTGRewardAdsWrapper  alloc] init];
    }

    [[MTGRewardAdManager sharedInstance] showVideo:unitIdStr withRewardId:rewardIdStr userId:userIdStr delegate:delegateObject viewController:UnityGetGLViewController() ];
}

void cleanRewardView (){
    
    [[MTGRewardAdManager sharedInstance] cleanAllVideoFileCache];
}

void setAlertDialogWithTitle(const char*  unitId,const char* title,const char* content,const char* confirmText,const char* cancelText){
    NSString *titleStr = [NSString stringWithUTF8String:title];
    NSString *contentStr = [NSString stringWithUTF8String:content];
    NSString *confirmTextStr = [NSString stringWithUTF8String:confirmText];
    NSString *cancelTextStr = [NSString stringWithUTF8String:cancelText];

    NSString *unitIdStr = [NSString stringWithUTF8String:unitId];
    
    [[MTGRewardAdManager sharedInstance] setAlertWithTitle:titleStr content:contentStr confirmText:confirmTextStr cancelText:cancelTextStr unitId:unitIdStr];
}


