namespace ChromaShift.Scripts.Managers
{
    public interface IGameState
    {
        GameStateType StateType { get; set; }
        
        void EnterState();
        void UpdateState();
        void ExitState();
    }
}