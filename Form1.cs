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
using CheckBox = System.Windows.Forms.CheckBox;
using System.Numerics;



namespace AlgoView
{


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.Black;
        }

        private Button HomeButton = ButtonMaker.MakeNewButton("Back to Home", 265, 50);
        private void ClickHomeButton(object sender, EventArgs e)
        {
            foreach (Control controls in this.Controls)
            {
                if (controls.Text != "Back to Home" && !(controls is PictureBox) && !(controls is ComboBox && controls.Width == 360))
                {
                    controls.Hide();
                }
            }
            algorithmSelector.SelectedIndex = 0;
            algorithmSelector.Enabled = true;
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
            element.Show();
        }
        

        private TextBox[] GetCurrentTextBoxes()
        {
            return this.Controls.OfType<TextBox>()
                .Where(numbox => numbox.Width == 35 && numbox.Height == 35)
                .OrderBy(numbox => numbox.Left)
                .ToArray();
        }


        private List<ListSnapshot> AlgorithmSteps = new List<ListSnapshot>();

        private List<string> StepExplainations = new List<string>();
        private Label StepLabel = LabelMaker.MakeNewLabel("",600, 30);

        private int CurrentStep = -1;
        private TextBox[] CurrentBoxes;

        private Button StepBackButton;
        private Button StepForwardbutton;

        private Label StepCount = LabelMaker.MakeNewLabel("", 150, 30);
        void UpdateStepCount()
        {
            StepCount.Text = "Step: " + Convert.ToString(CurrentStep);
        }


        private PlayBack PauseControl = new PlayBack();    

        private void PauseButtonClick(object sender, EventArgs e)
        {
            PauseControl.Pause();
        }
        private void ResumeButtonClick(object sender, EventArgs e)
        {
            PauseControl.Resume();
        }
        

        private void StepBackClick(object sender, EventArgs e)
        {
            if (CurrentStep > 0)
            {
                CurrentStep--;
                AlgorithmSteps[CurrentStep].Restore(CurrentBoxes);
                StepForwardbutton.Enabled = true;
                StepBackButton.Enabled = CurrentStep > 0;
                UpdateStepCount();
                StepLabel.Text = StepExplainations[CurrentStep];
            }
        }


        private void StepForwardClick(object sender, EventArgs e)
        {
            if (CurrentStep < AlgorithmSteps.Count - 1)
            {
                CurrentStep++;
                AlgorithmSteps[CurrentStep].Restore(CurrentBoxes);
                StepBackButton.Enabled = true;
                StepForwardbutton.Enabled = CurrentStep < AlgorithmSteps.Count - 1;
                UpdateStepCount();
                StepLabel.Text = StepExplainations[CurrentStep];
            }
        }
        


        private const string SearchQuestion = "Enter the first number in the left box and the last in the right box: ";
        private const string SortQuestion = "Enter the first number in the left box and the last in the right box(sort): ";


        private void DrawList(TextBox[] boxlist, TextBox firstnum, TextBox lastnum)
        {
            int spacing = 38;

            for (int i = 0; i < boxlist.Length; i++)
            {
                if (int.TryParse(boxlist[i].Text, out int result))
                {
                    int x = (int)((i - (boxlist.Length - 1) / 2.0) * spacing);
                    PositionInListUI(boxlist[i], 400, x);

                    this.Resize += (s, e) =>
                    {
                        for (int j = 0; j < boxlist.Length; j++)
                        {
                            int newX = (int)((j - (boxlist.Length - 1) / 2.0) * spacing);
                            Center(boxlist[j], 400, newX);
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
        }

        public void NewStepStack(TextBox[] numbers)
        {
            StepCount.Show();
            AlgorithmSteps[CurrentStep].Restore(numbers);
            StepForwardbutton.Enabled = true;
            StepBackButton.Enabled = false;
            CurrentBoxes = numbers;
            StepForwardbutton.Show();
            StepBackButton.Show();
            StepLabel.Text = StepExplainations[0];
        }


        private void SetUpListUI(string inputquestion, string buttonname, Action<TextBox[]> onListCreated)
        {
            Label askuserinput = LabelMaker.MakeNewLabel(inputquestion, 600, 50);
            PositionInListUI(askuserinput, 400, 0);

            Panel labeloutline = PanelMaker.MakeNewPanel("", 610, 60);
            PositionInListUI(labeloutline, 395, 0);
            labeloutline.SendToBack();

            TextBox firstnum = BoxMaker.MakeNewBox("", 30);
            PositionInListUI(firstnum, 500, -50);

            TextBox lastnum = BoxMaker.MakeNewBox("", 30);
            PositionInListUI(lastnum, 500, 50);

            Button makelist = ButtonMaker.MakeNewButton(buttonname, 100, 50);
            PositionInListUI(makelist, 900, 0);

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
                    if(last - first > 49)
                    {
                        MessageBox.Show("Range of numbers is 50 - otherwise won't fit on screen");
                        lastnum.Clear();
                        firstnum.Clear();
                        return;
                    }

                    SearchListMaker numberlistmaker = new SearchListMaker();
                    TextBox[] boxlist = numberlistmaker.MakeList(firstnum.Text, lastnum.Text);

                    DrawList(boxlist, firstnum, lastnum);

                    askuserinput.Hide();
                    firstnum.Hide();
                    lastnum.Hide();
                    labeloutline.Hide();

                    Application.DoEvents();

                    this.BeginInvoke((MethodInvoker)(() =>
                    {
                        onListCreated(boxlist);
                    }));

                    makelist.Enabled = false;
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
                listType.Items.Add("List type?");
                listType.Items.Add("Randomised");
                listType.Items.Add("Reversed");
                listType.Items.Add("Random no repeats");
                listType.SelectedIndex = 0;
                PositionInListUI(listType, 750, 0);

                SortListMaker numberlistmaker = new SortListMaker();
                TextBox[] boxlist = Array.Empty<TextBox>();

                string listType_reverse = listType.SelectedItem.ToString();
                makelist.Enabled = false;

                listType.SelectedIndexChanged += (object sender, EventArgs e)=>
                {
                    listType_reverse = listType.SelectedItem.ToString();
                    if (listType_reverse == "Randomised")
                    {
                        makelist.Enabled = true;
                    }
                    else if (listType_reverse == "Reversed")
                    {
                        makelist.Enabled = true;
                    }
                    else if (listType_reverse == "Random no repeats")
                    {
                        makelist.Enabled = true;
                    }
                };

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
                    if (last - first > 49)
                    {
                        MessageBox.Show("Max range of numbers is 50 - otherwise won't fit on screen");
                        lastnum.Clear();
                        firstnum.Clear();
                        return;
                    }

                    foreach (TextBox box in boxlist)
                    {
                        this.Controls.Remove(box);
                        box.Dispose();
                    }

                    if (listType_reverse == "Reversed")
                    {
                        boxlist = numberlistmaker.MakeReverseList(firstnum.Text, lastnum.Text);
                    }
                    else if (listType_reverse == "Randomised")
                    {
                        boxlist = numberlistmaker.MakeRandomList(firstnum.Text, lastnum.Text);
                    }
                    else if(listType_reverse == "Random no repeats")
                    {
                        boxlist = numberlistmaker.RandomListNoRepeats(firstnum.Text, lastnum.Text);
                    }

                    listType.Enabled = false;

                    DrawList(boxlist, firstnum, lastnum);

                    askuserinput.Hide();
                    firstnum.Hide();
                    lastnum.Hide();
                    labeloutline.Hide();

                    Application.DoEvents();

                    this.BeginInvoke((MethodInvoker)(() =>
                    {
                        onListCreated(boxlist);
                    }));
                    makelist.Enabled = false;
                };
            }

            if (StepBackButton == null)
            {
                StepBackButton = ButtonMaker.MakeNewButton("Step back", 250, 50);
                PositionInListUI(StepBackButton, 750, -400);
                StepBackButton.Click += StepBackClick;
                StepBackButton.Hide(); 
            }
            else
            {
                StepBackButton.Hide();
            }

            if (StepForwardbutton == null)
            {
                StepForwardbutton = ButtonMaker.MakeNewButton("Step forward", 250, 50);
                PositionInListUI(StepForwardbutton, 750, 400);
                StepForwardbutton.Click += StepForwardClick;
                StepForwardbutton.Hide();
            }
            else
            {
                StepForwardbutton.Hide();
            }
        }


        private ComboBox algorithmSelector;

        private int sortspeed = 1;
        
        private void Form1_Load(object sender, EventArgs e)
        {
            PictureBox logo = new PictureBox();
            logo.Size = new Size(250,236);
            PositionInListUI(logo, 14,0);
            logo.Image = Image.FromFile("AlgoViewLogo.png");
            Label AppName = LabelMaker.MakeNewLabel("AlgoView", 275, 55);
            AppName.Font = new Font("OCR A Extended", 25, FontStyle.Bold);
            AppName.ForeColor = Color.SkyBlue;
            PositionInListUI(AppName, 910, -825);

            PositionInListUI(HomeButton, 15, -820);
            HomeButton.Click += ClickHomeButton;

            algorithmSelector = new ComboBox();
            algorithmSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            algorithmSelector.Size = new Size(360, 50);
            algorithmSelector.Font = new Font("OCR A Extended", 10, FontStyle.Regular);
            algorithmSelector.BackColor = Color.Black;
            algorithmSelector.ForeColor = Color.Turquoise;
            algorithmSelector.Items.Add("Select Algorithm");
            algorithmSelector.Items.Add("Bubble Sort");
            algorithmSelector.Items.Add("Insertion Sort");
            algorithmSelector.Items.Add("Merge sort");
            algorithmSelector.Items.Add("Binary Search");
            algorithmSelector.Items.Add("Exponential Search");
            PositionInListUI(algorithmSelector, 290, 0);


            algorithmSelector.SelectedIndexChanged += (s, e) =>
            {
                string selectedAlgorithm = algorithmSelector.SelectedItem.ToString();

                if (selectedAlgorithm == "Bubble Sort")
                {
                    CheckBox sortmode = CheckBoxMaker.MakeNewCheckBox("Auto-Sort");
                    PositionInListUI(sortmode, 700, 0);

                    algorithmSelector.Enabled = false;

                    Label speedlabel = LabelMaker.MakeNewLabel("Enter speed (1-10): ", 260, 30);
                    TextBox speedinput = BoxMaker.MakeNewBox("", 30);
                    PositionInListUI(speedlabel, 780, -20);
                    PositionInListUI(speedinput, 780, 130);
                    speedlabel.Visible = false;
                    speedinput.Visible = false;
                    int speed = 1;

                    sortmode.CheckedChanged += (sender, e) =>
                    {
                        speedlabel.Visible = sortmode.Checked;
                        speedinput.Visible = sortmode.Checked;
                    };

                    SetUpListUI(SortQuestion, "Enter", async (TextBox[] numbers) =>
                    {
                        if(sortmode.Checked == false)
                        {
                            StepForwardbutton.Show();
                            StepBackButton.Show();
                            AlgorithmSteps.Clear();
                            StepExplainations.Clear();
                            CurrentStep = 0;
                            ListMethods.BubbleSort(numbers, AlgorithmSteps, StepExplainations);
                            PositionInListUI(StepCount, 325, 0);
                            PositionInListUI(StepLabel, 300, 400);

                            if (AlgorithmSteps.Count > 0)
                            {
                                NewStepStack(numbers);
                            }
                            else
                            {
                                MessageBox.Show("No steps were generated.");
                            }
                        }
                        else
                        {
                            sortmode.Enabled = false;

                            Button pausebutton = ButtonMaker.MakeNewButton("||", 150, 40);
                            PositionInListUI(pausebutton, 357,0);
                            Button resumebutton = ButtonMaker.MakeNewButton("▶︎", 150, 40);
                            PositionInListUI(resumebutton, 357,0);

                            speedlabel.Show();
                            speedinput.Show();

                            pausebutton.Click += (object sender, EventArgs e) =>
                            {
                                PauseControl.Pause();
                                pausebutton.Enabled = false;
                                pausebutton.Hide();
                                resumebutton.Enabled = true;
                                resumebutton.Show();
                            }; 
                            resumebutton.Click += (object sender, EventArgs e) =>
                            {
                                PauseControl.Resume();
                                resumebutton.Enabled = false;
                                resumebutton.Hide();
                                pausebutton.Enabled = true;
                                pausebutton.Show();
                            };


                            if(!int.TryParse(speedinput.Text, out int result) || result < 1 || result > 10)
                            {
                                MessageBox.Show("Valid integer between 1 and 10 not inputted, executing with default speed 1");
                                await ListMethods.BubbleSortAuto(numbers, PauseControl, 50);
                            }
                            else
                            {
                                speed = 50 * result;
                                await ListMethods.BubbleSortAuto(numbers, PauseControl, speed);
                            }
                        }

                        sortmode.Enabled = false;
                        sortmode.Visible = false;
                    });
                }

                else if (selectedAlgorithm == "Binary Search")
                {
                    algorithmSelector.Enabled = false;

                    Label numtofind = LabelMaker.MakeNewLabel("input number to search for:", 385, 30);
                    PositionInListUI(numtofind, 750, 0);

                    TextBox input = BoxMaker.MakeNewBox("", 30);
                    PositionInListUI(input, 750, 210);

                    Label left = LabelMaker.MakeNewLabel("Left", 90, 30);
                    PositionInListUI(left, 675, -150);
                    left.BackColor = Color.Crimson;
                    left.ForeColor = Color.Black;

                    Label right = LabelMaker.MakeNewLabel("Right", 90, 30);
                    PositionInListUI(right, 675, 150);
                    right.BackColor = Color.Blue;
                    right.ForeColor = Color.Black;

                    Label mid = LabelMaker.MakeNewLabel("Middle", 90, 30);
                    PositionInListUI(mid, 675, 0);
                    mid.BackColor = Color.Turquoise;
                    mid.ForeColor = Color.Black;

                    SetUpListUI(SearchQuestion, "Enter", (TextBox[] numbers) =>
                    {
                        if(int.TryParse(input.Text, out int result))
                        {
                            numtofind.Show();
                            AlgorithmSteps.Clear();
                            StepExplainations.Clear();
                            CurrentStep = 0;
                            ListMethods.BinarySearch(numbers, Convert.ToInt32(input.Text), AlgorithmSteps, StepExplainations);
                            PositionInListUI(StepLabel, 300, 400);
                            PositionInListUI(StepCount, 325, 0);
                            StepCount.Hide();

                            if (AlgorithmSteps.Count > 0)
                            {
                                NewStepStack(numbers);
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

                            StepBackButton.Hide();
                            StepForwardbutton.Hide();
                            StepCount.Hide();
                            input.Clear();
                        }
                    });
                }
                else if (selectedAlgorithm == "Insertion Sort")
                {
                    algorithmSelector.Enabled = false;

                    SetUpListUI(SortQuestion, "Enter", (TextBox[] numbers) =>
                    {
                        StepForwardbutton.Show();
                        StepBackButton.Show();
                        AlgorithmSteps.Clear();
                        StepExplainations.Clear();
                        ListMethods.InsertionSort(numbers, AlgorithmSteps, StepExplainations);
                        PositionInListUI(StepCount, 325, 0);
                        PositionInListUI(StepLabel, 300, 400);

                        CurrentStep = 0;

                        if (AlgorithmSteps.Count > 0)
                        {
                            NewStepStack(numbers);
                        }
                        else
                        {
                            MessageBox.Show("No steps were generated.");
                        }
                    });
                }
                else if (selectedAlgorithm == "Exponential Search")
                {
                    algorithmSelector.Enabled = false;

                    Label numtofind = LabelMaker.MakeNewLabel("input number to search for:", 385, 30);
                    PositionInListUI(numtofind, 750, 0);

                    TextBox input = BoxMaker.MakeNewBox("", 30);
                    PositionInListUI(input, 750, 210);

                    Label left = LabelMaker.MakeNewLabel("Lower Bound", 225, 30);
                    PositionInListUI(left, 675, -300);
                    left.BackColor = Color.Crimson;
                    left.ForeColor = Color.Black;

                    Label right = LabelMaker.MakeNewLabel("Upper Bound", 225, 30);
                    PositionInListUI(right, 675, 300);
                    right.BackColor = Color.Blue;
                    right.ForeColor = Color.Black;

                    Label mid = LabelMaker.MakeNewLabel("Target", 110, 30);
                    PositionInListUI(mid, 675, 0);
                    mid.BackColor = Color.Turquoise;
                    mid.ForeColor = Color.Black;

                    SetUpListUI(SearchQuestion, "Enter", (TextBox[] numbers) =>
                    {
                        if (int.TryParse(input.Text, out int result))
                        {
                            numtofind.Show();
                            AlgorithmSteps.Clear();
                            CurrentStep = 0;
                            StepExplainations.Clear();
                            ListMethods.ExponentialSearch(numbers, Convert.ToInt32(input.Text), AlgorithmSteps, StepExplainations);
                            PositionInListUI(StepCount, 325, 0);
                            PositionInListUI(StepLabel, 300, 400);
                            StepCount.Hide();

                            if (AlgorithmSteps.Count > 0)
                            {
                                NewStepStack(numbers);
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

                            StepBackButton.Hide();
                            StepForwardbutton.Hide();
                            StepCount.Hide();
                            input.Clear();
                        }
                    });
                }
                else if (selectedAlgorithm == "Merge sort")
                {
                    algorithmSelector.Enabled = false; 
                    
                    CheckBox sortmode = CheckBoxMaker.MakeNewCheckBox("Auto-Sort");
                    PositionInListUI(sortmode, 700, 0);

                    TextBox speedinput = BoxMaker.MakeNewBox("", 30);
                    Label speedlabel = LabelMaker.MakeNewLabel("Enter speed (1-10): ", 260, 30);
                    PositionInListUI(speedlabel, 780, -20);
                    PositionInListUI(speedinput, 780, 130);
                    int speed = 1;

                    SetUpListUI(SortQuestion, "Enter", async (TextBox[] numbers) =>
                    {
                        if (!sortmode.Checked)
                        {
                            StepForwardbutton.Show();
                            StepBackButton.Show();
                            AlgorithmSteps.Clear();
                            StepExplainations.Clear();
                            ListMethods.MergeSort(numbers, AlgorithmSteps, StepExplainations);
                            PositionInListUI(StepCount, 325, 0);
                            PositionInListUI(StepLabel, 300, 400);

                            CurrentStep = 0;

                            if (AlgorithmSteps.Count > 0)
                            {
                                NewStepStack(numbers);
                            }
                            else
                            {
                                MessageBox.Show("No steps were generated.");
                            }
                        }
                        else
                        {
                            if (int.TryParse(speedinput.Text, out int result))
                            {
                                speed = 50 * result;
                                await ListMethods.MergeSortAuto(numbers, speed);
                            }
                        }
                    });
                }
                else if (selectedAlgorithm == "Depth first search")
                {
                    algorithmSelector.Enabled = false;
                    SetUpListUI("Enter the first number in the left box and the last in the right box: ", "Enter", (TextBox[] numbers) =>
                    {

                    });
                }
                else if (selectedAlgorithm == "Breadth first search")
                {
                    algorithmSelector.Enabled = false;
                    SetUpListUI("Enter the first number in the left box and the last in the right box: ", "Enter", (TextBox[] numbers) =>
                    {
                        
                    });
                }
            };
        }
    }
}