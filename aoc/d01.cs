class d01
{
    public void Run()
    {
        List<string> list = new();

        var lines = File.ReadLines(@"..\..\..\inputs\01.txt").ToList();
        foreach (var line in lines)
        {
            list.Add(line);
        }

        int counter = 0;
        _ = list.Aggregate(50, (res, x) =>
        {
            res += int.Parse(x[1..]) * (x[0] == 'L' ? -1 : 1);
            if (res % 100 == 0) counter++;
			return res;
        });
        Console.WriteLine(counter);

		counter = 0;
		_ = list.Aggregate(50, (res, x) =>
		{
            var sign = x[0] == 'L' ? -1 : 1;
			var cur = int.Parse(x[1..]) * sign;
            for (int i = 0; i < Math.Abs(cur); i++)
            {
                res += sign;
                if (res % 100 == 0) counter++;
			}
			return res;
		});
		Console.WriteLine(counter);
	}
}
