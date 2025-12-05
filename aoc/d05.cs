using System.Linq;

class d05 : baseD
{
    public void Run()
    {
        List<(long, long)> fresh = new();
        List<(long, long)> ranges = new();
		HashSet<long> available = new();

		var lines = File.ReadLines(@"..\..\..\inputs\05.txt").ToList();

		bool isAvailable = false;
		foreach (var line in lines)
		{
			if (line.Length == 0 && !isAvailable)
			{
				isAvailable = true;
				continue;
			}

			if (!isAvailable)
			{
				fresh.Add((line.Split('-')[0].ToInt64(), line.Split('-')[1].ToInt64()));
				continue;
			}

			available.Add(line.ToInt64());
		}

		fresh = fresh.OrderBy(x => x.Item1).ToList();
		var start = fresh.First().Item1;
		var end = fresh.First().Item2;
		for (int i = 1; i < fresh.Count; i++)
		{
			var f = fresh[i];
			if (f.Item1 > end)
			{
				ranges.Add((start, end));
				start = f.Item1;
				end = f.Item2;
				continue;
			}

			if (f.Item2 > end)
			{
				end = f.Item2;
			}
		}
		ranges.Add((start, end));

		var count = available.Sum(a => ranges.Where(f => a >= f.Item1 && a <= f.Item2).Count());
		var count2 = ranges.Sum(f => f.Item2 - f.Item1 + 1);
		Console.WriteLine(count);
		Console.WriteLine(count2);
	}
}
