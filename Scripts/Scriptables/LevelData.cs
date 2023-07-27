using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using UnityEngine.UI;


/// <summary>
/// Level data.
/// This Object needs to be attached to the Level's "LevelData" GameObject as well as The SectorMap's Sector button (in both the OnClick.SetSceneToBeLoaded and in the Level Data Holder)
/// </summary>

public class LevelData : ScriptableObject {

	public string Code;
	public string Name;
	public string Description;
	public string ThreatLevel;
	public string HostileTypes;
	public SceneData[] levelSceneList;
	public int scorePotential;
	public ZoneMapData[] availableZoneMaps = new ZoneMapData[5];
	[FormerlySerializedAs("startingFusionCores")] public GameColor[] currentFusionCores;
	public AudioClip descriptionAudio;
    public Sprite levelImage;
    public SceneCamPathManager cameraPathObject;
}
