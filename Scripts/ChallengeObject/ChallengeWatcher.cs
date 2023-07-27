using System;
using UnityEngine;
using UnityEngine.Events;

namespace ChromaShift.Scripts.ChallengeObject
{
   
    
    public class ChallengeWatcher : MonoBehaviour
    {

        [Serializable]
        public class ChallengeCompleteEvent : UnityEvent<GameObject> {}
        
        [SerializeField]
        private ChallengeCompleteEvent onComplete = new ChallengeCompleteEvent();
        
        [SerializeField]
        private String challengeCode;

        [ShowOnly]
        public ChallengeState currentChallengeState;
        public bool isChallengeCompleted = false;
        
        void Start()
        {
            
            currentChallengeState = ChallengeObjectManager.Instance.CheckChallengeStatus(ChallengeObjectManager.Instance.challengeObjects, challengeCode);

            if (currentChallengeState == ChallengeState.Completed)
            {
                onComplete.Invoke(null);
            }
            
            if (currentChallengeState == ChallengeState.Active)
            {
                isChallengeCompleted = ChallengeObjectManager.Instance.CheckAgainstChallengeStatus(ChallengeObjectManager.Instance.challengeObjects, challengeCode,ChallengeState.Completed);
                
                if (isChallengeCompleted)
                    onComplete.Invoke(null);
                
            }
            
            
        }
        
    }
}
