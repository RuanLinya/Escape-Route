using Mono.Csv; // transder Mono.csv
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class GameLogic : MonoBehaviour
{
    public GameObject Smoke; // Create Smokes
    public GameObject Arrows; // Create arrows
    public GameObject StartLine; // Create a green line 
    public GameObject FinishTheStudy; // Create finish the Study Canvas 
    public GameObject WelcomeConvas; // Create welcome Canvas 
    public GameObject Exits;// Create escape exits 
    public InputField show_userInput;// Input partipant ID
    public Button StartButton;// Creat finish Button to save the participant ID
    private Boolean stopwatchActive; // create variable that is active as long as the stopwatch is active
    private float timeSinceStart; // start time 
    private int passedExitID;//creating a variable that holds the Exit IDnumber that has been passed
    public AudioClip firealarmSound; // create fire alarm Sound
    private AudioSource audioSource; // create a audio Source
    public Transform TransformOfEscapeRouteSigns; // acquire the Transform of Escape Route Signs
    private AudioSource[] EscapeRouteSigns; // create Escape Route Sign Array
    public GameObject SetCondition;// Create set Condition Canvas 
    public string conditionNum = "1"; // create condition nummer
    public string conditionName = "without audio"; // create condition name to add to the excel for more readability
    public Boolean isAudioActive = false; // create a boolean to set audio of escape route signs to active or not depending on selected condition
    private CsvFileWriter csvFileWriter;// write csv File 
    private CsvFileReader csvFileReader; // read csv File 
    public List<List<string>> allrows = new List<List<string>>(); // string of list informations - csv File 

    // fuctions ..................................................................................................................
    public void toggleAudioActive()
    {
        isAudioActive = !isAudioActive; //set the isAudioActive variable to the opposite
        if (isAudioActive)
        {
            setWithAudio(); // set audio to active
        }
        else
        {
            setWithoutAudio(); // set audio to inactive 
        }
    }
    
    // set condition functions
    void setWithoutAudio() // without audio 
    {
        conditionNum = "1";
        conditionName = "without audio" ;
        Debug.Log("audio inactive");
    }
    void setWithAudio() // with audio 
    {
        conditionNum = "2";
        conditionName = "with audio";
        Debug.Log("audio active");
    }

    void start()
    {
        stopwatchActive = false; // stopwatchActive function inactive
        Smoke.SetActive(false);// Smokes active
        Arrows.SetActive(true); // yellow Arrows on the floor active
        StartLine.SetActive(false); // green start line on the floor active 
        FinishTheStudy.SetActive(false); //  finish the study canvas active 
        WelcomeConvas.SetActive(true); //welcome convas canvas  inactive
        Exits.SetActive(false);// escape exits inactive to avoid Trigger
        timeSinceStart = 0; // start time is 0
        SetCondition.SetActive(true); //  set Condition Canvas active 
    }
    public void Condition()
    {
        SetCondition.SetActive(false); //  setCondition Canvas inactive
       
    }
    private void Awake() // set up a Audio
    {
        // Sound of Escape Route Signs loop
        EscapeRouteSigns = TransformOfEscapeRouteSigns.GetComponentsInChildren<AudioSource>(); // get Transform of Escpe Route Signs
        // Loop playback Escpe Route Signs
        foreach (AudioSource item in EscapeRouteSigns) // foreach a list of  Escape Route Signs
        {
            item.loop = true;
        }
        // Sound of fire alarm loop and set Volume of sound 
        audioSource = transform.GetComponent<AudioSource>();
        if (audioSource == null)
        {

            audioSource = this.gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.volume = 0.1f;

        }

        audioSource.clip = firealarmSound;
    }
    
    void Update()
    {   
        if (stopwatchActive == true)

        {
            timeSinceStart += Time.deltaTime;  //calcualtes seconds since start of stopwatch
            Exits.SetActive(true); // escape exits active to Trigger
        }
        
    }

    // function on what happens when a participant exits triggers
    public void OnTriggerExit (Collider other) // Participant exits triggers
    {
    // Set a Trigger of red square
        if (other.CompareTag("redsquare")) // Participants exits the red Square
        {
            audioSource.Play();
            Smoke.SetActive(true);
            Arrows.SetActive(false);
            StartLine.SetActive(true);
            WelcomeConvas.SetActive(false);
        }

        // Set a Trigger of Start Line
        if (other.CompareTag("StartLine")) // Participants crosses the start line
        {   
            Startwatch();

            // Only play sound of Escape Route Signs in the with audio condition
            if (isAudioActive)
            {
                // Sound of Escape Route Signs play
                foreach (AudioSource item in EscapeRouteSigns) // foreach a list of  Escape Route Signs
                {
                    item.Play();
                }
            }
        }
        // set passedExit variable to the ID number of the exit that has been passed by the player
        if (other.CompareTag("Exit1"))
        {   
            passedExitID = 1;
            Debug.Log("Exit1");        
            Stopwatch();
            Logging();
        }
        if (other.CompareTag("Exit2"))
        {   
            passedExitID = 2;
            Debug.Log("Exit2");
            Stopwatch();
            Logging();  
        }
        if (other.CompareTag("Exit3"))
        {   
            passedExitID = 3;
            Debug.Log("Exit3");
            Stopwatch();
            Logging();
        }
        if (other.CompareTag("Exit4"))
        {
            passedExitID = 4;
            Debug.Log("Exit4");
            Stopwatch();
            Logging();
        }
    }
    //create Startwatch function that start the time  
    public void Startwatch()
    {
        stopwatchActive = true;

    }
    //create Stopwatch function that stop the time 
    public void Stopwatch()
    {
        stopwatchActive = false;
        audioSource.Stop();
        Smoke.SetActive(false);
        FinishTheStudy.SetActive(true); //activates the finish the study canvases
        // Sound of Escape Route Signs stop
        foreach (AudioSource item in EscapeRouteSigns) // foreach a list of  Escape Route Signs
        {
            item.Stop();
        }
    }

    //############# LOGGING---- CONDITION NUMBER, CONDITION NAME,PARTICIPANT ID,EXIT,TIME IN ONE CSV.file ######################//

    //gets the path of the directory where we want to store our logs
    static string GetDirectoryPath()
    {
        return Application.dataPath + "/StreamingAssets/_Logs/";

    }

    //if your folder and file has not been created yet, it will be created. This also ensures your data goes to the correct file and folder:

    public void Logging()
    {     // read informations of csv File 
        allrows = CsvFileReader.ReadAll(Application.streamingAssetsPath + "/_Logs/Studydata.csv", Encoding.UTF8);
        List<string> xx = new List<string>();
        // informations of csv File 
        xx.Add(conditionNum);// writes the selected condition number
        xx.Add(conditionName);//writes the selected condition name for more readability
        xx.Add(show_userInput.text);//writes the ParticipantID since start 
        xx.Add(passedExitID.ToString());//writes the Exit ID 
        xx.Add(timeSinceStart.ToString());//writes the time since start 
        // write informations of csv File 
        allrows.Add(xx);
        csvFileWriter = new CsvFileWriter(Application.streamingAssetsPath + "/_Logs/Studydata.csv");
        csvFileWriter.WriteAll(allrows);
        csvFileWriter.Dispose();
    }
}
