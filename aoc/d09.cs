class d09
{
	public void Run()
	{
		var tiles = File.ReadLines(@"..\..\..\inputs\09.txt").Select(l => new DPoint(l.Split(',')[0].ToInt32(), l.Split(',')[1].ToInt32())).ToList();
		
		long max = 0;
		for (int i = 0; i < tiles.Count; i++)
		{
			var cur = tiles[i];
			for (int j = i+1; j < tiles.Count; j++)
			{
				var next = tiles[j];
				var area = (long)(Math.Abs(cur.X - next.X) + 1) * (Math.Abs(cur.Y - next.Y) + 1);
				if (area > max) max = area;
			}
		}

		Console.WriteLine(max);
	}
}
