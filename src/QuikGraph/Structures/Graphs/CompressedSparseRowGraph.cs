﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if SUPPORTS_AGGRESSIVE_INLINING
using System.Runtime.CompilerServices;
#endif
using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// Directed graph data structure using a compressed sparse row representation.
    /// (http://www.cs.utk.edu/~dongarra/etemplates/node373.html)
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
#if SUPPORTS_SERIALIZATION
    [Serializable]
#endif
    [DebuggerDisplay("VertexCount = {" + nameof(VertexCount) + "}, EdgeCount = {" + nameof(EdgeCount) + "}")]
    public sealed class CompressedSparseRowGraph<TVertex> : IVertexListGraph<TVertex, SEquatableEdge<TVertex>>, IEdgeListGraph<TVertex, SEquatableEdge<TVertex>>
#if SUPPORTS_CLONEABLE
        , ICloneable
#endif
    {
#if SUPPORTS_SERIALIZATION
        [Serializable]
#endif
        private struct Range
        {
            public readonly int Start;
            public readonly int End;

            public Range(int start, int end)
            {
                Debug.Assert(start >= 0, "Must be positive");
                Debug.Assert(start <= end, $"Must be less that {nameof(start)} ({start}).");

                Start = start;
                End = end;
            }

            public int Length => End - Start;
        }

        private CompressedSparseRowGraph(
            [NotNull] Dictionary<TVertex, Range> outEdgeStartRanges,
            [NotNull, ItemNotNull] TVertex[] outEdges)
        {
            Debug.Assert(outEdgeStartRanges != null);
            Debug.Assert(outEdges != null);

            _outEdgeStartRanges = outEdgeStartRanges;
            _outEdges = outEdges;
        }

        /// <summary>
        /// Converts the given <paramref name="visitedGraph"/> to a <see cref="CompressedSparseRowGraph{TVertex}"/>.
        /// </summary>
        /// <param name="visitedGraph">Graph to convert.</param>
        /// <typeparam name="TEdge">Edge type.</typeparam>
        /// <returns>A corresponding <see cref="CompressedSparseRowGraph{TVertex}"/>.</returns>
        [NotNull]
        public static CompressedSparseRowGraph<TVertex> FromGraph<TEdge>(
            [NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph)
            where TEdge : IEdge<TVertex>
        {
            if (visitedGraph is null)
                throw new ArgumentNullException(nameof(visitedGraph));

            var outEdgeStartRanges = new Dictionary<TVertex, Range>(visitedGraph.VertexCount);
            var outEdges = new TVertex[visitedGraph.EdgeCount];

            int start = 0;
            int index = 0;
            foreach (TVertex vertex in visitedGraph.Vertices)
            {
                int end = start + visitedGraph.OutDegree(vertex);
                var range = new Range(start, end);
                outEdgeStartRanges.Add(vertex, range);

                foreach (TEdge edge in visitedGraph.OutEdges(vertex))
                {
                    outEdges[index++] = edge.Target;
                }

                start = end;
                Debug.Assert(index == end);
            }

            Debug.Assert(index == outEdges.Length);

            return new CompressedSparseRowGraph<TVertex>(outEdgeStartRanges, outEdges);
        }

        #region IGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsDirected => true;

        /// <inheritdoc />
        public bool AllowParallelEdges => false;

        #endregion

        #region IVertexSet<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsVerticesEmpty => VertexCount == 0;

        /// <inheritdoc />
        public int VertexCount => _outEdgeStartRanges.Count;

        /// <inheritdoc />
        public IEnumerable<TVertex> Vertices => _outEdgeStartRanges.Keys.AsEnumerable();

        /// <inheritdoc />
        public bool ContainsVertex(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return _outEdgeStartRanges.ContainsKey(vertex);
        }

        #endregion

        #region IEdgeSet<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsEdgesEmpty => EdgeCount == 0;

        /// <inheritdoc />
        public int EdgeCount => _outEdges.Length;

        [NotNull, ItemNotNull]
        private readonly TVertex[] _outEdges;

        [NotNull]
        private readonly Dictionary<TVertex, Range> _outEdgeStartRanges;

        /// <inheritdoc />
        public IEnumerable<SEquatableEdge<TVertex>> Edges
        {
            get
            {
                foreach (KeyValuePair<TVertex, Range> pair in _outEdgeStartRanges)
                {
                    TVertex source = pair.Key;
                    Range range = pair.Value;
                    for (int i = range.Start; i < range.End; ++i)
                    {
                        TVertex target = _outEdges[i];
                        yield return new SEquatableEdge<TVertex>(source, target);
                    }
                }
            }
        }

        /// <inheritdoc />
        public bool ContainsEdge(SEquatableEdge<TVertex> edge)
        {
            return ContainsEdge(edge.Source, edge.Target);
        }

        #endregion

        #region IIncidenceGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool ContainsEdge(TVertex source, TVertex target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (_outEdgeStartRanges.TryGetValue(source, out Range range))
            {
                for (int i = range.Start; i < range.End; ++i)
                {
                    if (EqualityComparer<TVertex>.Default.Equals(_outEdges[i], target))
                        return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public bool TryGetEdge(TVertex source, TVertex target, out SEquatableEdge<TVertex> edge)
        {
            if (ContainsEdge(source, target))
            {
                edge = new SEquatableEdge<TVertex>(source, target);
                return true;
            }

            edge = default(SEquatableEdge<TVertex>);
            return false;
        }

        /// <inheritdoc />
        public bool TryGetEdges(TVertex source, TVertex target, out IEnumerable<SEquatableEdge<TVertex>> edges)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (_outEdgeStartRanges.TryGetValue(source, out Range range))
            {
                edges = GetEdges();
                return true;
            }

            edges = null;
            return false;

            #region Local function

            IEnumerable<SEquatableEdge<TVertex>> GetEdges()
            {
                for (int i = range.Start; i < range.End; ++i)
                {
                    if (EqualityComparer<TVertex>.Default.Equals(_outEdges[i], target))
                        yield return new SEquatableEdge<TVertex>(source, target);
                }
            }

            #endregion
        }

        #endregion

        #region IImplicitGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsOutEdgesEmpty(TVertex vertex)
        {
            return OutDegree(vertex) == 0;
        }

        /// <inheritdoc />
        public int OutDegree(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_outEdgeStartRanges.TryGetValue(vertex, out Range range))
                return range.Length;
            throw new VertexNotFoundException();
        }

        /// <inheritdoc />
        public IEnumerable<SEquatableEdge<TVertex>> OutEdges(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));
            return OutEdgesIterator(vertex);
        }

        [Pure]
        [NotNull]
#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private IEnumerable<SEquatableEdge<TVertex>> OutEdgesIterator([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            if (_outEdgeStartRanges.TryGetValue(vertex, out Range range))
            {
                for (int i = range.Start; i < range.End; ++i)
                    yield return new SEquatableEdge<TVertex>(vertex, _outEdges[i]);
            }
            else
                throw new VertexNotFoundException();
        }

        /// <inheritdoc />
        public bool TryGetOutEdges(TVertex vertex, out IEnumerable<SEquatableEdge<TVertex>> edges)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_outEdgeStartRanges.ContainsKey(vertex))
            {
                edges = OutEdgesIterator(vertex);
                return true;
            }

            edges = null;
            return false;
        }

        /// <inheritdoc />
        public SEquatableEdge<TVertex> OutEdge(TVertex vertex, int index)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            if (_outEdgeStartRanges.TryGetValue(vertex, out Range range))
            {
                int targetIndex = range.Start + index;
                return new SEquatableEdge<TVertex>(vertex, _outEdges[targetIndex]);
            }

            throw new VertexNotFoundException();
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Clones this graph.
        /// </summary>
        /// <returns>Cloned graph.</returns>
        [Pure]
        [NotNull]
        public CompressedSparseRowGraph<TVertex> Clone()
        {
            var ranges = new Dictionary<TVertex, Range>(_outEdgeStartRanges);
            var edges = (TVertex[])_outEdges.Clone();
            return new CompressedSparseRowGraph<TVertex>(ranges, edges);
        }

#if SUPPORTS_CLONEABLE
        /// <inheritdoc />
        object ICloneable.Clone()
        {
            return Clone();
        }
#endif

        #endregion
    }
}