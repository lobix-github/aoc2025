using System.Collections;

class d11
{
	public void Run()
	{
		var paths = new Dictionary<string, List<string>>();
		
		var lines = File.ReadLines(@"..\..\..\inputs\11.txt").ToList();
		foreach (var line in lines)
		{
			var parts = line.Split(':');
			paths[parts[0]] = parts[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
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
	}
}
