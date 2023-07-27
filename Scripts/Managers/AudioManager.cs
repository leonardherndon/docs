using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace CS_Audio {
	public class AudioManager : MonoBehaviour {

		private static AudioManager _instance;
		public List<AudioClip> musicClips;
		public List<AudioClip> soundFXClips;
		public AudioClip deniedClip;
		public AudioClip acceptClip;
		public AudioClip startClip;
		public AudioClip backClip;
		public AudioMixer audioMixer;
		public AudioMixerGroup mixerGroupSFX;
		public AudioMixerGroup mixerGroupMusic;
		public AudioMixerGroup mixerGroupVoice;
		public AudioSource[] audioSources;
		public AudioSource musicAudioSource;
		public AudioSource pickUpAudioSource;
		public AudioSource voiceAudioSource;
		public AudioSource uiAudioSource;
		public AudioSource playerAudioSource;

		static public bool isActive {
			get {
				return _instance != null;
			}
		}

		//Singleton pattern implementation
		public static AudioManager Instance {
			get 
			{
				if (_instance == null) 
				{
					_instance = Object.FindObjectOfType (typeof(AudioManager)) as AudioManager;


					if (_instance == null)
					{
						GameObject go = new GameObject("_audiomanager");
						DontDestroyOnLoad(go);
						_instance = go.AddComponent<AudioManager>();
					}
				}
				return _instance;
			}
		}

		public void Awake() {

			mixerGroupSFX = audioMixer.FindMatchingGroups ("Sound FX") [0];
			mixerGroupMusic = audioMixer.FindMatchingGroups ("Music")[0];
			mixerGroupVoice = audioMixer.FindMatchingGroups ("Voice")[0];

			//		fastPoolObjects = new List<GameObject> ();
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void PlayClipWrap(int sourceID = 0, int clipID = 0, AudioClip rawClip = null, bool isLoopable = false, float delayTime = 0)
		{
			StartCoroutine(PlayClip(sourceID, clipID, rawClip, isLoopable, delayTime));
		}

		public IEnumerator PlayClip(int sourceID = 0, int clipID = 0, AudioClip rawClip = null, bool isLoopable = false,
			float delayTime = 0)
		{
			if (delayTime > 0)
			{
				yield return new WaitForSeconds(delayTime);
			}
			if(rawClip != null)
			{
				if (isLoopable)
				{
					audioSources[sourceID].clip = rawClip;
					audioSources[sourceID].loop = true;
					audioSources[sourceID].Play();
				}
				else
				{
					audioSources[sourceID].PlayOneShot(rawClip);
				}
			}
			else if(clipID != 0)
			{
				audioSources[sourceID].PlayOneShot(soundFXClips[clipID]);
			}
		}

        public bool isPlaying(int sourceID = 0, int clipID = 0)
        {
            if (audioSources[sourceID].clip == soundFXClips[clipID] && audioSources[sourceID].isPlaying)
                return true;
            return false;
        }

        public void StopPlayingClip(int sourceID = 0)
        {
            audioSources[sourceID].Stop();
        }

        public void ChangeBGM(AudioClip newTrack)
        {
	        if (musicAudioSource.clip.name == newTrack.name)
		        return;
	        musicAudioSource.Stop();
	        musicAudioSource.clip = newTrack;
	        musicAudioSource.Play();
        }
	}
}