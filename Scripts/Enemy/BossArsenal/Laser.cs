using System;
using System.Collections;
using Chronos;
using Jacovone.Meshes;
using QuickEngine.Extensions;
using UnityEngine;
using UnityEngine.Rendering;

namespace ChromaShift.Scripts.Enemy.BossArsenal
{
    public class Laser : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private CollisionController _collisionController;
        private Clock _clock;

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _collisionController = GetComponent<CollisionController>();
            _clock = Timekeeper.instance.Clock("Enemy");

//            _lineRenderer.material = new Material(Shader.Find("Standard"));

//            _lineRenderer.material.SetColor("_EmissionColor", Color.green);
//            _lineRenderer.material.albedo;
//            _lineRenderer.shadowCastingHippMode = ShadowCastingMode.Off;
//            _lineRenderer.receiveShadows = false;

            StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            var length = 0f;
            var maxLength = 100f;
            var maxWidth = 10f;
            var startWidth = .5f;
            var dt = _clock.deltaTime *3f;
            
            var width = startWidth;
            
            _lineRenderer.startWidth = width;
            _lineRenderer.endWidth = width;
            
            while (length < (maxLength *.9))
            {
                length = Mathf.Lerp(length, maxLength, dt);
                width = Mathf.Lerp(width, maxWidth, dt);
                
                _lineRenderer.endWidth = width;
                _lineRenderer.SetPosition(1, new Vector3(length * -1, 0));
                
                hitCheck(transform.position, length);

                yield return null;
            }

            width = startWidth;
            length = 0f;
            
            while (length < (maxLength *.9))
            {
                length = Mathf.Lerp(length, maxLength, dt);
                width = Mathf.Lerp(width, maxWidth, dt);

                _lineRenderer.startWidth = width;
                _lineRenderer.SetPosition(0, new Vector3(length * -1, 0));
                
                hitCheck(_lineRenderer.GetPosition(1), length);
                yield return null;
            }
            
            Destroy(gameObject);
        }

        private void hitCheck(Vector3 position, float length)
        {
            RaycastHit[] raycastHits = new RaycastHit[10];
            
            var size = Physics.RaycastNonAlloc(position, Vector3.left, raycastHits, length);
            
            for (var i = 0; i < size; i++)
            {
                if (raycastHits[i].transform.name != GameManager.Instance.playerShip.transform.name)
                {
                    continue;
                }
                
                Debug.Log("@TODO gonna try to kill the player.");
                Debug.Log("Color index: " + _collisionController.CSM.CurrentColor);
                

//                GameManager.Instance.playerShip.KillPlayer(ImpactType.LaserImpact);
                //_projectileCollisionManager.ManageHostileHitOnPlayer(transform);
                GameManager.Instance.playerShip.CoC.HandleCollision(gameObject,gameObject.GetComponent<ICollidible>().DamageDataBlocks,gameObject.GetComponent<ICollidible>().StatusEffectDataBlocks);
            }
        }
    }
}