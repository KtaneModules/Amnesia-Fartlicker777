using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class Amnesia : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;

   public KMSelectable[] GonzoPornography;
   public KMSelectable[] Parkinsons;

   public Material[] ColorMats;

   public GameObject MainScreen;
   public GameObject SmallScreen;

   int[] ColorAmounts = new int[4]; //BLUE GREEN ORANGE RED
   private List<int> ColorsInOrder = new List<int> { };
   int CurrentColor;
   int PreviousColor = 5;
   int SolvedMods;
   int SubmissionInput;
   int TotalMods;

   float StartingTime;

   static readonly string[] ColorsForLogging = { "blue", "green", "orange", "red" };

   bool[] PreviousSelectedColorsForSubmission = new bool[4];
   bool[] SelectedColorForSubmission = new bool[4];
   bool AreAllColorsChosen = true;
   bool IsSolvable;
   bool StageDelay;

   static int moduleIdCounter = 1;
   int moduleId;
   private bool moduleSolved;

   void Awake () {
      moduleId = moduleIdCounter++;

      foreach (KMSelectable Button in GonzoPornography) {
         Button.OnInteract += delegate () { ButtonPress(Button); return false; };
      }
      foreach (KMSelectable Button in Parkinsons) {
         Button.OnInteract += delegate () { NotNumericalPress(Button); return false; };
      }
   }

   void Start () {
      StartingTime = Bomb.GetTime();
   }

   void ButtonPress (KMSelectable Button) {
      if (moduleSolved) {
         Audio.PlaySoundAtTransform("M", transform);
      }
      Button.AddInteractionPunch();
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Button.transform);
      for (int i = 0; i < 10; i++) {
         if (Button == GonzoPornography[i] && IsSolvable) {
            SubmissionInput *= 10;
            SubmissionInput += i;
         }
      }
   }

   void NotNumericalPress (KMSelectable Button) {
      Button.AddInteractionPunch();
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Button.transform);
      if (Button == Parkinsons[0] && IsSolvable && AreAllColorsChosen && SubmissionInput == 0) {
         StartCoroutine(StrikeFlash());
         AreAllColorsChosen = false;
      }
      else if (Button == Parkinsons[0] && IsSolvable) {//<
         SubmissionInput = (SubmissionInput - (SubmissionInput % 10)) / 10;
      }
      else if (Button == Parkinsons[1] && IsSolvable) {//S
         AreAllColorsChosen = false;
         for (int i = 0; i < 4; i++) {
            if (SubmissionInput == ColorAmounts[i] && SelectedColorForSubmission[i]) {
               Audio.PlaySoundAtTransform("N", transform);
               GetComponent<KMBombModule>().HandlePass();
               Debug.LogFormat("[Amnesia #{0}] You submitted the correct number. Module disarmed.", moduleId);
               moduleSolved = true;
               return;
            }
         }
         GetComponent<KMBombModule>().HandleStrike();
         Debug.LogFormat("[Amnesia #{0}] You submitted {1}. Strike.", moduleId, SubmissionInput);
         SubmissionInput = 0;
         SubmissionColorChooser();
      }
   }

   void SubmissionColorChooser () {
      IsSolvable = true;
      CurrentColor = UnityEngine.Random.Range(0, 4);
      AreAllColorsChosen = true;
      for (int i = 0; i < 4; i++) {
         if (!PreviousSelectedColorsForSubmission[i]) {
            AreAllColorsChosen = false;
         }
      }
      if (AreAllColorsChosen) {
         for (int j = 0; j < 4; j++) {
            PreviousSelectedColorsForSubmission[j] = false;
         }
      }
      while (PreviousSelectedColorsForSubmission[CurrentColor]) {
         CurrentColor = UnityEngine.Random.Range(0, 4);
      }
      for (int i = 0; i < 4; i++) {
         SelectedColorForSubmission[i] = false;
      }
      Debug.LogFormat("[Amnesia #{0}] It shows color {1}. Input {2}.", moduleId, ColorsForLogging[CurrentColor], ColorAmounts[CurrentColor]);
      SmallScreen.GetComponent<MeshRenderer>().material = ColorMats[CurrentColor];
      PreviousSelectedColorsForSubmission[CurrentColor] = true;
      SelectedColorForSubmission[CurrentColor] = true;
   }

   void Update () {
      if (IsSolvable) {
         return;
      }
      TotalMods = Bomb.GetSolvableModuleNames().Count;
      SolvedMods = Bomb.GetSolvedModuleNames().Count;
      if (TotalMods == 1) {
         GetComponent<KMBombModule>().HandlePass();
      }
      if (Bomb.GetTime() * 4 <= StartingTime || SolvedMods * 2 >= TotalMods) {
         SubmissionColorChooser();
      }
      else if ((int) Math.Floor(Bomb.GetTime()) % 60 == 0 && !StageDelay && Bomb.GetTime() != StartingTime) {
         StartCoroutine(WaitForMinuteToPass());
         CurrentColor = UnityEngine.Random.Range(0, 4);
         while (CurrentColor == PreviousColor) {
            CurrentColor = UnityEngine.Random.Range(0, 4);
         }
         PreviousColor = CurrentColor;
         ColorsInOrder.Add(CurrentColor);
         Audio.PlaySoundAtTransform("M", transform);
         switch (CurrentColor) {
            case 0:
               ColorAmounts[0]++;
               Debug.LogFormat("[Amnesia #{0}] Blue has now flashed {1} time(s). Current time is {2} minutes.", moduleId, ColorAmounts[0], Bomb.GetTime() / 60);
               MainScreen.GetComponent<MeshRenderer>().material = ColorMats[0];
               break;
            case 1:
               ColorAmounts[1]++;
               Debug.LogFormat("[Amnesia #{0}] Green has now flashed {1} time(s). Current time is {2} minutes.", moduleId, ColorAmounts[1], Bomb.GetTime() / 60);
               MainScreen.GetComponent<MeshRenderer>().material = ColorMats[1];
               break;
            case 2:
               ColorAmounts[2]++;
               Debug.LogFormat("[Amnesia #{0}] Orange has now flashed {1} time(s). Current time is {2} minutes.", moduleId, ColorAmounts[2], Bomb.GetTime() / 60);
               MainScreen.GetComponent<MeshRenderer>().material = ColorMats[2];
               break;
            case 3:
               ColorAmounts[3]++;
               Debug.LogFormat("[Amnesia #{0}] Red has now flashed {1} time(s). Current time is {2} minutes.", moduleId, ColorAmounts[3], Bomb.GetTime() / 60);
               MainScreen.GetComponent<MeshRenderer>().material = ColorMats[3];
               break;
         }
      }
   }

   IEnumerator WaitForMinuteToPass () {
      StageDelay = true;
      yield return new WaitForSecondsRealtime(2f);
      StageDelay = false;
   }

   IEnumerator StrikeFlash () {
      for (int i = 0; i < ColorsInOrder.Count(); i++) {
         MainScreen.GetComponent<MeshRenderer>().material = ColorMats[ColorsInOrder[i]];
         yield return new WaitForSecondsRealtime(.33f);
      }
   }

   //I add the twitch play
#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"!{0} submit 123 to submit 123. Do !{0} cycle to cycle stages (only when you have struck four times).";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      yield return null;
      Command.Trim();
      if (Regex.IsMatch(Command, @"^\s*Cycle\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) {
         SubmissionInput = 0;
         Parkinsons[0].OnInteract();
         yield break;
      }
      string[] parameters = Command.Split(' ');
      if (Regex.IsMatch(parameters[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) {
         for (int i = 0; i < parameters[1].Length; i++) {
            for (int j = 0; j < 10; j++) {
               if (parameters[1][i].ToString() == j.ToString()) {
                  GonzoPornography[j].OnInteract();
                  int TPPoints = (int) Math.Ceiling((float) ColorsInOrder.Count() / 2);
                  yield return "awardpointsonsolve " + TPPoints;
               }
            }
         }
         Parkinsons[1].OnInteract();
      }
   }

   IEnumerator TwitchHandleForcedSolve () {
      while (!IsSolvable) {
         yield return true;
      }
      for (int i = 0; i < 4; i++) {
         if (SelectedColorForSubmission[i]) {
            yield return ProcessTwitchCommand(ColorAmounts[i].ToString());
         }
      }
   }
}
