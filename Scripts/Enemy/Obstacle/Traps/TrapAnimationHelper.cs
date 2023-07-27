using UnityEngine;

namespace ChromaShift.Scripts.Enemy.Obstacle.Traps
{
    public class TrapAnimationHelper : MonoBehaviour {
        [SerializeField]
        private ITrap<GameObject, Collider> trapScript;
        
        [SerializeField]
        private Hostile trapObject;

        public void Start()
        {
            trapScript = GetComponentInParent<ITrap<GameObject, Collider>>();
        }
        public void SpawnFromAnim()
        {
            Debug.Log("TrapScript: Spawn");
            trapScript.Spawn();
        }

        public void PlayAudio()
        {
            trapObject.GetComponent<Hostile>()._audioSource.Play();
        }
    }
}
