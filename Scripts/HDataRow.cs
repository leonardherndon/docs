using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChromaShift.Scripts.HermesDataNodes
{
    public class HDataRow : MonoBehaviour
    {
        public int id;
        public GameColor[] nodeIndexes;

        //Need to Dependency Inject this to the NodeArray
        //We Need to get child nodes and place their current color indexes in here
    }
}