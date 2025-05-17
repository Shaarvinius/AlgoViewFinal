using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml.Linq;

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
            algorithmSelector.Items.Add("Tree Traversal");
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
                    SetUpListUI("Enter a list of numbers separated by spaces: ","Enter", (TextBox[] numbers) =>
                    {
                        
                    });
                }
                else if (selectedAlgorithm == "Insertion Sort")
                {
                    SetUpListUI("Enter a list of numbers separated by spaces: ", "Enter", (TextBox[] numbers) =>
                    {
                        
                    });
                }
                else if (selectedAlgorithm == "Tree Traversal")
                {

                }
                else if (selectedAlgorithm == "Merge sort")
                {
                    SetUpListUI("Enter a list of numbers separated by spaces: ", "Enter", (TextBox[] numbers) =>
                    {
                        
                    });
                }
            };

        }

        private void SetUpListUI(string inputquestion,string buttonname, Action<TextBox[]> onListCreated)
        {
            Button userinput = ButtonMaker.MakeNewButton(inputquestion, 600, 50);
            //userinput.Enabled = false;
            PositionInListUI(userinput, 444, 0);

            TextBox numlist = BoxMaker.MakeNewBox("", 600);
            PositionInListUI(numlist, 550, 0);

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
            };
        }

        private void PositionInListUI(Control element,int topoffset, int midoffset)
        {
            this.Controls.Add(element);
            Center(element, topoffset, midoffset);
            this.Resize += (s, e) => Center(element, topoffset, midoffset);
        }

        private void swapsort(TextBox a, TextBox b)
        {
            int b1 = Convert.ToInt32(a.Text);
            int b2 = Convert.ToInt32(b.Text);

            int tempnum = 0;

            if (b1 > b2)
            {
                tempnum = b2;
                b2 = b1;
                b1 = tempnum;

                a.Text = Convert.ToString(b1);
                b.Text = Convert.ToString(b2);
            }
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
                list[i].Multiline = true;
                list[i].Size = new Size(50,50);
                list[i].TextAlign = HorizontalAlignment.Center;
                list[i].Text = input[i];
                list[i].ForeColor = Color.Turquoise;
                list[i].BackColor = Color.Black;
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

    public static class LabelMaker 
    {
        public static Label MakeNewLabel(string labelname, int width)
        {
            Label label = new Label();
            label.Size = new Size(width, 25);
            label.Text = labelname;
            label.TextAlign = ContentAlignment.MiddleCenter;
            return label;
        }
    }
}
