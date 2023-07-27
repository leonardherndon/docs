using UnityEngine;
using System.Collections;
using System.Configuration;
using ChromaShift.Scripts.Player.Upgrade;
using ChromaShift.Scripts.SaveGame;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class CheckPointTrigger : MonoBehaviour
{

	public int checkPointIndex;
	public Image checkpointIndicator;
	[SerializeField, HideInInspector] private bool _isForCheckpointBeacon;

	[ShowInInspector]
	public bool IsForCheckpointBeacon
	{
		get => _isForCheckpointBeacon;
		set => _isForCheckpointBeacon = value;
	}

	void OnTriggerEnter(Collider other) {
		
		if (other.name == "PlayerShip")
		{
			var beacon = other.GetComponent<CheckpointBeacon>();

			if (_isForCheckpointBeacon && (beacon == null || !beacon.Modifier ))
			{
				return;
			}
			
			FlashIndicator();

			GameManager.Instance.checkPointIndex = checkPointIndex;
			SaveGameManager.Instance.SaveGameData(GameManager.Instance.currentSaveDataIndex);
			GetComponent<BoxCollider> ().enabled = false;
		}
	}

	public void FlashIndicator()
	{
		checkpointIndicator = GameObject.Find("AM_CheckpointIndicator").GetComponent<Image>();
		Sequence mySequence = DOTween.Sequence();
		mySequence.Append(checkpointIndicator.DOFade(1, 0.35f));
		mySequence.Append(checkpointIndicator.DOFade(0, 0.35f));
		mySequence.SetLoops(4, LoopType.Restart);
		mySequence.Play();
	}
}
