using AlgoView;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


public class ListMethods // a class containing all the list based algorithms
{
    private Form1 form;

    public ListMethods(Form1 form)
    {
        this.form = form;
    }
    public static void MergeSort(TextBox[] numbers, List<string> Steplabels, Form1 form)
    {
        if (numbers == null || numbers.Length <= 1)
            return;

        // helper to deterministic color from seed
        Color GetColorForSeed(int seed)
        {
            Random r = new Random(seed);
            int R = r.Next(40, 220);
            int G = r.Next(40, 220);
            int B = r.Next(40, 220);
            return Color.FromArgb(R, G, B);
        }

        // helper to pick readable foreground based on luminance
        Color ForeFor(Color c)
        {
            double lum = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
            return lum < 160 ? Color.White : Color.Black;
        }

        // helper to set the same colour for a segment and snapshot
        void ColourRange(int left, int right, Color col, string label = "")
        {
            for (int i = left; i <= right; i++)
            {
                numbers[i].BackColor = col;
                numbers[i].ForeColor = ForeFor(col);
            }
            form.PushSnapshot(new ListSnapshot(numbers));
            if (!string.IsNullOrEmpty(label))
                Steplabels.Add(label);
        }

        void Merge(int left, int middle, int right)
        {
            int leftsize = middle - left + 1;
            int rightsize = right - middle;

            string[] leftnumbers = new string[leftsize];
            Size[] leftsizes = new Size[leftsize];
            string[] rightnumbers = new string[rightsize];
            Size[] rightsizes = new Size[rightsize];

            for (int i = 0; i < leftsize; i++)
            {
                leftnumbers[i] = numbers[left + i].Text;
                leftsizes[i] = numbers[left + i].Size;
            }

            for (int i = 0; i < rightsize; i++)
            {
                rightnumbers[i] = numbers[middle + 1 + i].Text;
                rightsizes[i] = numbers[middle + 1 + i].Size;
            }

            int li = 0, ri = 0, k = left;

            while (li < leftsize && ri < rightsize)
            {
                int leftVal = int.Parse(leftnumbers[li]);
                int rightVal = int.Parse(rightnumbers[ri]);

                form.PushSnapshot(new ListSnapshot(numbers));
                Steplabels.Add("Compare " + leftVal + " and " + rightVal);

                if (leftVal > rightVal)
                {
                    Steplabels.Add($"{leftVal} > {rightVal}");
                }
                else if (leftVal < rightVal)
                {
                    Steplabels.Add($"{leftVal} < {rightVal}");
                }
                else
                {
                    Steplabels.Add($"{leftVal} = {rightVal}");
                }

                form.PushSnapshot(new ListSnapshot(numbers));

                if (leftVal <= rightVal)
                {
                    if (numbers[k].Text != leftnumbers[li])
                    {
                        numbers[k].Text = leftnumbers[li];
                        numbers[k].Size = leftsizes[li];
                        form.PushSnapshot(new ListSnapshot(numbers));
                        Steplabels.Add("Copy " + leftVal + " to index " + k);
                    }
                    li++;
                }
                else
                {
                    if (numbers[k].Text != rightnumbers[ri])
                    {
                        numbers[k].Text = rightnumbers[ri];
                        numbers[k].Size = rightsizes[ri];
                        form.PushSnapshot(new ListSnapshot(numbers));
                        Steplabels.Add("Copy " + rightVal + " to index " + k);
                    }
                    ri++;
                }
                k++;
            }

            while (li < leftsize)
            {
                if (numbers[k].Text != leftnumbers[li])
                {
                    numbers[k].Text = leftnumbers[li];
                    numbers[k].Size = leftsizes[li];
                    form.PushSnapshot(new ListSnapshot(numbers));
                    Steplabels.Add("Copy " + leftnumbers[li] + " to index " + k);
                }
                li++;
                k++;
            }

            while (ri < rightsize)
            {
                if (numbers[k].Text != rightnumbers[ri])
                {
                    numbers[k].Text = rightnumbers[ri];
                    numbers[k].Size = rightsizes[ri];
                    form.PushSnapshot(new ListSnapshot(numbers));
                    Steplabels.Add("Copy " + rightnumbers[ri] + " to index " + k);
                }
                ri++;
                k++;
            }
        }

        void Sort(int left, int right, Color segColor)
        {
            ColourRange(left, right, segColor, "Segment " + left + "-" + right + " split");

            if (left < right)
            {
                int middle = (left + right) / 2;

                Color leftColor = GetColorForSeed(left * 10007 + middle);
                Color rightColor = GetColorForSeed((middle + 1) * 10009 + right);

                Sort(left, middle, leftColor);
                Sort(middle + 1, right, rightColor);

                Merge(left, middle, right);

                ColourRange(left, right, segColor, "Merged " + left + "-" + right);
            }
        }

        Color topColor = GetColorForSeed(numbers.Length * 7919);
        Sort(0, numbers.Length - 1, topColor);

        form.PushSnapshot(new ListSnapshot(numbers));
        Steplabels.Add("Done");
    }

    public static async Task MergeSortAuto(TextBox[] numbers, PlayBack pausectrl, int speed, TrackBar SpeedSlider)// automatic merge sort
    {
        if (numbers == null || numbers.Length <= 1)
            return;

        Color MakeColor(int seed)
        {
            Random r = new Random(seed);
            int R = r.Next(40, 220);
            int G = r.Next(40, 220);
            int B = r.Next(40, 220);
            return Color.FromArgb(R, G, B);
        }

        Color PickFore(Color c)
        {
            double lum = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
            return lum < 160 ? Color.White : Color.Black;
        }

        async Task ColourRange(int left, int right, Color col)
        {
            for (int i = left; i <= right; i++)
            {
                numbers[i].BackColor = col;
                numbers[i].ForeColor = PickFore(col);
            }

            speed = 500 / SpeedSlider.Value;
            await Task.Delay(speed);
            await pausectrl.WaitIfPaused();
        }

        async Task Merge(int left, int middle, int right)
        {
            int leftsize = middle - left + 1;
            int rightsize = right - middle;

            string[] leftnumbers = new string[leftsize];
            Size[] leftsizes = new Size[leftsize];
            string[] rightnumbers = new string[rightsize];
            Size[] rightsizes = new Size[rightsize];

            for (int i = 0; i < leftsize; i++)
            {
                leftnumbers[i] = numbers[left + i].Text;
                leftsizes[i] = numbers[left + i].Size;
            }

            for (int i = 0; i < rightsize; i++)
            {
                rightnumbers[i] = numbers[middle + 1 + i].Text;
                rightsizes[i] = numbers[middle + 1 + i].Size;
            }

            int li = 0, ri = 0, k = left;

            while (li < leftsize && ri < rightsize)
            {
                if (int.Parse(leftnumbers[li]) <= int.Parse(rightnumbers[ri]))
                {
                    numbers[k].Text = leftnumbers[li];
                    numbers[k].Size = leftsizes[li];
                    li++;
                }
                else
                {
                    numbers[k].Text = rightnumbers[ri];
                    numbers[k].Size = rightsizes[ri];
                    ri++;
                }

                speed = 500 / SpeedSlider.Value;
                await Task.Delay(speed);
                await pausectrl.WaitIfPaused();
                k++;
            }

            while (li < leftsize)
            {
                numbers[k].Text = leftnumbers[li];
                numbers[k].Size = leftsizes[li];
                li++;
                k++;

                speed = 500 / SpeedSlider.Value;
                await Task.Delay(speed);
                await pausectrl.WaitIfPaused();
            }

            while (ri < rightsize)
            {
                numbers[k].Text = rightnumbers[ri];
                numbers[k].Size = rightsizes[ri];
                ri++;
                k++;

                speed = 500 / SpeedSlider.Value;
                await Task.Delay(speed);
                await pausectrl.WaitIfPaused();
            }
        }

        async Task Sort(int left, int right, Color currentColor)
        {
            await ColourRange(left, right, currentColor);

            if (left < right)
            {
                int mid = (left + right) / 2;

                Color leftColor = MakeColor(left * 1237 + mid * 17);
                Color rightColor = MakeColor((mid + 1) * 199 + right * 31);

                await Sort(left, mid, leftColor);

                await Sort(mid + 1, right, rightColor);

                speed = 500 / SpeedSlider.Value;
                await Task.Delay(speed);
                await pausectrl.WaitIfPaused();
                await Merge(left, mid, right);

                await ColourRange(left, right, currentColor);
            }
        }

        Color rootColor = MakeColor(numbers.Length * 4001);
        await Sort(0, numbers.Length - 1, rootColor);
    }

    public static void InsertionSort(TextBox[] list,List<string> Steplabels, Form1 form) // insertion sort
    {
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

                form.PushSnapshot(new ListSnapshot(list));

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
            form.PushSnapshot(new ListSnapshot(list));
        }

        list[0].BackColor = Color.Aquamarine;
        list[0].ForeColor = Color.Black;
        form.PushSnapshot(new ListSnapshot(list));
        Steplabels.Add("Done");
    }

    public static async Task InsertionSortAuto(TextBox[] list, PlayBack pausectrl, int speed, TrackBar SpeedSlider) // automatic insertion sort
    {
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

                speed = 500 / SpeedSlider.Value;
                await Task.Delay(speed);
                await pausectrl.WaitIfPaused();


                list[index].BackColor = Color.Aquamarine;
                list[index].ForeColor = Color.Black;
                list[index].Text = leftVal.ToString();
                Size tempSize = list[index].Size;
                list[index].Size = list[index - 1].Size;
                list[index - 1].Size = tempSize;
                list[index - 1].Text = content.ToString();

                speed = 500 / SpeedSlider.Value;
                await Task.Delay(speed);
                await pausectrl.WaitIfPaused();

                index--;
            }

            list[index].Text = content.ToString();
        }

        list[0].BackColor = Color.Aquamarine;
        list[0].ForeColor = Color.Black;

        speed = 500 / SpeedSlider.Value;
        await Task.Delay(speed);
        await pausectrl.WaitIfPaused();
    }



    public static void BubbleSort(TextBox[] list, List<string> StepLabels, Form1 form) // bubble sort with step by step feature integrated
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
                form.PushSnapshot(new ListSnapshot(list));

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
                form.PushSnapshot(new ListSnapshot(list));

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
                    form.PushSnapshot(new ListSnapshot(list));

                    swapped = true;
                }
                else
                {
                    list[i].BackColor = Color.Black;
                    list[i + 1].BackColor = Color.Black;
                    list[i].ForeColor = Color.Turquoise;
                    list[i + 1].ForeColor = Color.Turquoise;

                    StepLabels.Add("No swap needed");
                    form.PushSnapshot(new ListSnapshot(list));
                }
            }
        }

        form.PushSnapshot(new ListSnapshot(list));
        StepLabels.Add("Sorting done");
    }


    public static async Task BubbleSortAuto(TextBox[] list, PlayBack pausectrl, int speed, TrackBar SpeedSlider) // automatic bubble sort
    {
        int length = list.Length;

        while (length > 0)
        {
            length--;

            for (int i = 0; i < length; i++)
            {
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

                speed = 500 / SpeedSlider.Value;
                await Task.Delay(speed);
                await pausectrl.WaitIfPaused();

                if (currentVal > nextVal)
                {
                    string temp = list[i].Text;
                    list[i].Text = list[i + 1].Text;
                    list[i + 1].Text = temp;

                    Size tempsize = list[i + 1].Size;
                    list[i + 1].Size = list[i].Size;
                    list[i].Size = tempsize;

                    speed = 500 / SpeedSlider.Value;
                    await Task.Delay(speed);
                    await pausectrl.WaitIfPaused();
                }

                list[i].BackColor = Color.Black;
                list[i + 1].BackColor = Color.Black;
                list[i].ForeColor = Color.Turquoise;
                list[i + 1].ForeColor = Color.Turquoise;
                speed = 500 / SpeedSlider.Value;
                await Task.Delay(speed);
                await pausectrl.WaitIfPaused();
            }
        }
    }


    public static void BinarySearch(TextBox[] list, int numtofind, List<string> StepLabels, Form1 form) // binary search
    {
        int left = 0;
        int right = list.Length - 1;
        bool found = false;

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

            form.PushSnapshot(new ListSnapshot(list));
            StepLabels.Add("L: " + list[left].Text + "  M: " + list[mid].Text + "  R: " + list[right].Text);

            int midVal = Convert.ToInt32(list[mid].Text);

            if (midVal < numtofind)
            {
                form.PushSnapshot(new ListSnapshot(list));
                StepLabels.Add(midVal + " < " + numtofind);
                left = mid + 1;
            }
            else if (midVal > numtofind)
            {
                form.PushSnapshot(new ListSnapshot(list));
                StepLabels.Add(midVal + " > " + numtofind);
                right = mid - 1;
            }
            else
            {
                form.PushSnapshot(new ListSnapshot(list));
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


    public static void ExponentialSearch(TextBox[] list, int target, List<string> StepLabels, Form1 form) // exponential search
    {
        int item;
        int upperbound = 1;
        int lowerbound = 0;
        int lastupperbound = 0;

        int first = Convert.ToInt32(list[0].Text);
        if (first == target)
        {
            list[0].BackColor = Color.Turquoise;
            list[0].ForeColor = Color.Black;
            form.PushSnapshot(new ListSnapshot(list));
            StepLabels.Add("Found " + target + " at index 0");
            MessageBox.Show(target + " found at index 0");
            return;
        }
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

            form.PushSnapshot(new ListSnapshot(list));
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

        form.PushSnapshot(new ListSnapshot(list));
        StepLabels.Add("Confirmed range for binary search");

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

            form.PushSnapshot(new ListSnapshot(list));
            StepLabels.Add("L: " + list[lowerbound].Text + "  M: " + list[mid].Text + "  R: " + list[upperbound].Text);

            midVal = Convert.ToInt32(list[mid].Text);

            if (midVal < target)
            {
                lowerbound = mid + 1;
                form.PushSnapshot(new ListSnapshot(list));
                StepLabels.Add(midVal + " < " + target);
            }
            else if (midVal > target)
            {
                upperbound = mid - 1;
                form.PushSnapshot(new ListSnapshot(list));
                StepLabels.Add(midVal + " > " + target);
            }
            else
            {
                form.PushSnapshot(new ListSnapshot(list));
                StepLabels.Add("Found " + target);
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