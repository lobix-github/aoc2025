class Vertex
{
    public string name;
    public int status;
    public int predecessor;
    public int pathLength;

    public Vertex(IntComplex vertex) : this(vertex.ToString()) { }
	
    public Vertex(string name)
    {
        this.name = name;
    }

    public override string ToString() => this.name;
}

class DijkstraGraph
{
    public readonly int MAX_VERTICES = 150 * 150;

    int n, e;
    int[,] adj;
    Vertex[] vertexList;

    private readonly int TEMPORARY = 1;
    private readonly int PERMANENT = 2;
    private readonly int NIL = -1;
    private readonly int INFINITY = int.MaxValue;
    public HashSet<string> Vertexes = [];

    public DijkstraGraph()
    {
        adj = new int[MAX_VERTICES, MAX_VERTICES];
        vertexList = new Vertex[MAX_VERTICES];
    }

    public bool ContainsVertex(IntComplex vertex) => Vertexes.Contains(vertex.ToString());

	public IEnumerable<(string, int)> FindPaths(IntComplex source)
	{
        foreach (var path in FindPaths(source.ToString())) yield return path;
	}
	
    public IEnumerable<(string, int)> FindPaths(string source)
    {
        int s = GetIndex(source);

        Dijkstra(s);

        for (int v = 0; v < n; v++)
        {
            if (vertexList[v].pathLength != INFINITY)
            {
                yield return (vertexList[v].name, FindPath(s, v).Item1);
            }
        }
    }

    public (int, HashSet<IntComplex>) FindPath(IntComplex source, IntComplex destination)
    {
        var result = FindPath(source.ToString(), destination.ToString());
        return (result.Item1, result.Item2.Select(x => x.ToIntComplex()).ToHashSet());
    }

    public (int, HashSet<string>) FindPath(string source, string destination)
	{
		int s = GetIndex(source);

		Dijkstra(s);

		for (int v = 0; v < n; v++)
		{
			if (vertexList[v].pathLength != INFINITY)
			{
				if (vertexList[v].name.Equals(destination))
				{
					return FindPath(s, v);
				}
			}
		}

		return (INFINITY, null);
	}

	public IEnumerable<IntComplex> GetVisited()
    {
        foreach (var v in GetVisitedString()) yield return v.ToIntComplex();
    }

	public IEnumerable<string> GetVisitedString()
    {
        int i = 0;
        while (vertexList[i] != null) yield return vertexList[i++].name;
    }

	public void InsertVertex(IntComplex vertex) => InsertVertex(vertex.ToString());

	public void InsertVertex(string name) 
    {
        if (!Vertexes.Contains(name)) vertexList[n++] = new Vertex(name);
    }

	public void InsertEdge(IntComplex v1, IntComplex v2, int wt) => InsertEdge(v1.ToString(), v2.ToString(), wt);
	
    public void InsertEdge(string s1, string s2, int wt)
    {
        int u = GetIndex(s1);
        int v = GetIndex(s2);
        if (u == v)
            throw new InvalidOperationException("Not a valid edge");

        adj[u, v] = wt;
        e++;
    }

    private void Dijkstra(int s)
    {
        int v, c;

        for (v = 0; v < n; v++)
        {
            vertexList[v].status = TEMPORARY;
            vertexList[v].pathLength = INFINITY;
            vertexList[v].predecessor = NIL;
        }

        vertexList[s].pathLength = 0;

        while (true)
        {
            c = TempVertexMinPL();

            if (c == NIL)
                return;

            vertexList[c].status = PERMANENT;

            for (v = 0; v < n; v++)
            {
                if (IsAdjacent(c, v) && vertexList[v].status == TEMPORARY)
                    if (vertexList[c].pathLength + adj[c, v] < vertexList[v].pathLength)
                    {
                        vertexList[v].predecessor = c;
                        vertexList[v].pathLength = vertexList[c].pathLength + adj[c, v];
                    }
            }
        }
    }

    private int TempVertexMinPL()
    {
        int min = INFINITY;
        int x = NIL;
        for (int v = 0; v < n; v++)
        {
            if (vertexList[v].status == TEMPORARY && vertexList[v].pathLength < min)
            {
                min = vertexList[v].pathLength;
                x = v;
            }
        }
        return x;
    }

    private (int, HashSet<string>) FindPath(int s, int v)
    {
        int u;
        int[] path = new int[n];
        int sd = 0;
        int count = 0;
		HashSet<string> visited = [];

        while (v != s)
        {
            count++;
            path[count] = v;
            u = vertexList[v].predecessor;
            visited.Add(vertexList[v].name);
			sd += adj[u, v];
            v = u;
        }
        count++;
        path[count] = s;

        return (sd, visited);
    }

    private int GetIndex(string s)
    {
        for (int i = 0; i < n; i++)
            if (s.Equals(vertexList[i].name))
                return i;
        throw new InvalidOperationException("Invalid Vertex");
    }

    private bool IsAdjacent(int u, int v) => adj[u, v] != 0;
}

namespace Dijkstra1
{
    class Program
    {
        //A connected to B
        //B connected to A, C , D
        //C connected to B, D
        //D connected to B, C , E
        //E connected to D.
        static List<List<String>> input1 = new List<List<string>>{
               new List<String>() {"A","0","1","0","0","0"},
               new List<String>() {"B","1","0","1","1","0"},
               new List<String>() {"C","0","1","0","1","0"},
               new List<String>() {"D","0","1","1","0","1"},
               new List<String>() {"E","0","0","0","1","0"}
            };
        //A  |   0 1 2 2 3  |
        //B  |   1      0      1      1      2       |
        //C  |   2      1      0      1      2       | 
        //D  |   2      1      1      0      1       |
        //E  |   3      2      2      1      0       |
        static List<List<String>> input2 = new List<List<string>>{
               new List<String>() {"A","0","1","2","2","3"},
               new List<String>() {"B","1","0","1","1","2"},
               new List<String>() {"C","2","1","0","1","2"},
               new List<String>() {"D","2","1","1","0","1"},
               new List<String>() {"E","3","2","2","1","0"}
            };
        static public void Dijkstra()
        {
            CGraph cGraph;
            cGraph = new CGraph(input1);
            Console.WriteLine("-------------Input 1 -------------");
            cGraph.PrintGraph();
            cGraph = new CGraph(input2);
            Console.WriteLine("-------------Input 2 -------------");
            cGraph.PrintGraph();
        }
        class CGraph
        {
            List<Node> graph = new List<Node>();
            public CGraph(List<List<String>> input)
            {
                foreach (List<string> inputRow in input)
                {
                    Node newNode = new Node();
                    newNode.name = inputRow[0];
                    newNode.distanceDict = new Dictionary<string, Path>();
                    newNode.visited = false;
                    newNode.neighbors = new List<Neighbor>();
                    //for (int index = 1; index < inputRow.Count; index++)
                    //{
                    //    //skip diagnol values so you don't count a nodes distance to itself.
                    //    //node count start at zero
                    //    // index you have to skip the node name
                    //    //so you have to subtract one from the index
                    //    if ((index - 1) != nodeCount)
                    //    {
                    //        string nodeName = input[index - 1][0];
                    //        int distance = int.Parse(inputRow[index]);
                    //        newNode.distanceDict.Add(nodeName, new List<string>() { nodeName });
                    //    } 
                    //}
                    graph.Add(newNode);
                }
                //initialize neighbors using predefined dictionary
                for (int nodeCount = 0; nodeCount < graph.Count; nodeCount++)
                {
                    for (int neighborCount = 0; neighborCount < graph.Count; neighborCount++)
                    {
                        //add one to neighbor count to skip Node name in index one
                        if (input[nodeCount][neighborCount + 1] != "0")
                        {
                            Neighbor newNeightbor = new Neighbor();
                            newNeightbor.node = graph[neighborCount];
                            newNeightbor.distance = int.Parse(input[nodeCount][neighborCount + 1]);
                            graph[nodeCount].neighbors.Add(newNeightbor);
                            Path path = new Path();
                            path.nodeNames = new List<string>() { input[neighborCount][0] };
                            //add one to neighbor count to skip Node name in index one
                            path.totalDistance = int.Parse(input[nodeCount][neighborCount + 1]);
                            graph[nodeCount].distanceDict.Add(input[neighborCount][0], path);
                        }
                    }
                }

                foreach (Node node in graph)
                {
                    foreach (Node nodex in graph)
                    {
                        node.visited = false;
                    }
                    TransverNode(node);
                }
            }
            public class Neighbor
            {
                public Node node { get; set; }
                public int distance { get; set; }
            }
            public class Path
            {
                public List<string> nodeNames { get; set; }
                public int totalDistance { get; set; }
            }
            public class Node
            {
                public string name { get; set; }
                public Dictionary<string, Path> distanceDict { get; set; }
                public Boolean visited { get; set; }
                public List<Neighbor> neighbors { get; set; }
            }
            static void TransverNode(Node node)
            {
                if (!node.visited)
                {
                    node.visited = true;
                    foreach (Neighbor neighbor in node.neighbors)
                    {
                        TransverNode(neighbor.node);
                        string neighborName = neighbor.node.name;
                        int neighborDistance = neighbor.distance;
                        //compair neighbors dictionary with current dictionary
                        //update current dictionary as required
                        foreach (string key in neighbor.node.distanceDict.Keys)
                        {
                            if (key != node.name)
                            {
                                int neighborKeyDistance = neighbor.node.distanceDict[key].totalDistance;
                                if (node.distanceDict.ContainsKey(key))
                                {
                                    int currentDistance = node.distanceDict[key].totalDistance;
                                    if (neighborKeyDistance + neighborDistance < currentDistance)
                                    {
                                        List<string> nodeList = new List<string>();
                                        nodeList.AddRange(neighbor.node.distanceDict[key].nodeNames);
                                        nodeList.Insert(0, neighbor.node.name);
                                        node.distanceDict[key].nodeNames = nodeList;
                                        node.distanceDict[key].totalDistance = neighborKeyDistance + neighborDistance;
                                    }
                                }
                                else
                                {
                                    List<string> nodeList = new List<string>();
                                    nodeList.AddRange(neighbor.node.distanceDict[key].nodeNames);
                                    nodeList.Insert(0, neighbor.node.name);
                                    Path path = new Path();
                                    path.nodeNames = nodeList;
                                    path.totalDistance = neighbor.distance + neighborKeyDistance;
                                    node.distanceDict.Add(key, path);
                                }
                            }
                        }
                    }
                }
            }
            public void PrintGraph()
            {
                foreach (Node node in graph)
                {
                    Console.WriteLine("Node : {0}", node.name);
                    foreach (string key in node.distanceDict.Keys.OrderBy(x => x))
                    {
                        Console.WriteLine(" Distance to node {0} = {1}, Path : {2}", key, node.distanceDict[key].totalDistance, string.Join(",", node.distanceDict[key].nodeNames.ToArray()));
                    }
                }
            }
        }
    }
}