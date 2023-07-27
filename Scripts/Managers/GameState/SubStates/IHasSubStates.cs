using System.Collections.Generic;

namespace ChromaShift.Scripts.Managers
{
    //SubStates can technically be used under multiple GameStates, but this should be avoided. If the need arises to use
    //a substate in multiple places then it's worth reviewing the GameState's organization.
    // It's also worth noting that the order of the substate within a list will matter depending on the GameState.
    public interface IHasSubStates
    {
        public List<BaseGameSubState> SubStateList { get; set; }
        
        void SwitchSubState(BaseGameSubState newSubState);
        void InitSubState(List<BaseGameSubState> SubStateList);
    }
}