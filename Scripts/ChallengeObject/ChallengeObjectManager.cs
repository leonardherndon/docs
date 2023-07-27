using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ChromaShift.Scripts.ChallengeObject
{
    public class ChallengeObjectManager : MonoBehaviour
    {
        private static ChallengeObjectManager _instance;
        
        #if UNITY_EDITOR
        
        [PropertyOrder(0)]
        [Button(ButtonSizes.Gigantic), GUIColor(0.45f, .65f, 1f)]
        public void BuildList()
        {
            challengeObjects = Resources.LoadAll<ChallengeObject>("ChallengeObjects");
        }
        
        
        [PropertyOrder(1)]
        [Button(ButtonSizes.Large), GUIColor(0.3f, 1f, 0.6f)]
        private void NewChallenge()
        {
            LevelEditorManager.CreateChallengeObject();
        }
        #endif
        
        
        [PropertyOrder(2)]
        [InlineProperty]
        [ListDrawerSettings(NumberOfItemsPerPage = 10), LabelText("CHALLENGE OBJECTS")]
        [SerializeField] public ChallengeObject[] challengeObjects;
        
        public delegate void OnChallengeStateChangeHandler(ChallengeObject challengeObject);

        public event OnChallengeStateChangeHandler OnChallengeStateChange = delegate(ChallengeObject cO) {  };
        
        public static ChallengeObjectManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Object.FindObjectOfType(typeof(ChallengeObjectManager)) as ChallengeObjectManager;

                    if (_instance == null)
                    {
                        GameObject go = new GameObject("_challengeManager");
                        DontDestroyOnLoad(go);
                        _instance = go.AddComponent<ChallengeObjectManager>();
                    }
                }

                return _instance;
            }
        }
        
        
        //SAVING AND LOADING CHALLENGES WILL BE DONE AGAINST THE MANAGER'S LIST OF CHALLENGES. 

        public void SaveChallengeProgress()
        {
            
            foreach (ChallengeObject challenge in challengeObjects)
            {
                var testInt = PlayerPrefs.GetInt("Challenge_" + challenge.challengeCode + "_PointsCurrent",-1);

                if (testInt != -1)
                {
                    PlayerPrefs.SetInt("Challenge_" + challenge.challengeCode + "_PointsCurrent", challenge.pointsCurrent);
                }
            }
        }

        public void LoadChallengeProgress()
        {
            foreach (ChallengeObject challenge in challengeObjects)
            {
                var points = PlayerPrefs.GetInt("Challenge_" + challenge.challengeCode + "_PointsCurrent",-1);

                if (points != -1)
                {
                    challenge.pointsCurrent = points;
                }
            }
        }
        
        public void SaveChallengeStatus()
        {
            foreach (ChallengeObject challenge in challengeObjects)
            {
                var testString = PlayerPrefs.GetString("Challenge_" + challenge.challengeCode + "_Status","none");

                if (testString != "none")
                {
                    PlayerPrefs.SetString("Challenge_" + challenge.challengeCode + "_Status", challenge.challengeState.ToString());
                }
            }
        }

        public void LoadChallengeStatus()
        {
            foreach (ChallengeObject challenge in challengeObjects)
            {
                var currentState = PlayerPrefs.GetString("Challenge_" + challenge.challengeCode + "_Status","none");

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
            }
        }
        
        
        public ChallengeState CheckChallengeStatus(ChallengeObject[] challengeList, String challengeCode = null)
        {
            foreach (ChallengeObject challengeObject in challengeList)
            {
                if (challengeCode != challengeObject.challengeCode)
                    continue;
                else
                {
                    return challengeObject.challengeState;
                }
            }

            return ChallengeState.Null;
        }
        
        public bool CheckAgainstChallengeStatus(ChallengeObject[] challengeList, String challengeCode = null, ChallengeState stateCheck = ChallengeState.Null)
        {
            if (challengeCode == null)
                return false;
            
            ChallengeState currentState = CheckChallengeStatus(challengeList, challengeCode);
            if (currentState == stateCheck)
                return true;
            
            return false;
        }
        
        public void CTSuccess(ChallengeObject[] challengeList = null, String challengeCode = null)
        {
            if (challengeCode == null)
                return;

            if (challengeList == null)
            {
                challengeList = challengeObjects;
            }

            foreach (ChallengeObject challengeObject in challengeList)
            {
                if (challengeCode != challengeObject.challengeCode)
                    continue;
                
                challengeObject.pointsCurrent++;
                if (challengeObject.pointsCurrent >= challengeObject.pointsMax && challengeObject.challengeState == ChallengeState.Active)
                {
                    challengeObject.pointsCurrent = challengeObject.pointsMax;
                    ChangeChallengeState(challengeObject,ChallengeState.Completed);
                }
            }
        }

        public void ActivateChallenges(ChallengeObject[] challengeList)
        {
            foreach (ChallengeObject challenge in challengeList)
            {
                if(challenge.challengeState == ChallengeState.Unassigned)
                {
                    challenge.pointsCurrent = 0;
                    ChangeChallengeState(challenge, ChallengeState.Active);
                }
            }
        }

        

        public void UnassignChallenges(ChallengeObject[] challengeList)
        {
            foreach (ChallengeObject challenge in challengeList)
            {
                if (challenge.challengeState == ChallengeState.Active)
                {
                    challenge.pointsCurrent = 0;
                    ChangeChallengeState(challenge, ChallengeState.Unassigned);
                }
            }
        }
        
        
        public void ActivateChallengeObject(string challengeCode)
        {
            foreach (ChallengeObject challengeObject in challengeObjects)
            {
                if (challengeCode != challengeObject.challengeCode)
                    continue;
                if(challengeObject.challengeState != ChallengeState.Unassigned)
                    continue;
                
                challengeObject.pointsCurrent = 0;
                ChangeChallengeState(challengeObject,ChallengeState.Active);
            }
        }
        
        public void UnassignChallengeObject(string challengeCode)
        {
            foreach (ChallengeObject challengeObject in challengeObjects)
            {
                if (challengeCode != challengeObject.challengeCode)
                    continue;
                if(challengeObject.challengeState == ChallengeState.Completed)
                    continue;
                
                challengeObject.pointsCurrent = 0;
                ChangeChallengeState(challengeObject,ChallengeState.Unassigned);
            }
        }

        public void ChangeChallengeState(ChallengeObject cO, ChallengeState state)
        {
            cO.challengeState = state;
            OnChallengeStateChange?.Invoke (cO);
        }
        
        

        public void ResetChallenges(ChallengeScope scope = ChallengeScope.Solo, String resetCode = "xx-xx-xx-xx")
        {
            
            String[] spearator = {"-"};

            // using the method 
            String[] resetCodes = resetCode.Split(spearator, 4, 
                StringSplitOptions.RemoveEmptyEntries);

            foreach (ChallengeObject challenge in challengeObjects)
            {
               
                String[] challengeCodes = challenge.challengeCode.Split(spearator, 4, 
                    StringSplitOptions.RemoveEmptyEntries);
                
                if (challenge.challengeScope == scope)
                {
                    switch (scope)
                    {
                        case ChallengeScope.Solo:
                            if (challenge.challengeCode == resetCode)
                            {
                                challenge.pointsCurrent = 0;
                                ChangeChallengeState(challenge, ChallengeState.Unassigned);
                            }
                            break;
                        case ChallengeScope.Scene:
                            
                            if (resetCodes[0] == challengeCodes[0] && resetCodes[1] == challengeCodes[1] && resetCodes[2] == challengeCodes[2])
                            {
                                challenge.pointsCurrent = 0;
                                ChangeChallengeState(challenge, ChallengeState.Unassigned);
                            }
                            break;
                
                        case ChallengeScope.Sector:
                            if (resetCodes[0] == challengeCodes[0] && resetCodes[1] == challengeCodes[1])
                            {
                                challenge.pointsCurrent = 0;
                                ChangeChallengeState(challenge, ChallengeState.Unassigned);
                            }
                            break;
                        case ChallengeScope.Tier:
                            if (resetCodes[0] == challengeCodes[0])
                            {
                                challenge.pointsCurrent = 0;
                                ChangeChallengeState(challenge, ChallengeState.Unassigned);
                            }
                            break;
                    }
                    
                }
            }
            
        }
    }
}
