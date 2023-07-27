using UnityEngine;

namespace ChromaShift.Settings
{
    public interface IKeybind
    {
        KeyCode MoveUpKey();
        KeyCode MoveDownKey();
        KeyCode ShiftUpKey();
        KeyCode ShiftDownKey();
    }
}