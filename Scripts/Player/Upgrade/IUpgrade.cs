namespace ChromaShift.Scripts.Player.Upgrade
{
    /// <summary>
    /// This is to allow for Lists of the generic interface version
    /// </summary>
    public interface IUpgrade
    {
        /// <summary>
        /// Moving this here to allow it's usage within the factory
        /// </summary>
        void Remove();
    }

    public interface IUpgrade<T> : IUpgrade
    {
        T Modifier { get; set; }
        
        void Awake();
//        void Start();
        void OnDestroy();
    }
}