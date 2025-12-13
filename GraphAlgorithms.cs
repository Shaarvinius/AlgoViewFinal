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
        // Breadth-First Search (BFS) for undirected graphs
        public static void BFS(Form1 form, int startNode)
        {
            int n = form.GraphNodes.Length;
            bool[] visited = new bool[n];
            Queue<int> queue = new Queue<int>();

            // Initial snapshot
            form.PushGraphStep(new GraphSnapshot((Button[])form.GraphNodes.Clone(), (bool[])form.GraphVisited.Clone(), null), "Initial state");

            visited[startNode] = true;
            form.GraphVisited[startNode] = true;
            queue.Enqueue(startNode);

            form.GraphNodes[startNode].BackColor = Color.Aquamarine;
            form.GraphNodes[startNode].ForeColor = Color.Black;

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();

                // Processing snapshot
                form.GraphNodes[current].BackColor = Color.Crimson;
                form.GraphNodes[current].ForeColor = Color.White;
                form.PushGraphStep(new GraphSnapshot((Button[])form.GraphNodes.Clone(), (bool[])form.GraphVisited.Clone(), null), $"Processing node {current}");

                foreach (var neighbor in form.Graph[current])
                {
                    int nextNode = neighbor.to;
                    if (!visited[nextNode])
                    {
                        visited[nextNode] = true;
                        form.GraphVisited[nextNode] = true;

                        // Highlight edge and push snapshot
                        form.PushGraphStep(new GraphSnapshot((Button[])form.GraphNodes.Clone(), (bool[])form.GraphVisited.Clone(), (current, nextNode)), $"Traverse edge {current} -> {nextNode}");

                        // Highlight neighbor node
                        form.GraphNodes[nextNode].BackColor = Color.DarkBlue;
                        form.GraphNodes[nextNode].ForeColor = Color.White;

                        form.PushGraphStep(new GraphSnapshot((Button[])form.GraphNodes.Clone(), (bool[])form.GraphVisited.Clone(), null), $"Visited node {nextNode}");

                        form.GraphNodes[nextNode].BackColor = Color.Aquamarine;
                        form.GraphNodes[nextNode].ForeColor = Color.Black;

                        queue.Enqueue(nextNode);
                    }
                }

                // Restore current node color
                form.GraphNodes[current].BackColor = Color.Aquamarine;
                form.GraphNodes[current].ForeColor = Color.Black;
            }

            form.PushGraphStep(new GraphSnapshot((Button[])form.GraphNodes.Clone(), (bool[])form.GraphVisited.Clone(), null), "BFS Complete");
        }
    }
}
