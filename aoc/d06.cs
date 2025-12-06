
using System.Linq;

class d06 : baseD
{
    public void Run()
    {
        List<List<long>> list = new();
		List<bool> ops = new();

		var lines = File.ReadLines(@"..\..\..\inputs\06.txt").ToList();

		foreach (var line in lines)
		{
			try
			{
				list.Add(line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(x => x.ToInt64()).ToList());
			}
			catch
			{
				ops = line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(x => x.Trim()[0] == '+').ToList();
			}
		}

		List<long> results = new();
		for (int i = 0; i < list[0].Count; i++)
		{
			long result = list[0][i];
			for (int j = 1; j < list.Count; j++)
			{
				if (ops[i]) result += list[j][i]; else result *= list[j][i];
			}
			results.Add(result);
		}

		Console.WriteLine(results.Sum());
	}
}
