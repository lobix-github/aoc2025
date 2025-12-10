using System.Collections;

class d10 : baseD
{
	public void Run()
	{
		var lines = File.ReadLines(@"..\..\..\inputs\10.txt").ToList();
		var machines = new List<Machine>();
		foreach (var line in lines)
		{
			var parts = line.Split(' ');
			BitArray ba = new(parts[0][1..^1].Select(p => p == '#').ToArray());
			var buttons = parts[1..^1].Select(p =>
			{
				var idxs = p[1..^1].Split(',').Select(int.Parse).ToArray();
				return new BitArray(Enumerable.Range(0, idxs.Last() + 1).Select(i => idxs.Contains(i)).ToArray());
			}).ToList();
			machines.Add(new Machine(ba, buttons));
		}

		foreach (var machine in machines)
		{
			var max = Math.Max(machine.result.Length, machine.buttons.Select(b => b.Length).Max());
			machine.result.Length = max;
			foreach (var button in machine.buttons)
			{
				button.Length = max;
			}
		}

		var results = new List<int>();
		foreach (var machine in machines)
		{
			bool found = false;
			for (int i = 1; i <= machine.buttons.Count; i++)
			{
				var combs = GetCombinations(machine.buttons, i);
				foreach (var comb in combs)
				{
					BitArray test = new(machine.result);
					foreach (var c in comb)
					{
						test.Xor(c);
					}
					if (!test.HasAnySet())
					{
						results.Add(i);
						found = true;
						break;
					}
				}
				if (found) break;
			}
		}

		Console.WriteLine(results.Sum());
	}

	public record Machine(BitArray result, List<BitArray> buttons);
}
