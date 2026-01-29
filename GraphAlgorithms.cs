using AlgoView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace AlgoView
{
    public static class GraphAlgorithms
    {
        public struct WeightedEdge
        {
            public int To;
            public int Weight;

            public WeightedEdge(int to, int weight)
            {
                To = to;
                Weight = weight;
            }
        }

        // Breadth-First Search (BFS) for undirected graphs
        public static void BFS(Form1 form, int startNode)
        {
            int n = form.GraphNodes.Length;
            bool[] visited = new bool[n];
            Queue<int> queue = new Queue<int>();
            List<char> traversedNodes = new List<char>();

            form.PushGraphStep(
                new GraphSnapshot((Button[])form.GraphNodes.Clone(), (bool[])form.GraphVisited.Clone(), null),
                "Initial state\nNodes visited: "
            );
            form.Invalidate();

            visited[startNode] = true;
            form.GraphVisited[startNode] = true;
            queue.Enqueue(startNode);
            traversedNodes.Add((char)('A' + startNode));

            form.GraphNodes[startNode].BackColor = Color.Aquamarine;
            form.GraphNodes[startNode].ForeColor = Color.Black;

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();

                form.GraphNodes[current].BackColor = Color.Crimson;
                form.GraphNodes[current].ForeColor = Color.White;

                form.PushGraphStep(
                    new GraphSnapshot((Button[])form.GraphNodes.Clone(), (bool[])form.GraphVisited.Clone(), null),
                    $"Check node {(char)('A' + current)} for neighbours\nNodes visited: {string.Join(",", traversedNodes)}"
                );
                form.Invalidate();

                foreach (var neighbor in form.Graph[current])
                {
                    int nextNode = neighbor.to;
                    if (!visited[nextNode])
                    {
                        visited[nextNode] = true;
                        form.GraphVisited[nextNode] = true;
                        traversedNodes.Add((char)('A' + nextNode));

                        form.PushGraphStep(
                            new GraphSnapshot((Button[])form.GraphNodes.Clone(), (bool[])form.GraphVisited.Clone(), (current, nextNode)),
                            $"Traverse edge {(char)('A' + current)} -> {(char)('A' + nextNode)}\nNodes visited: {string.Join(",", traversedNodes)}"
                        );
                        form.Invalidate();

                        form.GraphNodes[nextNode].BackColor = Color.DarkBlue;
                        form.GraphNodes[nextNode].ForeColor = Color.White;

                        form.PushGraphStep(
                            new GraphSnapshot((Button[])form.GraphNodes.Clone(), (bool[])form.GraphVisited.Clone(), null),
                            $"Visited node {(char)('A' + nextNode)}\nNodes visited: {string.Join(",", traversedNodes)}"
                        );
                        form.Invalidate();

                        form.GraphNodes[nextNode].BackColor = Color.Aquamarine;
                        form.GraphNodes[nextNode].ForeColor = Color.Black;

                        queue.Enqueue(nextNode);
                    }
                }
                form.GraphNodes[current].BackColor = Color.Aquamarine;
                form.GraphNodes[current].ForeColor = Color.Black;
            }
            form.PushGraphStep(
                new GraphSnapshot((Button[])form.GraphNodes.Clone(), (bool[])form.GraphVisited.Clone(), null),
                "BFS Complete\nNodes visited: " + string.Join(",", traversedNodes)
            );
            form.Invalidate();
        }


        public static void DFS(Form1 form, int startNode)
        {
            int n = form.GraphNodes.Length;
            bool[] visited = new bool[n];
            List<char> traversedNodes = new List<char>();

            void DFSVisit(int current)
            {
                visited[current] = true;
                form.GraphVisited[current] = true;
                traversedNodes.Add((char)('A' + current));

                form.GraphNodes[current].BackColor = Color.Crimson;
                form.GraphNodes[current].ForeColor = Color.White;

                form.PushGraphStep(
                    new GraphSnapshot((Button[])form.GraphNodes.Clone(), (bool[])form.GraphVisited.Clone(), null),
                    "Visit node " + (char)('A' + current) + "\nNodes visited: " + string.Join(",", traversedNodes)
                );
                form.Invalidate();

                foreach (var edge in form.Graph[current])
                {
                    int next = edge.to;

                    if (!visited[next])
                    {
                        form.CurrentHighlightedEdge = (current, next);

                        form.PushGraphStep(
                            new GraphSnapshot((Button[])form.GraphNodes.Clone(), (bool[])form.GraphVisited.Clone(), (current, next)),
                            "Traverse edge " + (char)('A' + current) + " -> " + (char)('A' + next) + "\nNodes visited: " + string.Join(",", traversedNodes)
                        );
                        form.Invalidate();

                        DFSVisit(next);
                    }
                }

                form.GraphNodes[current].BackColor = Color.Aquamarine;
                form.GraphNodes[current].ForeColor = Color.Black;

                form.PushGraphStep(
                    new GraphSnapshot((Button[])form.GraphNodes.Clone(), (bool[])form.GraphVisited.Clone(), null),
                    "Backtrack from node " + (char)('A' + current) + "\nNodes visited: " + string.Join(",", traversedNodes)
                );
                form.Invalidate();
            }

            form.PushGraphStep(
                new GraphSnapshot((Button[])form.GraphNodes.Clone(), (bool[])form.GraphVisited.Clone(), null),
                "Initial state\nNodes visited: "
            );
            form.Invalidate();

            DFSVisit(startNode);

            form.PushGraphStep(
                new GraphSnapshot((Button[])form.GraphNodes.Clone(), (bool[])form.GraphVisited.Clone(), null),
                "DFS Complete\nNodes visited: " + string.Join(",", traversedNodes)
            );
            form.Invalidate();
        }


        public static void Dijkstra(Form1 form, int start)
        {
            int n = form.GraphNodes.Length;
            int[] dist = new int[n];
            int[] prev = new int[n];

            for (int i = 0; i < n; i++)
            {
                dist[i] = int.MaxValue;
                prev[i] = -1;
            }

            MinPriorityQueue pq = new MinPriorityQueue();
            dist[start] = 0;
            pq.Enqueue(start, 0);

            // Record starting node distance before any snapshot
            char startChar = (char)('A' + start);
            form.DistanceHistory[startChar].Add(0);

            // Push initial snapshot WITHOUT distanceHistory first
            form.PushGraphStep(
                new GraphSnapshot(
                    (Control[])form.GraphNodes.Clone(),
                    (bool[])form.GraphVisited.Clone(),
                    null,
                    null // <-- don't pass DistanceHistory yet
                ),
                $"Start at node {startChar}"
            );

            while (pq.Count > 0)
            {
                int u = pq.Dequeue();
                if (form.GraphVisited[u]) continue;

                form.GraphVisited[u] = true;

                // Highlight current node
                form.GraphNodes[u].BackColor = Color.Orange;
                form.GraphNodes[u].ForeColor = Color.Black;

                char nodeChar = (char)('A' + u);
                var history = form.DistanceHistory[nodeChar];
                if (history.Count == 0 || history.Last() != dist[u])
                    history.Add(dist[u]);

                form.PushGraphStep(
                    new GraphSnapshot(
                        (Control[])form.GraphNodes.Clone(),
                        (bool[])form.GraphVisited.Clone(),
                        null,
                        form.DistanceHistory
                    ),
                    $"Visit node {nodeChar}"
                );

                foreach (var edge in form.Graph[u])
                {
                    int v = edge.to;
                    int w = edge.weight;
                    if (form.GraphVisited[v]) continue;

                    form.CurrentHighlightedEdge = (u, v);
                    int alt = dist[u] + w;
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                        pq.Enqueue(v, alt);

                        char vChar = (char)('A' + v);
                        var vHistory = form.DistanceHistory[vChar];
                        if (vHistory.Count == 0 || vHistory.Last() != dist[v])
                            vHistory.Add(dist[v]);

                        form.PushGraphStep(
                            new GraphSnapshot(
                                (Control[])form.GraphNodes.Clone(),
                                (bool[])form.GraphVisited.Clone(),
                                (u, v),
                                form.DistanceHistory
                            ),
                            $"Update {vChar} via {nodeChar} (distance {dist[v]})"
                        );
                    }
                }
            }
            form.CurrentHighlightedEdge = null;
        }


        static string NodeName(int i)
        {
            return ((char)('A' + i)).ToString();
        }

        class MinPriorityQueue
        {
            private List<(int node, int dist)> heap = new List<(int, int)>();
            public int Count => heap.Count;
            public void Enqueue(int node, int dist)
            {
                heap.Add((node, dist));
                HeapifyUp(heap.Count - 1);
            }

            public int Dequeue()
            {
                var root = heap[0].node;
                heap[0] = heap[^1];
                heap.RemoveAt(heap.Count - 1);
                if (heap.Count > 0)
                    HeapifyDown(0);
                return root;
            }

            private void HeapifyUp(int i)
            {
                if (i == 0) return;
                int p = (i - 1) / 2;

                if (heap[i].dist < heap[p].dist)
                {
                    (heap[i], heap[p]) = (heap[p], heap[i]);
                    HeapifyUp(p);
                }
            }

            private void HeapifyDown(int i)
            {
                int l = 2 * i + 1;
                int r = 2 * i + 2;
                int s = i;

                if (l < heap.Count && heap[l].dist < heap[s].dist) s = l;
                if (r < heap.Count && heap[r].dist < heap[s].dist) s = r;

                if (s != i)
                {
                    (heap[i], heap[s]) = (heap[s], heap[i]);
                    HeapifyDown(s);
                }
            }
        }
    }
}