using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using QuikGraph;
using QuikGraph.Serialization;

namespace Quik
{
    [TestFixture]
    public class Testee
    {

        [Test]
        public void Testeeee()
        {
            var graph = new BidirectionalGraph<Node, TaggedEdge<Node, EdgeData>>();
            var node1 = new Node(1, 2);
            var node2 = new Node(1, 3);
            graph.AddVertex(node1);
            graph.AddVertex(node2);
            graph.AddEdge(new TaggedEdge<Node, EdgeData>(node1, node2, new EdgeData(1)));

            Save(graph);
        }


        private static void Save(BidirectionalGraph<Node, TaggedEdge<Node, EdgeData>> graph)
        {
            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.NewLineOnAttributes = true;
            xmlWriterSettings.Indent = true;
            var sw = new StringWriter();
            var writer = XmlWriter.Create(sw, xmlWriterSettings);
            graph.SerializeToGraphML<Node, TaggedEdge<Node, EdgeData>,
                BidirectionalGraph<Node, TaggedEdge<Node, EdgeData>>>(writer);
            writer.Close();
            Console.Write(sw.ToString());
        }
    }

    public class EdgeData
    {
        [XmlAttribute("weight")]
        public float weight { get; set; }

        public EdgeData(float weight)
        {
            this.weight = weight;
        }
    }

    public class Node
    {
        [XmlAttribute("x")]
        public float x { get; set; }

        [XmlAttribute("y")]
        public float y { get; set; }

        public Node(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}