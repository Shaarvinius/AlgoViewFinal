namespace AlgoView
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Center(Control ctrl, int topoffset)
        {
            ctrl.Left = (this.ClientSize.Width - ctrl.Width) / 2;
            ctrl.Top = topoffset;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TextBox title = new TextBox();
            title.Width = 150;
            title.Height = 25;
            title.BackColor = Color.Empty;

            int x = (this.ClientSize.Width / 2) - (title.Width / 2);

            title.Location = new Point(x, 25);
            title.Text = "AlgoView";
            this.Controls.Add(title);

            Center(title, 25);
            this.Resize += (s, e) => Center(title, 25);
        }
    }
}
