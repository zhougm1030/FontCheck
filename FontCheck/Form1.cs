using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FontCheck.lib;

namespace FontCheck
{
    public partial class Form1 : Form
    {
        public delegate void setTextHandler(ProgressInfo info);
        Regex reg = new Regex(@"txt|js|html|json|java|cs|css|php|py");
        int fileTotal = 0;
        int fileCount = 0;
        string logFileName = "1.txt";
        Regex dirReg = null;

        string root = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            progressBar1.Hide();
            label1.Hide();
            label2.Hide();
            
            this.label1.Text = "";
            this.label2.Text = "";


        }

        private void SetLabelPath(ProgressInfo info) {

            if (progressBar1.InvokeRequired == true)
            {
                setTextHandler set = new setTextHandler(SetLabelPath);
                progressBar1.Invoke(set, new object[] { info });
            }
            else
            {
                progressBar1.Value = info.I;
            }
            
            if (label1.InvokeRequired == true)
            {
                setTextHandler set = new setTextHandler(SetLabelPath);
                label1.Invoke(set, new object[] { info });
            }
            else {
                label1.Text = info.FilePath;
                
            }
          
            if (info.I == this.fileTotal) {
                closeThread();
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
            DirectoryInfo root = new DirectoryInfo(textBox1.Text);
            checkedListBox1.Items.Clear();
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                checkedListBox1.Items.Add(d.ToString() , true);
            }
            this.root = textBox1.Text;

        }
        private void run() {

            filer(textBox1.Text);
           
        }
        Thread thread = null;

        private void button2_Click(object sender, EventArgs e)
        {
            this.fileTotal = 0;
            this.fileCount = 0;


            string strCollected = string.Empty;
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (!checkedListBox1.GetItemChecked(i))
                {
                    if (strCollected == string.Empty)
                    {
                        strCollected =  checkedListBox1.GetItemText(checkedListBox1.Items[i]);
                    }
                    else
                    {
                        strCollected = strCollected + "|" + checkedListBox1.GetItemText(checkedListBox1.Items[i]);
                    }
                }
            }
            if (strCollected != string.Empty) {
                this.dirReg = new Regex(@strCollected);
            }
           

            progressBar1.Show();
            label1.Show();
            label2.Show();
            textBox1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
        
            getFileTotal(textBox1.Text);
            progressBar1.Minimum = 0;
            progressBar1.Maximum = this.fileTotal ;


            thread = new Thread(new ThreadStart(run));
            thread.IsBackground = true;
            thread.Start();
        }
      
        public void filer(string path)
        {
          
            DirectoryInfo di = new DirectoryInfo(path);
            ErrorFont ef = new ErrorFont();
            ef.LogFileName = di.Name;
            this.logFileName = @ef.LogFileName + ".txt";
           
            if (File.Exists( Directory.GetCurrentDirectory() + "\\" + this.logFileName)) {
                File.Delete(Directory.GetCurrentDirectory() + "\\" + this.logFileName);
            }
            GetDirectory(ef, path);

        }
        

        private void GetFileName(ErrorFont ef, string path)
        {
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (FileInfo f in root.GetFiles())
            {
                int i = (++this.fileCount);
                ProgressInfo pi = new ProgressInfo();
                pi.FilePath = f.FullName;
                Console.WriteLine(f.Name);
                pi.FileCount = i+"/"+this.fileTotal;
                pi.I = i;
                SetLabelPath(pi);
                //Application.DoEvents();
                if (reg.IsMatch(f.Extension))
                {
                    ef.Path = f.DirectoryName;
                    ef.FileName = f.Name;
                    ReadFile(f.FullName, ef);
                }
            }

        }

        private void GetDirectory(ErrorFont ef, string path)
        {
            GetFileName(ef, path);
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (DirectoryInfo d in root.GetDirectories())
            {

                if (path == this.root)
                {
                    if (this.dirReg == null)
                    {
                        GetDirectory(ef, d.FullName);
                    }
                    else {
                        if (!this.dirReg.IsMatch(d.Name))
                        {
                            GetDirectory(ef, d.FullName);
                        }
                    }
                }
                else {
                    GetDirectory(ef, d.FullName);
                }
            }
        }

        public void getFileTotal(string path)
        {
            DirectoryInfo root = new DirectoryInfo(path);
            this.fileTotal += root.GetFiles().Length;
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                if (path == this.root)
                {
                    if (this.dirReg == null)
                    {
                        getFileTotal(d.FullName);
                    }
                    else
                    {
                        if (!this.dirReg.IsMatch(d.Name))
                        {
                            getFileTotal(d.FullName);
                        }
                    }
                }
                else
                {
                    getFileTotal(d.FullName);
                }
            }

        }
        private void ReadFile(string path, ErrorFont ef)
        {

            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            String line;
            int lineNum = 0;

            while ((line = sr.ReadLine()) != null)
            {
                lineNum++;
                int col = 0;
                foreach (char t in line.ToString())
                {
                    col++;
                    string re = convertToShiftJis(t.ToString());

                    if (t.ToString() != re)
                    {
                        ef.LineNumber = lineNum.ToString();
                        ef.Font = t.ToString();
                        ef.ColumnNumber = col.ToString();
                        FileStream fs = null;
                        StreamWriter sw = null;
                        try
                        {
                           
                           
                            fs = new FileStream(this.logFileName, FileMode.Append);
                            sw = new StreamWriter(fs);
                            sw.WriteLine("L:" + ef.LineNumber + "\t C:" + ef.ColumnNumber + "\t[" + ef.Font + "]\t" + ef.FileName + "\t" + ef.Path);
                            //清空缓冲区
                            sw.Flush();
                            //关闭流
                            sw.Close();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        finally
                        {
                            if (fs != null)
                            {
                                fs.Close();
                            }

                        }
                    }
                }
            }
        }

        private string convertToShiftJis(string content)
        {
            Encoding orginal = Encoding.GetEncoding("utf-8");
            Encoding ShiftJis = Encoding.GetEncoding("Shift-JIS");
            byte[] unf8Bytes = orginal.GetBytes(content);
            byte[] myBytes = Encoding.Convert(orginal, ShiftJis, unf8Bytes);
            string JISContent = ShiftJis.GetString(myBytes);
            return JISContent;
        }

        private void closeThread() {

            if (thread != null) {
                if (thread.IsAlive) {
                    thread.Abort();
                   
                    textBox1.Enabled = true;
                    button1.Enabled = true;
                    button2.Enabled = true;

                    label1.Text = "";
                    label2.Text = "complete";
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
