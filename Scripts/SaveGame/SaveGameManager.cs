using System;
using ChromaShift.Scripts.ChallengeObject;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ChromaShift.Scripts.SaveGame
{
    public class SaveGameManager : MonoBehaviour
    {
        private static SaveGameManager _instance;

        public static SaveGameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Object.FindObjectOfType(typeof(SaveGameManager)) as SaveGameManager;

                    if (_instance == null)
                    {
                        GameObject go = new GameObject("_saveDataManager");
                        DontDestroyOnLoad(go);
                        _instance = go.AddComponent<SaveGameManager>();
                    }
                }

                return _instance;
            }
        }


        public void SaveMasterData()
        {
            PlayerPrefs.SetInt("GameManager_currentSaveDataObject", GameManager.Instance.currentSaveDataIndex);
        }

        public void SelectGameSaveProfile(int saveIndex)
        {
            PlayerPrefs.SetInt("GameManager_currentSaveDataObject", saveIndex);
        }

        
        public void LoadMasterData()
        {
            GameManager.Instance.currentSaveDataIndex = PlayerPrefs.GetInt("GameManager_currentSaveDataObject");
            /*var SaveDataList = Resources.LoadAll("SaveData");
            
            foreach(SaveGameData saveData in SaveDataList)
            {
                String[] spearator = {"_"};
                string saveName = saveData.name.ToString().Split(spearator, 4, 
                    StringSplitOptions.RemoveEmptyEntries)[0];
                //Debug.Log("SaveName: "+ saveName);
                if (GameManager.Instance.currentSaveDataIndex == int.Parse(saveName))
                {
                    GameManager.Instance.currentSaveDataObject = saveData;
                    return;
                }
            }*/
        }
        
        //SAVE GAME DATA
        public void SaveGameData(int saveIndex)
        {
            SaveGameDataInts(saveIndex);
            SaveGameDataStrings(saveIndex);
            SaveGameDataChallenges(saveIndex);
        }
        

        private void SaveGameDataInts(int saveIndex)
        {
            PlayerPrefs.SetInt(saveIndex + "_GameManager_currentSceneIndex", GameManager.Instance.currentSceneIndex);
            PlayerPrefs.SetInt(saveIndex + "_GameManager_currentCheckPointIndex", GameManager.Instance.checkPointIndex);
            PlayerPrefs.SetInt(saveIndex + "_GameManager_currentDeathCount", GameManager.Instance.currentDeathCount);
            PlayerPrefs.SetInt(saveIndex + "_GameManager_currentTimePlayed", GameManager.Instance.currentTimePlayed);
        }

        private void SaveGameDataStrings(int saveIndex)
        {
            PlayerPrefs.SetString(saveIndex + "_GameManager_currentLevelID", GameManager.Instance.currentLevelID);
        }

        private void SaveGameDataChallenges(int saveIndex)
        {
            foreach (ChallengeObject.ChallengeObject challenge in ChallengeObjectManager.Instance.challengeObjects)
            {
                PlayerPrefs.SetInt(saveIndex + "_Challenge_" + challenge.challengeCode + "_PointsCurrent", challenge.pointsCurrent);
                PlayerPrefs.SetString(saveIndex + "_Challenge_" + challenge.challengeCode + "_Status", challenge.challengeState.ToString());
            }
        }

        
        //LOAD GAME DATA
        public void LoadGameData(int saveIndex)
        {
            LoadGameDataInts(saveIndex);
            LoadGameDataStrings(saveIndex);
            LoadGameDataChallenges(saveIndex);
        }

        private void LoadGameDataInts(int saveIndex)
        {
            
            GameManager.Instance.currentSceneIndex = PlayerPrefs.GetInt(saveIndex + "_GameManager_currentSceneIndex");
            GameManager.Instance.checkPointIndex = PlayerPrefs.GetInt(saveIndex + "_GameManager_currentCheckPointIndex");
            GameManager.Instance.currentDeathCount = PlayerPrefs.GetInt(saveIndex + "_GameManager_currentDeathCount");
            GameManager.Instance.currentTimePlayed = PlayerPrefs.GetInt(saveIndex + "_GameManager_currentTimePlayed");
            
        }

        private void LoadGameDataStrings(int saveIndex)
        {
            GameManager.Instance.currentLevelID = PlayerPrefs.GetString(saveIndex + "_GameManager_currentLevelID");
        }
        private void LoadGameDataChallenges(int saveIndex)
        {
            foreach (ChallengeObject.ChallengeObject challenge in ChallengeObjectManager.Instance.challengeObjects)
            {
                //LOAD CHALLENGE STATE
                var currentState = PlayerPrefs.GetString(saveIndex + "_Challenge_" + challenge.challengeCode + "_Status","none");

                switch (currentState)
                {
                    case "null":
                        challenge.challengeState = ChallengeState.Null;
                        break;
                    case "unassigned":
                        challenge.challengeState = ChallengeState.Unassigned;
                        break;
                    case "active":
                        challenge.challengeState = ChallengeState.Active;
                        break;
                    case "completed":
                        challenge.challengeState = ChallengeState.Completed;
                        break;
                }
                
                //LOAD CHALLENGE PROGRESS
                var points = PlayerPrefs.GetInt(saveIndex + "_Challenge_" + challenge.challengeCode + "_PointsCurrent",-1);
                challenge.pointsCurrent = points;
                
            }
        }
    }
}