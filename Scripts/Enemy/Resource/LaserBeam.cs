using Sirenix.OdinInspector;

namespace ChromaShift.Scripts.Enemy.Resource
{
    [System.Serializable]
    public class LaserBeam : IResource
    {
        [ShowInInspector]
        private readonly GameColor _gameColor;

        public LaserBeam(GameColor gameColor)
        {
            _gameColor = gameColor;
        }

        public LaserBeam()
        {
        }

        public GameColor GetColor()
        {
            return _gameColor;
        }

        public string GetPrefabPath()
        {
            return "BossArsenal/LaserBeam";
        }

        public bool IsAttached()
        {
            return true;
        }
    }
}