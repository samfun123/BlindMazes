using KMBombInfoExtensions;
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class BlindMaze : MonoBehaviour
{
    private static int BlindMaze_moduleIdCounter = 1;
    private int BlindMaze_moduleId;
    public KMBombInfo BombInfo;
    public KMBombModule BombModule;
    public KMAudio Audio;
    public KMSelectable North;
    public KMSelectable East;
    public KMSelectable South;
    public KMSelectable West;
    public KMSelectable Reset;
    public MeshRenderer NorthMesh;
    public MeshRenderer EastMesh;
    public MeshRenderer SouthMesh;
    public MeshRenderer WestMesh;
    protected int MazeBased = 0;
    protected int MazeRot;
    private int currentMaze = -1;

    protected bool SOLVED = false;
    protected int LastDigit;
    protected string CurrentP = "";
    protected int CurX;
    protected int CurY;
    protected int RotX;
    protected int RotY;
    protected int SumNS;
    protected int SumEW;
    protected string[,] MazeWalls = new string[5, 5];
    protected int NumNorth;
    protected int NumEast;
    protected int NumSouth;
    protected int NumWest;
    protected int MazeNumber;
    protected string[] dir = new string[4];

    protected int REDKEY;
    protected bool NOYELLOW = true;
    protected int StartX;
    protected int StartY;

    private string TwitchHelpMessage = "Use !{0} NWSE, !{0} nwse, !{0} ULDR, or !{0} uldr to move North West South East. Use !{0} reset, or !{0} press reset to reset the maze back to starting position.";



    protected KMSelectable[] ProcessTwitchCommand(string TPInput)
    {
        TPInput = TPInput.ToLowerInvariant();
        if (TPInput.Equals("reset") || TPInput.Equals("press reset"))
        {
            return new KMSelectable[1] { Reset };
        }

        if (TPInput.StartsWith("move ") || TPInput.StartsWith("press ") || TPInput.StartsWith("walk ") || TPInput.StartsWith("submit "))
        {
            TPInput = TPInput.Substring(TPInput.IndexOf(" ", StringComparison.Ordinal) + 1);
        }

        List<KMSelectable> Moves = new List<KMSelectable>();
        foreach (char c in TPInput)
        {
            switch (c)
            {
                case 'n':
                case 'u':
                    Moves.Add(North);
                    break;
                case 'e':
                case 'r':
                    Moves.Add(East);
                    break;
                case 's':
                case 'd':
                    Moves.Add(South);
                    break;
                case 'w':
                case 'l':
                    Moves.Add(West);
                    break;
                default:
                    return null;
            }
        }
        return Moves.Count > 0
            ? Moves.ToArray()
            : null;
    }

    int GetSolvedCount()
    {
        return BombInfo.GetSolvedModuleNames().Count;
    }

    protected void Start()
    {
        BlindMaze_moduleId = BlindMaze_moduleIdCounter++;
        int ColNorth = UnityEngine.Random.Range(1, 6);
        int ColEast = UnityEngine.Random.Range(1, 6);
        int ColSouth = UnityEngine.Random.Range(1, 6);
        int ColWest = UnityEngine.Random.Range(1, 6);


        Reset.OnInteract += HandlePressReset;
        Reset.OnInteractEnded += HandleRelease;
        //check what the serial ends with and make an integer for it
        LastDigit = BombInfo.GetSerialNumberNumbers().Last();


        //Determine Values of the Knobs and Color the knobs 1RED 2GREEN 3WHITE 4GREY 5 YELLOW
        string ColNorthName = "";
        string ColEastName = "";
        string ColSouthName = "";
        string ColWestName = "";
        switch (ColNorth)
        {
            case 1:
                NumNorth = 1;
                NorthMesh.material.color = Color.red;
                REDKEY++;
                ColNorthName = "red";
                break;
            case 2:
                NumNorth = 5;
                NorthMesh.material.color = Color.green;
                ColNorthName = "green";
                break;
            case 3:
                NumNorth = 2;
                NorthMesh.material.color = Color.white;
                ColNorthName = "white";
                break;
            case 4:
                NumNorth = 2;
                NorthMesh.material.color = Color.grey;
                ColNorthName = "grey";
                break;
            case 5:
                NumNorth = 3;
                NorthMesh.material.color = Color.yellow;
                NOYELLOW = false;
                ColNorthName = "yellow";
                break;
        }

        switch (ColEast)
        {
            case 1:
                NumEast = 3;
                EastMesh.material.color = Color.red;
                REDKEY++;
                ColEastName = "red";
                break;
            case 2:
                NumEast = 1;
                EastMesh.material.color = Color.green;
                ColEastName = "green";
                break;
            case 3:
                NumEast = 5;
                EastMesh.material.color = Color.white;
                ColEastName = "white";
                break;
            case 4:
                NumEast = 5;
                EastMesh.material.color = Color.grey;
                ColEastName = "grey";
                break;
            case 5:
                NumEast = 2;
                EastMesh.material.color = Color.yellow;
                NOYELLOW = false;
                ColEastName = "yellow";
                break;
        }

        switch (ColSouth)
        {
            case 1:
                NumSouth = 3;
                SouthMesh.material.color = Color.red;
                REDKEY++;
                ColSouthName = "red";
                break;
            case 2:
                NumSouth = 2;
                SouthMesh.material.color = Color.green;
                ColSouthName = "green";
                break;
            case 3:
                NumSouth = 4;
                SouthMesh.material.color = Color.white;
                ColSouthName = "white";
                break;
            case 4:
                NumSouth = 3;
                SouthMesh.material.color = Color.grey;
                ColSouthName = "grey";
                break;
            case 5:
                NumSouth = 2;
                SouthMesh.material.color = Color.yellow;
                NOYELLOW = false;
                ColSouthName = "yellow";
                break;
        }

        switch (ColWest)
        {
            case 1:
                NumWest = 2;
                WestMesh.material.color = Color.red;
                REDKEY++;
                ColWestName = "red";
                break;
            case 2:
                NumWest = 5;
                WestMesh.material.color = Color.green;
                ColWestName = "green";
                break;
            case 3:
                NumWest = 3;
                WestMesh.material.color = Color.white;
                ColWestName = "white";
                break;
            case 4:
                NumWest = 1;
                WestMesh.material.color = Color.grey;
                ColWestName = "grey";
                break;
            case 5:
                NumWest = 4;
                WestMesh.material.color = Color.yellow;
                NOYELLOW = false;
                ColWestName = "yellow";
                break;
        }

        //Look for mazebased modules
        if (BombInfo.GetModuleNames().Contains("Mouse In The Maze"))
        { MazeBased++; }
        if (BombInfo.GetModuleNames().Contains("3D Maze"))
        { MazeBased++; }
        if (BombInfo.GetModuleNames().Contains("Hexamaze"))
        { MazeBased++; }
        if (BombInfo.GetModuleNames().Contains("Maze"))
        { MazeBased++; }
        if (BombInfo.GetModuleNames().Contains("Morse-A-Maze"))
        { MazeBased++; }
        if (BombInfo.GetModuleNames().Contains("Blind Maze"))
        { MazeBased++; }
        if (BombInfo.GetModuleNames().Contains("Polyhedral Maze"))
        { MazeBased++; }


        //determine rotation
        int MazeRule;
        if (BombInfo.GetBatteryCount(KMBI.KnownBatteryType.D) == 1 && BombInfo.GetBatteryCount(KMBI.KnownBatteryType.AA) == 0)
        {
            MazeRot = 20;
            MazeRule = 1;
        }
        else if (BombInfo.GetPorts().Distinct().Count() < 3)
        {
            MazeRot = 40;
            MazeRule = 2;
        }
        else if (BombInfo.GetSerialNumberLetters().Any("AEIOU".Contains) && BombInfo.GetOnIndicators().Contains("IND"))
        {
            MazeRot = 30;
            MazeRule = 3;
        }
        else if (REDKEY > 0 && NOYELLOW == true)
        {
            MazeRot = 40;
            MazeRule = 4;
        }
        else if (MazeBased > 2)
        {
            MazeRot = 30;
            MazeRule = 5;
        }
        else if (BombInfo.GetOffIndicators().Contains("MSA") && REDKEY > 1)
        {
            MazeRot = 20;
            MazeRule = 6;
        }
        else
        {
            MazeRot = 10;
            MazeRule = 7;
        }

        SumNS = (NumSouth + NumNorth + 4) % 5;
        SumEW = (NumEast + NumWest + 4) % 5;

        //Determine Current Position
        if (MazeRot == 10)
        {
            North.OnInteract += HandlePressN;
            East.OnInteract += HandlePressE;
            South.OnInteract += HandlePressS;
            West.OnInteract += HandlePressW;
            dir = new string[] { "North", "East", "South", "West"};
            CurX = SumNS;
            CurY = SumEW;
        }
        if (MazeRot == 20)
        {
            North.OnInteract += HandlePressW;
            East.OnInteract += HandlePressN;
            South.OnInteract += HandlePressE;
            West.OnInteract += HandlePressS;
            dir = new string[] { "East", "South", "West", "North" };
            CurX = SumEW;
            CurY = 4 - SumNS;
        }
        if (MazeRot == 30)
        {
            North.OnInteract += HandlePressS;
            East.OnInteract += HandlePressW;
            South.OnInteract += HandlePressN;
            West.OnInteract += HandlePressE;
            dir = new string[] { "South", "West", "North", "East" };
            CurX = 4 - SumNS;
            CurY = 4 - SumEW;
        }
        if (MazeRot == 40)
        {
            North.OnInteract += HandlePressE;
            East.OnInteract += HandlePressS;
            South.OnInteract += HandlePressW;
            West.OnInteract += HandlePressN;
            dir = new string[] { "West", "North", "East", "South" };
            CurX = 4 - SumEW;
            CurY = SumNS;
        }

        North.OnInteractEnded += HandleRelease;
        East.OnInteractEnded += HandleRelease;
        South.OnInteractEnded += HandleRelease;
        West.OnInteractEnded += HandleRelease;

        StartX = CurX;
        StartY = CurY;
        DebugLog("Maze Rotation is {0} degrees clockwise because of rule {1}", MazeRot * 9 - 90, MazeRule);
        DebugLog("North Key is {0}, making it's value {1}", ColNorthName, NumNorth);
        DebugLog("East Key is {0}, making it's value {1}", ColEastName, NumEast);
        DebugLog("South Key is {0}, making it's value {1}", ColSouthName, NumSouth);
        DebugLog("West Key is {0}, making it's value {1}", ColWestName, NumWest);

        DebugLog("Starting Location is [{0},{1}]", SumNS + 1, SumEW + 1);
    }

    protected void ButtonRotation(int x,int y)
    {
        switch (MazeRot)
        {
            case 10:
                RotX = x + 1;
                RotY = y + 1;
                break;
            case 20:
                RotX = 5 - y;
                RotY = x + 1;
                break;
            case 30:
                RotX = 5 - x;
                RotY = 5 - y;
                break;
            case 40:
                RotX = y + 1;
                RotY = 5 - x;
                break;
        }
    }

    protected void HandleRelease()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, transform);
    }

    protected bool ExitedMaze(string direction,string Dir)
    {
        if (!CurrentP.Contains(direction.Substring(0, 1))) return false;

        BombModule.HandlePass();
        SOLVED = true;
        DebugLog("Moving {0}: The module has been defused.", Dir);
        return true;
    }

    protected bool HandlePressN()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        North.AddInteractionPunch(0.5f);

        if (SOLVED || ExitedMaze("North",dir[0])) return false;

        if (CurrentP.Contains("U"))
        {
            ButtonRotation(CurX, CurY);
            DebugLog("There is a wall to the {0} at X = {1}, Y = {2}. Strike", dir[0], RotX, RotY);
            BombModule.HandleStrike();
        }
        else
        {
            CurY--;
            CurrentP = MazeWalls[CurY, CurX];
            ButtonRotation(CurX, CurY);
            DebugLog("Moving {0}: X = {1}, Y = {2}", dir[0], RotX, RotY);
        }
        return false;
    }

    protected bool HandlePressE()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        East.AddInteractionPunch(0.5f);

        if (SOLVED || ExitedMaze("East",dir[1])) return false;

        if (CurrentP.Contains("R"))
        {
            ButtonRotation(CurX, CurY);
            DebugLog("There is a wall to the {0} at X = {1}, Y = {2}. Strike.", dir[1], RotX, RotY);
            BombModule.HandleStrike();
        }
        else
        {
            CurX++;
            CurrentP = MazeWalls[CurY, CurX];
            ButtonRotation(CurX, CurY);
            DebugLog("Moving {0}: X = {1}, Y = {2}", dir[1], RotX, RotY);
        }

        return false;
    }

    protected bool HandlePressS()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        South.AddInteractionPunch(0.5f);

        if (SOLVED || ExitedMaze("South",dir[2])) return false;

        if (CurrentP.Contains("D"))
        {
            ButtonRotation(CurX, CurY);
            DebugLog("There is a wall to the {0} at X = {1}, Y = {2}. Strike.", dir[2], RotX, RotY);
            BombModule.HandleStrike();
        }
        else
        {
            CurY = CurY + 1;
            CurrentP = MazeWalls[CurY, CurX];
            ButtonRotation(CurX, CurY);
            DebugLog("Moving {0}: X = {1}, Y = {2}", dir[2], RotX, RotY);
        }
        return false;
    }

    protected bool HandlePressW()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        West.AddInteractionPunch(0.5f);

        if (SOLVED || ExitedMaze("West",dir[3])) return false;

        if (CurrentP.Contains("L"))
        {
            ButtonRotation(CurX, CurY);
            DebugLog("There is a wall to the {0} at X = {1}, Y = {2}. Strike.", dir[3], RotX, RotY);
            BombModule.HandleStrike();
        }
        else
        {
            CurX = CurX - 1;
            CurrentP = MazeWalls[CurY, CurX];
            ButtonRotation(CurX, CurY);
            DebugLog("Moving {0}: X = {1}, Y = {2}", dir[3], RotX, RotY);
        }
        return false;
    }

    protected bool HandlePressReset()
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        Reset.AddInteractionPunch(0.5f);
        if (SOLVED) return false;

        CurX = StartX;
        CurY = StartY;
        DebugLog("Current location reset back to Starting location: X = {0}, Y = {1}", SumNS + 1, SumEW + 1);
        return false;
    }

    private void Update()
    {
        if (currentMaze == GetSolvedCount() || SOLVED) return;
        currentMaze = GetSolvedCount();
        MazeNumber = (LastDigit + GetSolvedCount()) % 10;
        DebugLog("The Maze Number is now {0}", MazeNumber);
        UpdateMaze();
    }

    private void UpdateMaze()
    {
        switch (MazeNumber)
        {
            case 1:
                MazeWalls = new string[5, 5] {
                    { "U D L", "U R", "N R L", "U L", "U R" },
                    { "U L", "D", "R D", "R L", "R D L" },
                    { "L", "U", "U D", "D", "U R" },
                    { "R L", "R D L", "U R L", "U D L", "R" },
                    { "D L", "U D", "R D", "U D L", "R D" }
                };
                break;
            case 0:
                MazeWalls = new string[5, 5] {
                    { "U L", "U", "N D R", "L U", "U R" },
                    { "L R", "D L ", "U", "D R", "L R" },
                    { "L D", "U R", "L D", "U R", "L D R" },
                    { "L U", "R", "L U R", "D L", "U R" },
                    { "D L R", "L D", "D R", "D U L", "R D" }
                };
                break;
            case 2:
                MazeWalls = new string[5, 5] {
                    { "U L", "U R D", "N L", "U R D", "L U R" },
                    { "L D", "U D", "D", "U D", "R" },
                    { "L U D", "U", "U", "U", "D R" },
                    { "L U R", "R L", "R L", "L D", "U R" },
                    { "D L", "D R", "L D", "U D R", "L R D" }
                };
                break;
            case 3:
                MazeWalls = new string[5, 5] {
                    { "U L D", "U R", "L N D", "U", "U R" },
                    { "L U", "D", "D U", "D R", "L R" },
                    { "L", "U R", "U L", "U", "R D" },
                    { "L D R", "L R D", "R L", "L", "U R" },
                    { "D L U", "D U", " D R", "L D R", " L D R" }
                };
                break;
            case 4:
                MazeWalls = new string[5, 5] {
                    { "U L",   "U",     "N D",   "U R", "L U R" },
                    { "L D R", "L R",   "L U R", "L D", "R" },
                    { "L U",   "D",     "D R",   "L U", "D R" },
                    { "L R",   "U D L", "R U",   "D L", "R U" },
                    { "D L",   "U D R",     "D L",     "U D", "R D" }
                };
                break;
            case 5:
                MazeWalls = new string[5, 5] {
                    { "U L",   "U R", "L D N", "U D",   "U R"   },
                    { "L R",   "L",   "U R D", "U L",   "D R"   },
                    { "L R",   "L",   "U D",   "",      "U R"   },
                    { "L R D", "L R", "U L",   "R D",   "L R"   },
                    { "D L U", "R D", "D L",   "D U R", "L R D" }
                };
                break;
            case 6:
                MazeWalls = new string[5, 5] {
                    { "U L", "U", "N D", "U D", "U R"},
                    { "L R", "L D R", "L U", "U R", "L R"},
                    { "L R", "U D L", "D R", "L", "R"},
                    { "L D", "U R", "U D L", "R D", "L R"},
                    { "D L U", "D", "D R U", "D L U", "R D"}
                };
                break;
            case 7:
                MazeWalls = new string[5, 5] {
                    { "U L D", "U R",   "R N L", "U L R", "L U R" },
                    { "L U",   "R",     "R L",   "L",     "D R" },
                    { "L R",   "L D",   "R",     "L D",   "U R" },
                    { "L R",   "L R U", "L D",   "U R",   "L R" },
                    { "D L R", "L D",   "U D",   "D",     "R D" }
                };
                break;
            case 8:
                MazeWalls = new string[5, 5] {
                    { "U L",   "U D",   "N R",   "U L",   "U R D" },
                    { "L R",   "U L",   "D",     "D",     "U R" },
                    { "L R",   "L D",   "U R D", "U L D", "R" },
                    { "L D",   "U R D", "U L",   "U R",   "L R" },
                    { "D L U", "U D",   "D R",   "L D",   "R D" }
                };
                break;
            case 9:
                MazeWalls = new string[5, 5] {
                    { "U L R", "L U D", "N R", "L U", "U R"   },
                    { "L D",   "U R",   "L R", "L R", "L R D" },
                    { "L U R", "D L",   "D",   "",    "D R"     },
                    { "L",     "U R",   "U L", "",    "U D R"     },
                    { "D L R", "D L",   "D R", "D",   "U R D"   }
                };
                break;
        }


        CurrentP = MazeWalls[CurY, CurX];
}

    private void DebugLog(string log, params object[] args)
    {
        var logData = string.Format(log, args);
        Debug.LogFormat("[Blind Maze #{0}]: {1}", BlindMaze_moduleId, logData);
    }
}
