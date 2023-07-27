using System;
using Rewired.UI.ControlMapper;
using UnityEngine;

namespace ChromaShift.Scripts.Enemy
{
    public class BossMovement : MonoBehaviour
    {
        private MovableObjectController _movableObjectController;
        private MoveBlock _moveBlock;

        private void Start()
        {
            _movableObjectController = GameObject.FindObjectOfType<MovableObjectController>();

            if (!_movableObjectController)
            {
                throw new MissingReferenceException("Need to have the MovableObjectController as a component.");
            }

/*            if (_movableObjectController.useNewMovementSystem == false)
            {
                throw new NullReferenceException("Need to have the new movement system implemented.");
            }*/

            if (_movableObjectController.moveList == false)
            {
                // @TODO Determine if this is going to be really needed.
                throw new NullReferenceException("This is the Move Stack Reference and it is blank.");
            }

            if (_movableObjectController.moveList.MoveStack.Length <= 0)
            {
                throw new NullReferenceException("Need to have at least one thing in the MoveStack");
            }

            _moveBlock = _movableObjectController.moveList.MoveStack[0];
        }

        private void FixedUpdate()
        {
            switch (_moveBlock.movementType)
            {
                case MovementType.Hover:
                    Debug.Log("Just want to 'hover' in front of the player, since this is the boss.");
                    break;
                case MovementType.Stationary:
                    Debug.Log("Just waiting for the player to come along since this is the boss.");
                    break;
            }
        }
    }
}