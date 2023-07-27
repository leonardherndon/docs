public interface IGameTimers
{

    bool timerActive { get; set; }
    
    float timerTime { get; set; }
    
    
    void StartTimer();

    void StopTimer();

    void InitTimer();
}