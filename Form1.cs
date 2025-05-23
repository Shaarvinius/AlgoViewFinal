using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Threading;    
using System.Timers;   
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
namespace AlgoView
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.Black;
        }


        public void Center(Control ctrl, int topoffset, int midoffset)
        {
            ctrl.Left = ((this.ClientSize.Width - ctrl.Width) / 2) + midoffset;
            ctrl.Top = topoffset;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            PictureBox logo = new PictureBox();
            logo.Size = new Size(610,245);
            PositionInListUI(logo, 39,0);
            logo.Image = Image.FromFile("AlgoViewLogo.png");
            this.Controls.Add(logo);

            ComboBox algorithmSelector = new ComboBox();
            algorithmSelector.DropDownStyle = ComboBoxStyle.DropDown;
            algorithmSelector.Size = new Size(360, 50);
            algorithmSelector.Items.Add("Select Algorithm");
            algorithmSelector.Items.Add("Bubble Sort");
            algorithmSelector.Items.Add("Insertion Sort");
            algorithmSelector.Items.Add("Binary Search");
            algorithmSelector.Items.Add("Exponential Search");
            algorithmSelector.Items.Add("Breadth first search");
            algorithmSelector.Items.Add("Depth first search");
            algorithmSelector.Items.Add("Merge sort");
            algorithmSelector.SelectedIndex = 0;
            
            this.Controls.Add(algorithmSelector);

            Center(algorithmSelector, 333, 0);
            this.Resize += (s, e) => Center(algorithmSelector, 333, 0);

            algorithmSelector.SelectedIndexChanged += (s, e) =>
            {
                string selectedAlgorithm = algorithmSelector.SelectedItem.ToString();

                if (selectedAlgorithm == "Bubble Sort")
                {
                    SetUpListUI("Enter a list of numbers separated by spaces: ", "Enter", (TextBox[] numbers) =>
                    {
                        
                    });
                }
                else if (selectedAlgorithm == "Binary Search")
                {
                    Label numtofind = LabelMaker.MakeNewLabel("input number to search for", 200, 30);
                    PositionInListUI(numtofind, 625, 0);

                    TextBox input = BoxMaker.MakeNewBox("", 30);
                    PositionInListUI(input, 690, 0);

                    Label left = LabelMaker.MakeNewLabel("Left", 70, 30);
                    PositionInListUI(left, 825, -75);
                    left.BackColor = Color.LimeGreen;
                    left.ForeColor = Color.Black;

                    Label right = LabelMaker.MakeNewLabel("Right", 70, 30);
                    PositionInListUI(right, 825, 75);
                    right.BackColor = Color.Blue;
                    right.ForeColor = Color.Black;

                    Label mid = LabelMaker.MakeNewLabel("Middle", 70, 30);
                    PositionInListUI(mid, 825, 0);
                    mid.BackColor = Color.Turquoise;
                    mid.ForeColor = Color.Black;

                    SetUpListUI("Enter the first number in the left box and the last in the right box: ", "Enter", (TextBox[] numbers) =>
                    {
                        if (!int.TryParse(input.Text, out int result) && input.Text != "")
                        {
                            MessageBox.Show("Invalid character entered");
                            input.Clear();
                        }
                        else
                        {
                            ListMethods.BinarySearch(numbers, Convert.ToInt32(input.Text));
                        }
                    });
                }
                else if (selectedAlgorithm == "Insertion Sort")
                {
                    SetUpListUI("Enter the first number in the left box and the last in the right box: ", "Enter", (TextBox[] numbers) =>
                    {

                    });
                }
                else if (selectedAlgorithm == "Exponential Search")
                {
                    SetUpListUI("Enter the first number in the left box and the last in the right box: ", "Enter", (TextBox[] numbers) =>
                    {

                    });
                }
                else if (selectedAlgorithm == "Merge sort")
                {
                    SetUpListUI("Enter the first number in the left box and the last in the right box: ", "Enter", (TextBox[] numbers) =>
                    {

                    });
                }
                else if (selectedAlgorithm == "Depth first search")
                {
                    SetUpListUI("Enter the first number in the left box and the last in the right box: ", "Enter", (TextBox[] numbers) =>
                    {

                    });
                }
                else if (selectedAlgorithm == "Breadth first search")
                {
                    SetUpListUI("Enter the first number in the left box and the last in the right box: ", "Enter", (TextBox[] numbers) =>
                    {

                    });
                }
            };
        }


        private void SetUpListUI(string inputquestion,string buttonname, Action<TextBox[]> onListCreated)
        {
            Label userinput = LabelMaker.MakeNewLabel(inputquestion, 600, 50);
            PositionInListUI(userinput, 444, 0);

            Panel labeloutline = PanelMaker.MakeNewPanel("", 610, 60);
            PositionInListUI(labeloutline, 439, 0);
            labeloutline.SendToBack();

            TextBox firstnum = BoxMaker.MakeNewBox("", 30);
            PositionInListUI(firstnum, 550, -50);

            TextBox lastnum = BoxMaker.MakeNewBox("", 30);
            PositionInListUI(lastnum, 550, 50);

            Button makelist = ButtonMaker.MakeNewButton(buttonname, 100, 50);
            PositionInListUI(makelist, 750, 0);

            makelist.Click += (sender, args) =>
            {
                if (!int.TryParse(firstnum.Text, out int first) || !int.TryParse(lastnum.Text, out int last))
                {
                    MessageBox.Show("Invalid character inputted. Please enter integers.");
                    firstnum.Clear();
                    lastnum.Clear();
                    return;
                }
                if (first >= last)
                {
                    MessageBox.Show("First number should be less than the last.");
                    firstnum.Clear();
                    lastnum.Clear();
                    return;
                }

                ListMaker maker = new ListMaker();
                TextBox[] boxlist = maker.MakeList(firstnum.Text, lastnum.Text);

                for (int i = 0; i < boxlist.Length; i++)
                {
                    if (int.TryParse(boxlist[i].Text, out int result))
                    {
                        PositionInListUI(boxlist[i], 553, (i * 100 - 100 * (boxlist.Length / 2)));

                        this.Resize += (s, e) =>
                        {
                            for (int j = 0; j < boxlist.Length; j++)
                            {
                                Center(boxlist[j], 553, (j * 100 - 100 * (boxlist.Length / 2)));
                            }
                        };
                    }
                    else
                    {
                        MessageBox.Show("Invalid Character entered. Please enter only integers.");
                        firstnum.Clear();
                        lastnum.Clear();
                        break;
                    }
                }

                userinput.Hide();
                firstnum.Hide();
                lastnum.Hide();
                labeloutline.Hide();

                Application.DoEvents();

                this.BeginInvoke((MethodInvoker)(() =>
                {
                    onListCreated(boxlist);
                }));
            };
        }


        private void PositionInListUI(Control element,int topoffset, int midoffset)
        {
            this.Controls.Add(element);
            Center(element, topoffset, midoffset);
            this.Resize += (s, e) => Center(element, topoffset, midoffset);
        }
    }



    public class ListMaker
    {
        public TextBox[] MakeList(string firstnum, string lastnum)
        {
            int length = Convert.ToInt32(lastnum) - Convert.ToInt32(firstnum) + 1;
            TextBox[] list = new TextBox[length];

            for (int i = 0; i < length; i++)
            {
                list[i] = new TextBox();
                list[i].Size = new Size(50, 50);
                list[i].Text = Convert.ToString(Convert.ToInt32(firstnum) + i);
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
            label.ForeColor = Color.Turquoise;
            label.TextAlign = ContentAlignment.MiddleCenter;

            return label;
        }
    }


    public class ListMethods()
    {
        public static void BinarySearch(TextBox[] list, int numtofind) 
        {
            int left = 0;
            int right = list.Length - 1;
            bool found = false;
            int step = 0;

            while(left <= right)
            {
                step++;
                int mid = (left + right) / 2;
                Thread.Sleep(1000);

                list[mid].BackColor = Color.Turquoise;
                list[mid].ForeColor = Color.Black;

                list[left].BackColor = Color.LimeGreen;
                list[left].ForeColor = Color.Black;

                list[right].BackColor = Color.Blue;
                list[right].ForeColor = Color.Black;

                Application.DoEvents();

                if (Convert.ToInt32(list[mid].Text) < numtofind)
                {
                    list[left].BackColor = Color.Black;
                    list[left].ForeColor = Color.Turquoise;

                    left = mid+1;

                    list[mid].BackColor = Color.Black;
                    list[mid].ForeColor = Color.Turquoise;
                }
                else if(Convert.ToInt32(list[mid].Text) > numtofind)
                {
                    list[right].BackColor = Color.Black;
                    list[right].ForeColor = Color.Turquoise;

                    right = mid-1;

                    list[mid].BackColor = Color.Black;
                    list[mid].ForeColor = Color.Turquoise;
                }
                else if(Convert.ToInt32(list[mid].Text) == numtofind)
                {
                    Thread.Sleep(1500);
                    MessageBox.Show(numtofind + " found at index " + mid);
                    found = true;
                    break;
                }
            }


            if(!found)
            {
                Thread.Sleep(1500);
                MessageBox.Show("Number not found in array");
            }
        }
    }
}
