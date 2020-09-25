package com.bytedance.android;

import android.app.Activity;
import android.content.Context;
import android.os.Handler;
import android.os.Looper;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;
import com.bytedance.sdk.openadsdk.TTSplashAd;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.ImageRequest;
import com.android.volley.toolbox.Volley;

public class SplashAdManager {
    private Handler mHandler;
    private static volatile SplashAdManager sManager;
    private RequestQueue mQueue;
    private Context mContext;
    private View mSplashView;

    private SplashAdManager() {
        if (mHandler == null) {
            mHandler = new Handler(Looper.getMainLooper());
        }
    }

    public static SplashAdManager getSplashAdManager() {
        if (sManager == null) {
            synchronized (SplashAdManager.class) {
                if (sManager == null) {
                    sManager = new SplashAdManager();
                }
            }
        }
        return sManager;
    }

    public ViewGroup getRootLayout(Activity context) {
        if (context == null) {
            return null;
        }
        ViewGroup rootGroup = null;
        rootGroup = context.findViewById(android.R.id.content);
        return rootGroup;
    }

    public void addAdView(Activity context, View adView, ViewGroup.LayoutParams layoutParams) {
        if (context == null || adView == null || layoutParams == null) {
            return;
        }
        ViewGroup group = getRootLayout(context);
        if (group == null) {
            return;
        }
        group.addView(adView, layoutParams);
    }

    public void removeAdView(Activity context, View adView) {
        if (context == null || adView == null) {
            return;
        }
        ViewGroup group = getRootLayout(context);
        if (group == null) {
            return;
        }
        group.removeView(adView);
    }

    //广告使用完毕后，比如关闭或移除后，请调用destory释放资源。
    //相关调用注意放在主线程
    public void showSplashAd(final Context context, final TTSplashAd ad) {
        if (context == null || ad == null) {
            return;
        }
        mContext = context;
        if (mQueue == null) {
            mQueue = Volley.newRequestQueue(mContext);
        }

        mHandler.post(new Runnable() {
            @Override
            public void run() {
                mSplashView = ad.getSplashView();
                removeAdView((Activity) context, mSplashView);
                FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT);
                layoutParams.gravity = Gravity.CENTER;
                addAdView((Activity) context, mSplashView, layoutParams);
            }
        });
    }

//操作UI相关放在主线程
       public void destorySplashView(final Activity context) {
             if (context == null || mHandler == null) {
                 return;
             }
             mHandler.post(new Runnable() {
                 @Override
                 public void run() {
                     if (mSplashView != null) {
                         removeAdView(context, mSplashView);
                         mSplashView = null;
                     }
                 }
             });
         }
}
