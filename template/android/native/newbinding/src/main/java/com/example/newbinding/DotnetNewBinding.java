package com.example.newbinding;

import android.content.Context;
import android.os.Build;
import android.util.Log;

import com.ps19.mykotlinlibrary.LoginHelper;

public class DotnetNewBinding {

    public static String getString(Context context, String clientId) {
        // This assumes LoginHelper.invokeHandleLoginMicrosoft is a static method in Kotlin.
        // Adjust if it's an instance method instead.

            LoginHelper.Companion.invokeHandleLoginMicrosoft(context, clientId, result -> {
                Log.i("Error",result.toString());
            });


        // You might want to return some value or handle the asynchronous result.
        return "String from Java after invoking Kotlin!";
    }
}
