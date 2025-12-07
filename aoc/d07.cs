
class d07
{
	public void Run()
	{
		var beams = new HashSet<int>();

		var lines = File.ReadLines(@"..\..\..\inputs\07.txt").ToList();
		beams.Add(lines[0].IndexOf('S'));

		var count = 0;
		foreach (var line in lines.Skip(1))
		{
			for (int i = 0; i < line.Length; i++)
			{
				if (line[i] == '^' && beams.Contains(i))
				{
					beams.Add(i - 1);
					beams.Add(i + 1);
					beams.Remove(i);
					count++;
				}
			}

		}

		Console.WriteLine(count);
	}
}
