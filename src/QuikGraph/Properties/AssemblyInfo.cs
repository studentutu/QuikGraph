using System.Runtime.CompilerServices;
using QuikGraph;

[assembly: InternalsVisibleTo("QuikGraph.Data" + PublicKey.Key)]
[assembly: InternalsVisibleTo("QuikGraph.Graphviz" + PublicKey.Key)]
[assembly: InternalsVisibleTo("QuikGraph.MSAGL" + PublicKey.Key)]
[assembly: InternalsVisibleTo("QuikGraph.Petri" + PublicKey.Key)]
[assembly: InternalsVisibleTo("QuikGraph.Serialization" + PublicKey.Key)]
[assembly: InternalsVisibleTo("QuikGraphTestsCommon" + PublicKey.Key)]
[assembly: InternalsVisibleTo("QuikGraph.Tests" + PublicKey.Key)]
[assembly: InternalsVisibleTo("QuikGraph.Data.Tests" + PublicKey.Key)]
[assembly: InternalsVisibleTo("QuikGraph.GraphViz.Tests" + PublicKey.Key)]
[assembly: InternalsVisibleTo("QuikGraph.MSAGL.Tests" + PublicKey.Key)]
[assembly: InternalsVisibleTo("QuikGraph.Petri.Tests" + PublicKey.Key)]
[assembly: InternalsVisibleTo("QuikGraph.Serialization.Tests" + PublicKey.Key)]