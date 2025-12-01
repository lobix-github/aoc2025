using System.Numerics;

public static class DExts
{
    public static void AddReplace<T>(this List<T> list, T value, Predicate<T> match)
    {
        var pos = list.FindIndex(match);
        if (pos != -1)
            list[pos] = value;
        else
            list.Add(value);
    }

    public static void RemoveIfExists<T>(this List<T> list, Predicate<T> match)
    {
        var pos = list.FindIndex(match);
        if (pos != -1)
            list.RemoveAt(pos);
    }

    public static bool isInMap(this List<char[]>map, MarkPoint p) => p.Y >= 0 && p.Y < map.Count && p.X >= 0 && p.X < map[0].Length;

    public static int ToInt32(this string value) => int.Parse(value);
    public static long ToInt64(this string value) => long.Parse(value);
    public static BigInteger ToBigInt(this string value) => BigInteger.Parse(value);
    public static IntComplex ToIntComplex(this string value) => IntComplex.Parse(value, null);
}