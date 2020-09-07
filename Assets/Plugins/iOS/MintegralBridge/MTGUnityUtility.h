//
//  Copyright © 2016年 Mobvista. All rights reserved.
//


#import <Foundation/Foundation.h>


@interface MTGUnityUtility : NSObject

// Helper method to create C string copy
char* MTGUnityMakeStringCopy (const char* string);
+ (NSString *)stringFromCString:(const char *)string;
+ (NSString *)convertJsonFromObject:(id)classInstance withClassName:(NSString *)className;
+ (NSDictionary *)convertDictionaryFromObject:(id)classInstance withClassName:(NSString *)className;

@end
