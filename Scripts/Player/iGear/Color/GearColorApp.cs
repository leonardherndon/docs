using System;
using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts.Managers;
using ChromaShift.Scripts.ObjectAttributeSystem;
using ChromaShift.Scripts.Player.Upgrade;
using UnityEngine;
using Sirenix.OdinInspector;
using Chronos;
using CS_Audio;
using DG.Tweening;

public class GearColorApp : MonoBehaviour, IGearColorApp
{
    [SerializeField] 
    private GearSystem _gearSystem;
    protected PlayerShip playerShip;
    [SerializeField] private GameColor colorCache;
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private AttributeController _attributeController;
    [SerializeField]
    private float _abilityActivationTime = 0.4f;

    [SerializeField] private float _shieldedColorSwitchActivationCost = 10f;
    [SerializeField] private bool _isAbilityActive = false;
    public float WhiteColorResetTime;
    public float WhiteColorDisableTime;
    public bool isWhiteColorAllowed = true;
    private Manager _manager;
    private ChromaShiftManager _ShieldCSM;


    void Start()
    {
        playerShip = GameManager.Instance.playerShip;
        _gearSystem = GetComponent<GearSystem>();
        _gearSystem.colorSystem = this;
        _manager = GetComponent<Manager>();
        isWhiteColorAllowed = true;
        _attributeController = GetComponent<AttributeController>();

        _ShieldCSM = playerShip.shieldModel.GetComponent<ChromaShiftManager>();
    }

    private void FixedUpdate()
    {
        if (GameStateManager.Instance.CurrentState.StateType == GameStateType.Gameplay)
        {
            CheckColor();
        }
    }

    //Used by the radial menu. Called by UnityEvent within the inspector. Inspector event requires int for argument.
    public void RadialColorChange(int colorIndex = -1)
    {
        ChangeColor(ColorManager.Instance.ConvertIndexToGameColor(colorIndex));
    }
    
    public void ChangeColor(GameColor colorIndex)
    {
        
        if (playerShip.LS.CurrentHP <= 1)
        {
            Debug.Log("No battery Left. Cannot Switch Colors");
            playerShip.CantActivateAbility.PlayFeedbacks();
            return;
        }
        
        switch (colorIndex)
            {

                case GameColor.Red:
                    if (!_ShieldCSM.CheckForCore(GameColor.Red, playerShip.CoC.CSM.CoreInventory)) break;

                    _audioSource.pitch = 1.5f;
                    SetColor(GameColor.Red);
                    break;
                case GameColor.Yellow:
                    if (!_ShieldCSM.CheckForCore(GameColor.Red, playerShip.CoC.CSM.CoreInventory) ||
                        !_ShieldCSM.CheckForCore(GameColor.Green, playerShip.CoC.CSM.CoreInventory)) break;

                    _audioSource.pitch = 1.4f;
                    SetColor(GameColor.Yellow);
                    break;
                case GameColor.Green:
                    if (!_ShieldCSM.CheckForCore(GameColor.Green, playerShip.CoC.CSM.CoreInventory)) break;

                    _audioSource.pitch = 1.3f;
                    SetColor(GameColor.Green);
                    break;
                case GameColor.Blue:
                    if (!_ShieldCSM.CheckForCore(GameColor.Blue, playerShip.CoC.CSM.CoreInventory)) break;

                    _audioSource.pitch = 1.1f;
                    SetColor(GameColor.Blue);
                    break;
                case GameColor.Purple:
                    if (!_ShieldCSM.CheckForCore(GameColor.Red, playerShip.CoC.CSM.CoreInventory) ||
                        !_ShieldCSM.CheckForCore(GameColor.Blue, playerShip.CoC.CSM.CoreInventory)) break;

                    _audioSource.pitch = 1f;
                    SetColor(GameColor.Purple);
                    break;
                case GameColor.White: // white
                    if (!_ShieldCSM.CheckForCore(GameColor.Red, playerShip.CoC.CSM.CoreInventory) ||
                        !_ShieldCSM.CheckForCore(GameColor.Green, playerShip.CoC.CSM.CoreInventory) ||
                        !_ShieldCSM.CheckForCore(GameColor.Blue, playerShip.CoC.CSM.CoreInventory))
                    {
                        break;
                    }

                    _audioSource.pitch = 1.9f;
                    if (isWhiteColorAllowed && _manager.HasWhiteColorAbility)
                    {
                        if (playerShip.CSM.CurrentColor == GameColor.White)
                            return;

                        StartCoroutine(StartColorResetProcess(playerShip.CSM.CurrentColor, WhiteColorResetTime));
                        SetColor(GameColor.White);
                        StartCoroutine(WhiteColorDisable(WhiteColorDisableTime));
                    }

                    break;
                case GameColor.Cyan:
                    if (!_ShieldCSM.CheckForCore(GameColor.Green, playerShip.CoC.CSM.CoreInventory) ||
                        !_ShieldCSM.CheckForCore(GameColor.Blue, playerShip.CoC.CSM.CoreInventory)) break;

                    _audioSource.pitch = 1.2f;
                    SetColor(GameColor.Cyan);
                    break;
                default:
                    SetColor(GameColor.Grey);
                    break;
            }
    }
    
    
    public void CheckColor()
    {

        if (playerShip.CSM.CurrentColor == GameColor.Grey)
            return;


        switch (playerShip.CSM.CurrentColor)
        {
            case GameColor.Red:
                if (_ShieldCSM.CheckForCore(GameColor.Red, playerShip.CoC.CSM.CoreInventory))
                {
                    
                    SetColor(GameColor.Red);
                    return;
                }
                else
                {
                    Debug.Log("Fusion Core Inventory Does Not Support Switching to this color.");
                }

                break;
            case GameColor.Green:
                    
                if (_ShieldCSM.CheckForCore(GameColor.Green, playerShip.CoC.CSM.CoreInventory))
                {
                    
                    SetColor(GameColor.Green);
                    return;
                }
                else
                {
                    Debug.Log("Fusion Core Inventory Does Not Support Switching to this color.");
                }
                break;
            case GameColor.Blue:
                if (_ShieldCSM.CheckForCore(GameColor.Blue, playerShip.CoC.CSM.CoreInventory))
                {
                    
                    SetColor(GameColor.Blue);
                    return;
                }
                else
                {
                    Debug.Log("Fusion Core Inventory Does Not Support Switching to this color.");
                }

                break;
            case GameColor.Yellow:
                if (_ShieldCSM.CheckForCore(GameColor.Red, playerShip.CoC.CSM.CoreInventory) && _ShieldCSM.CheckForCore(GameColor.Green, playerShip.CoC.CSM.CoreInventory))
                {
                    
                    SetColor(GameColor.Yellow);
                    return;
                }
                else
                {
                   Debug.Log("Fusion Core Inventory Does Not Support Switching to this color.");
                }
                break;
            case GameColor.Purple:
                if (_ShieldCSM.CheckForCore(GameColor.Red, playerShip.CoC.CSM.CoreInventory) && _ShieldCSM.CheckForCore(GameColor.Blue, playerShip.CoC.CSM.CoreInventory))
                {
                    
                    SetColor(GameColor.Purple);
                    return;
                }
                else
                {
                    Debug.Log("Fusion Core Inventory Does Not Support Switching to this color.");
                }
                break;
            case GameColor.Cyan:
                if (_ShieldCSM.CheckForCore(GameColor.Green, playerShip.CoC.CSM.CoreInventory) && _ShieldCSM.CheckForCore(GameColor.Blue, playerShip.CoC.CSM.CoreInventory))
                {
                    
                    SetColor(GameColor.Cyan);
                    return;
                }
                else
                {
                    Debug.Log("Fusion Core Inventory Does Not Support Switching to this color.");
                }
                break;
            case GameColor.White:
                if (_ShieldCSM.CheckForCore(GameColor.Green, playerShip.CoC.CSM.CoreInventory) && _ShieldCSM.CheckForCore(GameColor.Blue, playerShip.CoC.CSM.CoreInventory) && _ShieldCSM.CheckForCore(GameColor.Red, playerShip.CoC.CSM.CoreInventory))
                {
                    
                    SetColor(GameColor.White);
                    return;
                }
                else
                {
                    Debug.Log("Fusion Core Inventory Does Not Support Switching to this color.");
                }

                break;

        }

        SetColor(GameColor.Grey);

    }

    public void SetColor(GameColor colorIndex)
    {
        if (colorCache == colorIndex)
            return;
        
        colorCache = playerShip.CSM.CurrentColor;
        
        StartCoroutine(nameof(ApplyColor), colorIndex);
    }

    public IEnumerator ApplyColor(GameColor colorIndex)
    {
        if(_isAbilityActive)
            yield break;

        _isAbilityActive = true;
        
        var delay = _attributeController.CalculateRecoveryAdjustment(_abilityActivationTime);
        
        yield return new WaitForSeconds(delay);
        
        playerShip.CoC.CSM.ChromaShift(colorIndex);
        playerShip.shieldModel.GetComponent<ChromaShiftManager>().ChromaShift(colorIndex);
        
        if(playerShip.isShieldActive)
            playerShip.LS.SpendLife(_shieldedColorSwitchActivationCost);
        
        if(colorIndex == GameColor.Grey)
            _audioSource.pitch = 0.75f;
        _audioSource.PlayOneShot(_audioSource.clip);

        _isAbilityActive = false;
    }

    private IEnumerator StartColorResetProcess(GameColor colorIndex, float time)
    {
        playerShip.PIC.DisableShieldAbility();
        playerShip.PIC.DisableColorShift();
        //playerShip.PIC.DisableTeleport();
        
        
        playerShip.animaFX.StartAnima();
        //playerShip.ActivateShield(time);
        isWhiteColorAllowed = false;
        var target = GameManager.Instance.mainCamera;
        var target1 = GameManager.Instance.mainCamera.CameraTargets[1];
        DOTween.To(() => target.OffsetX,
            x => target.OffsetX = x, 28f, 0.5f);

        
        playerShip.animaFX.ui.GetComponentInChildren<EnergyBar>().valueCurrent = 100;
        DOTween.To(()=> playerShip.animaFX.ui.GetComponentInChildren<EnergyBar>().valueCurrent, x=> playerShip.animaFX.ui.GetComponentInChildren<EnergyBar>().valueCurrent = x, 0, time);
        yield return new WaitForSeconds(time);
        playerShip.fusionCoreInventory.Clear();
        

        playerShip.GearMovementApp.SetSpeedGear(2, true);
        
        playerShip.animaFX.EndAnima();
        SetColor(colorIndex);
        playerShip.animaFX.ui.GetComponentInChildren<EnergyBar>().valueCurrent = 0;
        
        DOTween.To(() => target.OffsetX,
            x => target.OffsetX = x, 0f, 0.85f);
        //playerShip.animaFX.ui.gameObject.SetActive(false);
        playerShip.PIC.EnableShieldAbility();
        playerShip.PIC.EnableColorShift();
        //playerShip.PIC.EnableTeleport();

    }

    private IEnumerator WhiteColorDisable(float time)
    {
        yield return new WaitForSeconds(time);
        
        isWhiteColorAllowed = true;
    }
    
}