using UnityEngine;
using UnityEngine.Events;

public interface IColorNodeGroup
{
    ColorNodeSet[] NodeGroup { get; set; }
    void RunNodeColorChecks(GameColor newColor);
    UnityEvent Unlocked { get; set; }
}