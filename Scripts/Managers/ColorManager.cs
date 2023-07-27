using UnityEngine;
using System.Collections.Generic;
using ChromaShift.Scripts;

public class ColorManager : MonoBehaviour {

	private static ColorManager _instance;
	public List<Color> colorArray;
    public List<Color> hueColorArray;
    private enum ColorCompareState { Weaker, Stronger, Equal}
    //Singleton pattern implementation
    public static ColorManager Instance {
		get 
		{
			if (_instance == null) 
			{
				_instance = Object.FindObjectOfType (typeof(ColorManager)) as ColorManager;


				if (_instance == null)
				{
					GameObject go = new GameObject("_colormanager");
					DontDestroyOnLoad(go);
					_instance = go.AddComponent<ColorManager>();
				}
			}
			return _instance;
		}
	}
    
    public bool isColorStronger(GameObject a, GameObject b)
    {
        ColorCompareState result = CompareColors(a, b);
        if (result == ColorCompareState.Stronger)
            return true;
        else
            return false;
    }
    
    public bool isColorStrongerOrEqual(GameObject a, GameObject b)
    {
        ColorCompareState result = CompareColors(a, b);
        if (result == ColorCompareState.Stronger || result == ColorCompareState.Equal)
            return true;
        else
            return false;
    }

    public bool isColorEqual(GameObject a, GameObject b)
    {
        ColorCompareState result = CompareColors(a, b);
        if (result == ColorCompareState.Equal)
            return true;
        else
            return false;
    }

    public bool isColorWeaker(GameObject a, GameObject b)
    {
        ColorCompareState result = CompareColors(a, b);
        if (result == ColorCompareState.Weaker)
            return true;
        else
            return false;
    }

    public int GetColorMix(int a, int b)
    {
        return ColorMix(a, b);
    }
    /// <summary>
    /// THIS METHOD CHECKS TO SEE IF TWO OBJECTS COLORS MATCH OR IF ONE IS MORE POWERFUL THAN THE OTHER. 
    /// 4: BLACK
    /// 3: WHITE
    /// 2: YELLOW | PURPLE | CYAN
    /// 1: RED | GREEN | BLUE
    /// N: GREY
    /// -1: PINK // DEFAULT OR SOMETHING IS BROKEN
    /// </summary>
    /// <param name="firstObject"></param>
    /// <param name="secondObject"></param>
    /// <returns></returns>
    private ColorCompareState CompareColors(GameObject firstObject, GameObject secondObject)
    {
        //Debug.Log("Compare Colors: " + firstObject.name + " | " + secondObject.name);
        
        GameColor firstObjectColorIndex = firstObject.GetComponent<ChromaShiftManager>().CurrentColor;
        GameColor secondObjectColorIndex = secondObject.GetComponent<ChromaShiftManager>().CurrentColor;

        //Debug.Log("ColorManager -> CompareColors |  A: " + firstObjectColorIndex + ", B: " + secondObjectColorIndex);
        
        if (firstObjectColorIndex == GameColor.Grey && secondObjectColorIndex != firstObjectColorIndex)
        {
            return ColorCompareState.Weaker;
        }
        
        if (secondObjectColorIndex == GameColor.Grey && firstObjectColorIndex != secondObjectColorIndex)
        {
            return ColorCompareState.Stronger;
        }

        if (firstObjectColorIndex == secondObjectColorIndex)
        {
            return ColorCompareState.Equal;
        }


        switch (firstObjectColorIndex)
        {

            case GameColor.Red: //RED
                return ColorCompareState.Weaker;

            case GameColor.Green: //GREEN
                return ColorCompareState.Weaker;

            case GameColor.Blue: //BLUE
                return ColorCompareState.Weaker;

            case GameColor.Yellow: //YELLOW
                if (secondObjectColorIndex == GameColor.Red || secondObjectColorIndex == GameColor.Green)
                    return ColorCompareState.Stronger;
                else
                    return ColorCompareState.Weaker;

            case GameColor.Purple: //PURPLE
                if (secondObjectColorIndex == GameColor.Red || secondObjectColorIndex == GameColor.Blue)
                    return ColorCompareState.Stronger;
                else
                    return ColorCompareState.Weaker;

            case GameColor.Cyan: //CYAN
                if (secondObjectColorIndex == GameColor.Green || secondObjectColorIndex == GameColor.Blue)
                    return ColorCompareState.Stronger;
                else
                    return ColorCompareState.Weaker;

            case GameColor.White: //WHITE
                if (secondObjectColorIndex == GameColor.Red || secondObjectColorIndex == GameColor.Green || secondObjectColorIndex == GameColor.Blue ||
                    secondObjectColorIndex == GameColor.Yellow || secondObjectColorIndex == GameColor.Purple || secondObjectColorIndex == GameColor.Cyan)
                    return ColorCompareState.Stronger;
                else
                    return ColorCompareState.Weaker;

            case GameColor.Black: //BLACK
                return ColorCompareState.Stronger;

            default:
                Debug.LogWarning("ColorManager > CompareColor(): FirstObjectColorIndex is out of Range.");
                return ColorCompareState.Weaker;
        }

    }

    //METHOD IS INCOMPLETE. WILL FUNCTION FOR IT'S PURPOSE, BUT THERE ARE TONS OF HOLES THAT NEED TO BE ACCOUNTED FOR. #LEOISLAZYRIGHTNOW #KIDDING #IREALLYNEEDTOMOVEONFORTHISGAMEONDEADLINE
    private int ColorMix (int cIndex1, int cIndex2)
    {
        
        if(cIndex1 == cIndex2)
        {
            return cIndex1;   
        } else
        {
            if ((cIndex1 == 0) && (cIndex2 == 3))
                return 2;
            if ((cIndex1 == 0) && (cIndex2 == 4))
                return 5;
            if ((cIndex1 == 3) && (cIndex2 == 0))
                return 2;
            if ((cIndex1 == 3) && (cIndex2 == 4))
                return 10;
            if ((cIndex1 == 4) && (cIndex2 == 0))
                return 5;
            if ((cIndex1 == 4) && (cIndex2 == 3))
                return 10;
        }
        Debug.LogError("Color Mix was given colors that it could not handle.");
        return 9;
    }

    public Color GetColor(GameColor color)
    {
        Color outColor;
        switch (color)
        {
            case GameColor.Red:
                return colorArray[0];
                break;
            case GameColor.Orange:
                return colorArray[1];
                break;
            case GameColor.Yellow:
                return colorArray[2];
                break;
            case GameColor.Green:
                return colorArray[3];
                break;
            case GameColor.Blue:
                return colorArray[4];
                break;
            case GameColor.Purple:
                return colorArray[5];
                break;
            case GameColor.White:
                return colorArray[6];
                break;
            case GameColor.Black:
                return colorArray[7];
                break;
            case GameColor.Grey:
                return colorArray[8];
                break;
            case GameColor.Pink:
                return colorArray[9];
                break;
            case GameColor.Cyan:
                return colorArray[10];
                break;
            default:
                return colorArray[9];
                break;
        }

        return outColor;
    }

    public int ConvertColorToIndex(GameColor color)
    {
        switch (color)
        {
            case GameColor.Red:
                return 0;
            case GameColor.Orange:
                return 1;
            case GameColor.Yellow:
                return 2;
            case GameColor.Green:
                return 3;
            case GameColor.Blue:
                return 4;
            case GameColor.Purple:
                return 5;
            case GameColor.White:
                return 6;
            case GameColor.Black:
                return 7;
            case GameColor.Grey:
                return 8;
            case GameColor.Pink:
                return 9;
            case GameColor.Cyan:
                return 10;
            default:
                Debug.LogError("Default PINK[9] RETURNED");
                return 9;
        }
    }

    public GameColor ConvertIndexToGameColor(int index)
    {
        switch (index)
        {
            case 0:
                return GameColor.Red;
            case 1:
                return GameColor.Orange;
            case 2:
                return GameColor.Yellow;
            case 3:
                return GameColor.Green;
            case 4:
                return GameColor.Blue;
            case 5:
                return GameColor.Purple;
            case 6:
                return GameColor.White;
            case 7:
                return GameColor.Black;
            case 8:
                return GameColor.Grey;
            case 9:
                return GameColor.Pink;
            case 10:
                return GameColor.Cyan;
            default:
                Debug.LogError("Default PINK[9] RETURNED");
                return GameColor.Pink;
        }
        
    }
    
    public Color ConvertEnumToColor(GameColor color)
    {
        switch (color)
        {
            case GameColor.Red:
                return colorArray[0];
            case GameColor.Orange:
                return colorArray[1];
            case GameColor.Yellow:
                return colorArray[2];
            case GameColor.Green:
                return colorArray[3];
            case GameColor.Blue:
                return colorArray[4];
            case GameColor.Purple:
                return colorArray[5];
            case GameColor.White:
                return colorArray[6];
            case GameColor.Black:
                return colorArray[7];
            case GameColor.Grey:
                return colorArray[8];
            case GameColor.Pink:
                return colorArray[9];
            case GameColor.Cyan:
                return colorArray[10];
            default:
                Debug.LogError("Default PINK[9] RETURNED");
                return colorArray[9];
        }
    }


}
