using Sirenix.Serialization;
using UnityEngine;

[System.Serializable]
public class ColorNodeSet
{
    [SerializeField] public ColorNode ColorNode;
    public GameColor ColorIndex;

    public ColorNodeSet(ColorNode colorNode, GameColor colorIndex)
    {
        ColorNode = colorNode;
        ColorIndex = colorIndex;
    }
}