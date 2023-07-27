using UnityEngine;

namespace ChromaShift.Settings
{
    public class KeyboardKeybind : IKeybind
    {
        public KeyCode MoveUpKey()
        {
            return KeyCode.UpArrow;
        }

        public KeyCode MoveDownKey()
        {
            return KeyCode.DownArrow;
        }

        public KeyCode ShiftUpKey()
        {
            return KeyCode.RightArrow;
        }

        public KeyCode ShiftDownKey()
        {
            return KeyCode.LeftArrow;
        }
    }
}