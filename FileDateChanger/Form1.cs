using System;
using System.IO;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

namespace FileDateChanger
{
    public partial class Form1 : MaterialForm
    {
        public Form1()
        {
            InitializeComponent();

            listBox1.AllowDrop = true;
            listBox1.DragDrop += ListBox1_DragDrop;
            listBox1.DragEnter += ListBox1_DragEnter;
            listBox1.MouseDoubleClick += ListBox1_MouseDoubleClick;
            listBox1.MouseDown += ListBox1_MouseDown;
            listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
            listBox1.KeyDown += ListBox1_KeyDown;

            textBox1.LostFocus += TextBox1_LostFocus;
            textBox1.KeyDown += TextBox1_KeyDown;

            dateTimePicker1.ValueChanged += (s, e) => ShowDate();
            dateTimePicker2.ValueChanged += (s, e) => ShowDate();
            dateTimePicker3.ValueChanged += (s, e) => ShowDate();

            themeManager = MaterialSkinManager.Instance;
            themeManager.AddFormToManage(this);
            themeManager.ColorScheme = new ColorScheme
                (
                Primary.Blue500,
                Primary.Blue500,
                Primary.Blue500,
                Accent.LightBlue200,
                TextShade.BLACK
                );
        }


        private readonly INIFile INI = new INIFile();
        private readonly MaterialSkinManager themeManager = null;
        private int selectedIndex = -1;
        private bool editText = false;

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Enter:
                    EndEdit();
                    break;

                case Keys.Escape:
                    editText = false;
                    EndEdit();
                    break;
            }
        }

        private void TextBox1_LostFocus(object sender, EventArgs e)
        {
            if (editText)
            {
                EndEdit();
            }
        }

        private void BeginEdit()
        {
            if (selectedIndex >= 0)
            {
                Rectangle rectangle = listBox1.GetItemRectangle(selectedIndex);
                textBox1.Visible = true;
                textBox1.Location = new Point(listBox1.Left + rectangle.X, listBox1.Top + rectangle.Y);
                textBox1.Width = rectangle.Width + 2;
                textBox1.Text = Path.GetFileNameWithoutExtension(listBox1.Items[selectedIndex].ToString());
                textBox1.Focus();
                editText = true;
            }
        }

        private void EndEdit()
        {
            if (selectedIndex >= 0)
            {
                if (editText)
                {
                    string oldFullFileName = listBox1.Items[selectedIndex].ToString();
                    string oldName = Path.GetFileNameWithoutExtension(oldFullFileName);
                    string newName = textBox1.Text;

                    if (newName.Length > 0 && oldName != newName)
                    {
                        string newFullFileName = oldFullFileName.Replace(oldName, newName);
                        listBox1.Items[selectedIndex] = newFullFileName;

                        try
                        {
                            File.Move(oldFullFileName, newFullFileName);
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(exc.Message);
                        }
                    }
                }

                editText = false;
                textBox1.Visible = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShowDate();
            Location = new Point(INI.Parse("Main", "X"), INI.Parse("Main", "Y"));
            materialSwitch1.Checked = INI.Read("Main", "Theme").Equals("DARK");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            INI.Write("Main", "X", Location.X);
            INI.Write("Main", "Y", Location.Y);
            INI.Write("Main", "Theme", themeManager.Theme);
        }

        private void ListBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && listBox1.Items.Count > 0)
            {
                contextMenuStrip1.Show(MousePosition);
            }
        }

        private void ListBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop, false) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void ListBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string[] files = GetAllFiles(paths);
            AddItems(files);
        }

        private void ShowDate()
        {
            label4.Text = dateTimePicker1.Value.ToShortDateString();
            label5.Text = dateTimePicker2.Value.ToShortDateString();
            label6.Text = dateTimePicker3.Value.ToShortDateString();
        }

        private void ResetDate()
        {
            dateTimePicker1.ResetText();
            dateTimePicker2.ResetText();
            dateTimePicker3.ResetText();
        }

        private void ReadFileDate(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);

            dateTimePicker1.Value = fileInfo.CreationTime;
            dateTimePicker2.Value = fileInfo.LastWriteTime;
            dateTimePicker3.Value = fileInfo.LastAccessTime;

            ShowDate();
        }

        private void WriteFileDate(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);

            FileAttributes oldAttributes = fileInfo.Attributes;
            fileInfo.Attributes = FileAttributes.Normal;

            fileInfo.CreationTime = dateTimePicker1.Value;
            fileInfo.LastWriteTime = dateTimePicker2.Value;
            fileInfo.LastAccessTime = dateTimePicker3.Value;

            fileInfo.Attributes = oldAttributes;
        }

        private void ReadFolderDate(string folderName)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folderName);

            dateTimePicker1.Value = directoryInfo.CreationTime;
            dateTimePicker2.Value = directoryInfo.LastWriteTime;
            dateTimePicker3.Value = directoryInfo.LastAccessTime;

            ShowDate();
        }

        private void WriteFolderDate(string folderName)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folderName);

            FileAttributes oldAttributes = directoryInfo.Attributes;
            directoryInfo.Attributes = FileAttributes.Normal;

            directoryInfo.CreationTime = dateTimePicker1.Value;
            directoryInfo.LastWriteTime = dateTimePicker2.Value;
            directoryInfo.LastAccessTime = dateTimePicker3.Value;

            directoryInfo.Attributes = oldAttributes;
        }

        private void ListBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string name = listBox1.SelectedItem.ToString();
                System.Diagnostics.Process.Start(name);
            }
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedIndex = listBox1.SelectedIndex;
            if (selectedIndex >= 0 && !editText)
            {
                string name = listBox1.SelectedItem.ToString();
                if (File.Exists(name))
                {
                    ReadFileDate(name);
                }
                else
                {
                    ReadFolderDate(name);
                }
            }
        }

        private void ListBox1_KeyDown(object sender, KeyEventArgs e)
        {
            int index = listBox1.SelectedIndex;

            if (e.KeyCode == Keys.F3)
            {
                BeginEdit();
            }
            else if (index != -1 && e.KeyCode == Keys.Delete)
            {
                listBox1.Items.RemoveAt(index);
            }
        }

        private string[] GetAllFiles(params string[] paths)
        {
            List<string> result = new List<string>();
            foreach (string path in paths)
            {
                result.AddRange(GetFilesAndDirs(path));
            }
            return result.ToArray();
        }

        private List<string> GetFilesAndDirs(string path)
        {
            List<string> result = new List<string>();

            if (File.Exists(path))
            {
                result.Add(path);
            }
            else
            {
                result.Add($"{path}\\");
                foreach (string item in Directory.GetFileSystemEntries(path))
                {
                    if (File.Exists(item))
                    {
                        result.Add(item);
                    }
                    else
                    {
                        result.AddRange(GetFilesAndDirs(item));
                    }
                }
            }

            return result;
        }

        private void AddItems(params string[] files)
        {
            foreach (string file in files)
            {
                if (!listBox1.Items.Contains(file))
                {
                    listBox1.Items.Add(file);
                }
            }

            if (listBox1.Items.Count > 0 && listBox1.SelectedIndex < 0)
            {
                listBox1.SelectedIndex = 0;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                AddItems(openFileDialog1.FileNames);
            }
            else
            {
                MaterialSnackBar snackBar = new MaterialSnackBar("No files selected", 1300);
                snackBar.Show(this);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            ResetDate();
            MaterialSnackBar snackBar = new MaterialSnackBar("List cleared", 1300);
            snackBar.Show(this);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string name = listBox1.SelectedItem.ToString();
                if (File.Exists(name))
                {
                    WriteFileDate(name);
                }
                else
                {
                    WriteFolderDate(name);
                }

                MaterialSnackBar snackBar = new MaterialSnackBar("Done", 1300);
                snackBar.Show(this);
            }
            else
            {
                MaterialSnackBar snackBar = new MaterialSnackBar("No selected item", 1300);
                snackBar.Show(this);
            }
        }

        private void MaterialSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            if (materialSwitch1.Checked)
            {
                themeManager.Theme = MaterialSkinManager.Themes.DARK;
            }
            else
            {
                themeManager.Theme = MaterialSkinManager.Themes.LIGHT;
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeginEdit();
        }
    }
}