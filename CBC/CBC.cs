using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace DecrypterFC
{
    public partial class CBC : Form
    {
        public CBC()
        {
            InitializeComponent();
        }
        public static string Key = "hadideahymukihidepvgfjfrudtnbxxa";
        public static string IV = "ruvwddyilslsmoou";
        public static byte[] Decrypt(byte[] encrypted)
        {
            AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider();
            aesCryptoServiceProvider.BlockSize = 128;
            aesCryptoServiceProvider.KeySize = 256;
            aesCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(Key);
            aesCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(IV);
            aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
            aesCryptoServiceProvider.Mode = CipherMode.CBC;
            ICryptoTransform cryptoTransform = aesCryptoServiceProvider.CreateDecryptor(aesCryptoServiceProvider.Key, aesCryptoServiceProvider.IV);
            byte[] result = cryptoTransform.TransformFinalBlock(encrypted, 0, encrypted.Length);
            cryptoTransform.Dispose();
            return result;
        }
        public static byte[] Encrypt(byte[] text)
        {
            AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider();
            aesCryptoServiceProvider.BlockSize = 128;
            aesCryptoServiceProvider.KeySize = 256;
            aesCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(Key);
            aesCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(IV);
            aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
            aesCryptoServiceProvider.Mode = CipherMode.CBC;
            ICryptoTransform cryptoTransform = aesCryptoServiceProvider.CreateEncryptor(aesCryptoServiceProvider.Key, aesCryptoServiceProvider.IV);
            byte[] result = cryptoTransform.TransformFinalBlock(text, 0, text.Length);
            cryptoTransform.Dispose();
            return result;
        }
        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel3.Text = "";
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "All files(*.*)|*.*";
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                string input = OFD.FileName;
                string puth = Path.GetExtension(input);
                FileInfo fil = new FileInfo(input);
                toolStripStatusLabel1.Text = fil.DirectoryName;
                listBox1.Items.Insert(0, OFD.SafeFileName);
                listBox1.SelectedIndex = 0;
                toolStripStatusLabel3.Text = "Ready";
            }
            else
            {
                MessageBox.Show("No file selected", "Warning!");
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel3.Text = "";
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                string input = FBD.SelectedPath;
                string puth = Path.GetExtension(input);
                string puth1 = FBD.SelectedPath + "\\";
                toolStripStatusLabel1.Text = FBD.SelectedPath;
                int count = Directory.GetFiles(puth1).Length;
                toolStripStatusLabel2.Text = count.ToString();
                var dir = new DirectoryInfo(puth1);
                int i = 0;
                foreach (FileInfo file in dir.GetFiles()) // извлекаем все файлы и кидаем их в список 
                {
                    //listBox1.Items.Insert(i, Path.GetFileNameWithoutExtension(file.FullName)); // получаем полный путь к файлу и потом вычищаем ненужное, оставляем только имя файла. 
                    listBox1.Items.Insert(i, Path.GetFileName(file.FullName)); // получаем полный путь к файлу и потом вычищаем ненужное, оставляем только имя файла.
                    i++;
                }
                listBox1.SelectedIndex = 0;
                toolStripStatusLabel3.Text = "Ready";
            }
            else
            {
                MessageBox.Show("No folder selected", "Warning!");
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            string file = listBox1.GetItemText(listBox1.SelectedItem);
            string puth2 = toolStripStatusLabel1.Text + "\\" + file;
            FileInfo ff = new FileInfo(puth2);
            if ((ff.Length / 1024) > 1023)
            {

                listBox2.Items.Insert(0, "File size: " + ff.Length / 1048576 + " МБ");
            }
            else
            {
                listBox2.Items.Insert(0, "File size: " + ff.Length / 1024 + " КБ");
            }
            listBox2.Items.Insert(1, "Creation date: " + ff.CreationTime);
            listBox2.Items.Insert(2, "File folder: ");
            listBox2.Items.Insert(3, toolStripStatusLabel1.Text);
        }

        private void exportSelectedFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripProgressBar1.Value = 0;
            if (listBox1.Items.Count > 0)
            {
                toolStripProgressBar1.Maximum = 1;
                toolStripProgressBar1.Minimum = 0;
                string file = listBox1.GetItemText(listBox1.SelectedItem);
                string puth = toolStripStatusLabel1.Text + "\\";
                string puth2 = puth + file;
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.FileName = file;
                saveFileDialog1.Filter = "All files(*.*)|*.*";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string filename = saveFileDialog1.FileName;
                    if (puth2 == filename)
                    {
                        MessageBox.Show("Please choose another filename or folder for export", "Error!");
                        return;
                    }
                    else
                    {
                        byte[] bytes = Decrypt(File.ReadAllBytes(puth2));
                        File.WriteAllBytes(filename, bytes);
                        toolStripProgressBar1.Value = 1;
                    }
                    toolStripStatusLabel3.Text = "Finished";
                }
                else
                {
                    toolStripStatusLabel3.Text = "Ready";
                }
            }
            else
            {
                MessageBox.Show("No file to export", "Warning!");
            }
        }

        private void exportAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripProgressBar1.Value = 0;
            if (listBox1.Items.Count > 1)
            {
                string puth = toolStripStatusLabel1.Text + "\\";
                string pch = toolStripStatusLabel1.Text;
                int x = Int32.Parse(toolStripStatusLabel2.Text);
                toolStripProgressBar1.Maximum = x;
                toolStripProgressBar1.Minimum = 0;
                FolderBrowserDialog FBD = new FolderBrowserDialog();
                if (FBD.ShowDialog() == DialogResult.OK)
                {
                    string folderName = FBD.SelectedPath;
                    if (pch == folderName)
                    {
                        MessageBox.Show("Please choose another filename or folder for export", "Error!");
                        return;
                    }
                    else
                    {
                        for (int i = 0; i < x; i++)
                        {
                            toolStripProgressBar1.Value += 1;
                            listBox1.SelectedIndex = i;
                            string file = listBox1.GetItemText(listBox1.SelectedItem);
                            string puth1 = folderName + "\\" + file;
                            string puth2 = puth + file;
                            byte[] bytes = Decrypt(File.ReadAllBytes(puth2));
                            File.WriteAllBytes(puth1, bytes);
                        }
                        toolStripStatusLabel3.Text = "Finished";
                    }
                }
                else
                {
                    toolStripStatusLabel3.Text = "Ready";
                }
            }
            else if (listBox1.Items.Count > 0 && listBox1.Items.Count < 2)
            {
                exportSelectedFileToolStripMenuItem_Click(sender, e);
            }
            else
            {
                MessageBox.Show("No files to export", "Warning!");
            }
        }

        private void DecrypterFC_Load_1(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Visible = false;
            toolStripStatusLabel2.Visible = false;
            MaximizeBox = false;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About nextForm = new About();
            nextForm.Show();
        }

        private void exportSelectedFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            toolStripProgressBar1.Value = 0;
            if (listBox1.Items.Count > 0)
            {
                toolStripProgressBar1.Maximum = 1;
                toolStripProgressBar1.Minimum = 0;
                string file = listBox1.GetItemText(listBox1.SelectedItem);
                string puth = toolStripStatusLabel1.Text + "\\";
                string puth2 = puth + file;
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.FileName = file;
                saveFileDialog1.Filter = "All files(*.*)|*.*";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string filename = saveFileDialog1.FileName;
                    if (puth2 == filename)
                    {
                        MessageBox.Show("Please choose another filename or folder for export", "Error!");
                        return;
                    }
                    else
                    {
                        byte[] bytes = Encrypt(File.ReadAllBytes(puth2));
                        File.WriteAllBytes(filename, bytes);
                        toolStripProgressBar1.Value = 1;
                    }
                    toolStripStatusLabel3.Text = "Finished";
                }
                else
                {
                    toolStripStatusLabel3.Text = "Ready";
                }
            }
            else
            {
                MessageBox.Show("No file to export", "Warning!");
            }
        }

        private void exportAllFilesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            toolStripProgressBar1.Value = 0;
            if (listBox1.Items.Count > 1)
            {
                string puth = toolStripStatusLabel1.Text + "\\";
                string pch = toolStripStatusLabel1.Text;
                int x = Int32.Parse(toolStripStatusLabel2.Text);
                toolStripProgressBar1.Maximum = x;
                toolStripProgressBar1.Minimum = 0;
                FolderBrowserDialog FBD = new FolderBrowserDialog();
                if (FBD.ShowDialog() == DialogResult.OK)
                {
                    string folderName = FBD.SelectedPath;
                    if (pch == folderName)
                    {
                        MessageBox.Show("Please choose another filename or folder for export", "Error!");
                        return;
                    }
                    else
                    {
                        for (int i = 0; i < x; i++)
                        {
                            toolStripProgressBar1.Value += 1;
                            listBox1.SelectedIndex = i;
                            string file = listBox1.GetItemText(listBox1.SelectedItem);
                            string puth1 = folderName + "\\" + file;
                            string puth2 = puth + file;
                            byte[] bytes = Encrypt(File.ReadAllBytes(puth2));
                            File.WriteAllBytes(puth1, bytes);
                        }
                        toolStripStatusLabel3.Text = "Finished";
                    }
                }
                else
                {
                    toolStripStatusLabel3.Text = "Ready";
                }
            }
            else if (listBox1.Items.Count > 0 && listBox1.Items.Count < 2)
            {
                exportSelectedFileToolStripMenuItem_Click(sender, e);
            }
            else
            {
                MessageBox.Show("No files to export", "Warning!");
            }
        }
    }
}
