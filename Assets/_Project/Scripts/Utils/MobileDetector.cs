using UnityEngine;
using System.Runtime.InteropServices;

namespace ColorOfCrash.Utils
{
    public static class MobileDetector
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            [DllImport("__Internal")]
            private static extern int IsMobilebrowser();
        #endif

        public static bool IsMobile()
        {
            #if UNITY_EDITOR
                return false;
            #elif UNITY_WEBGL
                return IsMobilebrowser() == 1;
            #elif UNITY_ANDROID || UNITY_IOS
                return true;
            #else
                return false;
            #endif
        }
    }
}
