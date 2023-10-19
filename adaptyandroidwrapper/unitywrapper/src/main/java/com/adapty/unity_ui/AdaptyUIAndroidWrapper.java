package com.adapty.unity_ui;

import static com.adapty.unity_ui.Constants.ADAPTY_ERROR_CODE_DECODING_FAILED;
import static com.adapty.unity_ui.Constants.ADAPTY_ERROR_CODE_KEY;
import static com.adapty.unity_ui.Constants.ADAPTY_ERROR_CODE_WRONG_PARAMETER;
import static com.adapty.unity_ui.Constants.ADAPTY_ERROR_DECODING_FAILED_MESSAGE;
import static com.adapty.unity_ui.Constants.ADAPTY_ERROR_DETAIL_KEY;
import static com.adapty.unity_ui.Constants.ADAPTY_ERROR_KEY;
import static com.adapty.unity_ui.Constants.ADAPTY_ERROR_MESSAGE_KEY;
import static com.adapty.unity_ui.Constants.ADAPTY_SUCCESS_KEY;

import android.app.Activity;
import android.os.Handler;

import com.adapty.errors.AdaptyError;
import com.adapty.internal.crossplatform.CrossplatformHelper;
import com.adapty.internal.crossplatform.ui.AdaptyUiBridgeError;
import com.adapty.internal.crossplatform.ui.AdaptyUiEvent;
import com.adapty.internal.crossplatform.ui.CrossplatformUiHelper;
import com.adapty.models.AdaptyPaywall;
import com.adapty.unity.AdaptyAndroidCallback;
import com.adapty.unity.AdaptyAndroidMessageHandler;
import com.adapty.unity.AdaptyAndroidWrapper;
import com.unity3d.player.UnityPlayer;

import java.util.HashMap;
import java.util.Map;

public class AdaptyUIAndroidWrapper {

    private static Handler unityMainThreadHandler;

    public static void runOnUnityThread(Runnable runnable) {
        if(unityMainThreadHandler != null && runnable != null) {
            unityMainThreadHandler.post(runnable);
        }
    }

    public static void createView(
            String paywallJson,
            String locale,
            boolean preloadProducts,
            String personalizedOffersJson,
            AdaptyAndroidCallback callback
    ) {
        AdaptyPaywall paywall = parseJsonArgument(paywallJson, AdaptyPaywall.class);
        if (paywall == null) {
            String methodName = new Object() {}.getClass().getEnclosingMethod().getName();
            sendParameterError("paywallJson", methodName, callback);
            return;
        }

        if (locale == null) {
            String methodName = new Object() {}.getClass().getEnclosingMethod().getName();
            sendParameterError("locale", methodName, callback);
            return;
        }

        HashMap<String, Boolean> personalizedOffers = parseJsonArgument(personalizedOffersJson, HashMap.class);

        getHelper().handleCreateView(
            paywall,
            locale,
            preloadProducts,
            personalizedOffers,
            view -> sendSuccess(view, callback),
            error -> sendEmptyResultOrError(error, callback)
        );
    }

    public static void presentView(String id, AdaptyAndroidCallback callback) {
        if (id == null) {
            String methodName = new Object() {}.getClass().getEnclosingMethod().getName();
            sendParameterError("id", methodName, callback);
            return;
        }

        getHelper().handlePresentView(
            id,
            result -> sendEmptyResultOrError(null, callback),
            error -> sendBridgeError(error, callback)
        );
    }

    public static void dismissView(String id, AdaptyAndroidCallback callback) {
        if (id == null) {
            String methodName = new Object() {}.getClass().getEnclosingMethod().getName();
            sendParameterError("id", methodName, callback);
            return;
        }

        getHelper().handleDismissView(
            id,
            result -> sendEmptyResultOrError(null, callback),
            error -> sendBridgeError(error, callback)
        );
    }

    public static void handleUiEvents() {
        getHelper().setUiEventsObserver(event -> {
                sendUiEvent(event);
                return null;
            }
        );
    }

    private static <T> T parseJsonArgument(String json, Class<T> clazz) {
        if (isNullOrBlank(json)) return null;

        T result = null;
        try {
            result = getSerialization().fromJson(json, clazz);
        } catch (Exception e) { }

        return result;
    }

    private static boolean isNullOrBlank(String str) {
        return str == null || str.equals("") || str.trim().equals("");
    }

    private static void sendSuccess(Object data, AdaptyAndroidCallback callback) {
        Map<String, Object> message = new HashMap<>();
        message.put(ADAPTY_SUCCESS_KEY, data);
        runOnUnityThread(() -> callback.onHandleResult(getSerialization().toJson(message)));
    }

    private static void sendMessageWithResult(String message, AdaptyAndroidCallback callback) {
        runOnUnityThread(() -> callback.onHandleResult(message));
    }

    private static void sendParameterError(String paramKey, String methodName, AdaptyAndroidCallback callback) {
        Map<String, Object> message = new HashMap<>();
        Map<String, Object> errorMap = new HashMap<>();
        errorMap.put(ADAPTY_ERROR_MESSAGE_KEY, ADAPTY_ERROR_DECODING_FAILED_MESSAGE);
        errorMap.put(ADAPTY_ERROR_CODE_KEY, ADAPTY_ERROR_CODE_DECODING_FAILED);
        errorMap.put(ADAPTY_ERROR_DETAIL_KEY, createErrorDetailsString(paramKey, methodName));
        message.put(ADAPTY_ERROR_KEY, errorMap);
        sendMessageWithResult(getSerialization().toJson(message), callback);
    }

    private static void sendBridgeError(AdaptyUiBridgeError bridgeError, AdaptyAndroidCallback callback) {
        Map<String, Object> message = new HashMap<>();
        Map<String, Object> errorMap = new HashMap<>();
        errorMap.put(ADAPTY_ERROR_MESSAGE_KEY, bridgeError.getMessage());
        errorMap.put(ADAPTY_ERROR_CODE_KEY, ADAPTY_ERROR_CODE_WRONG_PARAMETER);
        message.put(ADAPTY_ERROR_KEY, errorMap);
        sendMessageWithResult(getSerialization().toJson(message), callback);
    }

    private static void sendUiEvent(AdaptyUiEvent event) {
        String eventJson = getSerialization().toJson(event.getData());
        sendDataToMessageHandler(event.getName(), eventJson);
    }

    private static String createErrorDetailsString(String paramKey, String methodName) {
        return "AdaptyPluginError.decodingFailed(Error while parsing parameter: " + paramKey + ", method: " + methodName + ")";
    }

    private static void sendEmptyResultOrError(AdaptyError error, AdaptyAndroidCallback callback) {
        Map<String, Object> message = new HashMap<>();
        if (error != null) {
            message.put(ADAPTY_ERROR_KEY, error);
        } else {
            message.put(ADAPTY_SUCCESS_KEY, true);
        }
        sendMessageWithResult(getSerialization().toJson(message), callback);
    }

    private static void sendDataToMessageHandler(String key, String data) {
        runOnUnityThread(() -> {
            AdaptyAndroidMessageHandler messageHandler = AdaptyAndroidWrapper.getMessageHandler();
            if (messageHandler != null) {
                messageHandler.onMessage(key, data);
            }
        });
    }

    private static volatile CrossplatformUiHelper _helper = null;

    private static CrossplatformUiHelper getHelper() {
        if (_helper != null) return _helper;
        synchronized (AdaptyUIAndroidWrapper.class) {
            if (_helper != null) return _helper;
            Activity activity = UnityPlayer.currentActivity;
            CrossplatformUiHelper.Companion.init(activity.getApplicationContext());
            CrossplatformUiHelper helper = CrossplatformUiHelper.Companion.getShared();
            helper.setActivity(activity);
            if(unityMainThreadHandler == null) {
                unityMainThreadHandler = new Handler();
                helper.setUiEventsObserver(event -> {
                        sendUiEvent(event);
                        return null;
                    }
                );
            }
            _helper = helper;
            return helper;
        }
    }

    private static CrossplatformHelper getSerialization() {
        return getHelper().getSerialization();
    }
}

class Constants {

    public static final String ADAPTY_SUCCESS_KEY = "success";
    public static final String ADAPTY_ERROR_KEY = "error";
    public static final String ADAPTY_ERROR_CODE_KEY = "adapty_code";
    public static final String ADAPTY_ERROR_MESSAGE_KEY = "message";
    public static final String ADAPTY_ERROR_DETAIL_KEY = "detail";
    public static final String ADAPTY_ERROR_DECODING_FAILED_MESSAGE = "Decoding failed";
    public static final int ADAPTY_ERROR_CODE_DECODING_FAILED = 2006;
    public static final int ADAPTY_ERROR_CODE_WRONG_PARAMETER = 3001;
}
