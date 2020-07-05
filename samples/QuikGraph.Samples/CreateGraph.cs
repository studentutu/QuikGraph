using System;
using System.Linq;
using NUnit.Framework;
using static QuikGraph.Samples.Helpers.GraphHelpers;

namespace QuikGraph.Samples
{
    /// <summary>
    /// Samples to create graphs.
    /// </summary>
    [TestFixture]
    public class CreateGraph
    {
        [Test]
        public void AdjacencyGraph()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            // Vertices
            graph.AddVertex(1);
            graph.AddVertexRange(new[] { 2, 3 });

            // Edges
            graph.AddEdge(new Edge<int>(1, 2));
            graph.AddEdgeRange(new[]
            {
                new Edge<int>(1, 1),
                new Edge<int>(1, 3)
            });

            // Both (if needed)
            graph.AddVerticesAndEdge(new Edge<int>(4, 5));
            graph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<int>(5, 6),
                new Edge<int>(6, 7)
            });

            ShowGraph(graph);
        }

        [Test]
        public void BidirectionalGraph()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            // Vertices
            graph.AddVertex(1);
            graph.AddVertexRange(new[] { 2, 3 });

            // Edges
            graph.AddEdge(new Edge<int>(1, 2));
            graph.AddEdgeRange(new[]
            {
                new Edge<int>(1, 1),
                new Edge<int>(1, 3)
            });

            // Both (if needed)
            graph.AddVerticesAndEdge(new Edge<int>(4, 5));
            graph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<int>(5, 6),
                new Edge<int>(6, 7)
            });

            ShowGraph(graph);
        }

        [Test]
        public void UndirectedGraph()
        {
            var graph = new UndirectedGraph<int, Edge<int>>();
            // Vertices
            graph.AddVertex(1);
            graph.AddVertexRange(new[] { 2, 3 });

            // Edges
            graph.AddEdge(new Edge<int>(1, 2));
            graph.AddEdgeRange(new[]
            {
                new Edge<int>(1, 1),
                new Edge<int>(1, 3)
            });

            // Both (if needed)
            graph.AddVerticesAndEdge(new Edge<int>(4, 5));
            graph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<int>(5, 6),
                new Edge<int>(6, 7)
            });

            ShowGraph(graph);
        }

        [Test]
        public void EdgeArrayToAdjacencyGraph()
        {
            Edge<int>[] edges =
            {
                new Edge<int>(1, 2),
                new Edge<int>(0, 1)
            };

            var graph = edges.ToAdjacencyGraph<int, Edge<int>>();
            ShowGraph(graph);
        }

        [Test]
        public void CommonGraphs()
        {
            // -----------------------------------------------------------------------------------------------------
            var adjacencyGraph = new AdjacencyGraph<int, Edge<int>>();
            adjacencyGraph.AddVerticesAndEdge(new Edge<int>(1, 2));
            ShowGraph(adjacencyGraph);

            // -----------------------------------------------------------------------------------------------------
            var arrayAdjacencyGraph = new ArrayAdjacencyGraph<int, Edge<int>>(adjacencyGraph);
            ShowGraph(arrayAdjacencyGraph);

            // -----------------------------------------------------------------------------------------------------
            var bidirectionalGraph = new BidirectionalGraph<int, Edge<int>>();
            bidirectionalGraph.AddVerticesAndEdge(new Edge<int>(1, 2));
            ShowGraph(bidirectionalGraph);

            // -----------------------------------------------------------------------------------------------------
            var arrayBidirectionalGraph = new ArrayBidirectionalGraph<int, Edge<int>>(bidirectionalGraph);
            ShowGraph(arrayBidirectionalGraph);

            // -----------------------------------------------------------------------------------------------------
            var bidirectionalAdapterGraph = new BidirectionalAdapterGraph<int, Edge<int>>(adjacencyGraph);
            ShowGraph(bidirectionalAdapterGraph);

            // -----------------------------------------------------------------------------------------------------
            var matrixGraph = new BidirectionalMatrixGraph<Edge<int>>(10);
            matrixGraph.AddEdge(new Edge<int>(1, 2));
            ShowGraph(matrixGraph);

            // -----------------------------------------------------------------------------------------------------
            var reversedBidirectionalGraph = new ReversedBidirectionalGraph<int, Edge<int>>(bidirectionalGraph);
            ShowGraph(reversedBidirectionalGraph);

            // -----------------------------------------------------------------------------------------------------
            var undirectedBidirectionalGraph = new UndirectedBidirectionalGraph<int, Edge<int>>(bidirectionalGraph);
            ShowGraph(undirectedBidirectionalGraph);

            // -----------------------------------------------------------------------------------------------------
            var undirectedGraph = new UndirectedGraph<int, Edge<int>>();
            undirectedGraph.AddVerticesAndEdge(new Edge<int>(1, 2));
            ShowGraph(undirectedGraph);

            // -----------------------------------------------------------------------------------------------------
            var arrayUndirectedGraph = new ArrayUndirectedGraph<int, Edge<int>>(undirectedGraph);
            ShowGraph(arrayUndirectedGraph);
        }

        [Test]
        public void EdgeListGraph()
        {
            var edgeListGraph = new EdgeListGraph<int, Edge<int>>();
            edgeListGraph.AddEdge(new Edge<int>(1, 2));
            edgeListGraph.AddVerticesAndEdge(new Edge<int>(1, 3));
            ShowEdgeListGraph(edgeListGraph);
        }

        [Test]
        public void ClusterGraph()
        {
            var wrappedGraph = new AdjacencyGraph<int, Edge<int>>();
            wrappedGraph.AddVertex(1);
            wrappedGraph.AddVerticesAndEdge(new Edge<int>(2, 3));
            var mainGraph = new ClusteredAdjacencyGraph<int, Edge<int>>(wrappedGraph);
            Console.WriteLine("Main:");
            ShowGraph(mainGraph);

            ClusteredAdjacencyGraph<int, Edge<int>> cluster = mainGraph.AddCluster();
            cluster.AddVerticesAndEdge(new Edge<int>(4, 5));
            Console.WriteLine("Cluster:");
            ShowGraph(cluster);
            Console.WriteLine("Main:");
            ShowGraph(mainGraph);
        }

        [Test]
        public void CompressedGraph()
        {
            var initialGraph = new AdjacencyGraph<int, Edge<int>>();
            initialGraph.AddVertexRange(new[] { 5, 6 });
            initialGraph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<int>(1, 2),
                new Edge<int>(2, 3),
                new Edge<int>(2, 4)
            });
            var compressedGraph = CompressedSparseRowGraph<int>.FromGraph(initialGraph);
            ShowCompressedGraph(compressedGraph);
        }

        [Test]
        public void DelegateGraph()
        {
            // A simple adjacency graph representation
            int[][] graph = new int[5][];
            graph[0] = new[] { 1 };
            graph[1] = new[] { 2, 3 };
            graph[2] = new[] { 3, 4 };
            graph[3] = new[] { 4 };
            graph[4] = new int[] { };

            var delegateGraph = Enumerable.Range(0, graph.Length)
                .ToDelegateVertexAndEdgeListGraph(
                    source => Array.ConvertAll(graph[source], target => new SEquatableEdge<int>(source, target)));

            // It's ready to use!
            ShowGraph(delegateGraph);
        }
    }
}
