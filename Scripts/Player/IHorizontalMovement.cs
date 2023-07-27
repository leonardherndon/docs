namespace ChromaShift.Scripts.Player
{
    public interface IHorizontalMovement
    {
        /// <summary>
        /// This is for the base speed that the ship will move at in neutral.
        /// </summary>
        /// <returns></returns>
        float GetBaseSpeed();
        
        /// <summary>
        /// This is the current acceleration rate.
        /// </summary>
        /// <returns></returns>
        float GetAccelerationRate();
        
        /// <summary>
        /// This is the current deceleration rate.
        /// </summary>
        /// <returns></returns>
        float GetDecelerationRate();
        
        /// <summary>
        /// This is what the maximum speed can be.
        /// </summary>
        /// <returns></returns>
        float GetMaxSpeed();
        
        /// <summary>
        /// This is the slowest speed can be.
        /// </summary>
        /// <returns></returns>
        float GetMinSpeed();
        
        /// <summary>
        /// This is what the enhanced acceleration rate currently is.
        /// </summary>
        /// <returns></returns>
        float GetEnhancedAccelerationRate();
        
        /// <summary>
        /// This is what the enhanced deceleration rate currently is.
        /// </summary>
        /// <returns></returns>
        float GetEnhancedDecelerationRate();
        
        /// <summary>
        /// This is what the maximum speed can be.
        /// </summary>
        /// <returns></returns>
        float GetEnhancedMaxSpeed();
        
        /// <summary>
        /// This is what the enhanced minimum speed can be
        /// </summary>
        /// <returns></returns>
        float GetEnhancedMinSpeed();
        
        /// <summary>
        /// This is the maximum distance that the speed can travel backwards.
        /// The maximum distance should be used to set a y-value as the mark
        /// for the maximum distance the ship can move backwards. From this
        /// y-value plus the max reverse distance will equal the point where it
        /// can be reset.
        /// </summary>
        /// <returns></returns>
        float GetMaximumReverseDistance();
    }
}