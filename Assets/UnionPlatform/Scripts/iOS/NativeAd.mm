//------------------------------------------------------------------------------
// Copyright (c) 2018-2019 Beijing Bytedance Technology Co., Ltd.
// All Right Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//------------------------------------------------------------------------------

#import <BUAdSDK/BUNativeAd.h>
#import <BUAdSDK/BUNativeAdRelatedView.h>
#import "UnityAppController.h"
#import "BUToUnityBundleHelper.h"
#import "BUToUnityAdManager.h"


extern const char* AutonomousStringCopy2(const char* string);

const char* AutonomousStringCopy2(const char* string)
{
    if (string == NULL) {
        return NULL;
    }
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

// IRewardVideoAdListener callbacks.
typedef void(*NativeAd_OnError)(int code, const char* message, int context);
typedef void(*NativeAd_OnNativeAdLoad)(void* nativeAd, int context, int slotType);

typedef void(*NativeAd_OnAdShow)(int context);
typedef void(*NativeAd_OnAdDidClick)(int context);
typedef void(*NativeAd_OnAdClose)(int context);

// The BURewardedVideoAdDelegate implement.
@interface NativeAd : NSObject
@end

@interface NativeAd () <BUNativeAdDelegate, BUVideoAdViewDelegate>
@property (nonatomic, strong) BUNativeAd *nativeAd;

@property (nonatomic, assign) int loadContext;
@property (nonatomic, assign) NativeAd_OnError onError;
@property (nonatomic, assign) NativeAd_OnNativeAdLoad onNativeAdLoad;

@property (nonatomic, assign) int interactionContext;
@property (nonatomic, assign) NativeAd_OnAdShow onAdShow;
@property (nonatomic, assign) NativeAd_OnAdDidClick onAdDidClick;
@property (nonatomic, assign) NativeAd_OnAdClose onAdClose;

@property (nonatomic, strong) UIView *customview;
@property (nonatomic, strong) UILabel *infoLabel;
@property (nonatomic, strong) UILabel *titleLabel;
@property (nonatomic, strong) UIImageView *imageView;
@property (nonatomic, strong) UIButton *actionButton;
@property (nonatomic, strong) UIButton *closeButton;
@property (nonatomic, strong) UILabel *adLabel;
@property (nonatomic, strong) BUNativeAdRelatedView *relatedView;
@property (nonatomic, assign) BUAdSlotAdType adType;

@property (nonatomic, assign) BOOL isNormalView;

@end

@implementation NativeAd

- (void)dealloc {
    _relatedView = nil;
    [_customview removeFromSuperview];
    _customview = nil;
}

+ (NativeAd *)sharedInstance {
    static NativeAd *manager = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        if(!manager) {
            manager = [[self alloc] init];
        }
    });
    return manager;
}

- (void)buildupViewFeed {
    
    CGFloat cusViewWidth = 290;
    CGFloat leftMargin = cusViewWidth/20;
    _relatedView = [[BUNativeAdRelatedView alloc] init];
    // Custom view test
    _customview = [[UIView alloc] initWithFrame:CGRectMake(5, 5, cusViewWidth, cusViewWidth)];
    _customview.backgroundColor = [UIColor grayColor];
    _customview.frame = CGRectMake(5, 5, cusViewWidth, cusViewWidth);
//    if(self.isNormalView) {
//        _customview.frame = CGRectMake(0, 200, self.view.frame.size.width,300);
//    }
    
    CGFloat swidth = CGRectGetWidth(_customview.frame);
    
    _infoLabel = [[UILabel alloc] initWithFrame:CGRectMake(leftMargin, leftMargin, swidth - leftMargin * 2, 30)];
    _infoLabel.backgroundColor = [UIColor magentaColor];
    _infoLabel.text = @"info";
    _infoLabel.adjustsFontSizeToFitWidth = YES;
    [_customview addSubview:_infoLabel];
    
    CGFloat buttonWidth = ceilf((swidth-4 * leftMargin)/3);
    _actionButton = [[UIButton alloc] initWithFrame:CGRectMake(CGRectGetMinX(_infoLabel.frame), CGRectGetMaxY(_infoLabel.frame)+5, buttonWidth, 30)];
    [_actionButton setTitle:@"action" forState:UIControlStateNormal];
    _actionButton.userInteractionEnabled = YES;
    _actionButton.backgroundColor = [UIColor orangeColor];
    _actionButton.titleLabel.adjustsFontSizeToFitWidth = YES;
    [_customview addSubview:_actionButton];
    
    _titleLabel = [[UILabel alloc] initWithFrame:CGRectMake(CGRectGetMaxX(_actionButton.frame)+5, CGRectGetMaxY(_infoLabel.frame)+5, 150, 30)];
    _titleLabel.backgroundColor = [UIColor clearColor];
    _titleLabel.text = @"title";
    _titleLabel.adjustsFontSizeToFitWidth = YES;
    [_customview addSubview:_titleLabel];
    
    _imageView = [[UIImageView alloc] init];
    _imageView.userInteractionEnabled = YES;
    _imageView.backgroundColor = [UIColor redColor];
    [_customview addSubview:_imageView];
    
    // add video view
    [_customview addSubview:self.relatedView.videoAdView];
    self.relatedView.videoAdView.delegate = self;
    
    // add logo view
    self.relatedView.logoImageView.frame = CGRectZero;
    [_customview addSubview:self.relatedView.logoImageView];
    // add dislike view
    self.relatedView.dislikeButton.frame = CGRectMake(CGRectGetMaxX(_infoLabel.frame) - 20, CGRectGetMaxY(_infoLabel.frame)+5, 24, 20);
    [_customview addSubview:self.relatedView.dislikeButton];
    // add ad lable
    self.relatedView.adLabel.frame = CGRectZero;
    [_customview addSubview:self.relatedView.adLabel];
}

- (void)buildupViewBanner {
    CGFloat margin = 5;
    CGFloat cusViewWidth = 200 - margin*2;
    CGFloat cusViewHeight = 200 - margin*2;
    CGFloat leftMargin = cusViewWidth/20;
    [_customview removeFromSuperview];
    _customview = nil;
    _relatedView = nil;
    _relatedView = [[BUNativeAdRelatedView alloc] init];
    // Custom view test
    _customview = [[UIView alloc] initWithFrame:CGRectMake(margin, margin+44, cusViewWidth, cusViewHeight)];
    _customview.backgroundColor = [UIColor grayColor];

    CGFloat swidth = CGRectGetWidth(_customview.frame);
    
    _infoLabel = [[UILabel alloc] initWithFrame:CGRectMake(leftMargin, leftMargin, swidth - leftMargin * 2, 30)];
    _infoLabel.backgroundColor = [UIColor magentaColor];
    _infoLabel.text = @"info";
    _infoLabel.adjustsFontSizeToFitWidth = YES;
    [_customview addSubview:_infoLabel];
    
    CGFloat buttonWidth = ceilf((swidth-4 * leftMargin)/3);
    _actionButton = [[UIButton alloc] initWithFrame:CGRectMake(CGRectGetMinX(_infoLabel.frame), CGRectGetMaxY(_infoLabel.frame)+5, buttonWidth, 30)];
    [_actionButton setTitle:@"action" forState:UIControlStateNormal];
    _actionButton.userInteractionEnabled = YES;
    _actionButton.backgroundColor = [UIColor orangeColor];
    _actionButton.titleLabel.adjustsFontSizeToFitWidth = YES;
    [_customview addSubview:_actionButton];
    
    _titleLabel = [[UILabel alloc] initWithFrame:CGRectMake(CGRectGetMaxX(_actionButton.frame)+5, CGRectGetMaxY(_infoLabel.frame)+5, 100, 30)];
    _titleLabel.backgroundColor = [UIColor clearColor];
    _titleLabel.text = @"AdsTitle";
    _titleLabel.adjustsFontSizeToFitWidth = YES;
    [_customview addSubview:_titleLabel];
    
    _imageView = [[UIImageView alloc] init];
    _imageView.userInteractionEnabled = YES;
    _imageView.backgroundColor = [UIColor redColor];
    [_customview addSubview:_imageView];
    
    // add video view
    [_customview addSubview:self.relatedView.videoAdView];
    // add logo view
    self.relatedView.logoImageView.frame = CGRectZero;
    [_customview addSubview:self.relatedView.logoImageView];
    // add dislike view
    self.relatedView.dislikeButton.frame = CGRectMake(CGRectGetMaxX(_infoLabel.frame) - 20, CGRectGetMaxY(_infoLabel.frame)+5, 24, 20);
    [_customview addSubview:self.relatedView.dislikeButton];
    // add ad lable
    self.relatedView.adLabel.frame = CGRectZero;
    [_customview addSubview:self.relatedView.adLabel];
}

- (void)buildupViewInterstial {
    CGFloat margin = 5;
    CGFloat cusViewWidth = 200 - margin*2;
    CGFloat cusViewHeight = 200 - margin*2;
    CGFloat leftMargin = cusViewWidth/20;
    [_customview removeFromSuperview];
    _customview = nil;
    _relatedView = nil;
    _relatedView = [[BUNativeAdRelatedView alloc] init];
    // Custom view test
    _customview = [[UIView alloc] initWithFrame:CGRectMake(margin, margin+44, cusViewWidth, cusViewHeight)];
    _customview.backgroundColor = [UIColor grayColor];
    
    CGFloat swidth = CGRectGetWidth(_customview.frame);
    
    _infoLabel = [[UILabel alloc] initWithFrame:CGRectMake(leftMargin, leftMargin, swidth - leftMargin * 2, 30)];
    _infoLabel.backgroundColor = [UIColor magentaColor];
    _infoLabel.text = @"info";
    _infoLabel.adjustsFontSizeToFitWidth = YES;
    [_customview addSubview:_infoLabel];
    
    CGFloat buttonWidth = ceilf((swidth-4 * leftMargin)/3);
    _actionButton = [[UIButton alloc] initWithFrame:CGRectMake(CGRectGetMinX(_infoLabel.frame), CGRectGetMaxY(_infoLabel.frame)+5, buttonWidth, 30)];
    [_actionButton setTitle:@"action" forState:UIControlStateNormal];
    _actionButton.userInteractionEnabled = YES;
    _actionButton.backgroundColor = [UIColor orangeColor];
    _actionButton.titleLabel.adjustsFontSizeToFitWidth = YES;
    [_customview addSubview:_actionButton];
    
    _titleLabel = [[UILabel alloc] initWithFrame:CGRectMake(CGRectGetMaxX(_actionButton.frame)+5, CGRectGetMaxY(_infoLabel.frame)+5, 100, 30)];
    _titleLabel.backgroundColor = [UIColor clearColor];
    _titleLabel.text = @"AdsTitle";
    _titleLabel.adjustsFontSizeToFitWidth = YES;
    [_customview addSubview:_titleLabel];
    
    _imageView = [[UIImageView alloc] init];
    _imageView.userInteractionEnabled = YES;
    _imageView.backgroundColor = [UIColor redColor];
    [_customview addSubview:_imageView];
    
    // add video view
    [_customview addSubview:self.relatedView.videoAdView];
    // add logo view
    self.relatedView.logoImageView.frame = CGRectZero;
    [_customview addSubview:self.relatedView.logoImageView];
    // add close btn view
    _closeButton = [[UIButton alloc] initWithFrame:CGRectMake(CGRectGetWidth(_customview.frame) - 24, 1, 24, 20)];
    // 插屏关闭按钮需自定义  开发者此处可不用sdk内资源  自定义close关闭按钮
    NSBundle *bundle = [BUToUnityBundleHelper resourceBundle];
    UIImage *img = [UIImage imageNamed:@"bu_close" inBundle:bundle compatibleWithTraitCollection:nil];
    [_closeButton setImage:img forState:UIControlStateNormal];
    [_closeButton addTarget:self action:@selector(InterstitialCloseAd:) forControlEvents:UIControlEventTouchUpInside];
    [_customview addSubview:_closeButton];
//    _closeButton.backgroundColor = [UIColor redColor];
    
    // add ad lable
    self.relatedView.adLabel.frame = CGRectZero;
    [_customview addSubview:self.relatedView.adLabel];
}

- (void)InterstitialCloseAd:(UIButton *)btn {
    
    if (self.onAdClose) {
        self.onAdClose(self.interactionContext);
    }
    
    _relatedView = nil;
    [_customview removeFromSuperview];
    _customview = nil;
}

- (void)refreshNativeAd:(BUNativeAd *)nativeAd {
    self.infoLabel.text = nativeAd.data.AdDescription;
    self.titleLabel.text = nativeAd.data.AdTitle;
    self.imageView.hidden = YES;
    BUMaterialMeta *adMeta = nativeAd.data;
    CGFloat contentWidth = CGRectGetWidth(_customview.frame) - 50;
    BUImage *image = adMeta.imageAry.firstObject;
    const CGFloat imageHeight = contentWidth * (image.height / image.width);
    CGRect rect = CGRectMake(25, CGRectGetMaxY(_actionButton.frame) + 5, contentWidth, imageHeight);
    self.relatedView.logoImageView.frame = CGRectMake(CGRectGetMaxX(rect) - 15 , CGRectGetMaxY(rect) - 15, 15, 15);
    self.relatedView.adLabel.frame = CGRectMake(CGRectGetMinX(rect), CGRectGetMaxY(rect) - 14, 26, 14);
    [self.relatedView refreshData:nativeAd];
    // imageMode decides whether to show video or not
    if (adMeta.imageMode == BUFeedVideoAdModeImage) {
        self.imageView.hidden = YES;
        self.relatedView.videoAdView.hidden = NO;
        self.relatedView.videoAdView.frame = rect;
    } else {
        self.imageView.hidden = NO;
        self.relatedView.videoAdView.hidden = YES;
        if (adMeta.imageAry.count > 0) {
            if (image.imageURL.length > 0) {
                self.imageView.frame = rect;
                UIImage *imagePic = [UIImage imageWithData:[NSData dataWithContentsOfURL:[NSURL URLWithString:image.imageURL]]];
                [self.imageView setImage:imagePic];
            }
        }
    }
    // Register UIView with the native ad; the whole UIView will be clickable.
    [nativeAd registerContainer:self.customview withClickableViews:@[self.infoLabel, self.actionButton]];
}


/**
 This method is called when native ad material loaded successfully.
 */
- (void)nativeAdDidLoad:(BUNativeAd *)nativeAd
{
    if (self.onNativeAdLoad) {
        self.onNativeAdLoad((__bridge void*)self, self.loadContext, (int)nativeAd.adslot.AdType);
    }
}

/**
 This method is called when native ad materia failed to load.
 @param error : the reason of error
 */
- (void)nativeAd:(BUNativeAd *)nativeAd didFailWithError:(NSError *_Nullable)error
{
    self.onError((int)error.code, AutonomousStringCopy2([[error localizedDescription] UTF8String]), self.loadContext);
}

/**
 This method is called when native ad slot has been shown.
 */
- (void)nativeAdDidBecomeVisible:(BUNativeAd *)nativeAd
{
    self.onAdShow(self.interactionContext);
}

/**
 This method is called when native ad is clicked.
 */
- (void)nativeAdDidClick:(BUNativeAd *)nativeAd withView:(UIView *_Nullable)view
{
    self.onAdDidClick(self.interactionContext);
}

/**
 This method is called when the user clicked dislike reasons.
 Only used for dislikeButton in BUNativeAdRelatedView.h
 @param filterWords : reasons for dislike
 */
- (void)nativeAd:(BUNativeAd *)nativeAd dislikeWithReason:(NSArray<BUDislikeWords *> *)filterWords
{
    if (self.onAdClose) {
        self.onAdClose(self.interactionContext);
    }
    
    _relatedView = nil;
    [_customview removeFromSuperview];
    _customview = nil;
}
@end

#if defined (__cplusplus)
extern "C" {
#endif

void UnionPlatform_NativeAd_Load(
    const char* slotID,
    int adCount,
    int nativeAdType,
    int width,
    int height,
    bool supportDeepLink,
    NativeAd_OnError onError,
    NativeAd_OnNativeAdLoad onNativeAdLoad,
    int context) {
        
    BUNativeAd *nad = [[BUNativeAd alloc] init];
    
    NativeAd* instance = [NativeAd sharedInstance];
    instance.nativeAd = nad;
    instance.onError = onError;
    instance.onNativeAdLoad = onNativeAdLoad;
    instance.loadContext = context;
    
    BUAdSlot *slot1 = [[BUAdSlot alloc] init];
    BUSize *imgSize1 = [[BUSize alloc] init];
    imgSize1.width = width;
    imgSize1.height = height;
    slot1.imgSize = imgSize1;
    slot1.ID = [[NSString alloc] initWithUTF8String:slotID];
    slot1.isOriginAd = YES;
    if (nativeAdType == 0) {
        [NativeAd sharedInstance].adType = BUAdSlotAdTypeBanner;
        slot1.AdType = BUAdSlotAdTypeBanner;
        slot1.position = BUAdSlotPositionTop;
        
        [instance buildupViewBanner];
    } else if (nativeAdType == 1) {
        [NativeAd sharedInstance].adType = BUAdSlotAdTypeInterstitial;
        slot1.AdType = BUAdSlotAdTypeInterstitial;
        slot1.position = BUAdSlotPositionMiddle;
        
        [instance buildupViewInterstial];
    } else {
        slot1.AdType = BUAdSlotAdTypeFeed;
        slot1.position = BUAdSlotPositionFeed;
        
        [instance buildupViewFeed];
    }
    slot1.isSupportDeepLink = supportDeepLink;
    nad.adslot = slot1;
    nad.rootViewController = GetAppController().rootViewController;
    nad.delegate = instance;
    [nad loadAdData];
    
    // 强持有，是引用加+1
    [[BUToUnityAdManager sharedInstance] addAdManager:instance];
    
    (__bridge_retained void*)instance;
}

void UnionPlatform_NativeAd_SetInteractionListener (
   void* nativeAdPtr,
    NativeAd_OnAdShow onAdShow,
    NativeAd_OnAdDidClick onAdDidClick,
    NativeAd_OnAdClose onAdClose,
    int context) {
    NativeAd* nativeAdP = (__bridge NativeAd*)nativeAdPtr;
    nativeAdP.onAdShow = onAdShow;
    nativeAdP.onAdDidClick = onAdDidClick;
    nativeAdP.onAdClose = onAdClose;
    nativeAdP.interactionContext = context;
}

void UnionPlatform_NativeAd_ShowNativeAd (void* nativeAdPtr,
                                          int slotType,
                                          float x,
                                          float y) {
    NativeAd* nativeAdP = (__bridge NativeAd*)nativeAdPtr;

    [nativeAdP refreshNativeAd:nativeAdP.nativeAd];
    
    CGFloat originX = x/[UIScreen mainScreen].scale;
    CGFloat originY = y/[UIScreen mainScreen].scale;
    
    nativeAdP.customview.frame = CGRectMake(originX,
    originY,
    CGRectGetWidth(nativeAdP.customview.frame),
    CGRectGetHeight(nativeAdP.customview.frame));
    
    [GetAppController().rootViewController.view addSubview:nativeAdP.customview];
}

void  UnionPlatform_NativeAd_Dispose(void* nativeAdPtr) {
    dispatch_async(dispatch_get_main_queue(), ^{
         (__bridge_transfer NativeAd*)nativeAdPtr;
        
        NativeAd* nativeAdP = (__bridge NativeAd*)nativeAdPtr;
        [nativeAdP.customview removeFromSuperview];
        
        [[BUToUnityAdManager sharedInstance] deleteAdManager:nativeAdP];
     });
}

#if defined (__cplusplus)
}
#endif
