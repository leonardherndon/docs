namespace ChromaShift.Scripts.ChallengeObject
{
    public class CTAvoidDetection : ChallengeTask
    {
        
        public void FixedUpdate()
        {
            if (pointsCurrent >= pointsMax)
            {
                OnSuccess();
                return;
            }
            
            if (pointsCurrent < 0)
            {
                OnFailure();
                return;
            }
            
            if(decayActive)
                pointsCurrent -= pointsDecayRate;
            
            if (pointsCurrent <= 0)
                pointsCurrent = 0;
        }
    }
}
