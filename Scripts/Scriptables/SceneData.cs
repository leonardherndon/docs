using UnityEngine;
using ChromaShift.Scripts.ChallengeObject;


/// <summary>
/// Scene data.
/// This Object Accompanies the Scene Object to give the GameManager the data required for things to work.
/// </summary>

public class SceneData : ScriptableObject {
	
	public string sceneName;
	public int buildSceneIndex;
	public Material skyBox;
	public int[] startingFusionCores;
	public bool useProjection = false;
	public GameObject projectionObject;
	public AudioClip sceneMusic;
	public SceneCamPathManager cameraPathObject;
	public Vector3 playerStartPosition = new Vector3(0,52,0);
	public Color[] sceneMainColors = new Color[2];
	public ChallengeObject[] challengeObjects;

}
