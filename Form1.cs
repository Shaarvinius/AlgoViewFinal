namespace AlgoView
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Center(Control ctrl, int topoffset, int midoffset)
        {
            ctrl.Left = ((this.ClientSize.Width - ctrl.Width) / 2) + midoffset;
            ctrl.Top = topoffset;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Label title = new Label();
            title.Width = 150;
            title.Height = 25;
            title.BackColor = Color.Empty;

            int x = (this.ClientSize.Width / 2) - (title.Width / 2);

            title.Location = new Point(x, 25);
            title.Text = "AlgoView";
            title.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(title);

            Center(title, 25, 0);
            this.Resize += (s, e) => Center(title, 25, 0);

            ComboBox algorithmSelector = new ComboBox();
            algorithmSelector.Width = 300;
            algorithmSelector.Items.Add("Bubble Sort");
            algorithmSelector.Items.Add("Insertion Sort");
            algorithmSelector.Items.Add("Binary Search");
            algorithmSelector.Items.Add("Tree Traversal");
            algorithmSelector.Items.Add("Basic swap test");
            algorithmSelector.Text = "Select Algorithm";

            this.Controls.Add(algorithmSelector);

            Center(algorithmSelector, 65, 0);
            
            this.Resize += (s, e) => Center(algorithmSelector, 65, 0);

            algorithmSelector.SelectedIndexChanged += (s, e) =>
            {
                string selectedAlgorithm = algorithmSelector.SelectedItem.ToString();

                if (selectedAlgorithm == "Bubble Sort")
                {
                    
                }
                else if (selectedAlgorithm == "Binary Search")
                {
                    
                }
                else if (selectedAlgorithm == "Insertion Sort")
                {
                    
                }
                else if (selectedAlgorithm == "Tree Traversal")
                {

                }
                else if (selectedAlgorithm == "Basic swap test")
                {
                    TextBox num1 = new TextBox();
                    num1.Size = new Size(25, 25);
                    num1.Location = new Point(500, 200);
                    num1.Text = "5";

                    this.Controls.Add(num1);
                    Center(num1, 300, -50);
                    this.Resize += (s, e) => Center(num1, 300, -50);

                    

                    TextBox num2 = new TextBox();
                    num2.Size = new Size(25, 25);
                    num2.Location = new Point(600, 200);
                    num2.Text = "3";

                    this.Controls.Add(num2);
                    Center(num2, 300, 50);
                    this.Resize += (s, e) => Center(num2, 300, 50);


                    Button basicsort = new Button();
                    basicsort.Size = new Size(100, 50);
                    basicsort.Location = new Point(550, 150);
                    basicsort.Text = "Sort";
                    this.Controls.Add(basicsort);

                    this.Controls.Add(basicsort);
                    Center(basicsort, 235, 0);
                    this.Resize += (s, e) => Center(basicsort, 235, 0);


                    basicsort.Click += (sender, args) =>
                    {
                        swapsort(num1, num2);
                    };
                    
                }
            };

        }

        private void swapsort(TextBox a, TextBox b)
        {
            int b1 = Convert.ToInt32(a.Text);
            int b2 = Convert.ToInt32(b.Text);

            Point temp;
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
}
