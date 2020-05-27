//
//  MTGInterstitialWrapper.mm 
//
//  Copyright © 2016年 Mobvista. All rights reserved.
//


#import "MTGInterstitialWrapper.h"
#import <MTGSDK/MTGSDK.h>
#import <MTGSDKInterstitial/MTGInterstitialAdManager.h>
#import "UnityInterface.h"


@interface MTGInterstitialWrapper()<MTGInterstitialAdLoadDelegate,MTGInterstitialAdShowDelegate>
@end

@implementation MTGInterstitialWrapper




/**
 *  Sent when the ad is successfully load , and is ready to be displayed
 */
- (void) onInterstitialLoadSuccess:(MTGInterstitialAdManager *)adManager{
    
    UnitySendMessage("MintegralManager","onInterstitialLoaded","");
}

/**
 *  Sent when there was an error loading the ad.
 *
 *  @param error An NSError object with information about the failure.
 */
- (void) onInterstitialLoadFail:(nonnull NSError *)error adManager:(MTGInterstitialAdManager * _Nonnull)adManager{
    
    if (!error) {
        UnitySendMessage("MintegralManager","onInterstitialFailed","");
    }else{
        const char * param = [error.localizedDescription UTF8String];
        UnitySendMessage("MintegralManager","onInterstitialFailed",param);
    }
}

/**
 *  Sent when the Interstitial success to open
 */
- (void) onInterstitialShowSuccess:(MTGInterstitialAdManager *)adManager{
    
    UnitySendMessage("MintegralManager","onInterstitialShown","");
}

/**
 *  Sent when the Interstitial failed to open for some reason
 *
 *  @param error An NSError object with information about the failure.
 */
- (void) onInterstitialShowFail:(nonnull NSError *)error adManager:(MTGInterstitialAdManager * _Nonnull)adManager{
    if (!error) {
        UnitySendMessage("MintegralManager","onInterstitialShownFailed","");
    }else{
        const char * param = [error.localizedDescription UTF8String];
        UnitySendMessage("MintegralManager","onInterstitialShownFailed",param);
    }
}

/**
 *  Sent when the Interstitial has been clesed from being open, and control will return to your app
 */
- (void) onInterstitialClosed:(MTGInterstitialAdManager *)adManager{

    UnitySendMessage("MintegralManager","onInterstitialDismissed","");
}

/**
 *  Sent after the Interstitial has been clicked by a user.
 */
- (void) onInterstitialAdClick:(MTGInterstitialAdManager *)adManager{

    UnitySendMessage("MintegralManager","onInterstitialClicked","");
}


@end



static MTGInterstitialWrapper* delegateObject = nil;

extern "C" {

    void *initInterstitial (const char* unitId,const char* adCategory);
    void preloadInterstitial (void *instance);
    void showInterstitial (void *instance);

}



void *initInterstitial (const char* unitId,const char* adCategory)
{
    NSString *unitIdStr = [NSString stringWithUTF8String:unitId];
    NSString *adCategoryStr = [NSString stringWithUTF8String:adCategory];
    MTGInterstitialAdCategory category = (MTGInterstitialAdCategory)[adCategoryStr integerValue];

    MTGInterstitialAdManager *manager = nil;
    manager = [[MTGInterstitialAdManager alloc] initWithUnitID:unitIdStr adCategory:category];

    
    if (delegateObject == nil){
        
        delegateObject = [[MTGInterstitialWrapper  alloc] init];
    }

    id instance = manager;
    
    return (__bridge_retained void*)instance;
    
}


void preloadInterstitial (void *instance){
    
    MTGInterstitialAdManager *adManager = (__bridge MTGInterstitialAdManager *)instance;
    [adManager loadWithDelegate:delegateObject];
}

void showInterstitial (void *instance){
    
    MTGInterstitialAdManager *adManager = (__bridge MTGInterstitialAdManager *)instance;
    [adManager showWithDelegate:delegateObject presentingViewController:UnityGetGLViewController() ];

}



