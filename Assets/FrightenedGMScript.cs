using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class FrightenedGMScript : MonoBehaviour
{

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMBombModule Module;
    public KMRuleSeedable Ruleseed;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;
    private bool activated;

    public KMSelectable[] ghosts;
    public KMSelectable pacMan;
    public TextMesh[] displays;

    private string[] ghostPositions = new string[];
    private Dir[] initialDisplayedDirections;
    private Dir[] displayedDirections;

    private Dir[] romTable;

    private int[] rngIndices;
    private Dir[] romTableDirections;
    private Dir[] finalDirections;

    private bool isHeld;
    private float timeHeld;


    void Awake()
    {
        moduleId = moduleIdCounter++;
        pacMan.OnInteract += () => { HoldBtn(); return false; };
        pacMan.OnInteractEnded += () => ReleaseBtn();
        for (int i = 0; i < 4; i++)
        {
            int ix = 0;
            ghosts[ix].OnInteract += () => { RotateArrow(ix); return false; };
        }
        Module.OnActivate += () => DisplayInfo();
    }

    void Start()
    {
        GenerateROMTable();
        GetDisplayedInfo();
        GetRNGIndices();
        GetStartingDirections();
    }
    void GenerateROMTable()
    {
        List<Dir> table = new List<Dir>(400);
        for (int i = 0; i < 101; i++)
            table.Add(Dir.Right);
        for (int i = 0; i < 114; i++)
            table.Add(Dir.Down);
        for (int i = 0; i < 120; i++)
            table.Add(Dir.Left);
        for (int i = 0; i < 65; i++)
            table.Add(Dir.Up);

        Ruleseed.GetRNG().ShuffleFisherYates(table);
        romTable = table.ToArray();
        Log("Generated ROM Table with rule seed {0}.", Ruleseed.GetRNG().Seed);
    }
    void GetDisplayedInfo()
    {

    }
    void GetRNGIndices()
    {
        rngIndices = new int[4];
        rngIndices[0] = Concat(Bomb.GetSerialNumberNumbers()) % 8192;
        for (int i = 1; i < 4; i++)
            rngIndices[i] = (5 * rngIndices[i - 1] + 1) % 8192;
        Log("The RNG indices are {0}.", rngIndices.Join(", "));
    }
    void GetStartingDirections()
    {
        romTableDirections = rngIndices.Select(x => romTable[x % 400]).ToArray();
        Log("Indexing the RNG indices mod 400 into the ROM Table gives starting directions of {0}.", romTable.Join(", "));
    }

    void DisplayInfo()
    {
        activated = true;
        for (int i = 0; i < 4; i++)
        {
            displayedDirections[i] = (Dir)Rnd.Range(0, 4);
            displays[i].text = string.Format("{0}\n\n{1}", ghostPositions[i], Data.arrows[displayedDirections[i]]);
        }
    }

    void HoldBtn()
    {
        if (isHeld)
            return;
        isHeld = true;
        pacMan.AddInteractionPunch(.75f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, pacMan.transform);
        timeHeld = Time.time;
    }
    void ReleaseBtn()
    {
        isHeld = false;
        pacMan.AddInteractionPunch(.25f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, pacMan.transform);
        if (!activated || moduleSolved)
            return;
        if (Time.time - timeHeld > .4f)
            Reset();
        else Submit();
    }
    void Reset()
    {

    }
    void Submit()
    {

    }
    void RotateArrow(int pos)
    {

    }
    int Concat(IEnumerable<int> nums)
    {
        int val = 0;
        foreach (int num in nums)
        {
            val *= 10;
            val += num;
        }
        return val;
    }
    void Log(string msg, params object[] args)
    {
        Debug.LogFormat("[Frightened Ghost Movement #{0}] {1}", moduleId, string.Format(msg, args));
    }
#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use <!{0} foobar> to do something.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim().ToUpperInvariant();
        List<string> parameters = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        yield return null;
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        yield return null;
    }
}