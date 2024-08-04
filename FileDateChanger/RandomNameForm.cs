using System;
using System.Drawing;
using System.Windows.Forms;

namespace FileDateChanger
{
    public partial class RandomNameForm : Form
    {
        private readonly Random random = new Random();

        public RandomNameForm()
        {
            InitializeComponent();
        }

        public RandomNameForm(string name) : this()
        {
            textBox1.Text = name;
            numericUpDown1.Value = name.Length;
            numericUpDown1.ValueChanged += NumericUpDown1_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Location = new Point(Owner.Location.X + ((Owner.Width - Width) / 2), Owner.Location.Y + ((Owner.Height - Height) / 2));
        }

        private void GetRandomName()
        {
            int length = (int)numericUpDown1.Value;
            textBox1.Text = GenerateRandomName(length);
        }

        private string GenerateRandomName(int length, string extension = "")
        {
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int count = length + extension.Length;
            char[] result = new char[count];

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(alphabet.Length);
                result[i] = alphabet[index];
            }

            for (int j = 0; j < extension.Length; j++)
            {
                result[length + j] = extension[j];
            }

            return new string(result);
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            GetRandomName();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            GetRandomName();
        }

        public string GetName()
        {
            return textBox1.Text;
        }
    }
}