namespace ChromaShift.Scripts.Managers
{
    public interface ISubState
    {
        public IHasSubStates parentState { get; set; }
        public GameSubStateType StateSubType { get; set; }
        void EnterSubState();
        void UpdateSubState();
        void ExitSubState();
    }
}