using UnityEngine;

namespace ColorOrCrash.Global.Models
{
    [CreateAssetMenu(fileName = "SceneDatabase", menuName = "Scriptable Objects/SceneDatabase")]
    public class SceneDatabase : ScriptableObject
    {
        [Scene]
        public string MainMenuScene;
        [Scene]
        public string GameScene;
        [Scene]
        public string TutorialScene;
    }
}
