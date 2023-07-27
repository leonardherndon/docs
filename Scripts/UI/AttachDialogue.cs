using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

public class AttachDialogue : MonoBehaviour
{

    public bool loadLocalFile = true;
    public string localFilename;// = Application.persistentDataPath + "/cs_dialogue.xml";
    public string fileUrl = @"http://leo-j.com/game_builds/cs/latest/articy/cs_dialogue.xml";
    public bool useInGameDatabase;
    public DialogueDatabase inGameDatabase;

    void Start()
    {
        
        //if (localFilename == null)
        //{
        //    localFilename = Application.persistentDataPath + "/cs_dialogue.xml";
        //}
        //ConverterPrefs myPrefs = new ConverterPrefs
        //{
        //    EncodingType = EncodingType.UTF8,
        //    StageDirectionsAreSequences = false,
        //    ConvertDropdownsAs = ConverterPrefs.ConvertDropdownsModes.Ints,
        //    ConvertSlotsAs = ConverterPrefs.ConvertSlotsModes.ID,
        //    RecursionMode = ConverterPrefs.RecursionModes.On,
        //    FlowFragmentMode = ConverterPrefs.FlowFragmentModes.Quests,
        //    DirectConversationLinksToEntry1 = false,
        //    Overwrite = true
        //};

        //DialogueDatabase database = null;

        //if (loadLocalFile || Application.internetReachability == NetworkReachability.NotReachable)
        //{
        //    //Debug.LogWarning("Using Local Dialogue File.");
        //    // Load local file:
        //    var xml = System.IO.File.ReadAllText(localFilename);
        //    //Debug.Log(xml);
        //    database = ArticyConverter.ConvertXmlDataToDatabase(xml, myPrefs);
        //}
        //else
        //{
        //    // Load from WWW:
        //    if (Application.internetReachability == NetworkReachability.NotReachable) yield return null;
        //    using (WWW www = new WWW(fileUrl))
        //    {
        //        //Debug.Log("XML file Downloading");
        //        yield return www;

        //        if (www.isDone)
        //        {
        //            //Debug.Log("Dialogue Database Download Completed");
        //            //Debug.Log(www.text);
        //            database = ArticyConverter.ConvertXmlDataToDatabase(www.text, myPrefs);
        //            DialogueManager.AddDatabase(database);
        //            DialogueManager.SendUpdateTracker();
        //        }
        //    }
        //}

        //if (database == null)
        //{
        //    Debug.Log("Dialogue Database is null!");
        //}
        //else
        //{
        //    //Debug.Log("Conversations in database:");
        //    //foreach (var conversation in database.conversations)
        //    //{
        //    //    Debug.Log(conversation.Title);
        //    //}
        //}

        if (useInGameDatabase)
        {
            DialogueManager.AddDatabase(inGameDatabase);
        }
    }
}