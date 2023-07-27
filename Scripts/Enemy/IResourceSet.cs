namespace ChromaShift.Scripts.Enemy
{
    public interface IResourceSet
    {
        IResource GetBodyResource();
        IResource GetPortArmResource();
        IResource GetPortFlapResource();
        IResource GetStarboardArmResource();
        IResource GetStarboardFlapResource();
        float GetDeployTime();
        void IncreaseDeployTime(float time);
    }
}