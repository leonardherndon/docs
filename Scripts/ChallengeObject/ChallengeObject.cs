using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ChromaShift.Scripts.ChallengeObject
{
    public enum ChallengeScope
    {
        Solo,
        Scene,
        Sector,
        Tier,
        Global
    }

    public enum ChallengeState
    {
        Unassigned,
        Active,
        Completed,
        Failed,
        Null
    }
    
    public class ChallengeObject : ScriptableObject
    {
        public ChallengeScope challengeScope;
        public ChallengeState challengeState;
        public String challengeCode; //Tier_Sector_Scene_ID
        [HideInInspector] private String _codeCached;
        public string description;
        
        public int pointsMax;
        public int pointsCurrent;

        
        #if UNITY_EDITOR
        
        [PropertyOrder(-1)]
        [Button(ButtonSizes.Gigantic), GUIColor(0.4f, 1f, 0.4f)]
        private void SaveChallenge()
        {
            int dupCheckCount = 0;
            
            if (challengeCode == null)
            {
                Debug.LogError("Challenge Object does not have a code. Please update and try again.");
                return;
            }

            ChallengeObjectManager.Instance.BuildList();
            if (ChallengeObjectManager.Instance.challengeObjects.Length == 0)
            {
                Debug.LogError(
                    "No Challenges Available to build list. Please Fix.");
                return;
            }
            
            foreach (ChallengeObject challenge in ChallengeObjectManager.Instance.challengeObjects)
            {
                if (challenge.challengeCode == challengeCode)
                {
                    dupCheckCount++;
                    if (dupCheckCount > 1)
                    {
                        Debug.LogError(
                            "Challenge Object shares the same code as another. Please update and try again.");
                        return;
                    }
                }
            }

            for (int i = 0; i < 3; i++)
            {
                PlayerPrefs.DeleteKey(i +"_Challenge_" + _codeCached + "_Status");
                PlayerPrefs.SetString(i +"_Challenge_" + challengeCode + "_Status", "Incomplete");

                PlayerPrefs.DeleteKey(i +"_Challenge_" + _codeCached + "_PointsCurrent");
                PlayerPrefs.SetInt(i +"_Challenge_" + challengeCode + "_PointsCurrent", 0);
            }

            AssetDatabase.RenameAsset("Assets/ChromaShift/Resources/ChallengeObjects/Challenge_"+_codeCached+".asset", "Challenge_"+challengeCode+".asset");
            AssetDatabase.Refresh();
            _codeCached = challengeCode;
        }

        [PropertyOrder(1)]
        [Button(ButtonSizes.Large), GUIColor(1f, 0.3f, 0.3f)]
        private void DeleteChallenge()
        {
            if (EditorUtility.DisplayDialog("Delete this Challenge from system?",
                "Are you sure you want to delete this challenge?" +
                "\n\n" +
                "[" + challengeCode + "]" +
                "\n\n" +
                "You cannot undo this.",
                "Delete", "Cancel"))
            {
                for (int i = 0; i < 3; i++)
                {
                    PlayerPrefs.DeleteKey(i + "_Challenge_" + challengeCode + "_Status");
                    PlayerPrefs.DeleteKey(i + "_Challenge_" + challengeCode + "_PointsCurrent");
                }

                AssetDatabase.DeleteAsset("Assets/ChromaShift/Resources/ChallengeObjects/Challenge_" + _codeCached +
                                          ".asset");
            }
        }
        #endif
        
    }
}
