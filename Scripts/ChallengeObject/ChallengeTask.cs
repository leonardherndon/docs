using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace ChromaShift.Scripts.ChallengeObject
{
    public class ChallengeTask : MonoBehaviour
    {
        [SerializeField] protected ChallengeObjectManager COM;

        [SerializeField] private ChallengeState state = ChallengeState.Active;
        [SerializeField] protected string challengeCode;
        [Range(-10f, 10f)]
        [SerializeField] protected float pointsCurrent = 0f;
        [SerializeField] protected float pointsMax = 10f;
        [SerializeField] protected float pointsFail = -10f;
        [SerializeField] protected float pointsFillRate = 1f;
        [SerializeField] protected float pointsDecayRate = 0f;
        [SerializeField] protected float decayMultiplier = 0f;
        [FormerlySerializedAs("colorDecayMultiplier")]
        [Range(0f, 5f)]
        [SerializeField] protected float colorDecayAdditive = 0f;
        [SerializeField] protected bool decayActive = false;
        [SerializeField] protected bool decaySafety = false;
        [SerializeField] protected float decaySafetyPoint = 0f;
        [SerializeField] protected bool canFail = false;
        [SerializeField] protected bool colorCheck = false;
        [SerializeField] private GameObject target;

        [SerializeField] UnityEvent OnChallengeTaskEntered;
        [SerializeField] UnityEvent OnChallengeTaskExited;
        [SerializeField] UnityEvent OnChallengeTaskSuccess;
        [SerializeField] UnityEvent OnChallengeTaskFail;
        
        
        public virtual void Start()
        {
            COM = ChallengeObjectManager.Instance;
        }
        public virtual void FixedUpdate()
        {
            if(state != ChallengeState.Active)
                return;
            
            if (pointsCurrent >= pointsMax)
            {
                OnSuccess();
                return;
            }

            if (decayActive)
            {
                pointsDecayRate = pointsDecayRate + (decayMultiplier * pointsDecayRate);
                pointsCurrent -= pointsDecayRate;
                if (decaySafety)
                {
                    if (pointsCurrent < decaySafetyPoint)
                        pointsCurrent = decaySafetyPoint;
                }
            }

            if (pointsCurrent <= pointsFail && canFail)
            {
                OnFailure();
                return;
            }
            
        }
        
        public virtual void OnSuccess()
        {
            InputSystem.OnContextButtonPress -= IncrementValue;
            Debug.Log(name + ": OnSuccess Fired");
            COM.CTSuccess(ChallengeObjectManager.Instance.challengeObjects, challengeCode);
            state = ChallengeState.Completed;
            OnChallengeTaskSuccess.Invoke();
        }

        public virtual void OnFailure()
        {
            InputSystem.OnContextButtonPress -= IncrementValue;
            Debug.Log(name + ": OnFailure Fired");
            state = ChallengeState.Failed;
            OnChallengeTaskFail.Invoke();
        }

        public virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnChallengeTaskEntered.Invoke();
                target = other.gameObject;
                InputSystem.OnContextButtonPress += IncrementValue;
                
                if (colorCheck)
                {
                    if (ColorManager.Instance.isColorStrongerOrEqual(other.gameObject, gameObject))
                    {
                        decayMultiplier = 0;
                    }
                    else
                    {
                        decayMultiplier = colorDecayAdditive;
                        Debug.Log("Failed Color Check on Increment");
                    }
                }

                if (decaySafety)
                    decaySafety = false;
                Debug.Log(name + ": OnTriggerEnter Fired");
            }
        }

        public virtual void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnChallengeTaskExited.Invoke();
                InputSystem.OnContextButtonPress -= IncrementValue;
                Debug.Log(name + ": OnTriggerExit Fired");
            }
        }

        public virtual void CalculateResult()
        {
            Debug.Log(name + ": CalculateResult Fired");
        }

        public virtual void IncrementValue()
        {
            if(!target) 
                return;
            
            if (colorCheck)
            {
                if (ColorManager.Instance.isColorStrongerOrEqual(target, gameObject))
                {
                    decayMultiplier = 0;
                    pointsCurrent += pointsFillRate;
                    Debug.Log(name + ": IncrementValue Fired | " + pointsCurrent);
                }
                else
                {
                    decayMultiplier = colorDecayAdditive;
                    Debug.Log("Failed Color Check on Increment");
                }
            }
        }
    }
}
