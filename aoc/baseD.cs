using System.Collections;
using System.Drawing;
using System.Numerics;

abstract class baseD
{
    protected static IntComplex Up = -IntComplex.ImaginaryOne;
    protected static IntComplex Down = IntComplex.ImaginaryOne;
    protected static IntComplex Left = -1;
	protected static IntComplex Right = 1;

    protected int ToInt(string val) => Convert.ToInt32(val);
    protected int ToIntFromHex(string val) => Convert.ToInt32(val, 16);
    protected long ToLong(string val) => Convert.ToInt64(val);
    protected BigInteger ToBigInt(string val) => BigInteger.Parse(val);

    protected List<string> rotate(List<string> list)
    {
        var result = new List<string>(list[0].Length);
        result.AddRange(Enumerable.Repeat(string.Empty, list[0].Length));
        for (int y = 0; y < list.Count; y++)
        {
            var line = list[y];
            for (int x = 0; x < line.Length; x++)
            {
                result[x] += line[x];
            }
        }

        return result;
    }

    public static void loopCycle<T>(int count, Func<int, T> jobReturningCacheKey)
    {
        var cache = new Dictionary<T, int>();
        var cycle = 1;
        while (cycle <= count)
        {
            var id = jobReturningCacheKey(cycle);

            if (cache.TryGetValue(id, out var cached))
            {
                var remaining = count - cycle;
                var loop = cycle - cached;

                var loopRemaining = remaining % loop;
                cycle = count - loopRemaining;
            }

            cache[id] = cycle++;
        }
    }

    protected static long calculateInners(HashSet<Point> plain)
    {
        var minX = plain.Select(x => x.X).Min();
        var minY = plain.Select(x => x.Y).Min();
        var maxX = plain.Select(x => x.X).Max();
        var maxY = plain.Select(x => x.Y).Max();

        var gr = plain.GroupBy(x => x.Y).OrderBy(g => g.Key).ToArray();
        long sum = 0;
        if (maxY - minY + 1 != gr.Count()) throw new Exception("sie zesralo");
        for (var i = 0; i < gr.Count(); i++)
        {
            var g = gr[i];
            var inner = true;
            var xs = g.OrderBy(p => p.X).ToArray();
            if (xs.Length > 1)
            {
                int? firstXInSeries = null;
                for (var idx = 1; idx < xs.Length; idx++)
                {
                    var prev = xs[idx - 1].X;
                    var cur = xs[idx].X;

                    if (prev == cur - 1)
                    {
                        if (firstXInSeries == null) firstXInSeries = prev;
                        continue;
                    }

                    if (firstXInSeries != null)
                    {
                        var differentDir = plain.Contains(new Point(firstXInSeries.Value, g.Key + 1)) ^ plain.Contains(new Point(prev, g.Key + 1));
                        if (!differentDir) inner = !inner;
                        firstXInSeries = null;
                    }

                    if (inner)
                    {
                        sum += cur - prev - 1;
                    }
                    inner = !inner;
                }
            }
        }
        return sum;
    }

    protected static IEnumerable<DPoint> GetNumberOfCorners(IEnumerable<MarkPoint> region)
    {
        // number of edges is equal to number of corners
        foreach (var item in GetNumberOfEdges(region))
        {
            yield return item;
        }
    }

    protected static IEnumerable<DPoint> GetNumberOfEdges(IEnumerable<MarkPoint> region)
    {
        foreach (var p in region.ToList())
		{
            var lu = new MarkPoint(p.X - 1, p.Y - 1, '.');
            var u = new MarkPoint(p.X, p.Y - 1, '.');
            var ru = new MarkPoint(p.X + 1, p.Y - 1, '.');
            var r = new MarkPoint(p.X + 1, p.Y, '.');
            var rb = new MarkPoint(p.X + 1, p.Y + 1, '.');
            var b = new MarkPoint(p.X, p.Y + 1, '.');
            var lb = new MarkPoint(p.X - 1, p.Y + 1, '.');
            var l = new MarkPoint(p.X - 1, p.Y, '.');

            if (!region.Contains(l) && !region.Contains(u))
            {
                yield return new DPoint(p.X, p.Y);
            }
            if (region.Contains(l) && region.Contains(u) && !region.Contains(lu))
            {
            yield return new DPoint(p.X, p.Y);
            }

            if (!region.Contains(u) && !region.Contains(r))
            {
            yield return new DPoint(p.X + 1, p.Y);
            }
            if (region.Contains(u) && region.Contains(r) && !region.Contains(ru))
            {
                yield return new DPoint(p.X + 1, p.Y);
            }

            if (!region.Contains(r) && !region.Contains(b))
            {
                yield return new DPoint(p.X + 1, p.Y + 1);
            }
            if (region.Contains(r) && region.Contains(b) && !region.Contains(rb))
            {
                yield return new DPoint(p.X + 1, p.Y + 1);
            }

            if (!region.Contains(b) && !region.Contains(l))
            {
                yield return new DPoint(p.X, p.Y + 1);
            }
            if (region.Contains(b) && region.Contains(l) && !region.Contains(lb))
            {
                yield return new DPoint(p.X, p.Y + 1);
            }
        }
    }

    protected static int TotalLengthOgEdges(List<char[]> map, IEnumerable<MarkPoint> region)
    {
        var perimiteres = 0;
        var minY = region.Select(x => x.Y).Min();
        var maxY = region.Select(x => x.Y).Max();
        for (int y = minY; y <= maxY; y++)
        {
            foreach (var p in region.Where(p => p.Y == y).OrderBy(p => p.X).Select(p => p))
            {
                var xs = region.Where(x => x.X == p.X);
                if (!sameX(p))
                {
                    perimiteres += 2;
                }
            }
        }

        var minX = region.Select(x => x.X).Min();
        var maxX = region.Select(x => x.X).Max();
        for (int x = minX; x <= maxX; x++)
        {
            foreach (var p in region.Where(p => p.X == x).OrderBy(p => p.Y).Select(p => p))
            {
                var ys = region.Where(x => x.Y == p.Y);
                if (!sameY(p))
                {
                    perimiteres += 2;
                }
            }
        }

        return perimiteres;

        bool sameX(MarkPoint p) => p.X - 1 >= 0 && map[p.Y][p.X - 1] == p.marker;
        bool sameY(MarkPoint p) => p.Y - 1 >= 0 && map[p.Y - 1][p.X] == p.marker;
    }

    static protected IEnumerable<MarkPoint> ParseRegions(List<char[]> map)
    {
		List<MarkPoint> regions = new();
        
        for (int y = 0; y < map.Count; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                var point = new MarkPoint(x, y, map[y][x]);
                if (!regions.SelectMany(x => x.visited).Contains(point))
                {
                    regions.Add(point);
                    crowl(map, point);
                }
            }
        }

        return regions;
    }

    private static void crowl(List<char[]> map, MarkPoint point)
    {
        var q = new Queue<MarkPoint>();
        q.Enqueue(point);
        while (q.Any())
        {
            var cur = q.Dequeue();

            if (cur.visited.Contains(cur))
            {
                continue;
            }
            cur.visited.Add(cur);

            TryQueue(cur.GetNext(cur.X - 1, cur.Y));
            TryQueue(cur.GetNext(cur.X + 1, cur.Y));
            TryQueue(cur.GetNext(cur.X, cur.Y - 1));
            TryQueue(cur.GetNext(cur.X, cur.Y + 1));

            void TryQueue(MarkPoint next)
            {
                if (map.isInMap(next) && map[next.Y][next.X] == cur.marker)
                {
                    q.Enqueue(next);
                }
            }
        }
    }

    protected string Reverse(string s)
	{
		char[] charArray = s.ToCharArray();
		Array.Reverse(charArray);
		return new string(charArray);
	}

	protected int[] AllIndexesOf(string str, string substr, bool ignoreCase = false)
	{
		int index = 0;
		var indexes = new List<int>();
		if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(substr)) return indexes.ToArray();

		while ((index = str.IndexOf(substr, index, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) != -1)
		{
			indexes.Add(index++);
		}

		return indexes.ToArray();
	}

    protected void SurroundMapWithEdges(HashSet<IntComplex> map)
    {
		var minX = map.Select(x => x.Real).Min();
		var maxX = map.Select(x => x.Real).Max();
		var minY = map.Select(x => x.Imaginary).Min();
		var maxY = map.Select(x => x.Imaginary).Max();

		Enumerable.Range(minY - 1, maxY - minY + 3).ToList().ForEach(x => map.Add(new IntComplex(minX - 1, x)));
        Enumerable.Range(minY - 1, maxY - minY + 3).ToList().ForEach(x => map.Add(new IntComplex(maxX + 1, x)));
        Enumerable.Range(minX - 1, maxX - minX + 3).ToList().ForEach(x => map.Add(new IntComplex(x, minY - 1)));
        Enumerable.Range(minX - 1, maxX - minX + 3).ToList().ForEach(x => map.Add(new IntComplex(x, maxY + 1)));
    }

	protected static IEnumerable<IList> Permutate(IList sequence, int count)
	{
		if (count == 1) yield return sequence;
		else
		{
			for (int i = 0; i < count; i++)
			{
				foreach (var perm in Permutate(sequence, count - 1))
					yield return perm;
				RotateRight(sequence, count);
			}
		}
	}

	protected static IEnumerable<List<T>> GetCombinations<T>(IList<T> items, int k)
	{
		if (k == 0)
		{
			yield return new List<T>();
			yield break;
		}

		for (int i = 0; i <= items.Count - k; i++)
		{
			foreach (var combo in GetCombinations(items.Skip(i + 1).ToList(), k - 1))
			{
				var result = new List<T> { items[i] };
				result.AddRange(combo);
				yield return result;
			}
		}
	}

	protected static void RotateRight(IList sequence, int count)
	{
		object tmp = sequence[count - 1];
		sequence.RemoveAt(count - 1);
		sequence.Insert(0, tmp);
	}


	public IEnumerable<IntComplex> ComplexDirs
	{
		get
		{
			yield return new IntComplex(0, -1); // N
			yield return new IntComplex(1, -1); // NE
			yield return new IntComplex(1, 0);  // E
			yield return new IntComplex(1, 1);  // SE
			yield return new IntComplex(0, 1);  // S
            yield return new IntComplex(-1, 1); // SW
			yield return new IntComplex(-1, 0); // W
            yield return new IntComplex(-1, -1); // NW
		}
	}
}

public class DCache<TKey, TValue>
{
    private readonly Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
    public TValue Get(TKey key, Func<TValue> getValue)
    {
        if (dict.TryGetValue(key, out var value))
        {
            return value;
        }
        
        value = getValue();
        dict[key] = value;
        return value;
    }
}

public class HashedHashSet<T> : HashSet<T>
{
    public HashedHashSet(IEnumerable<T> collection) : base(collection) { }

    public HashedHashSet() { }

    public override int GetHashCode()
    {
        var hash = 0;
        foreach (var item in this)
        {
            hash ^= item.GetHashCode();
        }
        return hash;
    }

}

public class CopyableHashedHashSet<T> : HashedHashSet<T> where T : ICopy<T>
{
    public CopyableHashedHashSet(IEnumerable<T> collection) : base(collection) { }

    public CopyableHashedHashSet() { }

    public CopyableHashedHashSet<T> Copy() => new CopyableHashedHashSet<T>(this.Select(x => x.Copy()));
}

public enum Dirs
{
    N = 0,
    E = 1,
    S = 2,
    W = 3
}

public class CPoint
{
    public readonly int X;
    public readonly int Y;

    public CPoint(int x, int y)
    {
        X = x;
        Y = y;
    }

    public virtual string id => $"x: {X}, y: {Y}";

    public override int GetHashCode() => id.GetHashCode();

    public override bool Equals(object obj) => obj is CPoint other && other.GetHashCode() == this.GetHashCode();

    public override string ToString() => $"{id}";
}

public record DPoint(int X, int Y) : ICopy<DPoint>
{
    public virtual string id => $"x: {X}, y: {Y}";

    public DPoint Copy() => new DPoint(X, Y);
}

public record class Point3D(int x, int y, int z) : ICopy<Point3D>
{
    public Point3D Copy() => new Point3D(x, y, z);
}

public record Vector3D(Point3D start, Point3D end) : ICopy<Vector3D>
{
    public bool IsIntersectingXY(Vector3D v) => XS.Intersect(v.XS).Any() && YS.Intersect(v.YS).Any();

    private IEnumerable<int> XS => Enumerable.Range(start.x, end.x - start.x + 1);
    private IEnumerable<int> YS => Enumerable.Range(start.y, end.y - start.y + 1);

    public Vector3D SetZ(int z) => new Vector3D(new Point3D(start.x, start.y, z), new Point3D(end.x, end.y, end.z - start.z + z));

    public Vector3D Copy() => new Vector3D(start.Copy(), end.Copy());
}

public interface ICopy<T>
{
    T Copy();
}

public class MarkPoint : CPoint
{
    public HashSet<MarkPoint> visited = new();
    public char marker;

    public MarkPoint(int X, int Y, char marker) : base(X, Y)
    {
        this.marker = marker;
    }

    public List<DPoint> edgeCorners = new();

    public MarkPoint GetNext(int x, int y) => new MarkPoint(x, y, marker) { visited = visited };

    public override string ToString() => $"{id}, plant: {marker}, sum: {visited.Count}";
};
