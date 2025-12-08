class d08 : baseD
{
	public void Run()
	{
		var points = new List<Point3D>();
		var circuits = new List<HashSet<Point3D>>();
		var dists = new List<(double, HashSet<Point3D>)>();

		var lines = File.ReadLines(@"..\..\..\inputs\08.txt").ToList();

		foreach (var line in lines)
		{
			var parts = line.Split(',');
			points.Add(new Point3D(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])));
		}

		for (int i = 0; i < points.Count - 1; i++)
		{
			for (int j = i+1; j < points.Count; j++)
			{
				var dist = 
					Math.Pow(points[i].x - points[j].x, 2) +
					Math.Pow(points[i].y - points[j].y, 2) +
					Math.Pow(points[i].z - points[j].z, 2);
				dists.Add((dist, new HashSet<Point3D> { points[i], points[j] }));
			}
		}

		dists.Sort((a, b) => a.Item1.CompareTo(b.Item1));
		var idx = 0;
		var count = 0;
		const int COUNT = 1000;
		//while (count++ < COUNT)
		while (true)
		{
			var r = dists[idx++];
			var cres = circuits.Where(c => r.Item2.Any(p => c.Contains(p)));
			if (cres.Any())
			{
				if (cres.Count() == 2)
				{
					var merge = cres.First().Union(cres.Last()).ToHashSet();
					circuits.RemoveAll(cres.Contains);
					circuits.Add(merge);
				}
				else
				{ 
					cres.First().Add(r.Item2.First());
					cres.First().Add(r.Item2.Last());
				}
			}
			else
			{
				circuits.Add(r.Item2);
			}

			if (circuits.First().Count == points.Count)
			{
				Console.WriteLine((long)r.Item2.First().x * r.Item2.Last().x);
			}
		}

		circuits = circuits.OrderByDescending(c => c.Count).ToList();
		Console.WriteLine(circuits[0].Count * circuits[1].Count * circuits[2].Count);
		//Console.WriteLine(circuits.OrderByDescending(c => c.Count).Take(3).Aggregate(1, (acc, c) => acc * c.Count));
	}
}
