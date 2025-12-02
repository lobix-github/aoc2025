class d02
{
    public void Run()
    {
        List<(long, long)> list = new();

        list = File.ReadAllText(@"..\..\..\inputs\02.txt").Split(',').Select(x => (x.Split('-')[0].ToInt64(), x.Split('-')[1].ToInt64())).ToList();

		long counter1 = 0;
		long counter2 = 0;
		foreach (var pair in list)
        {
            (long min, long max) = pair;
            for (long i = min; i <= max; i++)
            {
                var s = i.ToString();
                //if (s.Length % 2 != 0) continue;

                //string left = s.Substring(0, s.Length / 2);
                //string right = s.Substring(s.Length / 2);

                //if (left == right) counter1 += i;

                for (int len = 1; len <= s.Length / 2; len++)
                {
                    if (s.Length % len != 0) continue;

                    var first = s.Substring(0, len);
                    if (s.Chunk(len).All(x => new string(x) == first))
                    {
                        counter2 += i;
                        break;
                    }
                }
            }
		}

        Console.WriteLine(counter1);
        Console.WriteLine(counter2);
	}
}
