package com.redmins.core;

import android.app.Activity;
import android.os.CancellationSignal;
import android.util.Log;

import androidx.core.content.ContextCompat;
import androidx.credentials.Credential;
import androidx.credentials.CredentialManager;
import androidx.credentials.CredentialManagerCallback;
import androidx.credentials.CustomCredential;
import androidx.credentials.GetCredentialRequest;
import androidx.credentials.GetCredentialResponse;
import androidx.credentials.exceptions.GetCredentialException;

import com.google.android.libraries.identity.googleid.GetGoogleIdOption;
import com.google.android.libraries.identity.googleid.GoogleIdTokenCredential;
import com.unity3d.player.UnityPlayer;

import java.util.concurrent.Executor;

public class GoogleSignInBridge {

    private static final String TAG = "RedSignIn";

    public static void signIn(final Activity activity, final String webClientId,
                              final String callbackObject, final String callbackMethod) {
        // 1차: 인증된 계정만 대상으로 무음 로그인(복귀 유저)
        attempt(activity, webClientId, callbackObject, callbackMethod, true);
    }

    private static void attempt(final Activity activity, final String webClientId,
                                final String callbackObject, final String callbackMethod,
                                final boolean authorizedOnly) {
        CredentialManager credentialManager = CredentialManager.create(activity);

        GetGoogleIdOption googleIdOption = new GetGoogleIdOption.Builder()
                .setFilterByAuthorizedAccounts(authorizedOnly)
                .setServerClientId(webClientId)
                .setAutoSelectEnabled(true)
                .build();

        GetCredentialRequest request = new GetCredentialRequest.Builder()
                .addCredentialOption(googleIdOption)
                .build();

        Executor executor = ContextCompat.getMainExecutor(activity);

        CredentialManagerCallback<GetCredentialResponse, GetCredentialException> callback =
                new CredentialManagerCallback<GetCredentialResponse, GetCredentialException>() {
                    @Override
                    public void onResult(GetCredentialResponse response) {
                        handleResponse(response, callbackObject, callbackMethod);
                    }

                    @Override
                    public void onError(GetCredentialException e) {
                        if (authorizedOnly) {
                            // 1차(무음) 실패 시 계정 선택 UI로 2차 시도
                            Log.w(TAG, "Silent sign-in failed, retrying with account picker: " + e.getMessage());
                            attempt(activity, webClientId, callbackObject, callbackMethod, false);
                        } else {
                            Log.w(TAG, "Sign-in failed: " + e.getMessage());
                            sendResult(callbackObject, callbackMethod, "NULL");
                        }
                    }
                };

        credentialManager.getCredentialAsync(activity, request, (CancellationSignal) null, executor, callback);
    }

    private static void handleResponse(GetCredentialResponse response,
                                       String callbackObject, String callbackMethod) {
        try {
            handleResponseInternal(response, callbackObject, callbackMethod);
        } catch (Exception e) {
            // 파싱 실패 등 어떤 예외도 로그인 실패로 처리 (인트로 크래시 방지)
            Log.w(TAG, "Sign-in response handling failed: " + e);
            sendResult(callbackObject, callbackMethod, "NULL");
        }
    }

    private static void handleResponseInternal(GetCredentialResponse response,
                                               String callbackObject, String callbackMethod) {
        Credential credential = response.getCredential();

        if (credential instanceof CustomCredential
                && GoogleIdTokenCredential.TYPE_GOOGLE_ID_TOKEN_CREDENTIAL.equals(credential.getType())) {
            GoogleIdTokenCredential googleCred = GoogleIdTokenCredential.createFrom(credential.getData());
            String email = googleCred.getId();
            String idToken = googleCred.getIdToken();

            if (email != null && email.indexOf('@') >= 0) {
                // "email|idToken" (email에는 "|"가 올 수 없어 안전한 구분자)
                sendResult(callbackObject, callbackMethod, email + "|" + (idToken != null ? idToken : ""));
            } else {
                Log.w(TAG, "Google credential id is not an email: " + email);
                sendResult(callbackObject, callbackMethod, "NULL");
            }
        } else {
            Log.w(TAG, "Unexpected credential type: "
                    + (credential != null ? credential.getType() : "null"));
            sendResult(callbackObject, callbackMethod, "NULL");
        }
    }

    private static void sendResult(String callbackObject, String callbackMethod, String value) {
        UnityPlayer.UnitySendMessage(callbackObject, callbackMethod, value);
    }
}
