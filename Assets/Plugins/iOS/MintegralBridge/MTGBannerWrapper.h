//
//  MTGRewardAdsWrapper.h
//
//  Copyright © 2016年 Mobvista. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "MTGUnityUtility.h"
#import <MTGSDKBanner/MTGBannerAdView.h>
#import <MTGSDKBanner/MTGBannerAdViewDelegate.h>
#import <MTGSDK/MTGSDK.h>

@interface MTGBannerWrapper : NSObject

- (void)adjustAdViewFrameToShowAdView:(MTGBannerAdView *)adView bannerPosition:(int)bannerPosition;
+ (UIViewController*)unityViewController;

@end
