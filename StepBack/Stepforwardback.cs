using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class ListSnapshot
{
    public int[] Values;
    public Color[] BackColours;
    public Color[] ForeColours;
    public Size[] Heights;

    public ListSnapshot(TextBox[] NumBoxes)
    {
        int length = NumBoxes.Length;
        Values = new int[length];
        BackColours = new Color[length];
        ForeColours = new Color[length];
        Heights = new Size[length];

        for (int i = 0; i < length; i++)
        {
            int.TryParse(NumBoxes[i].Text, out Values[i]);
            BackColours[i] = NumBoxes[i].BackColor;
            ForeColours[i] = NumBoxes[i].ForeColor;
            Heights[i] = NumBoxes[i].Size;
        }
    }

    public void Restore(TextBox[] NumBoxes)
    {
        for (int i = 0; i < NumBoxes.Length; i++)
        {
            NumBoxes[i].Text = Values[i].ToString();
            NumBoxes[i].BackColor = BackColours[i];
            NumBoxes[i].ForeColor = ForeColours[i];
            NumBoxes[i].Size = Heights[i]; 
        }
    }
}

public class ListStack
{
    private List<ListSnapshot> statelist = new List<ListSnapshot>();

    public int Count
    {
        get { return statelist.Count; }
    }

    public void Push(ListSnapshot liststate)
    {
        statelist.Add(liststate);
    }

    public ListSnapshot Pop()
    {
        if (statelist.Count == 0)
        {
            return null;
        }

        int lastindex = statelist.Count - 1;
        ListSnapshot liststate = statelist[lastindex];
        statelist.RemoveAt(lastindex);
        return liststate;
    }

    public ListSnapshot Peek()
    {
        if (statelist.Count == 0)
        {
            return null;
        }

        return statelist[statelist.Count - 1];
    }

    public void Clear()
    {
        statelist.Clear();
    }
}


public class PlayBack 
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
