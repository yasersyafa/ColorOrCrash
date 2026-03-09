using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ColorOfCrash.Utils
{
    
    public static class YieldCollection
    {
        private static readonly Dictionary<float, WaitForSeconds> WaitForSecondsDict = new();
        private static readonly Dictionary<float, WaitForSecondsRealtime> WaitForSecondsRealtimeDict = new();
        public static readonly WaitUntil WaitForMouseClick = new(() => Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);
        
        public static WaitForSeconds WaitForSeconds(float seconds)
        {
            if (WaitForSecondsDict.TryGetValue(seconds, out var forSeconds))
            {
                return forSeconds;
            }

            return WaitForSecondsDict[seconds] = new WaitForSeconds(seconds);
        }

        public static WaitForSecondsRealtime WaitForSecondsRealtime(float seconds)
        {
            if (WaitForSecondsRealtimeDict.TryGetValue(seconds, out var forSeconds))
            {
                return forSeconds;
            }
            
            return WaitForSecondsRealtimeDict[seconds] = new WaitForSecondsRealtime(seconds);
        }

        public static IEnumerator WaitForMouseClickOrSeconds(float seconds)
        {
            var elapsedTime = 0f;
            while (elapsedTime < seconds)
            {
                if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                {
                    yield break; // Exit the coroutine if mouse is clicked
                }
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
        }

        public static IEnumerator WaitUntilOrSeconds(float seconds, Func<bool> predicate)
        {
            var elapsedTime = 0f;
            while (elapsedTime < seconds)
            {
                if (predicate())
                {
                    yield break;
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}