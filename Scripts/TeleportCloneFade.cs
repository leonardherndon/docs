using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TeleportCloneFade : MonoBehaviour
{
    private Sequence fadeTween;

    public float fadeTime = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        transform.parent = GameManager.Instance.spawnedObjectsHolder.transform;
        fadeTween = DOTween.Sequence();
        
        MeshRenderer[] rends = gameObject.GetComponentsInChildren<MeshRenderer>();
        
        fadeTween.Append(rends[0].material.DOFloat(0, "_Fade", fadeTime));
        fadeTween.Join(rends[1].material.DOFloat(0, "_Fade", fadeTime).OnComplete(DestroyMe));
    }

    public void DestroyMe()
    {
        //Debug.Log("Teleport Clone DestroyMe()");
        Destroy(gameObject);
    }
}
