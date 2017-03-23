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
        string filePath = null;
        string fileName = null;
        private void 打开ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //设置打开对话框的初始目录，默认目录为exe运行文件所在的路径
            ofd.InitialDirectory = Application.StartupPath;
            //设置打开对话框的标题
            ofd.Title = "请选择要打开的文件";
            //设置打开对话框可以多选
            ofd.Multiselect = false;
            //设置对话框打开的文件类型
            ofd.Filter = "数据文件|*.csv|所有文件|*.*";
            //设置文件对话框当前选定的筛选器的索引
            ofd.FilterIndex = 2;
            //设置对话框是否记忆之前打开的目录
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filePath = ofd.FileName;
                fileName = ofd.SafeFileName;
                textBox1.Clear();
                textBox1.AppendText("Have selected a file:" + fileName);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                textBox1.Clear();
                textBox1.AppendText("Select a file!");
            }
            else
            {
                using (TextFieldParser parser = new TextFieldParser(filePath))
                {
                    textBox1.Clear();
                    textBox1.AppendText("Parsing...\n");
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    //List<Dictionary<string, double> > list = new List<Dictionary<string, double> >();
                    List<double> list = new List<double>();
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        if (!fields[0].Equals("DataValue"))
                        {
                            continue;
                        }
                        //Dictionary<string, double> data = new Dictionary<string, double>();
                        //data.Add("vgs", Convert.ToDouble(fields[1]));
                        //data.Add("vds", Convert.ToDouble(fields[2]));
                        //data.Add("igs", Convert.ToDouble(fields[3]));
                        //data.Add("ids", Convert.ToDouble(fields[4]));
                        //data.Add("absids", Convert.ToDouble(fields[5]));
                        list.Add(Convert.ToDouble(fields[4]));
                        //textBox1.AppendText(fields[4] + ",");
                        //textBox1.AppendText("\n");
                    }
                    double max = 0.0;
                    double min = double.MaxValue;
                    double dmax = double.MinValue;
                    double dmin = double.MaxValue;
                    for (int i = 0; i < list.Count / 2; i++)
                    {
                        double n1 = Math.Abs(list[i]);
                        //double n2 = Math.Abs(list[i + 1]);
                        if (max < n1)
                        {
                            max = n1;
                        }
                        if (min > n1)
                        {
                            min = n1;
                        }
                        //textBox1.AppendText(list[i] + ",");
                        if (i < (list.Count / 2 - 1))
                        {
                            double temp2 = list[i + 1] - list[i];
                            if (dmax < temp2)
                            {
                                dmax = temp2;
                            }
                            if (dmin > temp2)
                            {
                                dmin = temp2;
                            }
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
                    double vg = decimal.ToDouble(numericUpDown4.Value) / 1000;
                    double w = decimal.ToDouble(numericUpDown5.Value);
                    double ee = decimal.ToDouble(numericUpDown6.Value);
                    double e0 = 8.854187817E-12;
                    double tox = decimal.ToDouble(numericUpDown7.Value) / 1000000000;
                    double vds = decimal.ToDouble(numericUpDown3.Value);
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
                    string res = String.Format("vg: {0}, w: {1}, ee: {2}, e0: {3}, tox: {4}, vds: {5}, cg: {6}, ratio: {7} \n", vg, w, ee, e0, tox, vds, cg, ratio);
                    textBox1.AppendText(res);
                    textBox1.AppendText("开关比：" + ioratio + "\n");
                    textBox1.AppendText("载流子迁移率：" + Math.Abs(miu) * 10000 + "\n");
                }
            }
        }

       
    }
}
