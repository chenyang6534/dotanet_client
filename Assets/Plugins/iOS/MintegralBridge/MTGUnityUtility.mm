//
//  Copyright © 2016年 Mobvista. All rights reserved.
//


#import "MTGUnityUtility.h"
#import <objc/runtime.h>


@implementation MTGUnityUtility

// Helper method to create C string copy
char* MTGUnityMakeStringCopy (const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}



+ (NSString *)stringFromCString:(const char *)string{

    if (string && string[0] != 0) {
        return [NSString stringWithUTF8String:string];
    }
    
    return nil;
}



+ (NSString *)convertJsonFromObject:(id)classInstance withClassName:(NSString *)className{
    
    if (!classInstance) {
        return nil;
    }
    if (!className) {
        return nil;
    }

    NSMutableDictionary *muteDictionary = [NSMutableDictionary dictionary];
    
    id theClass = objc_getClass([className UTF8String]);
    unsigned int outCount, i;
    objc_property_t *properties = class_copyPropertyList(theClass, &outCount);
    for (i = 0; i < outCount; i++) {
        objc_property_t property = properties[i];
        NSString *propertyName = [NSString stringWithCString:property_getName(property) encoding:NSUTF8StringEncoding];
        SEL propertySelector = NSSelectorFromString(propertyName);
        if ([classInstance respondsToSelector:propertySelector]) {

            NSString *temp = [NSString stringWithFormat:@"%@",[classInstance valueForKey:propertyName]];
            [muteDictionary setValue:temp forKey:propertyName];
        }
    }

    NSError *jsonError = nil;
    NSData *jsonData = nil;
    
    NSString *jsonString = nil;
    if ([NSJSONSerialization isValidJSONObject:muteDictionary]) {
        jsonData = [NSJSONSerialization dataWithJSONObject:muteDictionary options:0 error:&jsonError];
        jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    }
    
    return jsonString;
}


+ (NSDictionary *)convertDictionaryFromObject:(id)classInstance withClassName:(NSString *)className{
    
    NSMutableDictionary *muteDictionary = [NSMutableDictionary dictionary];
    
    id theClass = objc_getClass([className UTF8String]);
    unsigned int outCount, i;
    objc_property_t *properties = class_copyPropertyList(theClass, &outCount);
    for (i = 0; i < outCount; i++) {
        objc_property_t property = properties[i];
        NSString *propertyName = [NSString stringWithCString:property_getName(property) encoding:NSUTF8StringEncoding];
        SEL propertySelector = NSSelectorFromString(propertyName);
        if ([classInstance respondsToSelector:propertySelector]) {
            
            NSString *temp = [NSString stringWithFormat:@"%@",[classInstance valueForKey:propertyName]];
            [muteDictionary setValue:temp forKey:propertyName];
        }
    }
    return muteDictionary;
}


@end

