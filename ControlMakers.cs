using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public class ListMaker
{
    public TextBox[] MakeList(string firstnum, string lastnum)
    {
        int length = Convert.ToInt32(lastnum) - Convert.ToInt32(firstnum) + 1;
        TextBox[] list = new TextBox[length];

        for (int i = 0; i < length; i++)
        {
            list[i] = new TextBox();
            list[i].Multiline = true;
            list[i].Size = new Size((1500 / length), 30 + length + i * 8);
            list[i].Text = Convert.ToString(Convert.ToInt32(firstnum) + i);
            list[i].Font = new Font("OCR A Extended", 10, FontStyle.Regular);
            list[i].ForeColor = Color.Turquoise;
            list[i].BackColor = Color.Black;
            list[i].TextAlign = HorizontalAlignment.Center;
        }

        return list;
    }
    public TextBox[] MakeRandomList(string firstnum, string lastnum)
    {
        int length = Convert.ToInt32(lastnum) - Convert.ToInt32(firstnum) + 1;
        TextBox[] list = new TextBox[length];
        List<int> numbers = new List<int>();

        for (int i = 0; i < length; i++)
        {
            numbers.Add(Convert.ToInt32(firstnum) + i);
        }

        Random randomnum = new Random();
        int listelement;
         
        for (int i = 0; i < length; i++)
        {
            list[i] = new TextBox();
            list[i].Size = new Size((1500 / length), 30);
            listelement = randomnum.Next(Convert.ToInt32(firstnum), Convert.ToInt32(lastnum) + 1);
            list[i].Text = Convert.ToString(listelement);
            list[i].Font = new Font("OCR A Extended", 10, FontStyle.Regular);
            list[i].ForeColor = Color.Turquoise;
            list[i].BackColor = Color.Black;
            list[i].TextAlign = HorizontalAlignment.Center;
            list[i].Multiline = true;
            list[i].Size = new Size((1500/length), 15 + length * 10 - 8 * (numbers.Max() - Convert.ToInt32(list[i].Text)));
            list[i].ReadOnly = true;
        }

        return list;
    }

    public TextBox[] MakeReverseList(string firstnum, string lastnum)
    {
        int length = Convert.ToInt32(lastnum) - Convert.ToInt32(firstnum) + 1;
        TextBox[] list = new TextBox[length];

        for (int i = (length - 1); i > -1 ; i--)
        {
            list[i] = new TextBox();
            list[i].Multiline = true;
            list[i].Size = new Size((1500 / length), 30 + 8 * length - i * 8);
            list[i].Text = Convert.ToString(Convert.ToInt32(lastnum) - i);
            list[i].Font = new Font("OCR A Extended", 10, FontStyle.Regular);
            list[i].ForeColor = Color.Turquoise;
            list[i].BackColor = Color.Black;
            list[i].TextAlign = HorizontalAlignment.Center;
            list[i].ReadOnly = true;
        }

        return list;
    }

    public TextBox[] RandomListNoRepeats(string firstnum, string lastnum)
    {
        int length = Convert.ToInt32(lastnum) - Convert.ToInt32(firstnum) + 1;
        TextBox[] list = new TextBox[length];
        List<int> numbers = new List<int>();

        for (int i = 0 ; i < length; i++)
        {
            numbers.Add(Convert.ToInt32(firstnum) + i);
        }

        Random rng = new Random();
        //Fisher-Yates shuffle
        for(int i = length - 1; i > 0; i--)
        {
            int randomindex = rng.Next(i+1);
            int temp = numbers[i];
            numbers[i] = numbers[randomindex]; 
            numbers[randomindex] = temp;
        }

        for (int i = 0; i < length; i++)
        {
            list[i] = new TextBox();
            list[i].Font = new Font("OCR A Extended", 10, FontStyle.Regular);
            list[i].ForeColor = Color.Turquoise;
            list[i].BackColor = Color.Black;
            list[i].TextAlign = HorizontalAlignment.Center;
            list[i].Text = numbers[i].ToString();
            list[i].Multiline = true;
            list[i].Size = new Size((1500 / length), 15 + length * 10 - 8 * (numbers.Max() - Convert.ToInt32(list[i].Text)));
            list[i].ReadOnly = true;
        }

        int biggestnum;
        int smallestnum;

        for(int i = 0; i < length; i++)
        {
            
        }

        return list;
    }
}

public static class ControlMaker
{
    public static Button MakeNewButton(string buttonname, int width, int height)
    {
        Button custombutton = new Button();
        custombutton.Size = new Size(width, height);
        custombutton.Text = buttonname;
        custombutton.Font = new Font("OCR A Extended", 10, FontStyle.Regular);
        custombutton.FlatStyle = FlatStyle.Flat;
        custombutton.BackColor = Color.Black;
        custombutton.FlatAppearance.BorderColor = Color.Turquoise;
        custombutton.FlatAppearance.BorderSize = 2;
        custombutton.ForeColor = Color.Turquoise;
        return custombutton;
    }

    public static TextBox MakeNewBox(string boxname, int width)
    {
        TextBox box = new TextBox();
        box.Size = new Size(width, 25);
        box.Text = boxname;
        box.Font = new Font("OCR A Extended", 10, FontStyle.Regular);
        box.TextAlign = HorizontalAlignment.Center;
        box.ForeColor = Color.Turquoise;
        box.BackColor = Color.Black;
        return box;
    }

    public static Panel MakeNewPanel(string boxname, int width, int height)
    {
        Panel panel = new Panel();
        panel.Size = new Size(width, height);
        panel.BackColor = Color.Turquoise;
        return panel;
    }

    public static Label MakeNewLabel(string labelname, int width, int height)
    {
        Label label = new Label();
        label.Size = new Size(width, height);
        label.Text = labelname;
        label.Font = new Font("OCR A Extended", 11, FontStyle.Regular);
        label.ForeColor = Color.Turquoise;
        label.TextAlign = ContentAlignment.MiddleCenter;
        return label;
    }

    public static CheckBox MakeNewCheckBox(string condition)
    {
        CheckBox checkbox = new CheckBox();
        checkbox.Text = condition;
        checkbox.Checked = false;
        checkbox.AutoSize = true;
        checkbox.ForeColor = Color.Turquoise;
        checkbox.BackColor = Color.Black;
        checkbox.TextAlign = ContentAlignment.MiddleCenter;
        return checkbox;
    }
}