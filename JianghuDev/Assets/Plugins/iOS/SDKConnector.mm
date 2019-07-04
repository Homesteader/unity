#import "SDKConnector.h"

@interface SDKConnector : NSObject
@property (nonatomic, retain)   NSString*   unityMsgReceiver;
@end

//代码定义区域 C - Begin
@implementation SDKConnector

- (void)dealloc {
    [[NSNotificationCenter defaultCenter] removeObserver:self];
    self.unityMsgReceiver = nil;
    //[super dealloc];
}

+ (id)sharedInstance {
    static  SDKConnector*  s_instance = nil;
    if (nil == s_instance) {
        @synchronized(self) {
            if (nil == s_instance) {
                s_instance = [[self alloc] init];
            }
        }
    }
    return s_instance;
}

- (void)sendDictMessage:(NSString *)messageName param:(NSDictionary *)dict
{
    NSString *param = @"";
    for (NSString *key in dict)
    {
        if ([param length] == 0)
        {
            param = [param stringByAppendingFormat:@"%@=%@", key, [dict valueForKey:key]];
        }
        else
        {
            param = [param stringByAppendingFormat:@"&%@=%@", key, [dict valueForKey:key]];
        }
    }
    UnitySendMessage([_unityMsgReceiver UTF8String], [messageName UTF8String], [param UTF8String]);
}

- (void)sendMessage:(NSString *)messageName param:(NSString *)param
{
    UnitySendMessage([_unityMsgReceiver UTF8String], [messageName UTF8String], [param UTF8String]);
}

@end
//代码定义区域 C - End


//代码定义区域 B - Begin
#if defined(__cplusplus)
extern "C"{
#endif
    
    NSString* ConvertToNSString (const char* string)
    {
        if (string)
            return [NSString stringWithUTF8String: string];
        else
            return [NSString stringWithUTF8String: ""];
    }
	
	char* MakeStringCopy(const char* str)
    {
        if(NULL==str)
        {
            return  NULL;
        }
        char* s = (char*)malloc(strlen(str)+1);
        strcpy(s,str);
        return s;
    }
	
	BOOL ios_openOtherApp(const char*  appName)
    {
        NSString *paramStr = [NSString stringWithFormat:@"WoogiWorld://%", appName];		
		NSURL *url = [NSURL URLWithString:[paramStr stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding]];
		return [[UIApplication sharedApplication] openURL:url];
    }
	
	#if defined(__cplusplus)
}
#endif
//代码定义区域 B - End


//代码定义区域 A - Begin
#if defined(__cplusplus)
extern "C"{
#endif
    extern void UnitySendMessage(const char *, const char *, const char *);
    extern NSString* ConvertToNSString (const char* string);
#if defined(__cplusplus)
}
#endif
//代码定义区域 A - End
