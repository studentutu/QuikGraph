using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Serialization;

namespace Quik
{
    [TestFixture]
    public class Testee
    {

        public class MyEdge: TaggedEdge<Node, TplType<float>>//, ITagged<Data>, ITagged<Data2>, ITagged<TplType<float>>
        {
            //private Data _tag;
            //private Data2 _tag1;

            public MyEdge([NotNull] Node source, [NotNull] Node target) : base(source, target, new TplType<float>() { lolilol = 25 })
            {
            }

            [DefaultValue(3f)]
            [XmlAttribute("root")]
            public float Root { get; set; } = 5;

            //event EventHandler ITagged<Data>.TagChanged
            //{
            //    add => throw new NotImplementedException();
            //    remove => throw new NotImplementedException();
            //}

            //event EventHandler ITagged<TplType<float>>.TagChanged
            //{
            //    add => throw new NotImplementedException();
            //    remove => throw new NotImplementedException();
            //}
            //public TplType<float> Tag { get; set; }

            //Data2 ITagged<Data2>.Tag
            //{
            //    get => _tag1;
            //    set => _tag1 = value;
            //}

            //event EventHandler ITagged<Data2>.TagChanged
            //{
            //    add => throw new NotImplementedException();
            //    remove => throw new NotImplementedException();
            //}

            //Data ITagged<Data>.Tag
            //{
            //    get => _tag;
            //    set => _tag = value;
            //}
            
        }


        [Test]
        public void TagguedEdge_ReportBug()
        {
            var graph = new BidirectionalGraph<Node, TaggedEdge<Node, EdgeData>>();
            var node1 = new Node(1, 2);
            var node2 = new Node(1, 3);
            graph.AddVertex(node1);
            graph.AddVertex(node2);
            graph.AddEdge(new TaggedEdge<Node, EdgeData>(node1, node2, new EdgeData(1)));

            Save(graph);
        }

        [Test]
        public void CustomBiTaggedEdge_R()
        {
            var graph = new BidirectionalGraph<Node, MyEdge>();
            var node1 = new Node(1, 2);
            var node2 = new Node(1, 3);
            graph.AddVertex(node1);
            graph.AddVertex(node2);
            graph.AddEdge(new MyEdge(node1, node2));

            //graph.Edges.First().Tag = null;

            Save(graph);
        }

        public class TplType<T>
        {[DefaultValue(35f)]
            [XmlAttribute("Tpl")]
            public T lolilol { get; set; }
        }

        public class Data
        {
            [DefaultValue(10f)]
            [XmlAttribute("lolilol")]
            public float lolilol { get; set; } = 5;


            public Data()
            {
            }
        }

        public class Data2
        {
            [DefaultValue(20f)]
            [XmlAttribute("mmo")]
            public float Haha { get; set; }


            public Data2()
            {
            }
        }

        public class CustomTaggedEdge : Edge<Node>, ITagged<Data>
        {

            public CustomTaggedEdge([NotNull] Node source, [NotNull] Node target) : base(source, target)
            {
            }

            /// <inheritdoc />
            public event EventHandler TagChanged;

            [XmlAttribute("TagData")]
            public Data Tag { get; set; } = new Data();

            protected virtual void OnTagChanged()
            {
                TagChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public class Edgee : Edge<Node>
        {

            [XmlAttribute("weight")]
            public float Troll { get; set; } = 5;


            public Edgee([NotNull] Node source, [NotNull] Node target) : base(source, target)
            {
            }
        }

        [Test]
        public void EdgeWithDirectData_Working()
        {
            var graph = new BidirectionalGraph<Node, Edgee>();
            var node1 = new Node(1, 2);
            var node2 = new Node(1, 3);
            graph.AddVertex(node1);
            graph.AddVertex(node2);
            graph.AddEdge(new Edgee(node1, node2));

            Save(graph);
        }

        [Test]
        public void tagguedWithAttributeOnTag_ButrShouldBeBetterToHandleEarlier()
        {
            var graph = new BidirectionalGraph<Node, CustomTaggedEdge>();
            var node1 = new Node(1, 2);
            var node2 = new Node(1, 3);
            graph.AddVertex(node1);
            graph.AddVertex(node2);
            graph.AddEdge(new CustomTaggedEdge(node1, node2));

            Save(graph);
        }


        [Test]
        public void IdSpeciali()
        {
            var graph = new BidirectionalGraph<Node, Edgee>();
            var node1 = new Node(1, 2);
            var node2 = new Node(1, 3);
            graph.AddVertex(node1);
            graph.AddVertex(node2);
            graph.AddEdge(new Edgee(node1, node2));


            //Custom save
            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.NewLineOnAttributes = true;
            xmlWriterSettings.Indent = true;
            var sw = new StringWriter();
            var writer = XmlWriter.Create(sw, xmlWriterSettings);
            graph.SerializeToGraphML(
                writer,
                vertex => $"vertex({vertex.x},{vertex.y})",
                graph.GetEdgeIdentity());
            writer.Close();
            Console.Write(sw.ToString());
        }

        [Test]
        public void TestCodeWrite()
        {
            var graph = new BidirectionalGraph<Node, MyEdge>();
            var node1 = new Node(1, 2);
            var node2 = new Node(1, 3);
            graph.AddVertex(node1);
            graph.AddVertex(node2);
            graph.AddEdge(new MyEdge(node1, node2));

            MethodInfo write = typeof(XmlWriter).GetMethod(nameof(XmlWriter.WriteValue), new[] { typeof(float) });
            PropertyInfo root = typeof(MyEdge).GetProperty(nameof(MyEdge.Root));
            MethodInfo getRootMethod = root.GetGetMethod();

            PropertyInfo tag = typeof(MyEdge).GetProperty(nameof(MyEdge.Tag));
            MethodInfo gettagMethod = tag.GetGetMethod();

            PropertyInfo sub = typeof(TplType<float>).GetProperty(nameof(TplType<float>.lolilol));
            MethodInfo getSubMethod = sub.GetGetMethod();

            graph.Edges.First().Tag = null;

            //Custom save
            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.NewLineOnAttributes = true;
            xmlWriterSettings.Indent = true;
            var sw = new StringWriter();
            var writer = XmlWriter.Create(sw, xmlWriterSettings);
            //writer.WriteStartElement("data", GraphMLXmlResolver.GraphMLNamespace);
            //writer.WriteAttributeString("key", root.Name);
            //var a = getRootMethod.Invoke(graph.Edges.First(), null);
            //write.Invoke(writer, new []{ a });
            //writer.WriteEndElement();

            writer.WriteStartElement("data", GraphMLXmlResolver.GraphMLNamespace);
            writer.WriteAttributeString("key", root.Name);
            object t = gettagMethod.Invoke(graph.Edges.First(), null);
            if (t != null)
            {
                var b = getSubMethod.Invoke(t, null);
                write.Invoke(writer, new[] {b});
            }

            writer.WriteEndElement();


            writer.Close();
            Console.Write(sw.ToString());

            //// For each property of the type,
            //// write it to the XML writer (we need to take care of value types, etc...)

            //// writer.WriteStartAttribute("key");
            //generator.Emit(OpCodes.Ldarg_0);
            //generator.Emit(OpCodes.Ldstr, "key");
            //generator.Emit(OpCodes.Ldstr, info.Name);
            //generator.EmitCall(OpCodes.Callvirt, Metadata.WriteAttributeStringMethod, null);

            //// writer.WriteValue(v.xxx);
            //generator.Emit(OpCodes.Ldarg_0);
            //generator.Emit(OpCodes.Ldarg_1);
            //generator.EmitCall(OpCodes.Callvirt, getMethod, null);
            //EmitCallWriter(generator, writeMethod);
            
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

            var g2 = new BidirectionalGraph<Node, TaggedEdge<Node, EdgeData>>();
            g2.DeserializeFromGraphML<Node, TaggedEdge<Node, EdgeData>, BidirectionalGraph<Node, TaggedEdge<Node, EdgeData>>>(
                new StringReader(sw.ToString()),
                id => new Node(),
                (source, target, _) => new TaggedEdge<Node, EdgeData>(source, target, null));
        }

        private static void Save(BidirectionalGraph<Node, Edgee> graph)
        {
            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.NewLineOnAttributes = true;
            xmlWriterSettings.Indent = true;
            var sw = new StringWriter();
            var writer = XmlWriter.Create(sw, xmlWriterSettings);
            graph.SerializeToGraphML<Node, Edgee,
                BidirectionalGraph<Node, Edgee>>(writer);
            writer.Close();
            Console.Write(sw.ToString());
        }

        private static void Save(BidirectionalGraph<Node, CustomTaggedEdge> graph)
        {
            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.NewLineOnAttributes = true;
            xmlWriterSettings.Indent = true;
            var sw = new StringWriter();
            var writer = XmlWriter.Create(sw, xmlWriterSettings);
            graph.SerializeToGraphML<Node, CustomTaggedEdge,
                BidirectionalGraph<Node, CustomTaggedEdge>>(writer);
            writer.Close();
            Console.Write(sw.ToString());
        }

        private static void Save(BidirectionalGraph<Node, MyEdge> graph)
        {
            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.NewLineOnAttributes = true;
            xmlWriterSettings.Indent = true;
            var sw = new StringWriter();
            var writer = XmlWriter.Create(sw, xmlWriterSettings);
            graph.SerializeToGraphML<Node, MyEdge,
                BidirectionalGraph<Node, MyEdge>>(writer);
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

        //TODO tmp
        public Node()
        {
        }
    }
}