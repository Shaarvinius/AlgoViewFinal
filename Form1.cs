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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;



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
        
        // Subroutine to go back to start state to allow the selection of a new algorithm
        private void ClickHomeButton(object sender, EventArgs e)
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl != HomeButton && !(ctrl is PictureBox) && !(ctrl is ComboBox && ctrl.Width == 360))
                {
                    ctrl.Hide();
                }
            }

            algorithmSelector.SelectedIndex = 0;
            algorithmSelector.Enabled = true;

            // Clear graphs if present
            if (graphNodes != null)
            {
                foreach (var btn in graphNodes)
                {
                    this.Controls.Remove(btn);
                    btn.Dispose();
                }
                graphNodes = null;
            }

            graphEdges?.Clear();
            graph = null;
            graphVisited = null;
            currentHighlightedEdge = null;
            graphModeActive = false;

            Invalidate();
        }

        // Subroutines to align and position various controls
        public void Center(Control ctrl, int topoffset, int midoffset)
        {
            ctrl.Left = ((this.ClientSize.Width - ctrl.Width) / 2) + midoffset;
            ctrl.Top = topoffset;
        }

        private void PositionInUI(Control element, int topoffset, int midoffset)
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
        private Random rand = new Random();

        private List<string> StepExplainations = new List<string>();
        private Label StepLabel = ControlMaker.MakeNewLabel("", 600, 30);
        private Label StepCount = ControlMaker.MakeNewLabel("", 150, 30);

        private int CurrentStep = 0;
        private TextBox[] CurrentBoxes;

        private Button StepBackButton;
        private Button StepForwardbutton;


        private SteppingStack StepBackStack = new SteppingStack();
        private SteppingStack StepForwardStack = new SteppingStack();

        // Graph mode fields
        private Button[] graphNodes;
        private Dictionary<int, List<(int to, int weight)>> graph;
        private List<(int from, int to, int weight)> graphEdges;
        private bool[] graphVisited;
        private (int from, int to)? currentHighlightedEdge;
        private bool graphModeActive = false;
        private bool weightedMode = false;

        public Button[] GraphNodes => graphNodes;                      // read-only
        public Dictionary<int, List<(int to, int weight)>> Graph => graph; // read-only
        public List<(int from, int to, int weight)> GraphEdges => graphEdges; // read-only
        public bool[] GraphVisited => graphVisited;                   // read-only
        public (int from, int to)? CurrentHighlightedEdge             // read/write
        {
            get => currentHighlightedEdge;
            set => currentHighlightedEdge = value;
        }


        // There are only 2 possible questions to be asked for list algorithms
        private const string SearchQuestion = "Enter the lowest number in the left box and the highest in the right box: ";
        private const string SortQuestion = "Enter the lowest number in the left box and the highest in the right box(sort): ";

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


        // Group of algorithms to control forward and back stepping
        public void PushSnapshot(ISnapshot snapshot) // Adds a snapshot of the list and interface to the stepping back stack
        {
            StepBackStack.Push(snapshot);
            StepForwardStack.Clear();
            if (StepExplainations.Count < StepBackStack.Count)
                StepExplainations.Add("");
            CurrentStep = StepBackStack.Count - 1;
            UpdateStepCount();
        }

        public void PushGraphStep(ISnapshot snapshot, string label = "")
        {
            StepBackStack.Push(snapshot);
            StepForwardStack.Clear();

            // Add label only if not already present
            if (StepExplainations.Count < StepBackStack.Count)
                StepExplainations.Add(label);
            else
                StepExplainations[CurrentStep] = label;

            CurrentStep = StepBackStack.Count - 1;
            UpdateStepCount();
        }

        private void StepBackClick(object sender, EventArgs e) // logic for what happens when step back is clicked
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

        private void StepForwardClick(object sender, EventArgs e) // logic for what happens when step back is clicked
        {
            if (StepForwardStack.Count > 0)
            {
                ISnapshot next = StepForwardStack.Pop();
                StepBackStack.Push(next);
                next.Restore();
            }

            StepBackButton.Enabled = StepBackStack.Count > 1;
            StepForwardbutton.Enabled = StepForwardStack.Count > 0 && CurrentStep < StepBackStack.Count - 1;
            CurrentStep = StepBackStack.Count - 1;
            UpdateStepCount();
            StepLabel.Text = StepExplainations[StepBackStack.Count - 1];
        }

        public void RewindToFirstStep() // After the algorithms are executed, they are "rewinded" to step 1 for visualisation
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

        // helper for clearing both stacks for a new algorithm to begin
        // also helps with key interactive UI components and step counting/explainations
        public void StartNewAlgorithm(TextBox[] items) 
        {
            StepBackStack.Clear();
            StepForwardStack.Clear();
            StepExplainations.Clear();

            StepBackStack.Push(new ListSnapshot(items));
            StepExplainations.Add("Initial state");

            StepForwardbutton.Show();
            StepBackButton.Show();
            PositionInUI(StepCount, 325, 0);
            PositionInUI(StepLabel, 300, 400);

            CurrentStep = 0;
            UpdateStepCount();
            StepLabel.Text = StepExplainations[0];
        }

        private void DrawList(TextBox[] boxlist, TextBox firstnum, TextBox lastnum) // subroutine for dynamically sizing and displaying lists
        {
            int spacing = 38;

            for (int i = 0; i < boxlist.Length; i++)
            {
                if (int.TryParse(boxlist[i].Text, out int result))
                {
                    int x = (int)((i - (boxlist.Length - 1) / 2.0) * (boxlist[i].Width + 2));
                    PositionInUI(boxlist[i], 400, x);

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

        // Generate Buttons for graph nodes
        public Button[] CreateGraphNodes(int nodeCount)
        {
            Button[] nodes = new Button[nodeCount];

            for (int i = 0; i < nodeCount; i++)
            {
                nodes[i] = ControlMaker.MakeNewButton(i.ToString(), 50, 50);
                this.Controls.Add(nodes[i]);
            }

            PositionGraphNodesCircular(nodes); // center x,y and radius
            return nodes;
        }

        private void AddUndirectedEdge(int a, int b)
        {
            int weight = weightedMode ? rand.Next(1, 10) : 1;

            graph[a].Add((b, weight));
            graph[b].Add((a, weight));

            graphEdges.Add((a, b, weight));
        }

        private bool EdgeExists(int a, int b)
        {
            return graph[a].Any(e => e.to == b);
        }

        // Circular layout helper
        public void PositionGraphNodesCircular(Button[] nodes)
        {
            if (nodes == null || nodes.Length == 0)
                return;

            int n = nodes.Length;

            int baseRadius = Math.Max(160, n * 28);
            int radiusX = (int)(baseRadius * 1.25);
            int radiusY = (int)(baseRadius * 0.85);

            int centerX = this.ClientSize.Width / 2;
            int centerY = this.ClientSize.Height / 2 + 100;

            for (int i = 0; i < n; i++)
            {
                double angle = 2 * Math.PI * i / n;

                int x = centerX + (int)(radiusX * Math.Cos(angle)) - nodes[i].Width / 2;
                int y = centerY + (int)(radiusY * Math.Sin(angle)) - nodes[i].Height / 2;

                nodes[i].Location = new Point(x, y);
            }
        }
        private Point GetCenter(Control c)
        {
            return new Point(
                c.Left + c.Width / 2,
                c.Top + c.Height / 2
            );
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!graphModeActive || graphEdges == null || graphNodes == null)
                return;

            foreach (var edge in graphEdges)
            {
                int from = edge.from;
                int to = edge.to;
                int weight = edge.weight;

                if (from < 0 || to < 0 || from >= graphNodes.Length || to >= graphNodes.Length)
                    continue;

                Point p1 = GetCenter(graphNodes[from]);
                Point p2 = GetCenter(graphNodes[to]);

                Pen edgePen = Pens.White;

                if (currentHighlightedEdge.HasValue)
                {
                    var h = currentHighlightedEdge.Value;
                    if ((h.from == from && h.to == to) ||
                        (h.from == to && h.to == from))
                    {
                        edgePen = Pens.Red;
                    }
                }

                e.Graphics.DrawLine(edgePen, p1, p2);

                int midX = (p1.X + p2.X) / 2;
                int midY = (p1.Y + p2.Y) / 2;

                int dx = p2.X - p1.X;
                int dy = p2.Y - p1.Y;

                double length = Math.Sqrt(dx * dx + dy * dy);
                if (length == 0) length = 1;

                int offsetX = (int)(-dy / length * 12);
                int offsetY = (int)(dx / length * 12);

                Point weightPos = new Point(midX + offsetX, midY + offsetY);

                e.Graphics.DrawString(
                    weight.ToString(),
                    this.Font,
                    Brushes.Black,
                    weightPos
                );
            }
        }

        private void GenerateGraph(int nodeCount, string density)
        {
            rand = new Random();

            graph = new Dictionary<int, List<(int to, int weight)>>();
            graphEdges = new List<(int from, int to, int weight)>();

            for (int i = 0; i < nodeCount; i++)
                graph[i] = new List<(int, int)>();

            graphNodes = CreateGraphNodes(nodeCount);
            PositionGraphNodesCircular(graphNodes);

            if (density == "Sparse")
            {
                for (int i = 0; i < nodeCount; i++)
                {
                    int j = (i + 1) % nodeCount;
                    AddUndirectedEdge(i, j);
                }

                if (nodeCount > 4)
                {
                    int extraEdges = rand.Next(1, 3);
                    for (int k = 0; k < extraEdges; k++)
                    {
                        int a = rand.Next(nodeCount);
                        int b = rand.Next(nodeCount);
                        if (a != b && !EdgeExists(a, b))
                            AddUndirectedEdge(a, b);
                    }
                }
            }
            else if (density == "Medium")
            {
                for (int i = 0; i < nodeCount; i++)
                {
                    while (graph[i].Count < 2)
                    {
                        int j = rand.Next(nodeCount);
                        if (j != i && !EdgeExists(i, j))
                            AddUndirectedEdge(i, j);
                    }
                }
            }
            else if (density == "Complete")
            {
                for (int i = 0; i < nodeCount; i++)
                {
                    for (int j = i + 1; j < nodeCount; j++)
                        AddUndirectedEdge(i, j);
                }
            }

            Invalidate();
        }

        // This field is a textbox that shows the number to be found in a searching algorithm
        TextBox targetnum = ControlMaker.MakeNewBox("", 30);

        // Subroutine to display interactive controls on the screen and take inputs to format lists
        private void SetUpListUI(string inputquestion, string buttonname, Action<TextBox[]> onListCreated)
        {

            Label askuserinput = ControlMaker.MakeNewLabel(inputquestion, 600, 50);
            PositionInUI(askuserinput, 400, 0);

            Panel labeloutline = ControlMaker.MakeNewPanel("", 610, 60);
            PositionInUI(labeloutline, 395, 0);
            labeloutline.SendToBack();

            TextBox firstnum = ControlMaker.MakeNewBox("", 30);
            PositionInUI(firstnum, 500, -50);

            TextBox lastnum = ControlMaker.MakeNewBox("", 30);
            PositionInUI(lastnum, 500, 50);

            Button makelist = ControlMaker.MakeNewButton(buttonname, 100, 50);
            PositionInUI(makelist, 900, 0);

            PositionInUI(targetnum, 750, 210);

            Application.DoEvents(); // Waits for all pending tasks to be completed before going on to the next step

            if (inputquestion == SearchQuestion) //  For displaying a sorted list of textboxes for searching algorithms
            {
                makelist.Click += (sender, args) =>
                {
                    //Error handling for entering number range for lists
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

                    ListMaker numberlistmaker = new ListMaker(); // Using custom class to make a 1D array of textboxes
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

                // Giving options for types of lists
                listType.Items.Add("List type?");
                listType.Items.Add("Randomised");
                listType.Items.Add("Reversed");
                listType.Items.Add("Random no repeats");
                listType.SelectedIndex = 0;
                PositionInUI(listType, 750, 0);

                ListMaker numberlistmaker = new ListMaker(); // Using custom class to make a list of textboxes
                TextBox[] boxlist = Array.Empty<TextBox>(); // initially an empty 1D array to be added to and used to model a list

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
                PositionInUI(StepBackButton, 750, -400);
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
                PositionInUI(StepForwardbutton, 750, 400);
                StepForwardbutton.Click += StepForwardClick;
                StepForwardbutton.Hide();
            }
            else
            {
                StepForwardbutton.Hide();
            }
        }

        private void SetUpGraphUI(Action<Button[], List<(int from, int to, int weight)>> onGraphCreated)
        {
            graphModeActive = true;

            targetnum.Hide();


            Label nodeCountLabel = ControlMaker.MakeNewLabel("Number of nodes (3-14):", 250, 30);
            PositionInUI(nodeCountLabel, 350, 0);

            TextBox nodeCountBox = ControlMaker.MakeNewBox("", 50);
            PositionInUI(nodeCountBox, 390, 0);

            Label graphTypeLabel = ControlMaker.MakeNewLabel("Graph type: ", 200, 30);
            PositionInUI(graphTypeLabel, 430, 0);

            ComboBox densitySelector = new ComboBox();
            densitySelector.Items.AddRange(new string[] { "Sparse", "Medium", "Complete" });
            densitySelector.SelectedIndex = 0;
            densitySelector.DropDownStyle = ComboBoxStyle.DropDownList;
            densitySelector.Size = new Size(250, 50);
            PositionInUI(densitySelector, 500, 0);

            Button generateGraphButton = ControlMaker.MakeNewButton("Generate Graph", 150, 50);
            PositionInUI(generateGraphButton, 600, 0);

            generateGraphButton.Click += (s, e) =>
            {
                if (!int.TryParse(nodeCountBox.Text, out int nodeCount) || nodeCount < 3 || nodeCount > 14)
                {
                    MessageBox.Show("Enter a valid number of nodes (3-14).");
                    return;
                }

                string density = densitySelector.SelectedItem.ToString();

                if (density == "Medium" && nodeCount > 10)
                {
                    MessageBox.Show("Medium graphs support up to 10 nodes.");
                    return;
                }

                if (density == "Complete" && nodeCount > 7)
                {
                    MessageBox.Show("Complete graphs support up to 7 nodes.");
                    return;
                }

                GenerateGraph(nodeCount, density);
                Invalidate();

                nodeCountLabel.Hide();
                nodeCountBox.Hide();
                graphTypeLabel.Hide();
                densitySelector.Hide();
                generateGraphButton.Hide();

                onGraphCreated?.Invoke(graphNodes, graphEdges);
            };

            if (StepBackButton == null)
            {
                StepBackButton = ControlMaker.MakeNewButton("Step back", 250, 50);
                PositionInUI(StepBackButton, 750, -400);
                StepBackButton.Click += StepBackClick;
            }

            if (StepForwardbutton == null)
            {
                StepForwardbutton = ControlMaker.MakeNewButton("Step forward", 250, 50);
                PositionInUI(StepForwardbutton, 750, 400);
                StepForwardbutton.Click += StepForwardClick;
            }

            StepBackButton.Hide();
            StepForwardbutton.Hide();
        }


        // logic for pause and resume buttons appearing and disappearing
        public void AddPauseResumeButtons(Control parent,Action onPause, Action onResume)
        {
            Button pause = ControlMaker.MakeNewButton("||", 150, 40);
            Button resume = ControlMaker.MakeNewButton("▶︎", 150, 40);

            PositionInUI(pause, 357, 0);
            PositionInUI(resume, 357, 0);

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

        // Subroutine where the logo, logic and data structures are displayed, using SetUpListUI and various other subroutines and fields
        private void Form1_Load(object sender, EventArgs e)
        {
            PictureBox logo = new PictureBox();
            logo.Size = new Size(250, 236);
            PositionInUI(logo, 14, 0);
            logo.Image = Image.FromFile("AlgoViewLogo.png");
            Label AppName = ControlMaker.MakeNewLabel("AlgoView", 275, 55);
            AppName.Font = new Font("OCR A Extended", 25, FontStyle.Bold);
            AppName.ForeColor = Color.SkyBlue;
            PositionInUI(AppName, 910, -825);

            PositionInUI(HomeButton, 15, -820);
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
            algorithmSelector.Items.Add("DFS");
            algorithmSelector.Items.Add("BFS");
            algorithmSelector.Items.Add("Dijkstra's shortest path");
            algorithmSelector.SelectedIndex = 0;
            algorithmSelector.Refresh();
            PositionInUI(algorithmSelector, 290, 0);


            algorithmSelector.SelectedIndexChanged += (s, e) =>
            {
                string selectedAlgorithm = algorithmSelector.SelectedItem.ToString();

                if (selectedAlgorithm == "Bubble Sort") // Bubble sort logic
                {
                    CheckBox sortmode = ControlMaker.MakeNewCheckBox("Auto-Sort"); // To select if algorithm should be automatic or step by step
                    PositionInUI(sortmode, 700, 0);

                    algorithmSelector.Enabled = false;

                    TrackBar speedbar = ControlMaker.MakeNewTrackbar();
                    Label speedlabel = ControlMaker.MakeNewLabel("Speed", 260, 30);
                    PositionInUI(speedlabel, 830, 0);
                    PositionInUI(speedbar, 780, 0);
                    speedlabel.Visible = false;
                    speedbar.Visible = false;

                    sortmode.CheckedChanged += (sender, e) =>
                    {
                        speedlabel.Visible = sortmode.Checked; 
                        speedbar.Visible = sortmode.Checked;
                    };

                    // Once all inputs have been taken, algorithms are ready to execute and examine by user

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
                    PositionInUI(numtofind, 750, 0);

                    Label left = ControlMaker.MakeNewLabel("Left", 90, 30);
                    PositionInUI(left, 675, -150);
                    left.BackColor = Color.Crimson;
                    left.ForeColor = Color.Black;

                    Label right = ControlMaker.MakeNewLabel("Right", 90, 30); // labels showing which colour represents which place in a list
                    PositionInUI(right, 675, 150);
                    right.BackColor = Color.Blue;
                    right.ForeColor = Color.Black;

                    Label mid = ControlMaker.MakeNewLabel("Middle", 90, 30);
                    PositionInUI(mid, 675, 0);
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
                    PositionInUI(sortmode, 700, 0);

                    algorithmSelector.Enabled = false;

                    TrackBar speedbar = ControlMaker.MakeNewTrackbar();
                    Label speedlabel = ControlMaker.MakeNewLabel("Speed", 260, 30);
                    PositionInUI(speedlabel, 830, 0);
                    PositionInUI(speedbar, 780, 0);
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
                else if (selectedAlgorithm == "Exponential Search") // For exponential search
                {
                    algorithmSelector.Enabled = false;

                    Label numtofind = ControlMaker.MakeNewLabel("input number to search for:", 385, 30);
                    PositionInUI(numtofind, 750, 0);

                    Label left = ControlMaker.MakeNewLabel("Lower Bound", 225, 30);
                    PositionInUI(left, 675, -300);
                    left.BackColor = Color.Crimson;
                    left.ForeColor = Color.Black;

                    Label right = ControlMaker.MakeNewLabel("Upper Bound", 225, 30);
                    PositionInUI(right, 675, 300);
                    right.BackColor = Color.Blue;
                    right.ForeColor = Color.Black;

                    Label mid = ControlMaker.MakeNewLabel("Target", 110, 30);
                    PositionInUI(mid, 675, 0);
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
                    PositionInUI(sortmode, 700, 0);

                    algorithmSelector.Enabled = false;

                    TrackBar speedbar = ControlMaker.MakeNewTrackbar();
                    Label speedlabel = ControlMaker.MakeNewLabel("Speed", 260, 30);
                    PositionInUI(speedlabel, 830, 0);
                    PositionInUI(speedbar, 780, 0);
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
                        algorithmSelector.Enabled = false;

                        SetUpGraphUI((nodes, edges) =>
                        {
                            graphNodes = nodes;
                            graphEdges = edges;
                            graphVisited = new bool[nodes.Length];

                            Invalidate(); // triggers OnPaint to draw edges
                        });
                    });
                }
                else if (selectedAlgorithm == "BFS")
                {
                    algorithmSelector.Enabled = false;

                    graphModeActive = true;
                    weightedMode = false;
                    targetnum.Hide();

                    SetUpGraphUI((nodes, edges) =>
                    {
                        graphNodes = nodes;
                        graphEdges = edges;
                        graphVisited = new bool[nodes.Length];

                        StepBackStack.Clear();
                        StepForwardStack.Clear();
                        StepExplainations.Clear();
                       
                        PositionInUI(StepLabel, 300, 400);
                        GraphAlgorithms.BFS(this, 0);

                        StepBackButton.Show();
                        StepForwardbutton.Show();
                        StepCount.Show();
                        StepLabel.Show();

                        RewindToFirstStep();
                    });
                }
                else if (selectedAlgorithm == "Dijkstra's shortest path")
                {
                    algorithmSelector.Enabled = false;
                    SetUpListUI("Enter the first number in the left box and the last in the right box: ", "Enter", (TextBox[] numbers) =>
                    {
                        algorithmSelector.Enabled = false;

                        SetUpGraphUI((nodes, edges) =>
                        {
                            graphNodes = nodes;
                            graphEdges = edges;
                            graphVisited = new bool[nodes.Length];

                            Invalidate();
                        });
                    });
                }
            };
        }
    }
}