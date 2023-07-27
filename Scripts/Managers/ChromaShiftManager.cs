using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityRigidbody2D;
using DG.Tweening;
using ChromaShift.Scripts;
using ChromaShift.Scripts.Player;
using ChromaShift.Scripts.Managers;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public interface IChromaShiftManager
{
	void InitInventory();
	bool CheckForCore(GameColor colorIndex, FusionCore[] inventory);
	void SetColorFromInventory();
}

public struct ColorWeight
{
	[ReadOnly] public string Color;
	[Range(0,1f)] public float Weight;
}

public class ChromaShiftManager : SerializedMonoBehaviour, IChromaShiftManager
{
	[FormerlySerializedAs("_colorIndex")][SerializeField][BoxGroup("COLOR")][GUIColor("GetCurrentColor")]
	private GameColor currentColor = GameColor.Grey;
	public GameColor CurrentColor
	{ 
		get => currentColor;
	}
	[BoxGroup("COLOR")]
	[SerializeField][ReadOnly]
	private GameColor _previousColorIndex =  GameColor.Pink;

	[SerializeField][BoxGroup("COLOR")]
	private GameColor cachedColor =  GameColor.Pink;

	public GameColor CachedColor
	{
		get => cachedColor;
	}

	[SerializeField] private LumenType _lumenType;

	public LumenType LumenType
	{
		get => _lumenType;
		set => _lumenType = value;
	}

	[SerializeField] private LumenMode _lumenMode;

	public LumenMode LumenMode
	{
		get => _lumenMode;
		set => _lumenMode = value;
	}

	[SerializeField] private FusionCore[] _coreInventory;

	public FusionCore[] CoreInventory
	{
		get => _coreInventory;
		set => _coreInventory = value;
	}
	
	[SerializeField] private FusionCore[] _coreInventoryCache;

	public FusionCore[] CoreInventoryCache
	{
		get => _coreInventoryCache;
		set => _coreInventoryCache = value;
	}

	[SerializeField] private bool doesCoreRegen;

	public bool DoesCoreRegen
	{
		get => doesCoreRegen;
		set => doesCoreRegen = value;
	}
	
	[SerializeField] private float coreRegenTime = 2f;
	public float CoreRegenTime
	{
		get => coreRegenTime;
		set => coreRegenTime = value;
	}
	
	private bool isChromaShifting;
	[SerializeField] public bool instantSwitch = false; 
	public MeshRenderer[] rends;
	public MeshRenderer[] HDRPRends;
	public ParticleSystem[] particleSystems;
	public Light[] lights;
	public Image[] images;

	[FormerlySerializedAs("abilityActivationTime")] [FormerlySerializedAs("colorChangeSpeed")]
	public float shiftDuration = 0.75f;

	public bool isProtected;
	
	public bool isDormant = false;
	public GameObject[] meshEffects;

	public ICollidible CoC;
	
	[PropertySpace(SpaceBefore = 30)]
	[Title("Weight Random Selection")]
	[EnumToggleButtons, HideLabel]
	public GameColor weightTowards;
	public List<ColorWeight> ColorWeights = new List<ColorWeight>();
	
	public delegate void OnEventColorChange(GameColor newColor);
	public event OnEventColorChange OnColorChange = delegate { };
	
	[PropertyOrder(-1)]
	[Button(ButtonSizes.Large), GUIColor("GetToggleColor")]
	public void ToggleDormancy()
	{
		
		isDormant = !isDormant;
		if (isDormant)
		{
			DormantShutDown();
		}
		else
		{
			DormantWakeUp();
		}
		Debug.Log("Ran Toggle Dormancy");
	}

	private Color GetToggleColor()
	{
		if (!isDormant)
			return ColorManager.Instance.ConvertEnumToColor(GameColor.Green);
		else
			return ColorManager.Instance.ConvertEnumToColor(GameColor.Red);
	}
	
	public void DormantWakeUp()
	{
		isDormant = false;
		ChromaShift(_previousColorIndex);
		Debug.Log("Ran Wake up");
	}
	
	public void DormantShutDown()
	{
		isDormant = true;
		ChromaShift(GameColor.Grey);
		Debug.Log("Ran Shut down");
	}

	public Color GetCurrentColor()
	{
		return ColorManager.Instance.colorArray[ColorManager.Instance.ConvertColorToIndex(CurrentColor)];
	}
	void Awake()
	{

		if(CoC == null)
			CoC = gameObject.GetComponent<ICollidible>();
		InitInventory();

		//PLAYERSHIP RENDS MUST BE ASSIGNED IN THE EDITOR. THIS IS BECAUSE WE DON'T WANT THE FUSION CORE NODES TO CHANGE COLORS LIKE THE MAIN EMISSION.
		//THIS WILL PROBABLY HAVE TO BE DONE WITH THE BOSS AS WELL.
		if (!CoC.IsPlayerShip)
		{
			rends = gameObject.GetComponentsInChildren<MeshRenderer>();
		}


	}

	// Always Sets the Starting Color to White
	void Start()
	{
		isProtected = false;
		if(!isDormant)
			ChromaShift(CurrentColor);
		else
			ChromaShift(GameColor.Grey);
	}
	
	public void ChromaShift(GameColor newColorIndex)
	{
		//if (isChromaShifting)
		//return;

		if (isProtected)
			return;

		ChangeRenderColors(newColorIndex);
		
		_previousColorIndex = currentColor;
		isChromaShifting = true;
		//Debug.Log(gameObject.name + " is Now shifting colors");
		
		currentColor = newColorIndex;
		cachedColor = newColorIndex;
		OnColorChange?.Invoke(CurrentColor);
	}
	
	

	void ChangeRenderColors(GameColor newColorIndex)
	{
		Color newColor = ColorManager.Instance.colorArray[ColorManager.Instance.ConvertColorToIndex(newColorIndex)];
		if (instantSwitch)
			shiftDuration = 0;
		foreach (MeshRenderer rend in rends)
		{
			if (rend.material.HasProperty("_EmissionColor"))
			{
				//Debug.Log ("Inside Rend Loop Color Array Count: " + ColorManager.Instance.colorArray.Count);
				rend.material.DOColor(newColor, "_EmissionColor", shiftDuration);
			}

			if (rend.material.HasProperty("_RimColor"))
			{
				rend.material.DOColor(newColor, "_RimColor", shiftDuration);
			}

			if (rend.material.HasProperty("Tint"))
			{
				rend.material.DOColor(newColor, "Tint", shiftDuration);
			}

			if (rend.material.HasProperty("Tint Color"))
			{
				rend.material.DOColor(newColor, "Tint Color", shiftDuration);
			}

			if (rend.material.HasProperty("_TintColor"))
			{
				rend.material.DOColor(newColor, "_TintColor", shiftDuration);
			}
		}

		if (HDRPRends.Length > 0)
		{
			//Debug.Log("HD Rends Not Null");
			foreach (MeshRenderer hdrpRend in HDRPRends)
			{
				//Debug.Log("HD Rend");
				if (hdrpRend.material.HasProperty("_ChromaColor"))
				{
					hdrpRend.material.DOColor(newColor, "_ChromaColor", shiftDuration);
				}

				if (hdrpRend.material.HasProperty("_EmissiveColor"))
				{
					hdrpRend.material.DOColor(newColor, "_EmissiveColor", shiftDuration);
				}

			}
		}

		if (lights != null)
		{
			for (int i = 0; i < lights.Length; i++)
			{
				lights[i].color = newColor;
			}
		}

		if (particleSystems != null)
		{
			foreach (ParticleSystem pSystem in particleSystems)
			{
				ParticleSystem.MainModule newMain = pSystem.main;
				newMain.startColor = newColor;
				pSystem.GetComponent<ParticleSystemRenderer>().material
					.DOColor(newColor, "_EmissionColor", shiftDuration);
			}
		}

		if (meshEffects != null)
		{
			foreach (GameObject mesh in meshEffects)
			{
				//Debug.Log ("Inside Rend Loop Color Array Count: " + ColorManager.Instance.colorArray.Count);
				mesh.GetComponent<PSMeshRendererUpdater>().Color = newColor;
			}
		}
	}
	
	
	public void InitInventory()
	{
		if (CoC == null)
			return;
		
		if (CoC.IsPlayerShip)
		{
			_coreInventory = new[] {new FusionCore(GameColor.Red, true, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Green, true, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Blue, true, doesCoreRegen, CoreRegenTime)};
		}
		else
		{
			_coreInventory = GetActiveCoresFromColor(CurrentColor);
		}

		if(!CoC.IsPlayerShip)
			SetColorFromInventory();
	}

	public bool CheckForCore(GameColor colorIndex, FusionCore[] inventory)
	{
		for (int i = 0; i < inventory.Length; i++)
		{
			if (colorIndex == inventory[i].ColorIndex && inventory[i].IsActive == true)
				return true;
		}

		return false;
	}

	public FusionCore[] SetActiveCores(bool redCore, bool blueCore, bool greenCore)
	{
		FusionCore[] tempCores = new[] {new FusionCore(GameColor.Red, false, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Green, false, doesCoreRegen,CoreRegenTime), new FusionCore(GameColor.Blue, false, doesCoreRegen,CoreRegenTime)};

		if (redCore) tempCores[0].IsActive = true;
		if (blueCore) tempCores[1].IsActive = true;
		if (greenCore) tempCores[2].IsActive = true;

		return tempCores;
	}

	public void SetColorFromInventory()
	{
		//NO CORES
		if (_coreInventory.Length == 0)
		{
			currentColor = GameColor.Grey;
			return;
		}
		
		if (CheckForCore(GameColor.Red, _coreInventory)) {
			currentColor = GameColor.Red;
		}

		if (CheckForCore(GameColor.Green, _coreInventory)) {
			currentColor = GameColor.Green;
		}

		if (CheckForCore(GameColor.Blue, _coreInventory)) {
			currentColor = GameColor.Blue;

		}
		
		
		//2 COLOR VARIANTS

		if (CheckForCore(GameColor.Red, _coreInventory) && CheckForCore(GameColor.Green, _coreInventory)) {
			currentColor = GameColor.Yellow;
		}

		if (CheckForCore(GameColor.Red, _coreInventory) && CheckForCore(GameColor.Blue, _coreInventory)) {
			currentColor = GameColor.Purple;
		}

		if (CheckForCore(GameColor.Green, _coreInventory) && CheckForCore(GameColor.Blue, _coreInventory)) {
			currentColor = GameColor.Cyan;
		}
	
		//3 COLOR VARIANTS

		if (CheckForCore(GameColor.Red, _coreInventory) && CheckForCore(GameColor.Green, _coreInventory) && CheckForCore(GameColor.Blue, _coreInventory)) {
			currentColor = GameColor.White;
		}
	}

	public GameColor SetColorFromActiveCores()
	{
		
			bool[] ActiveNodes = new bool[3]{false,false,false};
		
		
			if (CheckForCore(GameColor.Red, _coreInventory)) {
				ActiveNodes[0] = true;
			}

			if (CheckForCore(GameColor.Green, _coreInventory)) {
				ActiveNodes[1] = true;
			}

			if (CheckForCore(GameColor.Blue, _coreInventory)) {
				ActiveNodes[2] = true;
			}
		
			//RED
			if (ActiveNodes.SequenceEqual(new bool[3] {true, false, false}))
			{
				return GameColor.Red;
			}
			//GREEN
			if (ActiveNodes.SequenceEqual(new bool[3] {false, true, false}))
			{
				return GameColor.Green;
			}
		
			//BLUE
			if (ActiveNodes.SequenceEqual(new bool[3] {false, false, true}))
			{
				return GameColor.Blue;
			}

			//Yellow
			if (ActiveNodes.SequenceEqual(new bool[3] {true, true, false}))
			{
				return GameColor.Yellow;
			}
			//Purple
			if (ActiveNodes.SequenceEqual(new bool[3] {true, false, true}))
			{
				return GameColor.Purple;
			}
		
			//Cyan
			if (ActiveNodes.SequenceEqual(new bool[3] {false, true, true}))
			{
				return GameColor.Cyan;
			}

			//White
			if (ActiveNodes.SequenceEqual(new bool[3] {true, true, true}))
			{
				return GameColor.White;
			}
		
			//NULL
			if (ActiveNodes.SequenceEqual(new bool[3] {false, false, false}))
			{
				return GameColor.Grey;
			}
			
			return GameColor.Pink;
	}

	public FusionCore[] GetActiveCoresFromColor(GameColor color)
	{
		CoreInventoryCache = _coreInventory;

		//SOLID COLORS
		if (color == GameColor.Red)
		{
			FusionCore[] cores = { new FusionCore(GameColor.Red, true, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Green,false, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Blue, false, doesCoreRegen, CoreRegenTime)};
			return cores;
		}
		if (color == GameColor.Green)
		{
			FusionCore[] cores = { new FusionCore(GameColor.Red, false, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Green,true, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Blue, false, doesCoreRegen, CoreRegenTime)};
			return cores;
		}
		if (color == GameColor.Blue)
		{
			FusionCore[] cores = { new FusionCore(GameColor.Red, false, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Green,false, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Blue, true, doesCoreRegen, CoreRegenTime)};
			return cores;
		}

		//2 COLOR VARIANTS

		if (color == GameColor.Yellow)
		{
			FusionCore[] cores = { new FusionCore(GameColor.Red, true, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Green,true, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Blue, false, doesCoreRegen, CoreRegenTime)};
			return cores;
		}
		if (color == GameColor.Purple)
		{
			FusionCore[] cores = { new FusionCore(GameColor.Red, true, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Green,false, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Blue, true, doesCoreRegen, CoreRegenTime)};
			return cores;
		}
		if (color == GameColor.Cyan)
		{
			FusionCore[] cores = { new FusionCore(GameColor.Red, false, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Green,true, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Blue, true, doesCoreRegen, CoreRegenTime)};
			return cores;
		}
	
		//3 COLOR VARIANTS

		if (color == GameColor.White)
		{
			FusionCore[] cores = { new FusionCore(GameColor.Red, true, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Green,true, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Blue, true, doesCoreRegen, CoreRegenTime)};
			return cores;
		}
		
		//NO CORES
		if (color == GameColor.Grey)
		{
			FusionCore[] cores = { new FusionCore(GameColor.Red, false, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Green,false, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Blue, false, doesCoreRegen, CoreRegenTime)};
			return cores;
		}
		else
		{
			Debug.LogWarning("Object does not fit the colorIndex. Please look into it.");
			FusionCore[] cores = { new FusionCore(GameColor.Red, false, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Green,false, doesCoreRegen, CoreRegenTime), new FusionCore(GameColor.Blue, false, doesCoreRegen, CoreRegenTime)};
			return cores;
		}
		
		
		return null;
	}


	/// <summary>
	/// Handle all of the logic that needs to occur when deactivating a
	/// FusionCore. Mainly set to inactive and kick off the regeneration logic.
	/// </summary>
	/// <param name="index"></param>
	public void DeactivateFusionCoreByColorIndex(GameColor index)
	{
		Debug.Log("Deactivate Core: " + index);
		foreach (FusionCore core in CoreInventory)
		{
			if (core.ColorIndex == index)
			{
				core.IsActive = false;
				StartCoroutine(core.Regeneration());
			}
		}
	}
	
	private void Reset()
	{
		InitializeWeights();
	}

	private void InitializeWeights()
	{
		ColorWeights.Clear();
            
		for (int i = 0; i < ColorManager.Instance.colorArray.Count; i++)
		{
			string colorString = Enum.GetName(typeof(GameColor), i);
			if(i == 0 || i == 3 || i == 4)
				ColorWeights.Add( new ColorWeight { Color = colorString, Weight = 0.5f });
			else
				ColorWeights.Add( new ColorWeight { Color = colorString, Weight = 0f });
		}
	}
	
	[HorizontalGroup("Split", 0.5f)]
	[Button("Adjust Weights for Color",ButtonSizes.Large),GUIColor(0.3f, 0.4f, 0)]
	private void AdjustWeightsForColor()
	{
		ColorWeights.Clear();
		int weightIndex = Array.IndexOf(Enum.GetValues(typeof(GameColor)), weightTowards);
		for (int i = 0; i < ColorManager.Instance.colorArray.Count; i++)
		{
			string colorString = Enum.GetName(typeof(GameColor), i);
			if(i == weightIndex)
				ColorWeights.Add( new ColorWeight { Color = colorString, Weight = 0.75f });
			else if(i == 0 || i == 3 || i == 4)
				ColorWeights.Add( new ColorWeight { Color = colorString, Weight = 0.25f });
			else
				ColorWeights.Add( new ColorWeight { Color = colorString, Weight = 0f }); 
		}
	}
	[HorizontalGroup("Split", 0.5f)]
	[Button("Select Random Color",ButtonSizes.Large),GUIColor(0.4f, 0.4f, 0.8f)]
	public void ShiftRandomColorWeighted()
	{
		float totalWeight = 0f;

		// Calculate the total weight of all items
		foreach (ColorWeight weight in ColorWeights)
		{
			totalWeight += weight.Weight;
		}

		// Generate a random value between 0 and the total weight
		float randomValue = Random.Range(0f, totalWeight);

		// Iterate through the items and subtract their weights from the random value
		// until the random value becomes less than or equal to zero, indicating the selected item
		for (int i = 0; i < ColorManager.Instance.colorArray.Count; i++)
		{
			randomValue -= ColorWeights[i].Weight;
			if (randomValue <= 0f)
			{
				currentColor = ColorManager.Instance.ConvertIndexToGameColor(i);
				return;
			}
		}

		// Return null if no item was selected (shouldn't happen if weights are correctly defined)
		return;
	}
}
