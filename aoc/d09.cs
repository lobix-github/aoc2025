using NetTopologySuite.Geometries;

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

		var coords = tiles.Select(t => new Coordinate(t.X, t.Y)).ToList();
		coords.Add(coords[0]);
		var factory = new GeometryFactory();
		var poly = factory.CreatePolygon(coords.ToArray());

		max = 0;
		for (int i = 0; i < coords.Count; i++)
		{
			var cur = coords[i];
			for (int j = i+1; j < coords.Count; j++)
			{
				var next = coords[j];
				var area = (long)((Math.Abs(cur.X - next.X) + 1) * (Math.Abs(cur.Y - next.Y) + 1));
				if (area > max)
				{
					var env = new Envelope(cur, next);
					var rect = factory.ToGeometry(env);
					if (rect.Within(poly))
					{
						max = area;
					}
				}
			}
		}

		Console.WriteLine(max);
	}
}
