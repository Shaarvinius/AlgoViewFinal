using AlgoView;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public interface ISnapshot
{
    void Restore();
}

public class ListSnapshot: ISnapshot // format for list interface snapshots 
{
    private TextBox[] boxes;
    private int[] values;
    private Color[] backColours;
    private Color[] foreColours;
    private Size[] sizes;

    public ListSnapshot(TextBox[] numBoxes)
    {
        boxes = numBoxes;

        int length = numBoxes.Length;

        values = new int[length];
        backColours = new Color[length];
        foreColours = new Color[length];
        sizes = new Size[length];

        for (int i = 0; i < length; i++)
        {
            int.TryParse(numBoxes[i].Text, out values[i]);
            backColours[i] = numBoxes[i].BackColor;
            foreColours[i] = numBoxes[i].ForeColor;
            sizes[i] = numBoxes[i].Size;
        }
    }

    public void Restore() // restores previous state of each textbox
    {
        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i].Text = values[i].ToString();
            boxes[i].BackColor = backColours[i];
            boxes[i].ForeColor = foreColours[i];
            boxes[i].Size = sizes[i];
        }
    }
}

public class GraphSnapshot : ISnapshot
{
    private Control[] nodes;
    private Color[] backColours;
    private Color[] foreColours;
    private Point[] positions;
    private Size[] sizes;
    private (int from, int to)? highlightedEdge;
    private bool[] visitedState;

    public GraphSnapshot(Control[] nodeControls, bool[] visited, (int from, int to)? edge = null)
    {
        nodes = nodeControls;
        highlightedEdge = edge;
        int length = nodeControls.Length;

        backColours = new Color[length];
        foreColours = new Color[length];
        positions = new Point[length];
        sizes = new Size[length];
        visitedState = new bool[length];

        for (int i = 0; i < length; i++)
        {
            backColours[i] = nodeControls[i].BackColor;
            foreColours[i] = nodeControls[i].ForeColor;
            positions[i] = nodeControls[i].Location;
            sizes[i] = nodeControls[i].Size;
            if (visited != null && i < visited.Length)
                visitedState[i] = visited[i];
        }
    }

    public void Restore()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].BackColor = backColours[i];
            nodes[i].ForeColor = foreColours[i];
            nodes[i].Location = positions[i];
            nodes[i].Size = sizes[i];
        }

        if (nodes.Length > 0 && nodes[0].FindForm() is Form1 form)
        {
            form.CurrentHighlightedEdge = highlightedEdge;
            form.Invalidate();
        }
    }

    public (int from, int to)? HighlightedEdge => highlightedEdge;
    public bool[] GetVisitedState() => (bool[])visitedState.Clone();
}


public class SteppingStack
{
    private List<ISnapshot> snapshots = new List<ISnapshot>(); // list that acts like a stack

    public void Push(ISnapshot step)
    {
        snapshots.Add(step);
    }

    public ISnapshot Pop()
    {
        if (snapshots.Count == 0) throw new InvalidOperationException("Empty stack");
        int last = snapshots.Count - 1;
        ISnapshot item = snapshots[last];
        snapshots.RemoveAt(last);
        return item;
    }

    public ISnapshot Peek()
    {
        if (snapshots.Count == 0) throw new InvalidOperationException("Empty stack");
        return snapshots[snapshots.Count - 1];
    }

    public int Count
    {
        get { return snapshots.Count; }
    }

    public void Clear()
    {
        snapshots.Clear();
    }
}



public class PlayBack // logic for pausing and resuming
{
    private bool paused = false;
    public async Task WaitIfPaused()
    {
        while (paused)
        {
            await Task.Delay(100);
        }
    }

    public void Pause()
    {
        paused = true;
    }
    public void Resume()
    {
        paused = false;
    }
    public bool IsPaused => paused;
}
