using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace fuv
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string fileName = "fuv";

        private void button1_Click(object sender, EventArgs e)
        {
            string command = textBox1.Text;
            if (command.StartsWith(":o"))
            {
                fileName = command.Substring(2);
                this.Text = fileName;
                textBox2.Text = File.ReadAllText(fileName);
                return;
            }
            if (command.StartsWith(":s"))
            {
                fileName = command.Substring(2);
                this.Text = fileName;
                File.WriteAllText(fileName, textBox2.Text);
                return;
            }

            if (command.StartsWith(":q"))
            {
                this.Close();
                return;
            }

            if (command.IndexOfAny("[\\^$.|?*+()".ToCharArray()) == -1)
            {
                this.Text = "Why bother?";
                textBox2.Text = string.Empty;
                return;
            }
            
            searchReplace(command);
        }

        private void searchReplace(string command)
        {
            var guid = Guid.NewGuid().ToString();
            var searchstring = command.Replace("\\\\", guid).Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\0", "\0").Replace(guid, "\\\\");
            int i = 0;
            int splitLocation = -1;
            foreach (var ch in searchstring)
            {
                if (ch.Equals('/') && (i == 0 || searchstring[i - 1] != '\\'))
                {
                    splitLocation = i;
                    break;
                }
                i++;
            }

            var replaceString = "";
            if (splitLocation != -1)
            {
                replaceString = searchstring.Substring(i + 1);
                searchstring = searchstring.Substring(0, splitLocation);
            }

            var reggie = new Regex(searchstring,  RegexOptions.Multiline);
            if (!reggie.IsMatch(textBox2.Text))
            {
                this.Text = "Why bother?";
                textBox2.Text = string.Empty;
                return;
            }
            textBox2.Text = reggie.Replace(textBox2.Text, replaceString);
        }
    }
}
