namespace ChromaShift.Scripts.Player
{
    public interface ILumenMeter
    {
        float Lumens { get; }
        void Add(float lumens);
        void Subtract(float lumens);
    }
}