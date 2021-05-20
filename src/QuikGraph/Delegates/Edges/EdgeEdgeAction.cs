using JetBrains.Annotations;

namespace QuikGraph
{
    /// <summary>
    /// Delegate to perform an action involving the 2 edges.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <param name="edge1">First edge.</param>
    /// <param name="edge2">Second edge.</param>
    public delegate void EdgeEdgeAction<TVertex, in TEdge>([NotNull] TEdge edge1, [NotNull] TEdge edge2)
        where TEdge : IEdge<TVertex>;
}