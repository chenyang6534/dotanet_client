//
//  MTGRewardAdsWrapper.mm
//
//  Copyright © 2016年 Mintegral. All rights reserved.
//


#import "MTGBannerWrapper.h"
#import <MTGSDKBanner/MTGBannerAdView.h>
#import <MTGSDKBanner/MTGBannerAdViewDelegate.h>
#import <MTGSDK/MTGSDK.h>
#import "UnityInterface.h"
    
@interface MTGBannerWrapper ()<MTGBannerAdViewDelegate>
@end

@implementation MTGBannerWrapper


- (void)adViewLoadSuccess:(MTGBannerAdView *)adView
{

    
    //This method is called when adView ad slot loaded successfully.
    UnitySendMessage("MintegralManager","onBannerLoadSuccessed","");
}

- (void)adViewLoadFailedWithError:(NSError *)error adView:(MTGBannerAdView *)adView
{
    
    //This method is called when adView ad slot failed to load.
    const char * param = [error.localizedDescription UTF8String];
    UnitySendMessage("MintegralManager","onBannerLoadFailed",param);
}


- (void)adViewWillLogImpression:(MTGBannerAdView *)adView
{
    //This method is called before the impression of an MTGBannerAdView object.
    UnitySendMessage("MintegralManager","onBannerImpression","");
}


- (void)adViewDidClicked:(MTGBannerAdView *)adView
{
    //This method is called when ad is clicked.
    UnitySendMessage("MintegralManager","onBannerClick","");
}

- (void)adViewWillLeaveApplication:(MTGBannerAdView *)adView
{
    //Sent when a user is about to leave your application as a result of tapping.Your application will be moved to the background shortly after this method is called.
    UnitySendMessage("MintegralManager","onBannerLeaveApp","");
}
- (void)adViewWillOpenFullScreen:(MTGBannerAdView *)adView
{
    //Would open the full screen view.Sent when openning storekit or openning the webpage in app.
    UnitySendMessage("MintegralManager","onBannerShowFullScreen","");
}
- (void)adViewCloseFullScreen:(MTGBannerAdView *)adView
{
    //Would close the full screen view.Sent when closing storekit or closing the webpage in app.
    UnitySendMessage("MintegralManager","onBannerCloseFullScreen","");
}

- (void)adViewClosed:(MTGBannerAdView *)adView
{
    UnitySendMessage("MintegralManager","onCloseBanner","");
}

- (void)adjustAdViewFrameToShowAdView:(MTGBannerAdView *)adView bannerPosition:(int)bannerPosition
{
    if (@available(iOS 11.0, *)) {
        UIView* superview = adView.superview;
        if (superview) {
            adView.translatesAutoresizingMaskIntoConstraints = NO;
            NSMutableArray<NSLayoutConstraint*>* constraints = [NSMutableArray arrayWithArray:@[
                [adView.widthAnchor constraintEqualToConstant:CGRectGetWidth(adView.frame)],
                [adView.heightAnchor constraintEqualToConstant:CGRectGetHeight(adView.frame)],
            ]];
            switch(bannerPosition) {
                case 0:
                    [constraints addObjectsFromArray:@[[adView.topAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.topAnchor],
                                                       [adView.leftAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.leftAnchor]]];
                    break;
                case 1:
                    [constraints addObjectsFromArray:@[[adView.topAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.topAnchor],
                                                       [adView.centerXAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.centerXAnchor]]];
                    break;
                case 2:
                    [constraints addObjectsFromArray:@[[adView.topAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.topAnchor],
                                                       [adView.rightAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.rightAnchor]]];
                    break;
                case 3:
                    [constraints addObjectsFromArray:@[[adView.centerXAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.centerXAnchor],
                                                       [adView.centerYAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.centerYAnchor]]];
                    break;
                case 4:
                    [constraints addObjectsFromArray:@[[adView.bottomAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.bottomAnchor],
                                                       [adView.leftAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.leftAnchor]]];
                    break;
                case 5:
                    [constraints addObjectsFromArray:@[[adView.bottomAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.bottomAnchor],
                                                       [adView.centerXAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.centerXAnchor]]];
                    break;
                case 6:
                    [constraints addObjectsFromArray:@[[adView.bottomAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.bottomAnchor],
                                                       [adView.rightAnchor constraintEqualToAnchor:superview.safeAreaLayoutGuide.rightAnchor]]];
                    break;
            }
            [NSLayoutConstraint activateConstraints:constraints];
            NSLog(@"setting adView frame: %@", NSStringFromCGRect(adView.frame));
        } else
            NSLog(@"adView.superview was nil! Was the ad view not added to another view?@");
    } else {
        // fetch screen dimensions and useful values
        CGRect origFrame = adView.frame;

        CGFloat screenHeight = [UIScreen mainScreen].bounds.size.height;
        CGFloat screenWidth = [UIScreen mainScreen].bounds.size.width;

        switch(bannerPosition) {
            case 0:
                origFrame.origin.x = 0;
                origFrame.origin.y = 0;
                adView.autoresizingMask = (UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleBottomMargin);
                break;
            case 1:
                origFrame.origin.x = (screenWidth / 2) - (origFrame.size.width / 2);
                adView.autoresizingMask = (UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleBottomMargin);
                break;
            case 2:
                origFrame.origin.x = screenWidth - origFrame.size.width;
                origFrame.origin.y = 0;
                adView.autoresizingMask = (UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleBottomMargin);
                break;
            case 3:
                origFrame.origin.x = (screenWidth / 2) - (origFrame.size.width / 2);
                origFrame.origin.y = (screenHeight / 2) - (origFrame.size.height / 2);
                adView.autoresizingMask = (UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleTopMargin | UIViewAutoresizingFlexibleBottomMargin);
                break;
            case 4:
                origFrame.origin.x = 0;
                origFrame.origin.y = screenHeight - origFrame.size.height;
                adView.autoresizingMask = (UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleTopMargin);
                break;
            case 5:
                origFrame.origin.x = (screenWidth / 2) - (origFrame.size.width / 2);
                origFrame.origin.y = screenHeight - origFrame.size.height;
                adView.autoresizingMask = (UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleTopMargin);
                break;
            case 6:
                origFrame.origin.x = screenWidth - adView.frame.size.width;
                origFrame.origin.y = screenHeight - origFrame.size.height;
                adView.autoresizingMask = (UIViewAutoresizingFlexibleLeftMargin | UIViewAutoresizingFlexibleTopMargin);
                break;
        }

        adView.frame = origFrame;
        NSLog(@"setting adView frame: %@", NSStringFromCGRect(origFrame));
    }
}

+ (UIViewController*)unityViewController
{
    return [[[UIApplication sharedApplication] keyWindow] rootViewController];
}



@end


static MTGBannerWrapper* delegateObject = nil;

extern "C" {

    void *createMTGBanner (const char*  unitId,int position,int adWidth, int adHeight, bool isShowCloseBtn);
    void destroyMTGBanner (void *instance);
}



void *createMTGBanner (const char*  unitId,int position,int adWidth, int adHeight, bool isShowCloseBtn){
    NSLog(@"createMTGBanner");
    NSString *unitIdStr = [NSString stringWithUTF8String:unitId];
    MTGBannerAdView *bannerAdView = nil;
    
    if (delegateObject == nil){
        
        delegateObject = [[MTGBannerWrapper  alloc] init];
    }
    bannerAdView = [[MTGBannerAdView alloc] initBannerAdViewWithBannerSizeType:MTGSmartBannerType unitId:unitIdStr rootViewController:nil];
    bannerAdView.delegate = delegateObject;
    
    if(isShowCloseBtn){
        bannerAdView.showCloseButton = MTGBoolYes;
    }else{
        bannerAdView.showCloseButton = MTGBoolNo;
    }
    
    [[MTGBannerWrapper unityViewController].view addSubview:bannerAdView];
    [bannerAdView loadBannerAd];

    bannerAdView.frame = CGRectMake(0, 0, adWidth, adHeight);

    [delegateObject adjustAdViewFrameToShowAdView:bannerAdView bannerPosition:position];

    id instance = bannerAdView;
    return (__bridge_retained void*)instance;
}


void destroyMTGBanner (void *instance){
    NSLog(@"destroyMTGBanner");
    MTGBannerAdView *bannerAdView = (__bridge MTGBannerAdView *)instance;
    [bannerAdView removeFromSuperview];
    [bannerAdView destroyBannerAdView];

    bannerAdView.delegate = nil;
    bannerAdView = nil;
}

