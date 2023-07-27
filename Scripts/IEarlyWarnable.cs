namespace ChromaShift.Scripts
{
    public interface IEarlyWarnable
    {
        /// <summary>
        /// This is to indicate that the object can raise a warning.
        /// </summary>
        /// <returns></returns>
        bool IsWarnable();
    }
}