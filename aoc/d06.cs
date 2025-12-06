
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

		for (int i = 0; i < lines.Count; i++)
		{
			lines[i] = lines[i].Replace(' ', '0');
		}
		lines = rotate(lines);
		for (int i = 0; i < lines.Count; i++)
		{
			lines[i] = lines[i].TrimEnd('+').TrimEnd('*');
			if (lines[i].ToInt64() == 0) lines[i] = " ";
			lines[i] = lines[i].TrimEnd('0');
		}
		var sum = 0L;
		var opsIdx = 0;
		bool first = true;
		results.Clear();
		foreach (var line in lines)
		{
			if (line == " ")
			{
				results.Add(sum);
				opsIdx++;
				first = true;
			}
			else if (first)
			{
				sum = line.ToInt64();
				first = false;
			}
			else if (ops[opsIdx]) sum += line.ToInt64(); else sum *= line.ToInt64();
		}
		results.Add(sum);
		Console.WriteLine(results.Sum());
	}
}
