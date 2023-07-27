using UnityEngine;

public class HoverState : IMoveState
{
    public string stateName = "HoverState";
    private readonly Hostile eObject;
    private readonly MovableObjectController mObject;
    private bool finishedHovering = false;
    private StickWithPlayer _stickWithPlayer;
    private bool _isBehindPlayer;

    private Rigidbody _rigidbody;
    private Rigidbody _playerRB;
    
    //Movement Params
    
    
    
    public HoverState(Hostile enemy)
    {
        mObject = enemy.MoC;
        eObject = enemy;
        _stickWithPlayer = GameManager.Instance.playerShip.hoverCollider.GetComponent<StickWithPlayer>();

        _rigidbody = eObject.GetComponent<Rigidbody>();
        _playerRB = GameManager.Instance.playerShip.GetComponent<Rigidbody>();
        
//        Debug.LogFormat("Possible rigidbody for hover state on {0}: {1}. Also, rb for player: {2}", enemy.ToString(), _rigidbody, _playerRB);
    }

    public void UpdateState()
    {
        //GatherParams(mObject.useBlockParams);
//        MoveObject(); // This one is only going to run on the update.
    }

    public void Update()
    {
        MoveObject();
    }

    /*private void GatherParams(bool useBlockParams = false)
    {
       
    }*/
    
    public void MoveObject()
    {
        if (mObject.isBehindPlayer != null && eObject.currentBlockIndex < mObject.isBehindPlayer.Count)
        {
            _isBehindPlayer = mObject.isBehindPlayer[eObject.currentBlockIndex];
        }
        if (eObject.enemyShipTrail != null && eObject.enemyShipTrail.activeSelf)
        {
            Debug.Log("Enemy Trail should be turning off.");
            eObject.enemyShipTrail.SetActive(false);
        }

        var xpos = GameManager.Instance.playerShip.transform.position.x + _stickWithPlayer.playerPositionOffset;

        if (_isBehindPlayer)
        {
            xpos = GameManager.Instance.playerShip.transform.position.x + (_stickWithPlayer.playerPositionOffset * -1);
        }

        // because of this here, we need to be calling this on Update not FixedUpdate so the object will smoothly be set to the correct position every frame.
        mObject.movableObject.transform.position = new Vector3(xpos, eObject.transform.position.y, 0f);
//        mObject.movableObject.transform.position = getWorldSpace();

//        Debug.LogFormat("The current velocity is {0} while player velocity is {1}", _rigidbody.velocity, _playerRB.velocity);

//        _rigidbody.AddForce(_playerRB.velocity, ForceMode.VelocityChange);
        var v = _playerRB.velocity.x * Vector3.right;
        _rigidbody.velocity = v;
    }

    private Vector3 getWorldSpace()
    {
        var cam = GameManager.Instance.mainCamera;

//        var height = cam.ScreenSizeInWorldCoordinates.y;
//        var width = cam.ScreenSizeInWorldCoordinates.x;
        Debug.Log(cam.ScreenSizeInWorldCoordinates);

        var height = 1080f;
        var width = 1920f;

        var pos = cam.GameCamera.ScreenToWorldPoint(new Vector3( width * .9f, height / 2, 10));
        
        Debug.LogFormat("h, w ({0}, {1}) becomes {2}", height, width, pos);
        
        return pos;
    }
}