class d03
{
    public void Run()
    {
        List<int> list = new();

        var sum = 0;
		var lines = File.ReadLines(@"..\..\..\inputs\03.txt").ToList();
		foreach (var line in lines)
		{
			list = line.Select(x => x.ToString().ToInt32()).ToList();
            var max1 = list.Max();
			var pos1 = list.IndexOf(max1);
			var isLast = false;
			if (pos1 == list.Count - 1)
			{
				list.RemoveAt(pos1);
				isLast = true;
			}
			else list = list.Skip(pos1 + 1).ToList();
            var max2 = list.Max();

			var r = isLast ? max2.ToString() + max1.ToString() : max1.ToString() + max2.ToString();

			sum += (r).ToInt32();
		}

        Console.WriteLine(sum);
	}
}
