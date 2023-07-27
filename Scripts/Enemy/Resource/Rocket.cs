namespace ChromaShift.Scripts.Enemy.Resource
{
    [System.Serializable]
    public class Rocket : IResource
    {
        public GameColor GetColor()
        {
            return GameColor.Red;
        }

        public string GetPrefabPath()
        {
            return "YumePrefabs/Obstacles/RocketTrap";
        }

        public bool IsAttached()
        {
            return false;
        }
    }
}