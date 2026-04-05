#if FIREBASE_AUTH
using System;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;

namespace RedMinS
{
    public class AnonymousAuthProvider : IAuthProvider
    {
        public string ProviderName => "Anonymous";
        public bool IsSignedIn => FirebaseAuth.DefaultInstance.CurrentUser != null;

        public void SignIn(Action<string> onSuccess, Action<string> onFail)
        {
            FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    onFail?.Invoke("Anonymous sign-in was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    onFail?.Invoke($"Anonymous sign-in failed: {task.Exception?.Message}");
                    return;
                }

                string uid = task.Result.User.UserId;
                Debug.Log($"[AnonymousAuthProvider] Signed in: {uid}");
                onSuccess?.Invoke(uid);
            });
        }

        public void SignOut()
        {
            FirebaseAuth.DefaultInstance.SignOut();
            Debug.Log("[AnonymousAuthProvider] Signed out.");
        }
    }
}
#endif
