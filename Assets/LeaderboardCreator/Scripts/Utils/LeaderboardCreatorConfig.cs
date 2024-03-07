using UnityEngine;

namespace Dan
{
    public class LeaderboardCreatorConfig : ScriptableObject
    {
        public bool isUpdateLogsEnabled = true;
        
        public TextAsset leaderboardsFile;
#if UNITY_EDITOR
        public TextAsset editorOnlyLeaderboardsFile;
#endif
    }
}