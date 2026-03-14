using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace ColorOfCrash.Utils
{
    public static class TextUtils
    {
        public static async UniTask CountUpAsync(this TMP_Text text, int start, int end, float duration, CancellationToken ct)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (ct.IsCancellationRequested) return;

                elapsed += Time.deltaTime;

                float progress = elapsed / duration;
                int current = Mathf.RoundToInt(Mathf.Lerp(start, end, progress));
                
                text.SetText(current.ToString());
                
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            text.SetText(end.ToString());
        }
    }
}
