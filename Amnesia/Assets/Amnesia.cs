using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class Amnesia : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable[] GonzoPornography;
    public KMSelectable[] Parkinsons;
    public Material[] Romping;
    public GameObject TimingIsEverything;
    public GameObject PercentGrey;

    int LunchTime = 0;
    float CrazyTalkWithAK = 0;
    int TheArena = 0;
    int[] Pressure = {0,0,0,0}; //BLUE GREEN ORANGE RED
    bool SimonForgets = false;
    int DirectionalButton = 0;
    int OmegaForget = 5;
    bool[] CookieClicker = {false,false,false,false};
    bool[] QRCode = {false,false,false,false};
    int AlphabeticalRuling = 0;
    bool MinecraftSurvival = false;
    bool PigpenRotations = false;
    string[] StickyNotes = {"blue","green","orange","red"};
    bool Binary = true;
    //bool Addition = false;
    private List<int> GoofysGame = new List<int>{};
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;

        foreach (KMSelectable Ryan in GonzoPornography) {
            Ryan.OnInteract += delegate () { RyanPress(Ryan); return false; };
        }
        foreach (KMSelectable SueetWall in Parkinsons) {
            SueetWall.OnInteract += delegate () { SueetWallPress(SueetWall); return false; };
        }
    }

    void Start() {
      CrazyTalkWithAK = Bomb.GetTime();
      /*if (LunchTime == 1) {
        GetComponent<KMBombModule>().HandlePass();
      }*/
    }

    void RyanPress(KMSelectable Ryan){
      Ryan.AddInteractionPunch();
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Ryan.transform);
      for (int i = 0; i < 10; i++) {
        if (Ryan == GonzoPornography[i] && SimonForgets == true) {
          AlphabeticalRuling *= 10;
          AlphabeticalRuling += i;
        }
      }
    }

    void SueetWallPress(KMSelectable SueetWall){
      SueetWall.AddInteractionPunch();
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, SueetWall.transform);
      Debug.Log(Binary);
      if (SueetWall == Parkinsons[0] && SimonForgets == true && Binary == true && AlphabeticalRuling == 0) {
        StartCoroutine(NeedlesslyComplicatedButton());
        Binary = false;
      }
      else if (SueetWall == Parkinsons[0] && SimonForgets == true) {//<
        AlphabeticalRuling = (AlphabeticalRuling - (AlphabeticalRuling % 10));
        AlphabeticalRuling /= 10;
      }
      else if (SueetWall == Parkinsons[1] && SimonForgets == true) {//S
        Binary = false;
        for (int i = 0; i < 4; i++) {
          if (AlphabeticalRuling == Pressure[i] && QRCode[i] == true) {
            PigpenRotations = true;
          }
        }
        if (PigpenRotations == true) {
          Audio.PlaySoundAtTransform("N", transform);
          GetComponent<KMBombModule>().HandlePass();
          Debug.LogFormat("[Amnesia #{0}] You submitted the correct number. Module disarmed.", moduleId);
        }
        else {
          GetComponent<KMBombModule>().HandleStrike();
          Debug.LogFormat("[Amnesia #{0}] You submitted {1}. Strike.", moduleId, AlphabeticalRuling);
          AlphabeticalRuling = 0;
          Alchemy();
        }
      }
    }

    void Alchemy(){
      SimonForgets = true;
      DirectionalButton = UnityEngine.Random.Range(0,4);
      Binary = true;
      for (int i = 0; i < 4; i++) {
        if (CookieClicker[i] == false) {
          Binary = false;
        }
      }
      if (Binary == true) {
        for (int j = 0; j < 4; j++) {
          CookieClicker[j] = false;
        }
      }
      while (CookieClicker[DirectionalButton] == true) {
        DirectionalButton = UnityEngine.Random.Range(0,4);
      }
      for (int i = 0; i < 4; i++) {
        QRCode[i] = false;
      }
      Debug.LogFormat("[Amnesia #{0}] It shows color {1}. Input {2}.", moduleId, StickyNotes[DirectionalButton], Pressure[DirectionalButton]);
      PercentGrey.GetComponent<MeshRenderer>().material = Romping[DirectionalButton];
      CookieClicker[DirectionalButton] = true;
      QRCode[DirectionalButton] = true;
    }

    void Update() {
      if (SimonForgets == true) {
        return;
      }
      LunchTime = Bomb.GetSolvableModuleNames().Count;
      TheArena = Bomb.GetSolvedModuleNames().Count;
      if (Bomb.GetTime() * 4 <= CrazyTalkWithAK || TheArena * 2 >= LunchTime) {
        Alchemy();
      }
      else if ((int)Math.Floor(Bomb.GetTime()) % 60 == 0 && MinecraftSurvival == false) {
        StartCoroutine(MinecraftParody());
        DirectionalButton = UnityEngine.Random.Range(0,4);
        while (DirectionalButton == OmegaForget) {
          DirectionalButton = UnityEngine.Random.Range(0,4);
        }
        OmegaForget = DirectionalButton;
        GoofysGame.Add(DirectionalButton);
        Audio.PlaySoundAtTransform("M", transform);
        switch (DirectionalButton) {
          case 0:
          Pressure[0] += 1;
          Debug.LogFormat("[Amnesia #{0}] Blue has now flashed {1} time(s).", moduleId, Pressure[0]);
          TimingIsEverything.GetComponent<MeshRenderer>().material = Romping[0];
          break;
          case 1:
          Pressure[1] += 1;
          Debug.LogFormat("[Amnesia #{0}] Green has now flashed {1} time(s).", moduleId, Pressure[1]);
          TimingIsEverything.GetComponent<MeshRenderer>().material = Romping[1];
          break;
          case 2:
          Pressure[2] += 1;
          Debug.LogFormat("[Amnesia #{0}] Orange has now flashed {1} time(s).", moduleId, Pressure[2]);
          TimingIsEverything.GetComponent<MeshRenderer>().material = Romping[2];
          break;
          case 3:
          Pressure[3] += 1;
          Debug.LogFormat("[Amnesia #{0}] Red has now flashed {1} time(s).", moduleId, Pressure[3]);
          TimingIsEverything.GetComponent<MeshRenderer>().material = Romping[3];
          break;
        }
      }
    }
    IEnumerator MinecraftParody(){
      MinecraftSurvival = true;
      yield return new WaitForSeconds(1f);
      MinecraftSurvival = false;
    }
    IEnumerator NeedlesslyComplicatedButton(){
      for (int i = 0; i < GoofysGame.Count(); i++) {
        TimingIsEverything.GetComponent<MeshRenderer>().material = Romping[GoofysGame[i]];
        yield return new WaitForSecondsRealtime(.33f);
      }
    }
}
