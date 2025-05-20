using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Threading;    
using System.Timers;   
using System.Diagnostics;
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
                    SetUpListUI("Enter a list of numbers separated by spaces: ", "Enter", (TextBox[] numbers) =>
                    {
                        ListMethods.BinarySearch(numbers, 16);
                    });
                }
                else if (selectedAlgorithm == "Insertion Sort")
                {
                    SetUpListUI("Enter a list of numbers separated by spaces: ", "Enter", (TextBox[] numbers) =>
                    {

                    });
                }
                else if (selectedAlgorithm == "Exponential Search")
                {
                    SetUpListUI("Enter a list of numbers separated by spaces: ", "Enter", (TextBox[] numbers) =>
                    {

                    });
                }
                else if (selectedAlgorithm == "Merge sort")
                {
                    SetUpListUI("Enter a list of numbers separated by spaces: ", "Enter", (TextBox[] numbers) =>
                    {

                    });
                }
                else if (selectedAlgorithm == "Depth first search")
                {
                    SetUpListUI("Enter a list of numbers separated by spaces: ", "Enter", (TextBox[] numbers) =>
                    {

                    });
                }
                else if (selectedAlgorithm == "Breadth first search")
                {
                    SetUpListUI("Enter a list of numbers separated by spaces: ", "Enter", (TextBox[] numbers) =>
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

            TextBox numlist = BoxMaker.MakeNewBox("", 600);
            PositionInListUI(numlist, 550, 0);

            Panel outline = PanelMaker.MakeNewPanel("", 610, 40);
            PositionInListUI(outline, 545, 0);
            outline.SendToBack();

            Button makelist = ButtonMaker.MakeNewButton(buttonname, 100, 50);
            PositionInListUI(makelist, 640, 0);
            

            makelist.Click += (sender, args) =>
            {
                string[] inputList = numlist.Text.Split(' ');
                ListMaker maker = new ListMaker();
                TextBox[] boxlist = maker.MakeList(inputList);

                for (int i = 0; i < inputList.Length; i++)
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
                        numlist.Clear();
                        break;
                    }
                }

                
                userinput.Hide();
                numlist.Hide();
                makelist.Hide();
                labeloutline.Hide();
                outline.Hide();

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
        public TextBox[] MakeList(string[] input)
        {
            TextBox[] list = new TextBox[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                list[i] = new TextBox();
                list[i].Size = new Size(50,50);
                list[i].Text = input[i];
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
            label.ForeColor = Color.White;
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
            while(left <= right)
            {
                int mid = (left + right) / 2;
                Thread.Sleep(1000);
                list[mid].BackColor = Color.Turquoise;
                list[mid].ForeColor = Color.Black;
                Application.DoEvents();
                if (Convert.ToInt32(list[mid].Text) < numtofind)
                {
                    left = mid+1;
                    list[mid].BackColor = Color.Black;
                    list[mid].ForeColor = Color.Turquoise;
                }
                else if(Convert.ToInt32(list[mid].Text) > numtofind)
                {
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
                ///Thread.Sleep(1000);
                
            }
            if(!found)
            {
                Thread.Sleep(1500);
                MessageBox.Show("Number not found in array");
            }
        }
    }
}
