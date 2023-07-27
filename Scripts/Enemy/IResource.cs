namespace ChromaShift.Scripts.Enemy
{
    public interface IResource
    {
        GameColor GetColor();
        string GetPrefabPath();
        bool IsAttached();
    }
}