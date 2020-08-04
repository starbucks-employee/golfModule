using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class golfScript : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable SubmitButton;
    public KMSelectable[] Arrowbuttons;
    public KMSelectable[] KeypadButtons;
    public GameObject Arrow;
    public GameObject GolfBall;
    public TextMesh Hole;
    public TextMesh Stroke;
    public TextMesh InputText;
    private int Jon = 9000000;

    int[] HoleStrokes = new int[9];
    int Handicap = 0;
    int WindDirection = 0;
    int CurrentHole = 1;
    int Total = 0;
    int Input = 0;
    float GolfBallDistance = 0.0767f;
    private List<int> ActualWindDirection = new List<int> {22, 45, 67, 90, 112, 135, 157, 180, 202, 225, 247, 270, 292, 315, 337, 360};
    private List<float> ArrowRotation = new List<float> {22.5f, 45f, 67.5f, 90f, 112.5f, 135f, 157.5f, 180f, 202.5f, 225f, 247.5f, 270f, 292.5f, 315f, 337.5f, 0f};
    private List<string> Terms = new List<string> {"Hole in One", "Eagle", "Birdie", "Par", "Bogey", "Double Bogey", "Triple Bogey", "+8", "+9", "+10", "+11"};
    private List<string> DirectionNames = new List<string> {"N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW"};




    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake() {
        moduleId = moduleIdCounter++;

        foreach (KMSelectable Arrow in Arrowbuttons) {
            Arrow.OnInteract += delegate () { ArrowPress(Arrow); return false; };
        }
        foreach (KMSelectable NumberButton in KeypadButtons)
        {
            NumberButton.OnInteract += delegate () { NumberPress(NumberButton); return false; };
        }
        SubmitButton.OnInteract += delegate () { PressSubmit(); return false; };
    }


    void Start() {
      for(int i = 0; i < 9; i++){
        HoleStrokes[i] = UnityEngine.Random.Range(1, 12);
      }
      WindDirection = UnityEngine.Random.Range(0,16);
      Debug.LogFormat("[Golf #{0}] The wind direction is {1} which is {2} degrees.", moduleId, DirectionNames[WindDirection], ActualWindDirection[WindDirection]);
      Arrow.transform.localEulerAngles = new Vector3(90f, 0f, ArrowRotation[WindDirection]);
      WindDirection = ActualWindDirection[WindDirection];
      Handicap = (((Bomb.GetModuleNames().Count()-9)*(113/WindDirection)) + 5500) % 55;
      Debug.LogFormat("[Golf #{0}] The Handicap value starts at {1}.", moduleId, Handicap);
      for(int i = 0; i < 9; i++){
      Debug.LogFormat("[Golf #{0}] Hole {1} was a(n) {2}, adds {3} strokes.", moduleId, i+1, Terms[HoleStrokes[i]-1], HoleStrokes[i]);
      }
      Stroke.text = Terms[HoleStrokes[0]-1];
      for(int i = 0; i < 9; i++){
        Total += HoleStrokes[i];
      }
      Debug.LogFormat("[Golf #{0}] The total amount of strokes is {1}.", moduleId, Total);
      Total += Handicap / 9;
      Debug.LogFormat("[Golf #{0}] Adding handicap divided by 9 gives {1}.", moduleId, Total);
    }

    void PressSubmit()
    {
      SubmitButton.AddInteractionPunch();
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress,SubmitButton.transform);
      if (Input == Total){
        Debug.LogFormat("[Golf #{0}] You entered {1}, module solved!", moduleId, Total);
        GetComponent<KMBombModule>().HandlePass();
        StartCoroutine(ping());
      }
      else{
        Debug.LogFormat("[Golf #{0}] You entered {1}, that is incorrect.", moduleId, Input);
        GetComponent<KMBombModule>().HandleStrike();
        Input &= 0;
        InputText.text = null;
      }
    }
    void ArrowPress(KMSelectable ArrowCunt)
    {
      ArrowCunt.AddInteractionPunch();
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress,ArrowCunt.transform);
          if(Arrow == Arrowbuttons[0])
            {
              if (CurrentHole != 1){
                CurrentHole -= 1;
                Stroke.text = Terms[HoleStrokes[CurrentHole - 1]-1];
                Hole.text = "Hole "+CurrentHole;
              }
            } else {
              if (CurrentHole != 9)
              {
                CurrentHole += 1;
                Stroke.text = Terms[HoleStrokes[CurrentHole - 1]-1];
                Hole.text = "Hole "+CurrentHole; }
            }
    }
    void NumberPress(KMSelectable NumberButton)
    {
      NumberButton.AddInteractionPunch();
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress,NumberButton.transform);
        for(int i = 0; i < 10; i++)
        {
            if (NumberButton == KeypadButtons[i] && Input.ToString().Length < 7)
            {
             Input = Input * 10 + i;
             InputText.text = Input.ToString();
            }
        }
    }
    IEnumerator ping(){
      Audio.PlaySoundAtTransform("ping_soundeffect", transform);
        for(int i = 0; i < 200; i++){
        yield return new WaitForSeconds(0.02f);
        GolfBallDistance += 0.05f;
        GolfBall.transform.localPosition = new Vector3(0.0189f, -0.01462f, GolfBallDistance);
      }
    }
 }
