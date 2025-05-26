using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Threading;    
using System.Timers;   
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Serialization;



namespace AlgoView
{


    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.Black;
        }

        private Button home = ButtonMaker.MakeNewButton("Back to Home", 265, 50);
        private void ClickHomeButton(object sender, EventArgs e)
        {
            foreach (Control controls in this.Controls)
            {
                if (controls.Text != "Back to Home" && !(controls is PictureBox) && !(controls is ComboBox && controls.Width == 360))
                {
                    controls.Hide();
                }
            }
        }

        public void Center(Control ctrl, int topoffset, int midoffset)
        {
            ctrl.Left = ((this.ClientSize.Width - ctrl.Width) / 2) + midoffset;
            ctrl.Top = topoffset;
        }


        private void PositionInListUI(Control element, int topoffset, int midoffset)
        {
            this.Controls.Add(element);
            Center(element, topoffset, midoffset);
            this.Resize += (s, e) => Center(element, topoffset, midoffset);
        }

        


        private TextBox[] GetCurrentTextBoxes()
        {
            return this.Controls.OfType<TextBox>()
                .Where(numbox => numbox.Width == 35 && numbox.Height == 35)
                .OrderBy(numbox => numbox.Left)
                .ToArray();
        }


        private List<ListSnapshot> AlgorithmSteps = new List<ListSnapshot>();
        private int currentStep = -1;
        private TextBox[] currentBoxes;

        private Button stepbackbutton;
        private Button stepforwardbutton;

        private Label stepcount = LabelMaker.MakeNewLabel("", 150, 30);
        void UpdateStepCount()
        {
            stepcount.Text = "Step: " + Convert.ToString(currentStep);
        }
        private void StepBackClick(object sender, EventArgs e)
        {
            if (currentStep > 0)
            {
                currentStep--;
                AlgorithmSteps[currentStep].Restore(currentBoxes);
                stepforwardbutton.Enabled = true;
                stepbackbutton.Enabled = currentStep > 0;
                UpdateStepCount();
            }
        }


        private void StepForwardClick(object sender, EventArgs e)
        {
            if (currentStep < AlgorithmSteps.Count - 1)
            {
                currentStep++;
                AlgorithmSteps[currentStep].Restore(currentBoxes);
                stepbackbutton.Enabled = true;
                stepforwardbutton.Enabled = currentStep < AlgorithmSteps.Count - 1;
                UpdateStepCount();
            }
        }


        private const string SearchQuestion = "Enter the first number in the left box and the last in the right box: ";
        private const string SortQuestion = "Enter the first number in the left box and the last in the right box(sort): ";

        private void SetUpListUI(string inputquestion, string buttonname, Action<TextBox[]> onListCreated)
        {
            Label askuserinput = LabelMaker.MakeNewLabel(inputquestion, 600, 50);
            PositionInListUI(askuserinput, 444, 0);

            Panel labeloutline = PanelMaker.MakeNewPanel("", 610, 60);
            PositionInListUI(labeloutline, 439, 0);
            labeloutline.SendToBack();

            TextBox firstnum = BoxMaker.MakeNewBox("", 30);
            PositionInListUI(firstnum, 550, -50);

            TextBox lastnum = BoxMaker.MakeNewBox("", 30);
            PositionInListUI(lastnum, 550, 50);

            Button makelist = ButtonMaker.MakeNewButton(buttonname, 100, 50);
            PositionInListUI(makelist, 750, 0);

            Application.DoEvents();

            if (inputquestion == SearchQuestion)
            {
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

                    SearchListMaker numberlistmaker = new SearchListMaker();
                    TextBox[] boxlist = numberlistmaker.MakeList(firstnum.Text, lastnum.Text);

                    int spacing = 50;

                    for (int i = 0; i < boxlist.Length; i++)
                    {
                        if (int.TryParse(boxlist[i].Text, out int result))
                        {
                            int x = (int)((i - (boxlist.Length - 1) / 2.0) * spacing);
                            PositionInListUI(boxlist[i], 553, x);

                            this.Resize += (s, e) =>
                            {
                                for (int j = 0; j < boxlist.Length; j++)
                                {
                                    int newX = (int)((j - (boxlist.Length - 1) / 2.0) * spacing);
                                    Center(boxlist[j], 553, newX);
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

                    askuserinput.Hide();
                    firstnum.Hide();
                    lastnum.Hide();
                    labeloutline.Hide();

                    Application.DoEvents();

                    if (stepbackbutton == null)
                    {
                        stepbackbutton = ButtonMaker.MakeNewButton("Step back", 250, 50);
                        PositionInListUI(stepbackbutton, 625, -400);
                        stepbackbutton.Click += StepBackClick;
                    }
                    else
                    {
                        stepbackbutton.Show();
                    }

                    if (stepforwardbutton == null)
                    {
                        stepforwardbutton = ButtonMaker.MakeNewButton("Step forward", 250, 50);
                        PositionInListUI(stepforwardbutton, 625, 400);
                        stepforwardbutton.Click += StepForwardClick;
                    }
                    else
                    {
                        stepforwardbutton.Show();
                    }

                    Application.DoEvents();

                    this.BeginInvoke((MethodInvoker)(() =>
                    {
                        onListCreated(boxlist);
                    }));
                };
            }
            else if(inputquestion == SortQuestion)
            {
                ComboBox listType= new ComboBox();
                listType.DropDownStyle = ComboBoxStyle.DropDownList;
                listType.Size = new Size(375, 50);
                listType.Font = new Font("OCR A Extended", 10, FontStyle.Regular);
                listType.BackColor = Color.Black;
                listType.ForeColor = Color.Turquoise;
                listType.Items.Add("Completely reverse List?");
                listType.Items.Add("Yes");
                listType.Items.Add("No");
                listType.SelectedIndex = 0;
                PositionInListUI(listType, 700, 0);

                SortListMaker numberlistmaker = new SortListMaker();
                TextBox[] boxlist = Array.Empty<TextBox>();


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
                    
                    string listType_reverse = listType.SelectedItem.ToString();

                    foreach (TextBox box in boxlist)
                    {
                        this.Controls.Remove(box);
                        box.Dispose();
                    }

                    if (listType_reverse == "Yes")
                    {
                        boxlist = numberlistmaker.MakeReverseList(firstnum.Text, lastnum.Text);
                    }
                    else if (listType_reverse == "No")
                    {
                        boxlist = numberlistmaker.MakeNormalList(firstnum.Text, lastnum.Text);
                    }

                    int spacing = 50;

                    for (int i = 0; i < boxlist.Length; i++)
                    {
                        if (int.TryParse(boxlist[i].Text, out int result))
                        {
                            int x = (int)((i - (boxlist.Length - 1) / 2.0) * spacing);
                            PositionInListUI(boxlist[i], 553, x);

                            this.Resize += (s, e) =>
                            {
                                for (int j = 0; j < boxlist.Length; j++)
                                {
                                    int newX = (int)((j - (boxlist.Length - 1) / 2.0) * spacing);
                                    Center(boxlist[j], 553, newX);
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

                    askuserinput.Hide();
                    firstnum.Hide();
                    lastnum.Hide();
                    labeloutline.Hide();

                    Application.DoEvents();

                    if (stepbackbutton == null)
                    {
                        stepbackbutton = ButtonMaker.MakeNewButton("Step back", 250, 50);
                        PositionInListUI(stepbackbutton, 625, -400);
                        stepbackbutton.Click += StepBackClick;
                    }
                    else
                    {
                        stepbackbutton.Show();
                    }

                    if (stepforwardbutton == null)
                    {
                        stepforwardbutton = ButtonMaker.MakeNewButton("Step forward", 250, 50);
                        PositionInListUI(stepforwardbutton, 625, 400);
                        stepforwardbutton.Click += StepForwardClick;
                    }
                    else
                    {
                        stepforwardbutton.Show();
                    }

                    Application.DoEvents();

                    this.BeginInvoke((MethodInvoker)(() =>
                    {
                        onListCreated(boxlist);
                    }));
                };
            }
        }


        

        private void Form1_Load(object sender, EventArgs e)
        {
            PictureBox logo = new PictureBox();
            logo.Size = new Size(610,245);
            PositionInListUI(logo, 39,0);
            logo.Image = Image.FromFile("AlgoViewLogo.png");
            this.Controls.Add(logo);

            PositionInListUI(home, 15, -820);
            home.Click += ClickHomeButton;

            ComboBox algorithmSelector = new ComboBox();
            algorithmSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            algorithmSelector.Size = new Size(360, 50);
            algorithmSelector.Font = new Font("OCR A Extended", 10, FontStyle.Regular);
            algorithmSelector.BackColor = Color.Black;
            algorithmSelector.ForeColor = Color.Turquoise;
            algorithmSelector.Items.Add("Select Algorithm");
            algorithmSelector.Items.Add("Bubble Sort");
            algorithmSelector.Items.Add("Insertion Sort");
            algorithmSelector.Items.Add("Binary Search");
            algorithmSelector.Items.Add("Exponential Search");
            algorithmSelector.Items.Add("Breadth first search");
            algorithmSelector.Items.Add("Depth first search");
            algorithmSelector.Items.Add("Merge sort");
            algorithmSelector.SelectedIndex = 0;
            PositionInListUI(algorithmSelector, 333, 0);

            

            algorithmSelector.SelectedIndexChanged += (s, e) =>
            {
                string selectedAlgorithm = algorithmSelector.SelectedItem.ToString();

                if (selectedAlgorithm == "Bubble Sort")
                {
                    SetUpListUI(SortQuestion, "Enter", (TextBox[] numbers) =>
                    {
                        
                    });
                }
                else if (selectedAlgorithm == "Binary Search")
                {
                    Label numtofind = LabelMaker.MakeNewLabel("input number to search for", 385, 30);
                    PositionInListUI(numtofind, 625, 0);

                    TextBox input = BoxMaker.MakeNewBox("", 30);
                    PositionInListUI(input, 690, 0);

                    Label left = LabelMaker.MakeNewLabel("Left", 90, 30);
                    PositionInListUI(left, 825, -150);
                    left.BackColor = Color.LimeGreen;
                    left.ForeColor = Color.Black;

                    Label right = LabelMaker.MakeNewLabel("Right", 90, 30);
                    PositionInListUI(right, 825, 150);
                    right.BackColor = Color.Blue;
                    right.ForeColor = Color.Black;

                    Label mid = LabelMaker.MakeNewLabel("Middle", 90, 30);
                    PositionInListUI(mid, 825, 0);
                    mid.BackColor = Color.Turquoise;
                    mid.ForeColor = Color.Black;

                    SetUpListUI(SearchQuestion, "Enter", (TextBox[] numbers) =>
                    {
                        if(int.TryParse(input.Text, out int result))
                        {
                            numtofind.Show();
                            AlgorithmSteps.Clear();
                            ListMethods.BinarySearch(numbers, Convert.ToInt32(input.Text), AlgorithmSteps);
                            currentStep = 0;
                            PositionInListUI(stepcount, 500, 0);
                            stepcount.Hide();

                            if (AlgorithmSteps.Count > 0)
                            {
                                stepcount.Show();
                                AlgorithmSteps[currentStep].Restore(numbers);
                                stepforwardbutton.Enabled = true;
                                stepbackbutton.Enabled = false;
                                currentBoxes = numbers;
                            }
                            else
                            {
                                MessageBox.Show("No steps were generated.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid character entered");
                            foreach(TextBox numberbox in numbers)
                            {
                                numberbox.Hide();
                            }
                            stepbackbutton.Hide();
                            stepforwardbutton.Hide();
                            stepcount.Hide();
                            input.Clear();
                        }
                    });
                }
                else if (selectedAlgorithm == "Insertion Sort")
                {
                    SetUpListUI(SortQuestion, "Enter", (TextBox[] numbers) =>
                    {
                        /*if (int.TryParse(input.Text, out int result))
                        {
                            numtofind.Show();
                            AlgorithmSteps.Clear();
                            //ListMethods.BinarySearch(numbers, Convert.ToInt32(input.Text), steps);
                            currentStep = 0;
                            if (AlgorithmSteps.Count > 0)
                            {
                                AlgorithmSteps[currentStep].Restore(numbers);
                                stepforwardbutton.Enabled = true;
                                stepbackbutton.Enabled = false;
                                currentBoxes = numbers;
                            }
                            else
                            {
                                MessageBox.Show("No steps were generated.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid character entered");
                            foreach (TextBox numberbox in numbers)
                            {
                                numberbox.Hide();
                            }
                            stepbackbutton.Hide();
                            stepforwardbutton.Hide();
                            input.Clear();
                        }*/
                    });
                }
                else if (selectedAlgorithm == "Exponential Search")
                {
                    SetUpListUI(SearchQuestion, "Enter", (TextBox[] numbers) =>
                    {

                    });
                }
                else if (selectedAlgorithm == "Merge sort")
                {
                    SetUpListUI(SortQuestion, "Enter", (TextBox[] numbers) =>
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
    }
}
