using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts;
using ChromaShift.Scripts.Managers;
using ChromaShift.Scripts.ObjectAttributeSystem;
using Chronos;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Serialization;

public class LifeSystemController : MonoBehaviour, ILifeSystem
{
	
	[Header("DEPENDENCIES")]
	private ICollidible _coC;
	private PlayerShip _playerShip;
	private AttributeController _attributeController;
	
	[Header("Scriptable")] [SerializeField]
	private LifeSystemData LSD;
	[SerializeField] private bool _isPlayer;
	
	[SerializeField] private float _currentHP = 10f;
	public float CurrentHP
	{
		get => _currentHP;
		set => _currentHP = value;
	}
	
	[SerializeField] private float _startingHP = 10f;
	public float StartingHP
	{
		get => _startingHP;
		set => _startingHP = value;
	}
	 
	[SerializeField] private float _totalHP = 10f;
	public float TotalHP
	{
		get => _totalHP;
		set => _totalHP = value;
	}
	
	[SerializeField] private bool _doesRegenHealth = false;

	[SerializeField] private float[] _regenRate = new float[4] {0.05f,0.25f,0.5f,0.75f};
	public float[] RegenRate
	{
		get => _regenRate;
		set => _regenRate = value;
	}

	[Header("[DEATH]")]
	[SerializeField] private bool _isDead = false;
	public bool IsDead
	{
		get => _isDead;
		set => _isDead = value;
	}
	
   [SerializeField] private const float DEATHCHECKTIMER = 2f;

   [Header("[SPECIAL FX]")]
    [SerializeField] private MMFeedbacks _feedbackTakeDamage;
    [SerializeField] private MMFeedbacks _deathEffect; 
    public float deathSequenceTimer = 0.2f;
    
    //EVENTS
    public delegate void OnLifeChanged(float current, float prev, float max, bool player);
    public event OnLifeChanged LifeChanged = delegate { };
    public event ILifeSystem.DeathHandler Died;
    public event ILifeSystem.LostHealthHandler LostHealth;
    
    private void OnEnable()
    {
       InitLifeSystem();
    }

    public void InitLifeSystem()
    {
	    if (!LSD)
	    {
		    Debug.LogError("No Life System Data Attached to " + gameObject.name + ". Please attach data to enable functionality");
		    return;
	    }
	    _playerShip = GameManager.Instance.playerShip;
	    _coC = GetComponent<ICollidible>();
	    
	    _attributeController = GetComponent<AttributeController>();
	    
	    //Get Parameters From Scriptable Data
	    _startingHP = LSD.StartingHP;
	    _totalHP = LSD.TotalHP;
	    _isPlayer = LSD.isPlayer;
	    _doesRegenHealth = LSD.doesRegenHealth;
	    _regenRate = LSD.regenRate;
	    if(_feedbackTakeDamage == null)
			_feedbackTakeDamage = LSD.feedbackTakeDamage;
	    if(_deathEffect == null)
			_deathEffect = LSD.deathEffect;
	    
	    ResetLifeSystem();
	    
	    
	    _coC.GotDamaged += ReceiveDamage;
	    _coC.GotVelocityDamaged += ReceiveVelocityDamage;
	    Died += KillSelf;
	    _coC.Wrecked += KillSelf;
    }

    public void ResetLifeSystem()
    {
	    _isDead = false;
	    _currentHP = _startingHP;
    }

	void FixedUpdate ()
	{
		if(_doesRegenHealth)
			RestoreLife(_regenRate[_coC.CSM.CoreInventory.Length]);
	}

	public void RestoreLife(float amount)
	{
		float previousHP = _currentHP;
		
		if(GameStateManager.Instance.CurrentState.StateType != GameStateType.Gameplay)
			return;

		if (_currentHP >= TotalHP)
			return;
		
		if (amount < 0)
		{
			//Debug.LogWarning("You can only add HP not remove. Please try UseLife or DrainLife");
			return;
		}
    
		CurrentHP += amount;
    
		if (CurrentHP > TotalHP)
			CurrentHP = TotalHP;
		
		LifeChanged?.Invoke(CurrentHP, previousHP, TotalHP, _isPlayer);
	}
	
	//ABILITIES USE SpendLife() and HOSTILES USE DrainLife() THIS WAY YOU CANNOT KILL YOURSELF BY USING ABILITIES
	public void SpendLife(float amount)
	{
    		
		if (amount < 0)
		{
			//Debug.LogWarning("You can only remove HP not add. Please try RegenLife");
			return;
		}

		float previousHP = _currentHP;
		//Debug.Log ("Charging BATTERY by: " + amount);
		CurrentHP -= amount;
    
		if (CurrentHP > TotalHP)
			CurrentHP = TotalHP;
    
		if (CurrentHP <= 0)
		{
			CurrentHP = 0.001f;
		}
		LifeChanged?.Invoke(CurrentHP, previousHP, TotalHP, _isPlayer);
	}
	
	public void DrainLife(float amount, ImpactType impactType = ImpactType.Null, bool isPlayerShip = false)
	{
		Debug.Log("Before Damage Adjustment Amount: " + amount);
		if (_attributeController != null)
			amount = _attributeController.CalculateDamageReductionAdjustment(amount);
		

		if(amount < 0) 
			amount = 0;
		float previousHP = _currentHP;
		_currentHP -= amount;
		Debug.Log (gameObject.name + ": Drain Life Instant by: " + amount);
		if (_currentHP > _totalHP)
			_currentHP = _totalHP;
		
		if (_currentHP <= 0)
		{
			//Debug.Log (gameObject.name + ": should be dead: " + currentHP);
			Died?.Invoke();
		}
		LifeChanged?.Invoke(CurrentHP, previousHP, TotalHP, _isPlayer);
		LostHealth?.Invoke(amount);
	}
	
	public IEnumerator DrainLifeDOT(float amount, int limit, ImpactType impactType, float tickRate = 1f)
	{
		//Debug.Log (gameObject.name + ": START Drain Life DOT: " + amount);
		int i = 0;

		while (i < limit)
		{
			DrainLife(amount, impactType);
			
			i++;
			yield return new WaitForSeconds(tickRate);
		}
		//Debug.Log (gameObject.name + ": END Drain Life DOT: " + amount);
	}
	
	public IEnumerator DrainLifeDelayed(float amount, int limit, ImpactType impactType, float delayTime = 1f)
	{
		//Debug.Log (gameObject.name + ": START Drain Life DOT: " + amount);
		int i = 0;
		
		yield return new WaitForSeconds(delayTime);

			while (i < limit)
		{
			DrainLife(amount, impactType);
			
			i++;
			yield return new WaitForSeconds(delayTime);
		}
		//Debug.Log (gameObject.name + ": END Drain Life DOT: " + amount);
	}

	public void ReceiveDamage(ImpactType impactType, List<DamageDataBlock> damageBlockList = null, ICollidible otherCoC = null, DamageSeverity severity = DamageSeverity.Normal)
	{
		//Debug.Log(gameObject.name +": Receive Damage.");
		if (damageBlockList == null)
			return;
       
		if (otherCoC == null)
		{
			Debug.Log(gameObject.name + ": Other Object does not have a ICollidible. Ignore Damage");
			return;
		}

		foreach (DamageDataBlock damageBlock in damageBlockList)
		{
			var damageRequest = gameObject.AddComponent<DamageRequest>();
			var sourceName = otherCoC.GObject.name;
			var id = damageRequest.GetInstanceID();
			damageRequest.CreateDamageRequest(damageBlock.damageApplicationType, damageBlock.damageAmount, damageBlock.limit, damageBlock.tickRate,
				damageBlock.startDelay, severity, impactType, otherCoC, sourceName);
			//_feedbackTakeDamage.GetComponent<MMFeedbackFlash>().FlashColor =
			//	ColorManager.Instance.colorArray[otherCoC.CSM.CurrentColor];
			_feedbackTakeDamage.PlayFeedbacks();
		}
	}

	public void ReceiveVelocityDamage(Collision other, ImpactType impactType = ImpactType.Physical, List<DamageDataBlock> damageBlockList = null, ICollidible otherCoC = null, float velocityMultiplier = 0, DamageSeverity severity = DamageSeverity.Normal)
	{
		if (other == null)
		{
			Debug.LogWarning("Velocity Damage requires Unity Collision Object. None Found.");
			return;
		}

		
		foreach (DamageDataBlock damageBlock in damageBlockList)
		{
			var damageRequest = gameObject.AddComponent<DamageRequest>();
			var sourceName = otherCoC.GObject.name;
			var id = damageRequest.GetInstanceID();
			//Debug.Log("Damage Velocity Multiplier is: " + velMultiplier);
			damageRequest.CreateDamageRequest(damageBlock.damageApplicationType, damageBlock.damageAmount, damageBlock.limit, damageBlock.tickRate,
				damageBlock.startDelay, severity, impactType, otherCoC, sourceName, velocityMultiplier);
			//_feedbackTakeDamage.GetComponent<MMFeedbackFlash>().FlashColor =
			//	ColorManager.Instance.colorArray[otherCoC.CSM.CurrentColor];
			_feedbackTakeDamage.PlayFeedbacks();
		}
	}

	public void KillSelf()
	{
		if (_isDead)
			return;

		StartCoroutine(DeathSequence());
		_isDead = true;
		Died -= KillSelf;
	}

	private IEnumerator DeathSequence()
	{
		//Spawn Effects
		_deathEffect.PlayFeedbacks();
		
		//Disable Collision
		GetComponent<Collider>().enabled = false;
		
		//This is a temporary solution. TODO: Deactivation should be handled in the feedback system so death effects can be timed appropriately
		yield return new WaitForSeconds(DEATHCHECKTIMER);
		if(gameObject.activeSelf)
			gameObject.SetActive(false);

		if (gameObject.CompareTag("Player"))
		{
			//Release Camera
			GameManager.Instance.mainCamera.OffsetX = 0;
			
			//Stop Player Ship Object from Moving.
			_playerShip.CleanUp();

			//Change State to GAME OVER
			GameManager.Instance.GameOver();
		}

		yield return null;
	}

}
