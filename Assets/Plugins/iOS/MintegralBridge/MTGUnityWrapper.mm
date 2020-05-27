//
//  MVUnityWrapper.mm
//
//  Copyright © 2016年 Mintegral. All rights reserved.
//


#import "MTGUnityWrapper.h"
#import <MTGSDK/MTGSDK.h>

void UnityPause( bool pause );

void UnitySendMessage( const char * className, const char * methodName, const char * param );


@implementation MTGUnityWrapper



@end


//C接口以供C#调用
extern "C" {
    
    void initApplication (const char* appId, const char* apiKey);
    
    //void preloadAppWallAds (const char* unitId);

    void preloadNative (const char* unitId,
                           const char* fbPlacementId,
                           bool autoCacheImage,
                           const char* adCategory,
                           const char* supportedTemplatesString);

    void showUserPrivateInfoTips ();

    void setUserPrivateInfoType (const char* statusKey, const char* statusType);

    int getAuthPrivateInfoStatus (const char* statusKey);
    
    void printPrivateInfoStatus ();
    
    void setConsentStatusInfoType (const int* statusType);
    
    bool getConsentStatusInfoType ();

    void setDoNotTrackStatusType(const int* statusType);
}


void initApplication (const char* appId, const char* apiKey)
{
    NSString *appIdStr = [NSString stringWithUTF8String:appId];
    NSString *apiKeyStr = [NSString stringWithUTF8String:apiKey];

    Class classInstance = NSClassFromString(@"MTGSDK");
    SEL selector = NSSelectorFromString(@"setChannelFlag:");
    
    NSString *pluginNumber = @"Y+H6DFttYrPQYcIe+F2F+F5/Hv==";
    
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Warc-performSelector-leaks"
    if ([classInstance respondsToSelector:selector]) {
        [classInstance performSelector:selector withObject:pluginNumber];
    }
#pragma clang diagnostic pop
    
   [[MTGSDK sharedInstance] setAppID:appIdStr ApiKey:apiKeyStr];
}


void preloadNative (const char* unitId,
                      const char* fbPlacementId,
                      bool autoCacheImage,
                      const char* adCategory,
                      const char* supportedTemplatesString)
{
    NSString *unitIdStr = [NSString stringWithUTF8String:unitId];
    NSString *fbPlacementIdStr = [NSString stringWithUTF8String:fbPlacementId];
    NSString *adCategoryStr = [NSString stringWithUTF8String:adCategory];

    MTGAdCategory category = (MTGAdCategory)[adCategoryStr integerValue];

    NSString *jsonString = [MTGUnityUtility stringFromCString:supportedTemplatesString];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:nil];
    
    NSArray *templates = [dict objectForKey:@"objects"];
    NSMutableArray *supportedTemplates = [[NSMutableArray alloc] init];
    
    for (NSDictionary *dict in templates) {
        MTGTemplate *mvtemplate = [[MTGTemplate alloc] init];
        mvtemplate.adsNum = [[dict valueForKey:@"adsNum"] unsignedIntegerValue];
        mvtemplate.templateType = (MTGAdTemplateType)[[dict valueForKey:@"templateType"] unsignedIntegerValue];
        [supportedTemplates addObject:mvtemplate];
    }

    [[MTGSDK sharedInstance] preloadNativeAdsWithUnitId:unitIdStr
                                   fbPlacementId:fbPlacementIdStr
                              supportedTemplates:supportedTemplates autoCacheImage:autoCacheImage
                                      adCategory:category];

}

/*
void preloadAppWallAds (const char*  unitId)
{
    NSString *unitIdStr = [NSString stringWithUTF8String:unitId];
    [[MTGSDK sharedInstance] preloadAppWallAdsWithUnitId:unitIdStr];
}
 */


void showUserPrivateInfoTips ()
{
    [[MTGSDK sharedInstance] showUserPrivateInfoTips:^(MTGUserPrivateTypeInfo *userPrivateTypeInfo, NSError *error) {
        if (error == nil) {

            NSString * info = [NSString stringWithFormat: @"isGeneralDataAllowed = %d ,isDeviceIdAllowed = %d ,isGpsAllowed = %d",userPrivateTypeInfo.isGeneralDataAllowed,userPrivateTypeInfo.isDeviceIdAllowed,userPrivateTypeInfo.isGpsAllowed];
            NSLog(@"%@", info);
             
            UnitySendMessage("MintegralManager", "onShowUserInfoTips", "YSE");

        }else{
            NSLog(@"%@", error.localizedDescription);
            printPrivateInfoStatus ();

            UnitySendMessage("MintegralManager", "onShowUserInfoTips", "NO");
        }
    }];
}


void setUserPrivateInfoType (const char* statusKey, const char* statusType)
{
  NSString *statusKeyStr = [NSString stringWithUTF8String:statusKey];
  NSString *statusTypeStr = [NSString stringWithUTF8String:statusType];
  BOOL isSet = NO;

  if([statusTypeStr isEqualToString:@"ON"]){
      isSet = YES;
  }else{
      isSet = NO;
  }


  if([statusKeyStr isEqualToString:@"authority_all_info"]){
      [[MTGSDK sharedInstance] setUserPrivateInfoType:MTGUserPrivateType_ALL agree:isSet];

  }else if([statusKeyStr isEqualToString:@"authority_general_data"]){
      [[MTGSDK sharedInstance] setUserPrivateInfoType:MTGUserPrivateType_GeneralData agree:isSet];

  }else if([statusKeyStr isEqualToString:@"authority_device_id"]){
    [[MTGSDK sharedInstance] setUserPrivateInfoType:MTGUserPrivateType_DeviceId agree:isSet];

  }else if([statusKeyStr isEqualToString:@"authority_gps"]){
    [[MTGSDK sharedInstance] setUserPrivateInfoType:MTGUserPrivateType_Gps agree:isSet];

  }

  printPrivateInfoStatus ();
}

int getAuthPrivateInfoStatus (const char* statusKey)
{
    NSString *statusKeyStr = [NSString stringWithUTF8String:statusKey];
    
    MTGUserPrivateTypeInfo *userPrivateTypeInfo = [[MTGSDK sharedInstance] userPrivateInfo];
    printPrivateInfoStatus ();
    
    if([statusKeyStr isEqualToString:@"authority_general_data"]){
        return userPrivateTypeInfo.isGeneralDataAllowed;
        
    }else if([statusKeyStr isEqualToString:@"authority_device_id"]){
        return userPrivateTypeInfo.isDeviceIdAllowed;
        
    }else if([statusKeyStr isEqualToString:@"authority_gps"]){
        return userPrivateTypeInfo.isGpsAllowed;
        
    }else{
        return 0;
    }
    
}


void printPrivateInfoStatus ()
{
    MTGUserPrivateTypeInfo *userPrivateTypeInfo = [[MTGSDK sharedInstance] userPrivateInfo];
    NSString * info = [NSString stringWithFormat: @"isGeneralDataAllowed = %d ,isDeviceIdAllowed = %d ,isGpsAllowed = %d",userPrivateTypeInfo.isGeneralDataAllowed,userPrivateTypeInfo.isDeviceIdAllowed,userPrivateTypeInfo.isGpsAllowed];
    NSLog(@"%@", info);
}

void setConsentStatusInfoType (const int* statusType)
{
    NSString *statusTypeStr = [NSString stringWithFormat:@"%d", statusType];
    
    if([statusTypeStr isEqualToString:@"1"]){
        [[MTGSDK sharedInstance] setConsentStatus:YES];
    }else{
        [[MTGSDK sharedInstance] setConsentStatus:NO];
    }
    
    printPrivateInfoStatus ();
}

bool getConsentStatusInfoType ()
{
    BOOL status = [[MTGSDK sharedInstance] consentStatus];
    NSLog(@"consentStatus = %d", status);
    return status;
}

void setDoNotTrackStatusType (const int* statusType)
{
    NSString *statusTypeStr = [NSString stringWithFormat:@"%d", statusType];
    
    if([statusTypeStr isEqualToString:@"1"]){
        [[MTGSDK sharedInstance] setDoNotTrackStatus:YES];
    }else{
        [[MTGSDK sharedInstance] setDoNotTrackStatus:NO];
    }

}


