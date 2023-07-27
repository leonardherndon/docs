using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChromaShift.Scripts.HermesDataNodes
{
    public class HermesDataNode : MonoBehaviour
    {
        [SerializeField] private Vector3 _id;
        [SerializeField] public Vector3 Id
        {
            get => _id;
            set => _id = value;
        }
        
        [SerializeField] private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }
        
        [SerializeField] private GameColor _colorIndex;

        public GameColor ColorIndex
        {
            get => _colorIndex;
            set => _colorIndex = value;
        }
        [SerializeField] private int[] _colorChoices;

        public int[] ColorChoices
        {
            get => _colorChoices;
            set => _colorChoices = value;
        }

        [SerializeField] private int[] colorSetPrimary = new int[3] {0, 3, 4};
        [SerializeField] private int[] colorSetSecondary = new int[6] {0, 3, 4, 2, 6, 10};
        [SerializeField] private ChromaShiftManager CSM;

        public void Start()
        {
            CSM = GetComponent<ChromaShiftManager>();
        }
        
        public void PushColorRandom(bool primary)
        {
            if(primary)
                PushColor(ColorManager.Instance.ConvertIndexToGameColor(colorSetPrimary[UnityEngine.Random.Range(0, colorSetPrimary.Length)]));
            else
                PushColor(ColorManager.Instance.ConvertIndexToGameColor(colorSetSecondary[UnityEngine.Random.Range(0, colorSetSecondary.Length)]));
        }
        
        public void PushColor(GameColor color)
        {
            CSM.ChromaShift(color);
            _colorIndex = color;
        }

        public void KillNode()
        {
            if (!_isActive)
                return;
            PushColor(GameColor.Grey);
            _isActive = false;
        }
        
        public void ActivateNode()
        {
            if (_isActive)
                return;
            PushColor(ColorManager.Instance.ConvertIndexToGameColor(ColorChoices[0]));
            _isActive = true;
        }
        
    }
}