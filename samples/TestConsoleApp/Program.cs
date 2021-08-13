using System;
using QuikGraph;
using QuikGraph.Serialization;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var TestGraph = new AdjacencyGraph<int, Edge<int>>();
            TestGraph.AddVertex(1);
            TestGraph.AddVertex(2);
            TestGraph.AddVertex(3);
            TestGraph.AddEdge(new Edge<int>(1, 2));
            TestGraph.AddEdge(new Edge<int>(3, 1));
            TestGraph.AddEdge(new Edge<int>(3, 2));

            try
            {
                TestGraph.SerializeToGraphML<int, Edge<int>, AdjacencyGraph<int, Edge<int>>>("./graph.graphml");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
