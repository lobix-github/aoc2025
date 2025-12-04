class d04 : baseD
{
    public void Run()
    {
        HashSet<IntComplex> map = new();

		var lines = File.ReadLines(@"..\..\..\inputs\04.txt").ToList();
		for (int y = 0; y < lines.Count; y++)
		{
			for (int x = 0; x < lines[y].Length; x++)
			{
				var c = lines[y][x];
				if (c == '@')
				{
					map.Add(new IntComplex(x, y));
				}
			}
		}

		var count = map.Sum(p => sumNeighbours(p) < 4 ? 1 : 0);
		Console.WriteLine(count);

		count = 0;
		var removed = 0;
		while((removed = map.RemoveWhere(p => sumNeighbours(p) < 4)) > 0)
		{
			count += removed;
		}

		Console.WriteLine(count);
	
		int sumNeighbours(IntComplex p) => ComplexDirs.Sum(dir => map.Contains(p + dir) ? 1 : 0);
	}
}
