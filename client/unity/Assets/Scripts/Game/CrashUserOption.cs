using UnityEngine;

namespace Game
{
    public static class CrashUserOption
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void LoadUserOption()
        {
            mute = PlayerPrefs.GetInt(nameof(mute), 0) != 0;
            muteMusic = PlayerPrefs.GetInt(nameof(muteMusic), 0) != 0;
            muteEffect = PlayerPrefs.GetInt(nameof(muteEffect), 0) != 0;
        }

        public static void SaveUserOption()
        {
            PlayerPrefs.SetInt(nameof(mute), mute? 1: 0);
            PlayerPrefs.SetInt(nameof(muteMusic), muteMusic? 1: 0);
            PlayerPrefs.SetInt(nameof(muteEffect), muteEffect? 1: 0);
        }
        
        public static bool mute { get; set; }
        public static bool muteMusic { get; set; }
        public static bool muteEffect { get; set; }
    }
}