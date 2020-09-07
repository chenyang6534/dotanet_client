//
//  MTGInterActiveWrapper.mm
//
//  Copyright © 2016年 Mintegral . All rights reserved.
//


#import "MTGInterActiveWrapper.h"
#import <MTGSDK/MTGSDK.h>
#import <MTGSDKInterActive/MTGInterActiveManager.h>
#import "UnityInterface.h"


@interface MTGInterActiveWrapper()<MTGInterActiveDelegate>
@end

@implementation MTGInterActiveWrapper


/**
 *  Sent when the ad is successfully load , and is ready to be displayed
 */
- (void) onInterActiveLoadSuccess:(MTGInterActiveResourceType)resourceType adManager:(MTGInterActiveManager *_Nonnull)adManager{
    
    UnitySendMessage("MintegralManager","onInterActiveLoaded","");
}

/**
 *  Called when the material of ad is successfully load , and is ready to be displayed
 */
- (void) onInterActiveMaterialloadSuccess:(MTGInterActiveResourceType)resourceType adManager:(MTGInterActiveManager *_Nonnull)adManager{
    
    UnitySendMessage("MintegralManager","onInterActiveMaterialLoaded","");
}

/**
 *  Sent when there was an error loading the ad.
 *
 *  @param error An NSError object with information about the failure.
 */
- (void) onInterActiveLoadFailed:(nonnull NSError *)error adManager:(MTGInterActiveManager *_Nonnull)adManager{
    
    if (!error) {
        UnitySendMessage("MintegralManager","onInterActiveFailed","");
    }else{
        const char * param = [error.localizedDescription UTF8String];
        UnitySendMessage("MintegralManager","onInterActiveFailed",param);
    }
}

/**
 *  Sent when the Interstitial success to open
 */
- (void) onInterActiveShowSuccess:(MTGInterActiveManager *_Nonnull)adManager{
    
    UnitySendMessage("MintegralManager","onInterActiveShown","");
}

/**
 *  Sent when the Interstitial failed to open for some reason
 *
 *  @param error An NSError object with information about the failure.
 */
- (void) onInterActiveShowFailed:(nonnull NSError *)error adManager:(MTGInterActiveManager *_Nonnull)adManager{
    if (!error) {
        UnitySendMessage("MintegralManager","onInterActiveShownFailed","");
    }else{
        const char * param = [error.localizedDescription UTF8String];
        UnitySendMessage("MintegralManager","onInterActiveShownFailed",param);
    }
}

/**
 *  Sent when the Interstitial has been clesed from being open, and control will return to your app
 */
- (void) onInterActiveAdDismissed:(MTGInterActiveManager *_Nonnull)adManager{

    UnitySendMessage("MintegralManager","onInterActiveDismissed","");
}

/**
 *  Sent after the Interstitial has been clicked by a user.
 */
- (void) onInterActiveAdClick:(MTGInterActiveManager *_Nonnull)adManager{

    UnitySendMessage("MintegralManager","onInterActiveClicked","");
}

/**
 Called when whether the user finished playing the interactive ad or not.
 
 @param adManager the manager used to show the ad.
 @param completeOrNot If YES, user has finished playing otherwise NO.
 @Attention Only got this callback for `MTGInterActiveResourceTypePlayable` ad. Availabled on 4.8.0 MTGInterActiveSDKVersion or later
 */
- (void) onInterActiveAdManager:(MTGInterActiveManager *_Nonnull)adManager playingComplete:(BOOL)completeOrNot{
    
    if(completeOrNot){
        UnitySendMessage("MintegralManager", "onInterActivePlayingComplete", "1");
    }else{
        UnitySendMessage("MintegralManager", "onInterActivePlayingComplete", "0");
    }
}


@end



static MTGInterActiveWrapper* delegateObject = nil;

extern "C" {

    void *initInterActive (const char* unitId);
    void loadInterActive (void *instance);
    void showInterActive (void *instance);
    int getInterActiveStatus (void *instance);
}



void *initInterActive (const char* unitId)
{
    NSString *unitIdStr = [NSString stringWithUTF8String:unitId];

    MTGInterActiveManager *manager = nil;
    manager = [[MTGInterActiveManager alloc] initWithUnitID:unitIdStr withViewController:UnityGetGLViewController()];
 
    if (delegateObject == nil){
        delegateObject = [[MTGInterActiveWrapper alloc] init];
    }
    
    manager.delegate = delegateObject;

    id instance = manager;
    
    return (__bridge_retained void*)instance;
}


void loadInterActive (void *instance){
    
    MTGInterActiveManager *adManager = (__bridge MTGInterActiveManager *)instance;
    [adManager loadAd];
}

void showInterActive (void *instance){
    
    MTGInterActiveManager *adManager = (__bridge MTGInterActiveManager *)instance;
    [adManager showAd];
}

int getInterActiveStatus (void *instance)
{
    
    MTGInterActiveManager *adManager = (__bridge MTGInterActiveManager *)instance;
    MTGInterActiveStatus status = [adManager readyStatus]; 
    
    if(status == MTGInterActiveStatusNoAds){
        return 0;
    }else if(status == MTGInterActiveStatusMaterialLoading){
        return 1;
    }else if(status == MTGInterActiveStatusMaterialCompleted){
        return 2;
    }else{
        return 0;
    }
    
}



