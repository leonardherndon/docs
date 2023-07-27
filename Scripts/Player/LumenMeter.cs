namespace ChromaShift.Scripts.Player
{
    public class LumenMeter: ILumenMeter
    {
        private float _lumens;
        
        public float Lumens { get => _lumens; }
        public void Add(float lumens)
        {
            _lumens += lumens;
        }

        public void Subtract(float lumens)
        {
            _lumens -= lumens;
        }
    }
}