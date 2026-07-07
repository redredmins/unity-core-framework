package com.redmins.core;

import android.app.Activity;
import android.content.Intent;

public class ShareBridge {

    public static void shareImage(Activity activity, String path, String message) {
        try {
            java.io.File file = new java.io.File(path);
            if (!file.exists()) {
                android.util.Log.w("RedShare", "shareImage: file not found: " + path);
                return;
            }

            android.net.Uri uri = androidx.core.content.FileProvider.getUriForFile(
                    activity, activity.getPackageName() + ".redfileprovider", file);

            Intent intent = new Intent(Intent.ACTION_SEND);
            intent.setType("image/png");
            intent.putExtra(Intent.EXTRA_STREAM, uri);
            intent.putExtra(Intent.EXTRA_TEXT, message);
            intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);

            activity.startActivity(Intent.createChooser(intent, null));
        } catch (Exception e) {
            android.util.Log.w("RedShare", "shareImage failed", e);
        }
    }
}
