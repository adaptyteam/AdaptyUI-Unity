#import <Foundation/Foundation.h>
#include "AdaptyUnityPluginCallback.h"
#include "UnityFramework/UnityFramework-Swift.h"

static NSString * cstringToString(const char *str) {
    return str ? [NSString stringWithUTF8String:str] : nil;
}

static const char * cstringFromString(NSString *str) {
    return str ? [str cStringUsingEncoding:NSUTF8StringEncoding] : nil;
}

extern "C" {
void AdaptyUIUnity_createPaywallView(const char  *paywall,
                                     const char  *locale,
                                     BOOL        preloadProducts,
                                     UnityAction callback) {
    [[AdaptyUIUnityPlugin shared]
     handleCreateView:cstringToString(paywall)
               locale:cstringToString(locale)
      preloadProducts:preloadProducts
           completion:^(NSString *_Nullable response) {
            SendCallbackToUnity(callback, response);
        }];
}

void AdaptyUIUnity_presentPaywallView(const char *viewId, UnityAction callback) {
    [[AdaptyUIUnityPlugin shared]
     handlePresentView:cstringToString(viewId)
            completion:^(NSString *_Nullable response) {
            SendCallbackToUnity(callback, response);
        }];
}

void AdaptyUIUnity_dismissPaywallView(const char *viewId, UnityAction callback) {
    [[AdaptyUIUnityPlugin shared]
     handleDismissView:cstringToString(viewId)
            completion:^(NSString *_Nullable response) {
            SendCallbackToUnity(callback, response);
        }];
}
}
