using System;
using ColorOrCrash.Vin.Core;
using ColorOrCrash.Vin.Managers;

namespace ColorOrCrash.Vin.Player
{
    public class PlayerColorManager
    {
        private GameSettings settings;
        private GameColor currentColor;
        private GameColor nextColor;
        private float colorTimer;
        private float maxTimer;

        public GameColor CurrentColor => currentColor;
        public GameColor NextColor => nextColor;
        public event Action<GameColor, GameColor, float, float> OnColorChanged; // current, next, timer, maxTimer

        public PlayerColorManager(GameSettings settings)
        {
            this.settings = settings;
        }

        public void Initialize()
        {
            currentColor = (GameColor)UnityEngine.Random.Range(0, 3);
            nextColor = GetRandomDifferentColor(currentColor);
            maxTimer = GetCurrentInterval();
            colorTimer = maxTimer;
            OnColorChanged?.Invoke(currentColor, nextColor, colorTimer, maxTimer);
        }

        public void Update(float deltaTime)
        {
            colorTimer -= deltaTime;

            if (colorTimer <= 0f)
            {
                ChangeToNextColor();
            }
        }

        private void ChangeToNextColor()
        {
            currentColor = nextColor;
            nextColor = GetRandomDifferentColor(currentColor);
            maxTimer = GetCurrentInterval(); // Recalculate interval based on current score
            colorTimer = maxTimer;
            OnColorChanged?.Invoke(currentColor, nextColor, colorTimer, maxTimer);
        }
        
        private float GetCurrentInterval()
        {
            int currentScore = GameManager.Instance != null ? GameManager.Instance.Score : 0;
            float interval = settings.startColorChangeInterval - (currentScore * settings.colorChangeIntervalDecreasePerScore);
            return UnityEngine.Mathf.Max(interval, settings.minColorChangeInterval);
        }

        private GameColor GetRandomDifferentColor(GameColor excludeColor)
        {
            GameColor newColor;
            do
            {
                newColor = (GameColor)UnityEngine.Random.Range(0, 3);
            } while (newColor == excludeColor);
            return newColor;
        }

        public float GetTimerNormalized()
        {
            return colorTimer / maxTimer;
        }
    }
}
