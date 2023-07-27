using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

public class MovementStackWindow : OdinEditorWindow
{


    private MovementStackWindow window;

    [InlineEditor]
    public MovementStack m_stack;

    [MenuItem("Tools/ChromaShift/Window/Movement Stack Editor")]
    public static void OpenWindow()
    {
       
        MovementStackWindow window = (MovementStackWindow)EditorWindow.GetWindow(typeof(MovementStackWindow), false, "Movement Stack Editor");
        Debug.Log("Let\'s Open the Window.");

    }

    [PropertyOrder(-10)]
    [HorizontalGroup]
    [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
    public void New() {
        LevelEditorManager.CreateNewMovementStack();
    }

    [HorizontalGroup]
    [Button(ButtonSizes.Large), GUIColor(0.3f, 0.9f, 1)]
    public void Duplicate() {
        MovementStack currentStack = Resources.Load<MovementStack>("MovementStacks/MS-" + m_stack.ID);
        MovementStack clone = Instantiate(currentStack) as MovementStack;

        string assetName = LevelEditorManager.GetAssetName();

        AssetDatabase.CreateAsset(clone, "Assets/ChromaShift/Resources/MovementStacks/MS-" + assetName + ".asset");
        AssetDatabase.SaveAssets();
        clone.ID = assetName;
        m_stack = clone;
    }

    [HorizontalGroup]
    [Button(ButtonSizes.Large), GUIColor(1, 0.2f, 0)]
    public void ClearStack() {
        if (!EditorUtility.DisplayDialog("Clear Movement Stack", "Do you want to remove all movement blocks from this stack? *NOTE* THIS CANNOT BE UNDONE!", "DELETE BLOCKS", "CANCEL"))
        {
            return;
        }
        m_stack.MoveStack = null;
    }

    
}

