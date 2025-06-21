using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class ListMethods
{

    public static void MergeSort(TextBox[] numbers, List<ListSnapshot> sortingsteps, List<string> Steplabels)
    {
        if (numbers == null || numbers.Length <= 1)
        {
            return;
        }

        void SetTextBox(TextBox target, string text, Size size)
        {
            target.Text = text;
            target.Size = size;
        }
        
        void ChangeColor(TextBox box, Color backcolor, Color textcolor)
        {
            box.BackColor = backcolor;
            box.ForeColor = textcolor;
        }

        void Merge(int left, int middle, int right)
        {
            int leftsize = middle - left + 1;
            int rightsize = right - middle;

            string[] leftnumbers = new string[leftsize];
            Size[] leftboxsizes = new Size[leftsize];
            string[] rightnumbers = new string[rightsize];
            Size[] rightboxsizes = new Size[rightsize];

            for(int z = 0; z < leftsize; z++)
            {
                leftnumbers[z] = numbers[left+z].Text;
                leftboxsizes[z] = numbers[left+z].Size;
            }

            for (int z = 0; z < rightsize; z++)
            {
                rightnumbers[z] = numbers[middle + 1 + z].Text;
                rightboxsizes[z] = numbers[middle + 1 + z].Size;
            }

            int i = 0;
            int j = 0;
            int k = left;

            while(i < leftsize && j < rightsize)
            {
                int leftVal = int.Parse(leftnumbers[i]);
                int rightVal = int.Parse(rightnumbers[j]);

                if(leftVal <= rightVal)
                {
                    sortingsteps.Add(new ListSnapshot(numbers));
                    Steplabels.Add("c");
                    SetTextBox(numbers[k], leftnumbers[i], leftboxsizes[i]);
                    i++;
                }
                else
                {

                    sortingsteps.Add(new ListSnapshot(numbers));
                    Steplabels.Add("d");
                    SetTextBox(numbers[k], rightnumbers[j], rightboxsizes[j]);
                    j++;
                }

                k++;
            }

            while(i < leftsize)
            {

                sortingsteps.Add(new ListSnapshot(numbers));
                Steplabels.Add("e");
                SetTextBox(numbers[k], leftnumbers[i], leftboxsizes[i]);
                i++;
                k++;
            }
            while(j < rightsize)
            {
                sortingsteps.Add(new ListSnapshot(numbers));
                Steplabels.Add("f");
                SetTextBox(numbers[k], rightnumbers[j], rightboxsizes[j]);
                j++;
                k++;
            }
        }
         

        void Sort(int left, int right)
        {
            if(left < right)
            {
                int middle = (left + right) / 2;
                Sort(left, middle);
                Sort(middle + 1, right);
                Merge(left, middle, right);
            }
        }

        Sort(0, numbers.Length - 1);
    }


    public static void InsertionSort(TextBox[] list, List<ListSnapshot> sortingsteps, List<string> Steplabels)
    {
        sortingsteps.Add(new ListSnapshot(list));
        Steplabels.Add("Initial state");

        for (int i = 1; i < list.Length; i++)
        {
            int content = Convert.ToInt32(list[i].Text);
            int index = i;

            while (index > 0 && Convert.ToInt32(list[index - 1].Text) > content)
            {
                int leftVal = Convert.ToInt32(list[index - 1].Text);
                list[index - 1].BackColor = Color.DarkBlue;
                list[index - 1].ForeColor = Color.White;
                list[index].BackColor = Color.Crimson;
                list[index].ForeColor = Color.White;

                sortingsteps.Add(new ListSnapshot(list));

                list[index].BackColor = Color.Aquamarine;
                list[index].ForeColor = Color.Black;
                list[index].Text = leftVal.ToString();
                Size tempSize = list[index].Size;
                list[index].Size = list[index - 1].Size;
                list[index - 1].Size = tempSize;
                list[index - 1].Text = content.ToString();    
                
                Steplabels.Add(leftVal + " > " + content);
                index--; 
            }


            list[index].Text = content.ToString();

            Steplabels.Add($"{content} inserted correctly");
            sortingsteps.Add(new ListSnapshot(list));
        }

        list[0].BackColor = Color.Aquamarine;
        list[0].ForeColor = Color.Black;
        sortingsteps.Add(new ListSnapshot(list));
        Steplabels.Add("Done");
    }



    public static void BubbleSort(TextBox[] list, List<ListSnapshot> sortingsteps, List<string> StepLabels)
    {
        int temp;
        int currentval;
        int nextval;
        int length = list.Length;
        bool swapped = true;

        while (length > 0 && swapped)
        {
            swapped = false;
            length--;

            for (int i = 0; i < length; i++)
            {
                currentval = Convert.ToInt32(list[i].Text);
                nextval = Convert.ToInt32(list[i + 1].Text);

                StepLabels.Add("Comparing " + currentval + " and " + nextval);
                sortingsteps.Add(new ListSnapshot(list));

                list[i].ForeColor = Color.White;
                list[i + 1].ForeColor = Color.White;

                if (currentval > nextval)
                {
                    list[i].BackColor = Color.Blue;
                    list[i + 1].BackColor = Color.DarkRed;
                    StepLabels.Add(currentval + " > " + nextval);
                }
                else
                {
                    list[i].BackColor = Color.DarkRed;
                    list[i + 1].BackColor = Color.Blue;
                    StepLabels.Add(currentval + " <= " + nextval);
                }
                sortingsteps.Add(new ListSnapshot(list));

                if (currentval > nextval)
                {
                    temp = nextval;
                    nextval = currentval;
                    currentval = temp;

                    list[i].Text = currentval.ToString();
                    list[i + 1].Text = nextval.ToString();

                    Size tempsize = list[i + 1].Size;
                    list[i + 1].Size = list[i].Size;
                    list[i].Size = tempsize;

                    list[i].BackColor = Color.Black;
                    list[i + 1].BackColor = Color.Black;
                    list[i].ForeColor = Color.Turquoise;
                    list[i + 1].ForeColor = Color.Turquoise;

                    StepLabels.Add("Swapped " + list[i + 1].Text + " and " + list[i].Text);
                    sortingsteps.Add(new ListSnapshot(list));

                    swapped = true;
                }
                else
                {
                    list[i].BackColor = Color.Black;
                    list[i + 1].BackColor = Color.Black;
                    list[i].ForeColor = Color.Turquoise;
                    list[i + 1].ForeColor = Color.Turquoise;

                    StepLabels.Add("No swap needed");
                    sortingsteps.Add(new ListSnapshot(list));
                }
            }
        }

        sortingsteps.Add(new ListSnapshot(list));
        StepLabels.Add("Sorting done");
    }


    public static async Task BubbleSortAuto(TextBox[] list, PlayBack pausebtn, int speed)
    {
        int length = list.Length;
        bool swapped = true;

        while (length > 0)
        {
            swapped = false;
            length--;

            for (int i = 0; i < length; i++)
            {
                await pausebtn.WaitIfPaused();

                int currentVal = Convert.ToInt32(list[i].Text);
                int nextVal = Convert.ToInt32(list[i + 1].Text);

                if (currentVal < nextVal)
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
                list[i + 1].ForeColor = Color.White;

                await Task.Delay(speed);
                await pausebtn.WaitIfPaused();

                if (currentVal > nextVal)
                {
                    string temp = list[i].Text;
                    list[i].Text = list[i + 1].Text;
                    list[i + 1].Text = temp;

                    Size tempsize = list[i + 1].Size;
                    list[i + 1].Size = list[i].Size;
                    list[i].Size = tempsize;

                    swapped = true;
                    await Task.Delay(speed);
                    await pausebtn.WaitIfPaused();
                }

                list[i].BackColor = Color.Black;
                list[i + 1].BackColor = Color.Black;
                list[i].ForeColor = Color.Turquoise;
                list[i + 1].ForeColor = Color.Turquoise;
                await Task.Delay(speed);
                await pausebtn.WaitIfPaused();
            }
        }
    }


    public static void BinarySearch(TextBox[] list, int numtofind, List<ListSnapshot> steps, List<string> StepLabels)
    {
        int left = 0;
        int right = list.Length - 1;
        bool found = false;

        steps.Add(new ListSnapshot(list));
        StepLabels.Add("Initial state");

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

            list[left].BackColor = Color.Crimson;
            list[left].ForeColor = Color.Black;

            list[right].BackColor = Color.Blue;
            list[right].ForeColor = Color.Black;

            steps.Add(new ListSnapshot(list));
            StepLabels.Add("L: " + list[left].Text + "  M: " + list[mid].Text + "  R: " + list[right].Text);

            int midVal = Convert.ToInt32(list[mid].Text);

            if (midVal < numtofind)
            {
                steps.Add(new ListSnapshot(list));
                StepLabels.Add(midVal + " < " + numtofind);
                left = mid + 1;
            }
            else if (midVal > numtofind)
            {
                steps.Add(new ListSnapshot(list));
                StepLabels.Add(midVal + " > " + numtofind);
                right = mid - 1;
            }
            else
            {
                steps.Add(new ListSnapshot(list));
                StepLabels.Add("Found " + numtofind);
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


    public static void ExponentialSearch(TextBox[] list, int target, List<ListSnapshot> steps, List<string> StepLabels)
    {
        int item;
        int upperbound = 1;
        int lowerbound = 0;
        int lastupperbound = 0;

        while (upperbound < list.Length)
        {
            item = Convert.ToInt32(list[upperbound].Text);

            for (int i = 0; i <= upperbound; i++)
            {
                list[i].BackColor = Color.DarkBlue;
                list[i].ForeColor = Color.White;
            }

            if (item < target)
            {
                lastupperbound = upperbound;
                upperbound = upperbound * 2;
            }
            else
            {
                break;
            }

            steps.Add(new ListSnapshot(list));
            StepLabels.Add(target + " > " + item + ", so double Search Range");
        }

        lowerbound = lastupperbound + 1;

        if(upperbound >= list.Length)
        {
            upperbound = list.Length - 1;
        }

        for (int i = lowerbound; i <= upperbound; i++)
        {
            list[i].BackColor = Color.DarkBlue;
            list[i].ForeColor = Color.White;
        }

        steps.Add(new ListSnapshot(list));
        StepLabels.Add("Confirming range for binary search");

        int midVal;
        bool numfound = false;

        while (lowerbound <= upperbound)
        {
            for (int i = 0; i < lowerbound; i++)
            {
                list[i].BackColor = Color.Black;
                list[i].ForeColor = Color.Turquoise;
            }

            int mid = (lowerbound + upperbound) / 2;

            list[lowerbound].BackColor = Color.Crimson;
            list[lowerbound].ForeColor = Color.White;

            list[upperbound].BackColor = Color.Orchid;
            list[upperbound].ForeColor = Color.White;

            list[mid].BackColor = Color.Turquoise;
            list[mid].ForeColor = Color.Black;

            steps.Add(new ListSnapshot(list));
            StepLabels.Add("L: " + list[lowerbound].Text + "  M: " + list[mid].Text + "  R: " + list[upperbound].Text);

            midVal = Convert.ToInt32(list[mid].Text);

            if (midVal < target)
            {
                lowerbound = mid + 1;
                steps.Add(new ListSnapshot(list));
                StepLabels.Add(midVal + " < " + target);
            }
            else if (midVal > target)
            {
                upperbound = mid - 1;
                steps.Add(new ListSnapshot(list));
                StepLabels.Add(midVal + " > " + target);
            }
            else
            {

                steps.Add(new ListSnapshot(list));
                StepLabels.Add("Found" + target);
                MessageBox.Show(target + " found at index " + mid);
                numfound = true;
                break;
            }

            for (int i = 0; i < lowerbound; i++)
            {
                list[i].BackColor = Color.Black;
                list[i].ForeColor = Color.Turquoise;
            }
            for(int i = upperbound+1; i <list.Length; i++)
            {
                list[i].BackColor = Color.Black;
                list[i].ForeColor = Color.Turquoise;
            }
        }

        if (!numfound)
        {
            MessageBox.Show("Number not found in array");
        }
    }
}