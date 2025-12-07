class d07
{
	public void Run()
	{
		var beams = new HashSet<int>();

		var lines = File.ReadLines(@"..\..\..\inputs\07.txt").ToList();
		beams.Add(lines[0].IndexOf('S'));
		var cols = Enumerable.Repeat(0L, lines[0].Length).ToList();
		cols[lines[0].IndexOf('S')]++;

		var count = 0;
		foreach (var line in lines.Skip(1))
		{
			for (int i = 0; i < line.Length; i++)
			{
				if (line[i] == '^' && beams.Contains(i))
				{
					beams.Add(i - 1);
					cols[i - 1] += cols[i];
					beams.Add(i + 1);
					cols[i + 1] += cols[i];
					beams.Remove(i);
					cols[i] = 0;
					count++;
				}
			}
		}
		Console.WriteLine(count);
		Console.WriteLine(cols.Sum());
	}
}
