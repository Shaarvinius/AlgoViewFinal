using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class SearchListMaker
{
    public TextBox[] MakeList(string firstnum, string lastnum)
    {
        int length = Convert.ToInt32(lastnum) - Convert.ToInt32(firstnum) + 1;
        TextBox[] list = new TextBox[length];

        for (int i = 0; i < length; i++)
        {
            list[i] = new TextBox();
            list[i].Size = new Size(35, 35);
            list[i].Text = Convert.ToString(Convert.ToInt32(firstnum) + i);
            list[i].Font = new Font("OCR A Extended", 10, FontStyle.Regular);
            list[i].ForeColor = Color.Turquoise;
            list[i].BackColor = Color.Black;
            list[i].TextAlign = HorizontalAlignment.Center;
        }

        return list;
    }
}


public class SortListMaker
{
    public TextBox[] MakeRandomList(string firstnum, string lastnum)
    {
        int length = Convert.ToInt32(lastnum) - Convert.ToInt32(firstnum) + 1;
        TextBox[] list = new TextBox[length];

        Random randomnum = new Random();
        int listelement;
        
        for (int i = 0; i < length; i++)
        {
            list[i] = new TextBox();
            list[i].Size = new Size(35, 35);

            listelement = randomnum.Next(Convert.ToInt32(firstnum), Convert.ToInt32(lastnum) + 1);
            list[i].Text = Convert.ToString(listelement);

            list[i].Font = new Font("OCR A Extended", 10, FontStyle.Regular);
            list[i].ForeColor = Color.Turquoise;
            list[i].BackColor = Color.Black;
            list[i].TextAlign = HorizontalAlignment.Center;
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
            list[i].Size = new Size(35, 35);
            list[i].Text = Convert.ToString(Convert.ToInt32(lastnum) - i);
            list[i].Font = new Font("OCR A Extended", 10, FontStyle.Regular);
            list[i].ForeColor = Color.Turquoise;
            list[i].BackColor = Color.Black;
            list[i].TextAlign = HorizontalAlignment.Center;
        }

        return list;
    }
}


public static class ButtonMaker
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
}


public static class BoxMaker
{
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
}


public static class PanelMaker
{
    public static Panel MakeNewPanel(string boxname, int width, int height)
    {
        Panel panel = new Panel();
        panel.Size = new Size(width, height);
        panel.BackColor = Color.Turquoise;

        return panel;
    }
}


public static class LabelMaker
{
    public static Label MakeNewLabel(string labelname, int width, int height)
    {
        Label label = new Label();
        label.Size = new Size(width, height);
        label.Text = labelname;
        label.Font = new Font("OCR A Extended", 10, FontStyle.Regular);
        label.ForeColor = Color.Turquoise;
        label.TextAlign = ContentAlignment.MiddleCenter;

        return label;
    }
}

public static class CheckBoxMaker
{
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
