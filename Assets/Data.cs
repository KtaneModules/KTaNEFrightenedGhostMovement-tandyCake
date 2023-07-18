using System.Collections.Generic;

public enum Dir
{
    Right,
    Down,
    Left,
    Up
}
public class Intersection
{
    public string coordinate;
    public List<Dir> outlets;
    public List<Dir> inlets;

    public static Intersection T(string coord, Dir wall)
    {
        List<Dir> all1 = All;
        List<Dir> all2 = All;
        all1.Remove(wall);
        all2.Remove(Data.Invert(wall));
        return new Intersection() { coordinate = coord, outlets = all1, inlets = all2 };
    }
    public static Intersection FourWay(string coord)
    {
        return new Intersection() { coordinate = coord, inlets = All, outlets = All};
    }
    public static Intersection Horiz(string coord, Dir wall)
    {
        List<Dir> ins = All;
        ins.Remove(wall);
        return new Intersection() { coordinate = coord, inlets = ins, outlets = new List<Dir>() { Dir.Left, Dir.Right } };
    }
    private static List<Dir> All
    { get { return new List<Dir>() { Dir.Right, Dir.Down, Dir.Left, Dir.Up }; } }
    
}
public static class Data
{
    public static List<Intersection> intersections = new List<Intersection>()
    {
        Intersection.T("F1", Dir.Up),
        Intersection.T("U1", Dir.Up),
        Intersection.T("A5", Dir.Left),
        Intersection.FourWay("F5"),
        Intersection.T("I5", Dir.Up),
        Intersection.T("L5", Dir.Down),
        Intersection.T("O5", Dir.Down),
        Intersection.T("R5", Dir.Up),
        Intersection.FourWay("U5"),
        Intersection.T("Z5", Dir.Right),
        Intersection.T("F8", Dir.Right),
        Intersection.T("U8", Dir.Left),
        Intersection.Horiz("L11", Dir.Down),
        Intersection.Horiz("O11", Dir.Down),
        Intersection.FourWay("F14"),
        Intersection.T("I14", Dir.Right),
        Intersection.T("R14", Dir.Left),
        Intersection.FourWay("U14"),
        Intersection.T("I17", Dir.Left),
        Intersection.T("R17", Dir.Right),
        Intersection.FourWay("F20"),
        Intersection.T("I20", Dir.Down),
        Intersection.T("R20", Dir.Down),
        Intersection.FourWay("U20"),
        Intersection.T("F23", Dir.Left),
        Intersection.T("I23", Dir.Up),
        Intersection.Horiz("L23", Dir.Down),
        Intersection.Horiz("O23", Dir.Down),
        Intersection.T("R23", Dir.Up),
        Intersection.T("U23", Dir.Right),
        Intersection.T("C26", Dir.Down),
        Intersection.T("X26", Dir.Down),
        Intersection.T("L29", Dir.Down),
        Intersection.T("O29", Dir.Down)
    };
    public static Dictionary<Dir, char> arrows = new Dictionary<Dir, char>()
    {
        { Dir.Right, '→' },
        { Dir.Down, '↓' },
        { Dir.Left, '←' },
        { Dir.Up, '↑' },
    };
    public static Dir Next(Dir d)
    {
        return (Dir)(((int)d + 1) % 4);
    }
    public static Dir Invert(Dir d)
    {
        return (Dir)(((int)d + 2) % 4);
    }
    private static List<Dir> FourWay
    { get { return new List<Dir>() { Dir.Right, Dir.Down, Dir.Left, Dir.Up }; } }
    private static List<Dir> Horiz
    { get { return new List<Dir>() { Dir.Left, Dir.Right }; }}
    private static List<Dir> TOut(Dir d)
    {
        List<Dir> all = FourWay;
        all.Remove(d);
        return all;
    }
    private static List<Dir> TIn(Dir d)
    {
        List<Dir> all = FourWay;
        all.Remove((Dir)(((int)d + 2) % 4));
        return all;
    }
}