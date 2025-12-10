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
using System.Drawing;



namespace AlgoView
{
    public partial class Form1
    {
        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.Black;
        }

        private Button HomeButton = ControlMaker.MakeNewButton("Back to Home", 265, 50);
        Label StartInstruction = ControlMaker.MakeNewLabel("Click white dropdown to select algorithm", 600, 30);
        Label BackInstruction = ControlMaker.MakeNewLabel("Click space or Back to Home to select another algorithm", 750, 30);
        
        // Subroutine to go back to start state to allow the selection of a new algorithm
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

        // Subroutines to align and position various controls
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

        // Subroutine used to hide or change textboxes in a list algorithm
        private TextBox[] GetCurrentTextBoxes()
        {
            return this.Controls.OfType<TextBox>()
                .Where(numbox => numbox.Width == 35 && numbox.Height == 35)
                .OrderBy(numbox => numbox.Left)
                .ToArray();
        }

        // All major fields are below

        private List<string> StepExplainations = new List<string>();
        private Label StepLabel = ControlMaker.MakeNewLabel("", 600, 30);

        private int CurrentStep = -1;
        private TextBox[] CurrentBoxes;

        private Button StepBackButton;
        private Button StepForwardbutton;

        private Label StepCount = ControlMaker.MakeNewLabel("", 150, 30);

        private SteppingStack StepBackStack = new SteppingStack();
        private SteppingStack StepForwardStack = new SteppingStack();


        // There are only 2 possible questions to be asked for list algorithms
        private const string SearchQuestion = "Enter the first number in the left box and the last in the right box: ";
        private const string SortQuestion = "Enter the first number in the left box and the last in the right box(sort): ";

        // Subroutines and fields used for the step backward / forward and pause/resume features for list algorithms
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

        public void PushSnapshot(ISnapshot snapshot)
        {
            StepBackStack.Push(snapshot);
            StepForwardStack.Clear();
            if (StepExplainations.Count < StepBackStack.Count)
                StepExplainations.Add("");
            CurrentStep = StepBackStack.Count - 1;
            UpdateStepCount();
        }

        private void StepBackClick(object sender, EventArgs e)
        {
            if (StepBackStack.Count > 1)
            {
                ISnapshot current = StepBackStack.Pop();
                StepForwardStack.Push(current);
                StepBackStack.Peek().Restore();
            }


            StepBackButton.Enabled = StepBackStack.Count > 1;
            StepForwardbutton.Enabled = StepForwardStack.Count > 0;
            CurrentStep = StepBackStack.Count - 1;
            UpdateStepCount();
            StepLabel.Text = StepExplainations[StepBackStack.Count - 1];
        }

        private void StepForwardClick(object sender, EventArgs e)
        {
            if (StepForwardStack.Count > 0)
            {
                ISnapshot next = StepForwardStack.Pop();
                StepBackStack.Push(next);
                next.Restore();
            }

            StepBackButton.Enabled = StepBackStack.Count > 1;
            StepForwardbutton.Enabled = StepForwardStack.Count > 0;
            CurrentStep = StepBackStack.Count - 1;
            UpdateStepCount();
            StepLabel.Text = StepExplainations[StepBackStack.Count - 1];
        }

        public void RewindToFirstStep()
        {
            while (StepBackStack.Count > 1)
            {
                var current = StepBackStack.Pop();
                StepForwardStack.Push(current);
            }

            if (StepBackStack.Count > 0)
                StepBackStack.Peek().Restore();

            CurrentStep = 0;
            UpdateStepCount();
        }


        public void StartNewAlgorithm(TextBox[] items)
        {
            StepBackStack.Clear();
            StepForwardStack.Clear();
            StepExplainations.Clear();

            StepBackStack.Push(new ListSnapshot(items));
            StepExplainations.Add("Initial state");

            StepForwardbutton.Show();
            StepBackButton.Show();
            PositionInListUI(StepCount, 325, 0);
            PositionInListUI(StepLabel, 300, 400);

            CurrentStep = 0;
            UpdateStepCount();
            StepLabel.Text = StepExplainations[0];
        }

        private void DrawList(TextBox[] boxlist, TextBox firstnum, TextBox lastnum)
        {
            int spacing = 38;

            for (int i = 0; i < boxlist.Length; i++)
            {
                if (int.TryParse(boxlist[i].Text, out int result))
                {
                    int x = (int)((i - (boxlist.Length - 1) / 2.0) * (boxlist[i].Width + 2));
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


        // This field is a textbox that shows the number to be found in a searching algorithm
        TextBox targetnum = ControlMaker.MakeNewBox("", 30);

        // Subroutine to display interactive controls on the screen and format data structures
        private void SetUpListUI(string inputquestion, string buttonname, Action<TextBox[]> onListCreated)
        {

            Label askuserinput = ControlMaker.MakeNewLabel(inputquestion, 600, 50);
            PositionInListUI(askuserinput, 400, 0);

            Panel labeloutline = ControlMaker.MakeNewPanel("", 610, 60);
            PositionInListUI(labeloutline, 395, 0);
            labeloutline.SendToBack();

            TextBox firstnum = ControlMaker.MakeNewBox("", 30);
            PositionInListUI(firstnum, 500, -50);

            TextBox lastnum = ControlMaker.MakeNewBox("", 30);
            PositionInListUI(lastnum, 500, 50);

            Button makelist = ControlMaker.MakeNewButton(buttonname, 100, 50);
            PositionInListUI(makelist, 900, 0);

            PositionInListUI(targetnum, 750, 210);

            Application.DoEvents(); // Waits for all pending tasks to be completed before going on to the next step

            if (inputquestion == SearchQuestion) //  For displaying a sorted list of textboxes for searching algorithms
            {
                makelist.Click += (sender, args) =>
                {
                    //  Error handling
                    if (!int.TryParse(firstnum.Text, out int first) || !int.TryParse(lastnum.Text, out int last) || !int.TryParse(targetnum.Text, out int validnum))
                    {
                        MessageBox.Show($"Invalid character inputted. Please enter integers greater than {int.MinValue} or less than {int.MaxValue}");
                        firstnum.Clear();
                        lastnum.Clear();
                        targetnum.Clear();
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
                        MessageBox.Show("Range of numbers is 50 - otherwise won't fit on screen");
                        lastnum.Clear();
                        firstnum.Clear();
                        return;
                    }

                    ListMaker numberlistmaker = new ListMaker(); // Using custom class to make a list of textboxes
                    TextBox[] boxlist = numberlistmaker.MakeList(firstnum.Text, lastnum.Text);

                    DrawList(boxlist, firstnum, lastnum); // Displays array

                    // Hides unneccessary controls for abstraction and ease of use
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
            else if (inputquestion == SortQuestion) // For sorting algorithms
            {
                targetnum.Hide();
                ComboBox listType = new ComboBox();
                listType.DropDownStyle = ComboBoxStyle.DropDownList;
                listType.Size = new Size(375, 50);
                listType.Font = new Font("OCR A Extended", 10, FontStyle.Regular);
                listType.BackColor = Color.Black;
                listType.ForeColor = Color.Turquoise;

                // Gives options for types of lists
                listType.Items.Add("List type?");
                listType.Items.Add("Randomised");
                listType.Items.Add("Reversed");
                listType.Items.Add("Random no repeats");
                listType.SelectedIndex = 0;
                PositionInListUI(listType, 750, 0);

                ListMaker numberlistmaker = new ListMaker(); // Using custom class to make a list of textboxes
                TextBox[] boxlist = Array.Empty<TextBox>(); // initially an empty array to be added to and used to model a list

                string listType_reverse = listType.SelectedItem.ToString();
                makelist.Enabled = false; // Error handling to prevent user creating a new list which would overlap

                listType.SelectedIndexChanged += (object sender, EventArgs e) =>
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

                    // error handling for user inputs (start/end of range, speed of sorting etc)
                    if (!int.TryParse(firstnum.Text, out int first) || !int.TryParse(lastnum.Text, out int last))
                    {
                        if (long.TryParse(firstnum.Text, out long start) || long.TryParse(firstnum.Text, out long end))
                        {
                            MessageBox.Show($"Inputted numbers are either less than {int.MinValue} or greater than {int.MaxValue}, please enter integers between these values");
                        }
                        else
                        {
                            MessageBox.Show("Invalid character inputted. Please enter integers.");
                        }

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

                    // Creating appropriate type of list with custom class
                    if (listType_reverse == "Reversed")
                    {
                        boxlist = numberlistmaker.MakeReverseList(firstnum.Text, lastnum.Text);
                    }
                    else if (listType_reverse == "Randomised")
                    {
                        boxlist = numberlistmaker.MakeRandomList(firstnum.Text, lastnum.Text);
                    }
                    else if (listType_reverse == "Random no repeats")
                    {
                        boxlist = numberlistmaker.RandomListNoRepeats(firstnum.Text, lastnum.Text);
                    }

                    listType.Enabled = false;// To ensure the user doesn't double click to prevent crashes

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

            // Creating and displaying step forward/back buttons
            if (StepBackButton == null)
            {
                StepBackButton = ControlMaker.MakeNewButton("Step back", 250, 50);
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
                StepForwardbutton = ControlMaker.MakeNewButton("Step forward", 250, 50);
                PositionInListUI(StepForwardbutton, 750, 400);
                StepForwardbutton.Click += StepForwardClick;
                StepForwardbutton.Hide();
            }
            else
            {
                StepForwardbutton.Hide();
            }
        }


        // logic for pause and resume buttons appearing and disappearing
        public void AddPauseResumeButtons(Control parent,Action onPause, Action onResume)
        {
            Button pause = ControlMaker.MakeNewButton("||", 150, 40);
            Button resume = ControlMaker.MakeNewButton("▶︎", 150, 40);

            PositionInListUI(pause, 357, 0);
            PositionInListUI(resume, 357, 0);

            resume.Hide();
            resume.Enabled = false;

            pause.Click += (object sender, EventArgs e) =>
            {
                onPause();
                pause.Hide();
                pause.Enabled = false;
                resume.Show();
                resume.Enabled = true;
            };

            resume.Click += (object sender, EventArgs e) =>
            {
                onResume();
                resume.Hide();
                resume.Enabled = false;
                pause.Show();
                pause.Enabled = true;
            };

            parent.Controls.Add(pause);
            parent.Controls.Add(resume);
        }

        private ComboBox algorithmSelector;

        private int sortspeed = 1;

        // Subroutine where the logo, logic and data structures are displayed, using SetUpListUI and various other subroutines and fields
        private void Form1_Load(object sender, EventArgs e)
        {
            PositionInListUI(StartInstruction, 75, -695);
            PositionInListUI(BackInstruction, 110, -656);
            PictureBox logo = new PictureBox();
            logo.Size = new Size(250, 236);
            PositionInListUI(logo, 14, 0);
            logo.Image = Image.FromFile("AlgoViewLogo.png");
            Label AppName = ControlMaker.MakeNewLabel("AlgoView", 275, 55);
            AppName.Font = new Font("OCR A Extended", 25, FontStyle.Bold);
            AppName.ForeColor = Color.SkyBlue;
            PositionInListUI(AppName, 910, -825);

            PositionInListUI(HomeButton, 15, -820);
            HomeButton.Click += ClickHomeButton;

            // Dropdown to select algorithm for visualisation
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

                if (selectedAlgorithm == "Bubble Sort") // Bubble sort logic
                {
                    CheckBox sortmode = ControlMaker.MakeNewCheckBox("Auto-Sort"); // To select if algorithm should be automatic or step by step
                    PositionInListUI(sortmode, 700, 0);

                    algorithmSelector.Enabled = false;

                    TrackBar speedbar = ControlMaker.MakeNewTrackbar();
                    Label speedlabel = ControlMaker.MakeNewLabel("Speed", 260, 30);
                    PositionInListUI(speedlabel, 830, 0);
                    PositionInListUI(speedbar, 780, 0);
                    speedlabel.Visible = false;
                    speedbar.Visible = false;

                    sortmode.CheckedChanged += (sender, e) =>
                    {
                        speedlabel.Visible = sortmode.Checked; 
                        speedbar.Visible = sortmode.Checked;
                    };

                    SetUpListUI(SortQuestion, "Enter", async (TextBox[] numbers) =>
                    {
                        if (sortmode.Checked == false)
                        {
                            StartNewAlgorithm(numbers);
                            ListMethods.BubbleSort(numbers, StepExplainations, this);
                            RewindToFirstStep();
                        }
                        else
                        {
                            sortmode.Enabled = false;
                            speedlabel.Show();
                            AddPauseResumeButtons(this, () => PauseControl.Pause(), () => PauseControl.Resume());

                            int result = speedbar.Value;
                            int speed = 500 / result;
                            await ListMethods.BubbleSortAuto(numbers, PauseControl, speed, speedbar);
                        }

                        sortmode.Enabled = false;
                        sortmode.Visible = false;
                    });
                }

                else if (selectedAlgorithm == "Binary Search")// for binary search logic
                {
                    algorithmSelector.Enabled = false;

                    Label numtofind = ControlMaker.MakeNewLabel("input number to search for:", 385, 30);
                    PositionInListUI(numtofind, 750, 0);

                    Label left = ControlMaker.MakeNewLabel("Left", 90, 30);
                    PositionInListUI(left, 675, -150);
                    left.BackColor = Color.Crimson;
                    left.ForeColor = Color.Black;

                    Label right = ControlMaker.MakeNewLabel("Right", 90, 30); // labels showing which colour represents which place in a list
                    PositionInListUI(right, 675, 150);
                    right.BackColor = Color.Blue;
                    right.ForeColor = Color.Black;

                    Label mid = ControlMaker.MakeNewLabel("Middle", 90, 30);
                    PositionInListUI(mid, 675, 0);
                    mid.BackColor = Color.Turquoise;
                    mid.ForeColor = Color.Black;

                    SetUpListUI(SearchQuestion, "Enter", (TextBox[] numbers) =>
                    {
                        numtofind.Show();
                        StartNewAlgorithm(numbers);
                        ListMethods.BinarySearch(numbers, Convert.ToInt32(targetnum.Text), StepExplainations, this); // Executes binary search
                        StepCount.Hide();
                        RewindToFirstStep();
                        StepLabel.Text = "";
                        StepCount.Hide();
                        targetnum.ReadOnly = true;
                    });

                    targetnum.Clear();
                    targetnum.ReadOnly = false;
                }
                else if (selectedAlgorithm == "Insertion Sort")// For insertion sort
                {
                    algorithmSelector.Enabled = false;

                    CheckBox sortmode = ControlMaker.MakeNewCheckBox("Auto-Sort"); // To select if algorithm should be automatic or step by step
                    PositionInListUI(sortmode, 700, 0);

                    algorithmSelector.Enabled = false;

                    TrackBar speedbar = ControlMaker.MakeNewTrackbar();
                    Label speedlabel = ControlMaker.MakeNewLabel("Speed", 260, 30);
                    PositionInListUI(speedlabel, 830, 0);
                    PositionInListUI(speedbar, 780, 0);
                    speedlabel.Visible = false;
                    speedbar.Visible = false;

                    sortmode.CheckedChanged += (sender, e) =>
                    {
                        speedlabel.Visible = sortmode.Checked;
                        speedbar.Visible = sortmode.Checked;
                    };

                    SetUpListUI(SortQuestion, "Enter", async (TextBox[] numbers) =>
                    {
                        if (sortmode.Checked == false)
                        {
                            StartNewAlgorithm(numbers);
                            ListMethods.InsertionSort(numbers, StepExplainations, this);
                            RewindToFirstStep();
                        }
                        else
                        {
                            sortmode.Enabled = false;
                            speedlabel.Show();
                            AddPauseResumeButtons(this, () => PauseControl.Pause(), () => PauseControl.Resume());

                            int result = speedbar.Value;
                            int speed = 500 / result;
                            await ListMethods.InsertionSortAuto(numbers, PauseControl, speed, speedbar);
                        }

                        sortmode.Enabled = false;
                        sortmode.Visible = false;
                    });
                }
                else if (selectedAlgorithm == "Exponential Search")
                {
                    algorithmSelector.Enabled = false;

                    Label numtofind = ControlMaker.MakeNewLabel("input number to search for:", 385, 30);
                    PositionInListUI(numtofind, 750, 0);

                    Label left = ControlMaker.MakeNewLabel("Lower Bound", 225, 30);
                    PositionInListUI(left, 675, -300);
                    left.BackColor = Color.Crimson;
                    left.ForeColor = Color.Black;

                    Label right = ControlMaker.MakeNewLabel("Upper Bound", 225, 30);
                    PositionInListUI(right, 675, 300);
                    right.BackColor = Color.Blue;
                    right.ForeColor = Color.Black;

                    Label mid = ControlMaker.MakeNewLabel("Target", 110, 30);
                    PositionInListUI(mid, 675, 0);
                    mid.BackColor = Color.Turquoise;
                    mid.ForeColor = Color.Black;

                    SetUpListUI(SearchQuestion, "Enter", (TextBox[] numbers) =>
                    {
                        numtofind.Show();
                        StartNewAlgorithm(numbers);
                        ListMethods.ExponentialSearch(numbers, Convert.ToInt32(targetnum.Text), StepExplainations, this);
                        RewindToFirstStep();
                        StepLabel.Text = "";
                        StepCount.Hide();
                        targetnum.ReadOnly = true;// Resets number to find so it can be inputted when the algorithm is selected again
                    });

                    targetnum.Clear();
                    targetnum.ReadOnly = false;
                }
                else if (selectedAlgorithm == "Merge sort") // For merge sort
                {
                    algorithmSelector.Enabled = false;

                    CheckBox sortmode = ControlMaker.MakeNewCheckBox("Auto-Sort"); // To select if algorithm should be automatic or step by step
                    PositionInListUI(sortmode, 700, 0);

                    algorithmSelector.Enabled = false;

                    TrackBar speedbar = ControlMaker.MakeNewTrackbar();
                    Label speedlabel = ControlMaker.MakeNewLabel("Speed", 260, 30);
                    PositionInListUI(speedlabel, 830, 0);
                    PositionInListUI(speedbar, 780, 0);
                    speedlabel.Visible = false;
                    speedbar.Visible = false;

                    sortmode.CheckedChanged += (sender, e) =>
                    {
                        speedlabel.Visible = sortmode.Checked;
                        speedbar.Visible = sortmode.Checked;
                    };

                    SetUpListUI(SortQuestion, "Enter", async (TextBox[] numbers) =>
                    {
                        if (!sortmode.Checked)
                        {
                            sortmode.Enabled = false;
                            StartNewAlgorithm(numbers);
                            ListMethods.MergeSort(numbers, StepExplainations, this); // Executes merge sort
                            RewindToFirstStep();
                        }
                        else
                        {
                            sortmode.Enabled = false;
                            AddPauseResumeButtons(this, () => PauseControl.Pause(), () => PauseControl.Resume());

                            int result = speedbar.Value;
                            int speed = 500 / result;
                            await ListMethods.MergeSortAuto(numbers, PauseControl, speed, speedbar);
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