using ChromaShift.Scripts.Enemy.Resource;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace ChromaShift.Scripts.Enemy
{
    public class ResourceSet : IResourceSet
    {
        [OdinSerialize,Range(0, 30)] private float _time;
        
        [Title("Lasers")]
        [OdinSerialize,Required("Pick Null Resource")] private IResource _body;
        [OdinSerialize,Required("Pick Null Resource")] private IResource _portArm;
        [OdinSerialize,Required("Pick Null Resource")] private IResource _starboardArm;
        
        [Title("Traps")]
        [OdinSerialize,Required("Pick Null Resource")] private IResource _portTrap;
        [OdinSerialize,Required("Pick Null Resource")] private IResource _starboardTrap;

        public ResourceSet(float time, IResource body, IResource portArm, IResource portTrap, IResource starboardArm,
            IResource starboardTrap)
        {
            _time = time;
            _body = body;
            _portArm = portArm;
            _portTrap = portTrap;
            _starboardArm = starboardArm;
            _starboardTrap = starboardTrap;
        }
        
        #if UNITY_EDITOR
        public ResourceSet()
        {
            _body = new Null();
            _portArm = new Null();
            _portTrap = new Null();
            _starboardArm = new Null();
            _starboardTrap = new Null();
        }
        #endif

        public IResource GetBodyResource()
        {
            return _body;
        }

        public IResource GetPortArmResource()
        {
            return _portArm;
        }

        public IResource GetPortFlapResource()
        {
            return _portTrap;
        }

        public IResource GetStarboardArmResource()
        {
            return _starboardArm;
        }

        public IResource GetStarboardFlapResource()
        {
            return _starboardTrap;
        }

        public float GetDeployTime()
        {
            return _time;
        }

        public void IncreaseDeployTime(float time)
        {
            _time += time;
        }
    }
}