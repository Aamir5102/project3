using System;
using System.Collections.Generic;
using System.Diagnostics;

class SocialNetworkInfluence
{
    // calculate influence scores in an unweighted graph using BFS
    public static Dictionary<int, double> CalculateInfluenceScoresUnweighted(Dictionary<int, List<int>> graph)
    {
        var influenceScores = new Dictionary<int, double>();

        foreach (var node in graph.Keys)
        {
            var distance = new Dictionary<int, int>();
            var visited = new HashSet<int>();
            var queue = new Queue<int>();

            foreach (var n in graph.Keys)
            {
                distance[n] = int.MaxValue;
            }
            distance[node] = 0;
            queue.Enqueue(node);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                visited.Add(current);

                foreach (var neighbor in graph[current])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        distance[neighbor] = distance[current] + 1;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            double totalDistance = 0;
            foreach (var d in distance.Values)
            {
                if (d != int.MaxValue)
                    totalDistance += d;
            }

            influenceScores[node] = totalDistance > 0 ? 1.0 / totalDistance : 0;
        }

        return influenceScores;
    }

    // calculate influence scores in a weighted graph using Dijkstras algorithm
    public static Dictionary<int, double> CalculateInfluenceScoresWeighted(Dictionary<int, List<(int, double)>> graph)
    {
        var influenceScores = new Dictionary<int, double>();

        foreach (var node in graph.Keys)
        {
            var distance = new Dictionary<int, double>();
            var pq = new PriorityQueue<int, double>();

            foreach (var n in graph.Keys)
            {
                distance[n] = double.MaxValue;
            }
            distance[node] = 0;
            pq.Enqueue(node, 0);

            while (pq.Count > 0)
            {
                var current = pq.Dequeue();

                foreach (var (neighbor, weight) in graph[current])
                {
                    if (distance[current] + weight < distance[neighbor])
                    {
                        distance[neighbor] = distance[current] + weight;
                        pq.Enqueue(neighbor, distance[neighbor]);
                    }
                }
            }

            double totalDistance = 0;
            foreach (var d in distance.Values)
            {
                if (d != double.MaxValue)
                    totalDistance += d;
            }

            influenceScores[node] = totalDistance > 0 ? 1.0 / totalDistance : 0;
        }

        return influenceScores;
    }

    // make a random unweighted graph
    static Dictionary<int, List<int>> GenerateUnweightedGraph(int nodeCount, int averageDegree)
    {
        var rand = new Random();
        var graph = new Dictionary<int, List<int>>();

        for (int i = 0; i < nodeCount; i++)
        {
            graph[i] = new List<int>();
            for (int j = 0; j < averageDegree; j++)
            {
                int neighbor = rand.Next(nodeCount);
                if (neighbor != i && !graph[i].Contains(neighbor))
                {
                    graph[i].Add(neighbor);
                    if (!graph.ContainsKey(neighbor))
                        graph[neighbor] = new List<int>();
                    graph[neighbor].Add(i);
                }
            }
        }

        return graph;
    }

    // make a random weighted graph
    static Dictionary<int, List<(int, double)>> GenerateWeightedGraph(int nodeCount, int averageDegree)
    {
        var rand = new Random();
        var graph = new Dictionary<int, List<(int, double)>>();

        for (int i = 0; i < nodeCount; i++)
        {
            graph[i] = new List<(int, double)>();
            for (int j = 0; j < averageDegree; j++)
            {
                int neighbor = rand.Next(nodeCount);
                double weight = Math.Round(rand.NextDouble() * 10, 2);
                if (neighbor != i && !graph[i].Exists(x => x.Item1 == neighbor))
                {
                    graph[i].Add((neighbor, weight));
                    if (!graph.ContainsKey(neighbor))
                        graph[neighbor] = new List<(int, double)>();
                    graph[neighbor].Add((i, weight));
                }
            }
        }

        return graph;
    }

    // measure and print execution time
    static void MeasureExecutionTime(string description, Action action)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        action();
        stopwatch.Stop();
        Console.WriteLine($"{description} Execution Time: {stopwatch.ElapsedMilliseconds} ms");
    }

    // program testing
    static void Main(string[] args)
    {
        var unweightedGraph = GenerateUnweightedGraph(1000, 5);
        var weightedGraph = GenerateWeightedGraph(1000, 5);

        Console.WriteLine("Testing Influence Score (Unweighted)...");
        MeasureExecutionTime("Unweighted Influence", () =>
        {
            var unweightedInfluence = CalculateInfluenceScoresUnweighted(unweightedGraph);
            foreach (var kvp in unweightedInfluence)
            {
                Console.WriteLine($"Node {kvp.Key}: Influence Score = {kvp.Value:F5}");
            }
        });

        Console.WriteLine("\nTesting Influence Score (Weighted)...");
        MeasureExecutionTime("Weighted Influence", () =>
        {
            var weightedInfluence = CalculateInfluenceScoresWeighted(weightedGraph);
            foreach (var kvp in weightedInfluence)
            {
                Console.WriteLine($"Node {kvp.Key}: Influence Score = {kvp.Value:F5}");
            }
        });
    }
}
