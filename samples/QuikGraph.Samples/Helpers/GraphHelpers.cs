using System;
using JetBrains.Annotations;

namespace QuikGraph.Samples.Helpers
{
    /// <summary>
    /// Helpers related to graph structures.
    /// </summary>
    public static class GraphHelpers
    {
        public static void ShowGraph<TVertex, TEdge>(
            [NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            Console.WriteLine("-------------------------------------------------------------------------");
            Console.WriteLine($"{(graph.IsDirected ? "D" : "Und")}irected graph ({graph.GetType().Name})");
            Console.WriteLine($"{graph.VertexCount} {(graph.VertexCount > 1 ? "Vertices" : "Vertex")} - {graph.EdgeCount} Edge{(graph.EdgeCount > 1 ? "s" : "")}");
            foreach (TVertex vertex in graph.Vertices)
            {
                Console.WriteLine($"V {vertex}");
            }
            foreach (TEdge edge in graph.Edges)
            {
                Console.WriteLine($"E {edge.Source} {(graph.IsDirected ? "" : "<")}-> {edge.Target}");
            }
            Console.WriteLine("-------------------------------------------------------------------------");
        }

        public static void ShowGraph<TVertex, TEdge>(
            [NotNull] IUndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            Console.WriteLine("-------------------------------------------------------------------------");
            Console.WriteLine($"Undirected graph ({graph.GetType().Name})");
            Console.WriteLine($"{graph.VertexCount} {(graph.VertexCount > 1 ? "Vertices" : "Vertex")} - {graph.EdgeCount} Edge{(graph.EdgeCount > 1 ? "s" : "")}");
            foreach (TVertex vertex in graph.Vertices)
            {
                Console.WriteLine($"V {vertex}");
            }
            foreach (TEdge edge in graph.Edges)
            {
                Console.WriteLine($"E {edge.Source} {(graph.IsDirected ? "" : "<")}-> {edge.Target}");
            }
            Console.WriteLine("-------------------------------------------------------------------------");
        }

        public static void ShowEdgeListGraph<TVertex, TEdge>(
            [NotNull] IEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            Console.WriteLine("-------------------------------------------------------------------------");
            Console.WriteLine($"{(graph.IsDirected ? "D" : "Und")}irected graph ({graph.GetType().Name})");
            Console.WriteLine($"{graph.VertexCount} {(graph.VertexCount > 1 ? "Vertices" : "Vertex")} - {graph.EdgeCount} Edge{(graph.EdgeCount > 1 ? "s" : "")}");
            foreach (TVertex vertex in graph.Vertices)
            {
                Console.WriteLine($"V {vertex}");
            }
            foreach (TEdge edge in graph.Edges)
            {
                Console.WriteLine($"E {edge.Source} {(graph.IsDirected ? "" : "<")}-> {edge.Target}");
            }
            Console.WriteLine("-------------------------------------------------------------------------");
        }

        public static void ShowCompressedGraph<TVertex>(
            [NotNull] IVertexListGraph<TVertex, SEquatableEdge<TVertex>> graph)
        {
            Console.WriteLine("-------------------------------------------------------------------------");
            Console.WriteLine($"{(graph.IsDirected ? "D" : "Und")}irected graph ({graph.GetType().Name})");
            Console.WriteLine($"{graph.VertexCount} {(graph.VertexCount > 1 ? "Vertices" : "Vertex")}");
            foreach (TVertex vertex in graph.Vertices)
            {
                Console.WriteLine($"V {vertex}");
            }
            Console.WriteLine("-------------------------------------------------------------------------");
        }
    }
}