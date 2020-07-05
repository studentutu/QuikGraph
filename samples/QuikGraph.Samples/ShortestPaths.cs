using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph.Algorithms;

namespace QuikGraph.Samples
{
    /// <summary>
    /// Samples to use shortest path algorithms.
    /// </summary>
    [TestFixture]
    public class ShortestPaths
    {
        private const string NewYork = "New York";
        private const string London = "London";
        private const string Prague = "Prague";
        private const string BuenosAires = "Buenos Aires";
        private const string Accra = "Accra";
        private const string Shanghai = "Shanghai";
        private const string Ushuaia = "Ushuaïa";

        #region Sample Helpers

        [Pure]
        [NotNull]
        private static AdjacencyGraph<string, TaggedEdge<string, int>> GetDAGCitiesGraph()
        {
            var cities = new AdjacencyGraph<string, TaggedEdge<string, int>>(); // A graph of cities
            cities.AddVertexRange(new[]
            {
                NewYork, London, Prague, BuenosAires, Accra, Shanghai, Ushuaia
            });
            cities.AddVerticesAndEdgeRange(new[]
            {
                new TaggedEdge<string, int>(NewYork, London, 3463),
                new TaggedEdge<string, int>(NewYork, Prague, 4086),
                new TaggedEdge<string, int>(NewYork, Shanghai, 7383),
                new TaggedEdge<string, int>(NewYork, BuenosAires, 5305),
                new TaggedEdge<string, int>(London, Prague, 643),
                new TaggedEdge<string, int>(London, Shanghai, 5723),
                new TaggedEdge<string, int>(London, BuenosAires, 6926),
                new TaggedEdge<string, int>(Prague, BuenosAires, 7352),
                new TaggedEdge<string, int>(Prague, Accra, 3196),
                new TaggedEdge<string, int>(Shanghai, Prague, 5278),
                new TaggedEdge<string, int>(Shanghai, Accra, 7840),
                new TaggedEdge<string, int>(BuenosAires, Accra, 4697)
            });

            return cities;
        }

        [Pure]
        [NotNull]
        private static AdjacencyGraph<string, TaggedEdge<string, int>> GetCitiesGraph()
        {
            AdjacencyGraph<string, TaggedEdge<string, int>> cities = GetDAGCitiesGraph();
            cities.AddVerticesAndEdgeRange(new[]
            {
                new TaggedEdge<string, int>(London, NewYork, 3463),
                new TaggedEdge<string, int>(Prague, London, 643),
                new TaggedEdge<string, int>(Prague, NewYork, 4086),
                new TaggedEdge<string, int>(Prague, Shanghai, 5278),
                new TaggedEdge<string, int>(Shanghai, London, 5723),
                new TaggedEdge<string, int>(Shanghai, NewYork, 7383),
                new TaggedEdge<string, int>(Accra, NewYork, 3179),
                new TaggedEdge<string, int>(Accra, London, 3179),
                new TaggedEdge<string, int>(Accra, Prague, 3196),
                new TaggedEdge<string, int>(Accra, Shanghai, 7840),
                new TaggedEdge<string, int>(Accra, BuenosAires, 4697),
                new TaggedEdge<string, int>(BuenosAires, NewYork, 5305),
                new TaggedEdge<string, int>(BuenosAires, London, 6926),
                new TaggedEdge<string, int>(BuenosAires, Prague, 7352)
            });

            return cities;
        }

        private static void ShowPath(
            [NotNull, InstantHandle] TryFunc<string, IEnumerable<TaggedEdge<string, int>>> tryGetPathFunc,
            [NotNull] string source,
            [NotNull] string target)
        {
            if (tryGetPathFunc(target, out IEnumerable<TaggedEdge<string, int>> path))
            {
                var bestPath = path as TaggedEdge<string, int>[] ?? path.ToArray();
                if (bestPath.Length > 1)
                {
                    int distance = 0;
                    for (int i = 0; i < bestPath.Length - 1; ++i)
                    {
                        Console.Write($"{bestPath[i].Source} -> ");
                        distance += bestPath[i].Tag;
                    }

                    var lastEdge = bestPath[bestPath.Length - 1];
                    distance += lastEdge.Tag;
                    Console.WriteLine($"{lastEdge.Source} -> {lastEdge.Target} ({distance})");
                }
                else
                {
                    Console.WriteLine(bestPath[0]);
                }
            }
            else
            {
                Console.WriteLine($"No path from {source} to {target}.");
            }
        }

        #endregion

        [Test]
        public void Dijkstra_ShortestPath()
        {
            AdjacencyGraph<string, TaggedEdge<string, int>> cities = GetCitiesGraph();
            Func<TaggedEdge<string, int>, double> citiesDistances = e => e.Tag; // A delegate that gives the distance between cities

            string sourceCity = NewYork;  // Starting city
            string targetCity = Shanghai; // Ending city

            // Compute shortest paths and returns a delegate vertex -> path
            var tryGetPath = cities.ShortestPathsDijkstra(citiesDistances, sourceCity);
            ShowPath(tryGetPath, sourceCity, targetCity);

            sourceCity = NewYork; // Starting city
            targetCity = Accra;   // Ending city
            ShowPath(tryGetPath, sourceCity, targetCity);

            sourceCity = NewYork; // Starting city
            targetCity = Ushuaia; // Ending city
            ShowPath(tryGetPath, sourceCity, targetCity);
        }

        [Test]
        public void AStar_ShortestPath()
        {
            AdjacencyGraph<string, TaggedEdge<string, int>> cities = GetCitiesGraph();
            Func<TaggedEdge<string, int>, double> citiesDistances = e => e.Tag; // A delegate that gives the distance between cities

            string sourceCity = NewYork;  // Starting city
            string targetCity = Shanghai; // Ending city

            // Compute shortest paths and returns a delegate vertex -> path
            var tryGetPath = cities.ShortestPathsAStar(
                citiesDistances,
                c => 1.0,
                sourceCity);
            ShowPath(tryGetPath, sourceCity, targetCity);

            sourceCity = NewYork; // Starting city
            targetCity = Accra;   // Ending city
            ShowPath(tryGetPath, sourceCity, targetCity);

            sourceCity = NewYork; // Starting city
            targetCity = Ushuaia; // Ending city
            ShowPath(tryGetPath, sourceCity, targetCity);
        }

        [Test]
        public void BellmanFord_ShortestPath()
        {
            AdjacencyGraph<string, TaggedEdge<string, int>> cities = GetCitiesGraph();
            Func<TaggedEdge<string, int>, double> citiesDistances = e => e.Tag; // A delegate that gives the distance between cities

            string sourceCity = NewYork;  // Starting city
            string targetCity = Shanghai; // Ending city

            // Compute shortest paths and returns a delegate vertex -> path
            var tryGetPath = cities.ShortestPathsBellmanFord(
                citiesDistances,
                sourceCity,
                out _);
            ShowPath(tryGetPath, sourceCity, targetCity);

            sourceCity = NewYork; // Starting city
            targetCity = Accra;   // Ending city
            ShowPath(tryGetPath, sourceCity, targetCity);

            sourceCity = NewYork; // Starting city
            targetCity = Ushuaia; // Ending city
            ShowPath(tryGetPath, sourceCity, targetCity);
        }

        [Test]
        public void DAG_ShortestPath()
        {
            AdjacencyGraph<string, TaggedEdge<string, int>> cities = GetDAGCitiesGraph();
            Func<TaggedEdge<string, int>, double> citiesDistances = e => e.Tag; // A delegate that gives the distance between cities

            string sourceCity = NewYork;  // Starting city
            string targetCity = Shanghai; // Ending city

            // Compute shortest paths and returns a delegate vertex -> path
            var tryGetPath = cities.ShortestPathsDag(
                citiesDistances,
                sourceCity);
            ShowPath(tryGetPath, sourceCity, targetCity);

            sourceCity = NewYork; // Starting city
            targetCity = Accra;   // Ending city
            ShowPath(tryGetPath, sourceCity, targetCity);

            sourceCity = NewYork; // Starting city
            targetCity = Ushuaia; // Ending city
            ShowPath(tryGetPath, sourceCity, targetCity);
        }
    }
}