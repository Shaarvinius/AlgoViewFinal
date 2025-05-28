using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public class ListMethods
{


    public static void BinarySearch(TextBox[] list, int numtofind, List<ListSnapshot> steps)
    {
        int left = 0;
        int right = list.Length - 1;
        bool found = false;

        steps.Add(new ListSnapshot(list));

        while (left <= right)
        {
            int mid = (left + right) / 2;

            for (int i = 0; i < list.Length; i++)
            {
                list[i].BackColor = Color.Black;
                list[i].ForeColor = Color.Turquoise;
            }

            list[mid].BackColor = Color.Turquoise;
            list[mid].ForeColor = Color.Black;

            list[left].BackColor = Color.LimeGreen;
            list[left].ForeColor = Color.Black;

            list[right].BackColor = Color.Blue;
            list[right].ForeColor = Color.Black;

            steps.Add(new ListSnapshot(list));

            int midVal = Convert.ToInt32(list[mid].Text);

            if (midVal < numtofind)
            {
                left = mid + 1;
            }
            else if (midVal > numtofind)
            {
                right = mid - 1;
            }
            else
            {
                MessageBox.Show(numtofind + " found at index " + mid);
                found = true;
                break;
            }
        }

        if (!found)
        {
            Thread.Sleep(1500);
            MessageBox.Show("Number not found in array");
        }
    }


    public static void BubbleSort(TextBox[] list, List<ListSnapshot> sortingsteps)
    {
        int temp;
        int currentval;
        int nextval;
        int length = list.Length;
        bool swapped = true;

        while(length > 0 && swapped)
        {
            swapped = false;
            length--;
            for(int i = 0; i < length; i++)
            {
                currentval = Convert.ToInt32(list[i].Text);
                nextval = Convert.ToInt32((list[i+1].Text));

                sortingsteps.Add(new ListSnapshot(list));
                list[i].ForeColor = Color.White;
                list[i + 1].ForeColor = Color.White;
                if (currentval > nextval)
                {
                    list[i + 1].BackColor = Color.DarkRed;
                    list[i].BackColor = Color.Blue;
                }
                else
                {
                    list[i].BackColor = Color.DarkRed;
                    list[i+1].BackColor = Color.Blue;
                }

                if(currentval > nextval)
                {
                    temp = nextval;
                    nextval = currentval;
                    currentval = temp;

                    sortingsteps.Add(new ListSnapshot(list));
                    list[i].BackColor = Color.Black;
                    list[i+1].BackColor = Color.Black;
                    list[i].ForeColor = Color.Turquoise;
                    list[i+1].ForeColor = Color.Turquoise;

                    list[i].Text = Convert.ToString(currentval);
                    list[i + 1].Text = Convert.ToString(nextval);
                    swapped = true;
                }
                else
                {
                    list[i].BackColor = Color.Black;
                    list[i + 1].BackColor = Color.Black;
                    list[i].ForeColor = Color.Turquoise;
                    list[i + 1].ForeColor = Color.Turquoise;
                }
            }
        }
        sortingsteps.Add(new ListSnapshot(list));
    }


    public static void BubbleSortAuto(TextBox[] list)
    {
        int length = list.Length;
        bool swapped = true;

        while(length > 0)
        {
            swapped = false;
            length--;

            for(int i = 0; i < length; i++)
            {
                int currentVal = Convert.ToInt32(list[i].Text); 
                int nextVal = Convert.ToInt32(list[i+1].Text);

                if(currentVal < nextVal)
                {
                    list[i].BackColor = Color.CornflowerBlue;
                    list[i + 1].BackColor = Color.Crimson;
                }
                else
                {
                    list[i + 1].BackColor = Color.CornflowerBlue;
                    list[i].BackColor = Color.Crimson;
                }
                list[i].ForeColor = Color.White;
                list[i+1].ForeColor = Color.White;

                list[i].Refresh();
                list[i + 1].Refresh();
                Thread.Sleep(30);

                if(currentVal > nextVal)
                {
                    string temp = list[i].Text;
                    list[i].Text = list[i + 1].Text;
                    list[i + 1].Text = temp;
                    swapped = true;

                    list[i].Refresh();
                    list[i + 1].Refresh();
                    Thread.Sleep(30);
                }

                list[i].BackColor = Color.Black;
                list[i + 1].BackColor = Color.Black;
                list[i].ForeColor = Color.Turquoise;
                list[i + 1].ForeColor = Color.Turquoise;

                list[i].Refresh();
                list[i + 1].Refresh();
                Thread.Sleep(30);
            }

            
        }
    }
}
