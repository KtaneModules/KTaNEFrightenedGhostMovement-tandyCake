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

    public KMSelectable[] ghosts;
    public KMSelectable pacMan;
    public TextMesh[] displays;

    private Intersection[] ghostPositions;
    private Dir[] initialDisplayedDirections = new Dir[4];
    private Dir[] displayedDirections = new Dir[4];

    private Dir[] romTable;

    private int[] rngIndices;
    private Dir[] romTableDirections;
    private Dir[] finalDirections = new Dir[4];

    private bool isHeld;
    private float timeHeld;


    void Awake()
    {
        moduleId = moduleIdCounter++;
        pacMan.OnInteract += () => { HoldBtn(); return false; };
        pacMan.OnInteractEnded += () => ReleaseBtn();
        for (int i = 0; i < 4; i++)
        {
            int ix = i;
            ghosts[ix].OnInteract += () => { RotateArrow(ix); return false; };
        }
    }

    void Start()
    {
        GenerateROMTable();
        GetDisplayedInfo();
        GetRNGIndices();
        GetStartingDirections();
        GetFinalDirections();
        SetDisplays();
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
        Data.intersections.Shuffle();
        ghostPositions = Data.intersections.Take(4).ToArray();
        for (int i = 0; i < 4; i++)
        {
            initialDisplayedDirections[i] = ghostPositions[i].inlets.PickRandom();
            displayedDirections[i] = initialDisplayedDirections[i];
        }
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
        Log("Indexing the RNG indices mod 400 into the ROM Table gives starting directions of {0}.", romTableDirections.Join(", "));
    }
    void GetFinalDirections()
    {
        for (int ghostIx = 0; ghostIx < 4; ghostIx++)
        {
            Dir finalDir = romTableDirections[ghostIx];
            Dir forbidden = Data.Invert(initialDisplayedDirections[ghostIx]);
            while (!ghostPositions[ghostIx].outlets.Contains(finalDir) || finalDir == forbidden)
                finalDir = Data.Next(finalDir);
            finalDirections[ghostIx] = finalDir;
        }
        Log("The final directions for each ghost are {0}.", finalDirections.Join(", "));
    }

    void SetDisplays()
    {
        for (int i = 0; i < 4; i++)
        {
            displays[i].text = string.Format("{0}\n\n{1}", ghostPositions[i].coordinate, Data.arrows[displayedDirections[i]]);
            displays[i].color = Color.white;
        }
    }

    void HoldBtn()
    {
        if (isHeld)
            return;
        isHeld = true;
        pacMan.AddInteractionPunch();
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, pacMan.transform);
        timeHeld = Time.time;
    }
    void ReleaseBtn()
    {
        isHeld = false;
        pacMan.AddInteractionPunch(.5f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, pacMan.transform);
        if (moduleSolved)
            return;
        Audio.PlaySoundAtTransform("GM press", transform);
        if (Time.time - timeHeld > 0.4)
            Reset();
        else Submit();
    }
    void Reset()
    {
        for (int i = 0; i < 4; i++)
            displayedDirections[i] = initialDisplayedDirections[i];
        SetDisplays();
    }
    void Submit()
    {
        bool allCorrect = true;
        for (int i = 0; i < 4; i++)
        {
            if (displayedDirections[i] == finalDirections[i])
                displays[i].color = Color.green;
            else
            {
                allCorrect = false;
                displays[i].color = Color.red;
            }
        }
        if (allCorrect)
            Solve();
        else Strike();
    }
    void Solve()
    {
        moduleSolved = true;
        Audio.PlaySoundAtTransform("GM solve", transform);
        Log("Submitted the directions {0}. Module solved!", displayedDirections.Join(", "));
        Module.HandlePass();
    }
    void Strike()
    {
        Audio.PlaySoundAtTransform("GM strike", transform);
        Log("Submitted the directions {0}. Strike!", displayedDirections.Join(", "));
        Module.HandleStrike();
    }
    void RotateArrow(int pos)
    {
        ghosts[pos].AddInteractionPunch();
        if (moduleSolved)
            return;
        Audio.PlaySoundAtTransform("GM press", transform);
        displayedDirections[pos] = Data.Next(displayedDirections[pos]);
        SetDisplays();
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
    private readonly string TwitchHelpMessage = @"Use <!{0} submit URDL> to submit directions up, right, down, left. Use <!{0} reset> to reset the module.";
#pragma warning restore 414
    
    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim().ToUpperInvariant();
        Match m = Regex.Match(command, "SUBMIT ([URDL]{4})");
        if (command == "RESET")
        {
            yield return null;
            pacMan.OnInteract();
            yield return new WaitUntil(() => Time.time - timeHeld > 0.4);
            pacMan.OnInteractEnded();
        }
        else if (m.Success)
        {
            yield return null;
            string answer = m.Groups[1].Value;
            for (int i = 0; i < 4; i++)
            {
                while (displayedDirections[i].ToString()[0] != answer[i])
                {
                    ghosts[i].OnInteract();
                    yield return new WaitForSeconds(.2f);
                }
            }
            pacMan.OnInteract();
            pacMan.OnInteractEnded();
            yield return new WaitForSeconds(.2f);
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        for (int i = 0; i < 4; i++)
        {
            while (displayedDirections[i] != finalDirections[i])
            {
                ghosts[i].OnInteract();
                yield return new WaitForSeconds(.2f);
            }
        }
        pacMan.OnInteract();
        pacMan.OnInteractEnded();
        yield return new WaitForSeconds(.2f);
    }
}