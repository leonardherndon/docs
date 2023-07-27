namespace ChromaShift.Scripts.Enemy.Resource
{
    [System.Serializable]
    public class LaserTrap : IResource
    {
        public GameColor GetColor()
        {
            return GameColor.Red;
        }

        public string GetPrefabPath()
        {
            return "YumePrefabs/Obstacles/LaserTrap";
        }

        public bool IsAttached()
        {
            return false;
        }
    }
}