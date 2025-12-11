using System.Collections;
using Utilities.Graph;

class d11
{
	public void Run()
	{
		var paths = new Dictionary<string, HashSet<string>>();
		
		var lines = File.ReadLines(@"..\..\..\inputs\11.txt").ToList();
		foreach (var line in lines)
		{
			var parts = line.Split(':');
			paths[parts[0]] = parts[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToHashSet();
		}

		var sum = 0L;
		var q = new Queue<string>();
		q.Enqueue("you");
		while (q.Count > 0)
		{
			var id = q.Dequeue();
			foreach (var item in paths[id])
			{
				if (item == "out") sum++; else q.Enqueue(item);
			}
		}

		Console.WriteLine(sum);

		var pcounter = new PathCounter<string>(paths);
		var p1 = new PathCounter<string>(paths).CountPaths("svr", "fft");
		var p2 = new PathCounter<string>(paths).CountPaths("fft", "dac");
		var p3 = new PathCounter<string>(paths).CountPaths("dac", "out");
		var p4 = new PathCounter<string>(paths).CountPaths("svr", "dac");
		var p5 = new PathCounter<string>(paths).CountPaths("dac", "fft");
		var p6 = new PathCounter<string>(paths).CountPaths("fft", "out");

		Console.WriteLine(p1 * p2 * p3 + p4 * p5 * p6);
	}
}
