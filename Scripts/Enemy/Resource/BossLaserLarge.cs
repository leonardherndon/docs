using Sirenix.OdinInspector;

namespace ChromaShift.Scripts.Enemy.Resource
{
    public class BossLaserLarge : IResource
    {
        [ShowInInspector]
        private readonly GameColor _gameColor;

        public BossLaserLarge(GameColor gameColor)
        {
            _gameColor = gameColor;
        }

        public BossLaserLarge()
        {
        }

        public GameColor GetColor()
        {
            return _gameColor;
        }

        public string GetPrefabPath()
        {
            return "BossArsenal/BossLaser_Large";
        }

        public bool IsAttached()
        {
            return true;
        }
    }
}