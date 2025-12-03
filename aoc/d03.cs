class d03
{
    public void Run()
    {
        List<int> list = new();

        long sum = 0;
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

		list = new();
		sum = 0;
		foreach (var _line in lines)
		{
			var res = new List<char>();
			var line = _line;
			for (var i = 1; i <= 12; i++)
			{
				var s = line.Substring(0, line.Length - (12 - res.Count - 1));
				var maxx = s.Max();
				res.Add(maxx);
				line = line.Substring(s.IndexOf(maxx) + 1, line.Length - s.IndexOf(maxx) - 1);
			}
			sum += new string(res.ToArray()).ToInt64();
		}


		Console.WriteLine(sum);
	}
}
