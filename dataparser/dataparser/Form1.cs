using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace dataparser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.ReadOnly = true;
            comboBox2.SelectedIndex = 0;
        }
        string[] filePaths = null;
        string[] fileNames = null;

        private bool isInRange(double num)
        {
            num = Math.Abs(num);
            if (num > 10 || num < 0.1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void parseData(string filePath, string fileName)
        {
            using (TextFieldParser parser = new TextFieldParser(filePath))
            {                
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                //List<Dictionary<string, double> > list = new List<Dictionary<string, double> >();
                List<double> list = new List<double>();
                int iFlag = 0;
                int vFlag = 0;
                double curV = decimal.ToDouble(numericUpDown1.Value);
                double vg = decimal.ToDouble(numericUpDown4.Value) / 1000;
                double w = decimal.ToDouble(numericUpDown5.Value);
                double ee = decimal.ToDouble(numericUpDown6.Value);
                double e0 = 8.854187817E-12;
                double tox = decimal.ToDouble(numericUpDown7.Value) / 1000000000;
                double vds = decimal.ToDouble(numericUpDown3.Value);
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    if (fields[0].ToLower().Contains("filename"))
                    {
                        return;
                    }
                    if (fields[0].ToLower().Equals("dataname"))
                    {
                        for (int i = 1; i < fields.Length; i++)
                        {
                            if (fields[i].ToLower().Equals("ids") || fields[i].ToLower().Equals("isd"))
                            {
                                iFlag = i;
                            }
                            else if (fields[i].ToLower().Equals("vgs"))
                            {
                                vFlag = i;
                            }
                        }
                        if (iFlag == 0 || vFlag == 0)
                        {
                            if (iFlag == 0)
                            {
                                textBox1.AppendText("Not find ids field");
                            }
                            if (vFlag == 0)
                            {
                                textBox1.AppendText("Not find vgs field");
                            }
                            return;
                        }
                    }
                    //Dictionary<string, double> data = new Dictionary<string, double>();
                    //data.Add("vgs", Convert.ToDouble(fields[1]));
                    //data.Add("vds", Convert.ToDouble(fields[2]));
                    //data.Add("igs", Convert.ToDouble(fields[3]));
                    //data.Add("ids", Convert.ToDouble(fields[4]));
                    //data.Add("absids", Convert.ToDouble(fields[5]));
                    else if (fields[0].ToLower().Equals("datavalue"))
                    {
                        if (Math.Round(Convert.ToDouble(fields[vFlag]), 3) == Math.Round(curV, 3))
                        {
                            list.Add(Convert.ToDouble(fields[iFlag]));
                            curV += vg;
                            if (curV > decimal.ToDouble(numericUpDown2.Value))
                            {
                                break;
                            }
                        }
                        else
                        {
                            textBox1.AppendText("ids data lost:" + fileName + "--->" + "vgs=" + curV);
                            return;
                        }

                    }
                    //textBox1.AppendText(fields[4] + ",");
                    //textBox1.AppendText("\n");
                }
                double max = double.MinValue;
                double min = double.MaxValue;
                double dmax = double.MinValue;
                double dmin = double.MaxValue;
                for (int i = 0; i < list.Count; i++)
                {
                    double n1 = Math.Abs(list[i]);
                    //double n2 = Math.Abs(list[i + 1]);
                    bool flag = false;
                    if (i < 3 || i > list.Count - 4)
                    {
                        flag = true;
                    }
                    else if (i > 2 && i < list.Count - 3 && isInRange(n1 / list[i - 3]) && isInRange(n1 / list[i - 2]) && isInRange(n1 / list[i - 1]) && isInRange(n1 / list[i + 1]) && isInRange(n1 / list[i + 2]) && isInRange(n1 / list[i + 3]))
                    {
                        flag = true;
                    }
                    if (max < n1 && flag)
                    {
                        max = n1;
                    }
                    if (min > n1 && flag)
                    {
                        min = n1;
                    }
                    
                    double temp2;
                    if (i == 0)
                    {
                        temp2 = list[i + 1] - list[i];
                    }
                    else if (i == list.Count - 1)
                    {
                        temp2 = list[i] - list[i - 1];
                    }
                    else
                    {
                        temp2 = (list[i + 1] - list[i - 1]) / 2;
                    }
                    if (dmax < temp2)
                    {
                        dmax = temp2;
                    }
                    if (dmin > temp2)
                    {
                        dmin = temp2;
                    }
                    

                }
                double ioratio = max / min;

                //                    for (int i = 0; i < list.Count - 1; i++)
                //                    {
                //                       double temp = Math.Abs(list[i] - list[i + 1]);
                //                       if (dmax < temp)
                //                      {
                //                           dmax = temp;
                //                       }
                //                    }

                double ratio = dmax / vg;
                double cg = ee * e0 / tox;
                double miu;
                if (comboBox2.SelectedIndex == 0)
                {
                    miu = dmax / vg / w / cg / vds;
                }
                else
                {
                    miu = dmin / vg / w / cg / vds;
                }
                //string res = String.Format("vg: {0}, w: {1}, ee: {2}, e0: {3}, tox: {4}, vds: {5}, cg: {6}, ratio: {7} \n", vg, w, ee, e0, tox, vds, cg, ratio);
                //textBox1.AppendText(res);
                textBox1.AppendText(fileName.Replace(' ', '_') + "  ");
                textBox1.AppendText(ioratio + "  ");
                textBox1.AppendText(Math.Abs(miu) * 10000 + "  \t");
                textBox1.AppendText(list.Count + "\n");
            }
        }

        private void 打开ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //设置打开对话框的初始目录，默认目录为exe运行文件所在的路径
            ofd.InitialDirectory = Application.StartupPath;
            //设置打开对话框的标题
            ofd.Title = "请选择要打开的文件";
            //设置打开对话框可以多选
            ofd.Multiselect = true;
            //设置对话框打开的文件类型
            ofd.Filter = "数据文件|*.csv|所有文件|*.*";
            //设置文件对话框当前选定的筛选器的索引
            ofd.FilterIndex = 1;
            //设置对话框是否记忆之前打开的目录
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filePaths = ofd.FileNames;
                fileNames = ofd.SafeFileNames;
                textBox1.Clear();
                textBox1.AppendText("Have selected files:" + fileNames.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (filePaths == null)
            {
                MessageBox.Show("Select files!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                textBox1.Clear();
                textBox1.AppendText("Parsing...\n");
                textBox1.AppendText("FileName \t\t\t\t\tOn-Off\t\t   Ratio\t\t DataCount\n");
                for (int i = 0; i < fileNames.Length; i++)
                {
                    parseData(filePaths[i], fileNames[i]);
                }
                textBox1.AppendText("Parse Done!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.EndsWith("Done!"))
            {
                MessageBox.Show("Need results!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "文本文件|*.txt|数据文件|*.csv|所有文件|*.*";
                sfd.FilterIndex = 1;
                sfd.RestoreDirectory = true;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string localFilePath = sfd.FileName;
                    //System.IO.File.WriteAllText(localFilePath, textBox1.Text);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(localFilePath))
                    {
                        foreach (string line in textBox1.Text.Split('\n'))
                        {
                            if (line.ToLower().Contains("pars"))
                            {
                                continue;
                            }
                            string[] separators = { ",", " " };
                            string[] items = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                            file.WriteLine(string.Join(",", items));
                        }
                    }
                }
            }
        }
    }
}
