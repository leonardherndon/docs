using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using System.Collections;
using Chronos;

public class TextTrigger : MonoBehaviour {


	public PlayerShip playerShip;
    [SerializeField]
    private Actor dialogActor = null;
	public TypeOutScript textBox;
	public Text textLabel;
    public Image textPortrait;
    public TextDataObject TDO;
	public bool requiredInput = false;
	public GameObject textBlock;
	public string textOverride;
    public string unityActorId;
	public int chosenText;
	public AudioSource aS;
    public float clearDelay = 2f;
    public string convoCode;

	void Awake () {
		aS = GetComponent<AudioSource> ();
	}
	void Start () {
        if (!GameManager.Instance.USEDIALOGSYSTEM)
            return;
        textBox = GameUIManager.Instance.UIElementsActiveMain[2].GetComponent<TypeOutScript>();
        textLabel = GameUIManager.Instance.UIElementsActiveMain[1].GetComponent<Text>();
        textPortrait = GameUIManager.Instance.UIElementsActiveMain[3].GetComponent<Image>();
        dialogActor = DialogueManager.MasterDatabase.GetActor(unityActorId);
        //Debug.Log("Actor Name is: " + dialogActor.Name);
		
        //Texture2D portText = dialogActor.GetPortraitTexture(1);
        //textPortrait.sprite = UITools.CreateSprite(portText);
        TDO = GameObject.Find ("ScriptManager").GetComponent<TextDataObject>();
	}
	void OnTriggerEnter(Collider other) {
        if (!GameManager.Instance.USEDIALOGSYSTEM)
            return;
        if (other.name == "PlayerShip") {

			if (requiredInput) {
				Timekeeper.instance.Clock ("Actives").LerpTimeScale (0, 0.5f, false);
			}

            DialogueManager.StartConversation(convoCode);

   //         GameUIManager.Instance.OpenTextBox(requiredInput);
   //         textLabel.text = performer;

			////GameUIManager.Instance.OpenTextBox (requiredInput);
				
			//if (textOverride == "" || textOverride == null)
			//	textBox.FinalText = TDO.dialogs [chosenText];
			//else
			//	textBox.FinalText = textOverride;

			//textBox.On = true;

			if(aS.clip)
				aS.Play ();

   //         StartCoroutine(CloseTextBox());

        }
			
	}

    public IEnumerator CloseTextBox()
    {
        float closeTimer = Time.time;
        
        while (Time.time < closeTimer + clearDelay)
        {
            yield return null;
        }

        GameUIManager.Instance.ClearTextBox();
    }
		
}
