using System;

namespace ChromaShift.Scripts
{
    [Serializable]
    public class EarlyWarnable : IEarlyWarnable
    {
        private readonly bool _isWarnable = true;

        public EarlyWarnable(bool isWarnable)
        {
            _isWarnable = isWarnable;
        }

        public bool IsWarnable()
        {
            return _isWarnable;
        }
    }
}