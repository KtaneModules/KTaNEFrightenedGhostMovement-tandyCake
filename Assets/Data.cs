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

    public static Intersection T(string coord, Dir d)
    {
        return new Intersection() { coordinate = coord, }
    }

    private static List<Dir> FourWay
    { get { return new List<Dir>() { Dir.Right, Dir.Down, Dir.Left, Dir.Up }} }
    
}
public static class Data
{
    public static List<Intersection> intersections = new List<Intersection>()
    {
        new Intersection( "F1", TOut(Dir.Up),  ),
        new Intersection( "U1", TOut(Dir.Up) ),
        new Intersection( "A5", TOut(Dir.Left) ),
        new Intersection( "F5", FourWay ),
        new Intersection( "I5", TOut(Dir.Up) ),
        new Intersection( "R5", TOut(Dir.Up) ),
        new Intersection( "U5", FourWay ),
        new Intersection( "Z5", TOut(Dir.Right) ),
        new Intersection( "F8", TOut(Dir.Right) ),
        new Intersection( "U8", TOut(Dir.Left) ),
        new Intersection( "L11", Horiz ),
        new Intersection( "O11", Horiz ),
        new Intersection( "F14", FourWay ),
        new Intersection( "I14", TOut(Dir.Right) ),
        new Intersection( "R14", TOut(Dir.Left) ),
        new Intersection( "U14", FourWay ),
        new Intersection( "I17", TOut(Dir.Left) ),
        new Intersection( "R17", TOut(Dir.Right) ),
        new Intersection( "F20", FourWay ),
        new Intersection( "I20", TOut(Dir.Down) ),
        new Intersection( "R20", TOut(Dir.Down) ),
        new Intersection( "U20", FourWay ),
        new Intersection( "F23", TOut(Dir.Left) ),
        new Intersection( "I23", TOut(Dir.Up) ),
        new Intersection( "L23", Horiz ),
        new Intersection( "O23", Horiz ),
        new Intersection( "R23", TOut(Dir.Up) ),
        new Intersection( "U23", TOut(Dir.Right) ),
        new Intersection( "C26", TOut(Dir.Down) ),
        new Intersection( "X26", TOut(Dir.Down) ),
        new Intersection( "L29", TOut(Dir.Down) ),
        new Intersection( "O29", TOut(Dir.Down) )
    };
    public static Dictionary<Dir, char> arrows = new Dictionary<Dir, char>()
    {
        { Dir.Right, '→' },
        { Dir.Down, '↓' },
        { Dir.Left, '←' },
        { Dir.Up, '↑' },
    };
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