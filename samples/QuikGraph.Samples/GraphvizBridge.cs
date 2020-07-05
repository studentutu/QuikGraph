using System;
using NUnit.Framework;
using QuikGraph.Graphviz;
using QuikGraph.Graphviz.Dot;

namespace QuikGraph.Samples
{
    /// <summary>
    /// Samples to use Graphviz bridge.
    /// </summary>
    [TestFixture]
    public class GraphvizBridge
    {
        [Test]
        public void RenderGraphWithGraphviz()
        {
            var edges = new[]
            {
                new Edge<int>(1, 2),
                new Edge<int>(0, 1),
                new Edge<int>(0, 3),
                new Edge<int>(2, 3)
            };
            
            var graph = edges.ToAdjacencyGraph<int, Edge<int>>();
            Console.WriteLine(graph.ToGraphviz());
        }

        [Test]
        public void AdvancedRenderGraphWithGraphviz()
        {
            var edges = new[]
            {
                new Edge<int>(1, 2),
                new Edge<int>(0, 1),
                new Edge<int>(0, 3),
                new Edge<int>(2, 3)
            };
            
            var graph = edges.ToAdjacencyGraph<int, Edge<int>>();
            string dot = graph.ToGraphviz(algorithm =>
            {
                algorithm.CommonVertexFormat.ToolTip = "A vertex";
                algorithm.FormatVertex += (sender, args) =>
                {
                    args.VertexFormat.Label = $"My vertex {args.Vertex}";
                };
                algorithm.FormatEdge += (sender, args) =>
                {
                    args.EdgeFormat.Label = new GraphvizEdgeLabel
                    {
                        Value = $"My edge {args.Edge.Source} -> {args.Edge.Target}"
                    };
                };
            });

            Console.WriteLine(dot);
        }
    }
}