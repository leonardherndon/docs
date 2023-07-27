using System;
using System.Collections;
using System.Collections.Generic;
using uGUIPanelManager;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;

public class CutSceneController : MonoBehaviour
{
    public PlayableDirector director;
    public RectTransform dialoguePanel;
    public GameObject gamePlayPanel;
    private void Awake()
    {
        if (!director)
            director = GetComponent<PlayableDirector>();
      
    }

    public void StartCutScene()
    {
        if (!dialoguePanel)
            dialoguePanel = GameObject.Find("DialogueUI").GetComponent<RectTransform>();
        if (!gamePlayPanel)
            gamePlayPanel =  GameObject.Find("AM_GamePlay");
        Debug.Log("CutScene Started");
        uGUIManager.SetPanelState("Panel_CutSceneBars", PanelState.Show, additional: true, queued: false, instant: false);
        gamePlayPanel.GetComponent<CanvasGroup>().DOFade(0f, 1f);
        gamePlayPanel.GetComponent<RectTransform>().DOMoveY(gamePlayPanel.GetComponent<RectTransform>().position.y + 100f, 1, false);
        dialoguePanel.DOMoveY(dialoguePanel.position.y + 100f, 1, false);
        
    }
    
    public void EndCutScene()
    {
        if (!dialoguePanel)
            dialoguePanel = GameObject.Find("DialogueUI").GetComponent<RectTransform>();
        if (!gamePlayPanel)
            gamePlayPanel =  GameObject.Find("AM_GamePlay");
        Debug.Log("CutScene Stopped.");
        uGUIManager.SetPanelState("Panel_CutSceneBars", PanelState.Hide, additional: true, queued: false, instant: false);
        gamePlayPanel.GetComponent<CanvasGroup>().DOFade(1f, 1f);
        gamePlayPanel.GetComponent<RectTransform>().DOMoveY(gamePlayPanel.GetComponent<RectTransform>().position.y - 100f, 1, false);
        dialoguePanel.DOMoveY(dialoguePanel.position.y - 100f, 1, false);
    }
}
 