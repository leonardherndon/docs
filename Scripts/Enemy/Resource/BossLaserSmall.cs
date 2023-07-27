using Sirenix.OdinInspector;

namespace ChromaShift.Scripts.Enemy.Resource
{
    [System.Serializable]
    public class BossLaserSmall : IResource
    {
        [ShowInInspector]
        private readonly GameColor _gameColor;

        public BossLaserSmall(GameColor gameColor)
        {
            _gameColor = gameColor;
        }

        public BossLaserSmall()
        {
        }

        public GameColor GetColor()
        {
            return _gameColor;
        }

        public string GetPrefabPath()
        {
            return "BossArsenal/BossLaser_Small";
        }

        public bool IsAttached()
        {
            return true;
        }
    }
}