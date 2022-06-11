/***
 * 我的第一个C#程序,用基础的语句写了一个简单的导出Minecraft资源文件的批处理
 * 像那些JSON解析还不会,用了一个最笨的办法用来提取JSON关键信息
 * 虽然写的不多,代码很烂,但是用的时间挺长的;总之程序能运行达到自己目地就行
 *
 *
 * 原理:
 *      文件夹目录:assets____indexes(存放着需要解析的各版本JSON文件 *.json)
 *                     |___objects(存放着加密的文件 文件前两个字母创建文件夹 文件夹里面才有加密的本体)
 *
 *                               ______________输出时改变文件名___________________
 *                               ↓                                            ↑
 * json部分资源格式:       ↓------------↓                     |可以不校验文件大小|  ↑
 *      "minecraft/icons/minecraft.icns": ← (这是输出路径)    |但需要保证文件正确|  ↑
 *      {                                                              ↓      ↑
 *      "hash": "991b421dfd401f115241601b2b373140a8d78572", "size": 114786    ↑
 *               ↑_____________这是加密的文件______________↑         ↑文件大小↑   ↑
 *      },                          ↓                                         ↑
 *                           读取这个文件并复制-----------------------------------
 ***/

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minecraft_Assets_Export
{
    public partial class MainWindow : Form
    {
        //初始化值---------------------------------------------------------------//
        private static readonly string User = Environment.UserName;//获取当前用户

        private string path = @"C:\Users\" + User + @"\AppData\Roaming\.minecraft\assets\indexes";//默认路径[初始化][公共]
        private string path1 = @"C:\Users\" + User + @"\AppData\Roaming\.minecraft\assets\indexes";//默认路径[初始化][公共]

        //组合调用---------------------------------------------------------------//
        private string TIME()//获取当前时间
        {
            return "[" + DateTime.Now.ToLongTimeString().ToString() + "]";
        }

        private string HASH(string a)//获取hash值
        {
            string b = null;
            string[] ss = a.Split('\"');
            for (int i = 0; i < ss.Length - 1; i++)
            {
                if (i % 2 == 1)
                    b += ss[i];
            }
            string c = b.Replace("objects", "");
            string d = c.Replace("hash", "\n\"");
            string e = d.Replace("size", ";\"\n");
            string v = null;
            string[] cc = e.Split('\"');
            for (int j = 0; j < cc.Length - 1; j++)
            {
                if (j % 2 == 1)
                    v += cc[j];
            }
            string w = v.Replace(";", "\n");
            string x = w.TrimEnd('\n');
            return x;
        }

        private string OUTPUTPATH(string a)//获取文件相对输出路径
        {
            string b = null;
            string[] ss = a.Split('\"');
            for (int i = 0; i < ss.Length - 1; i++)
            {
                if (i % 2 == 1)
                    b += ss[i];
            }
            string c = b.Replace("objects", "\"");
            string d = c.Replace("hash", ";\"\n");
            string e = d.Replace("size", "\n\"");
            string v = null;
            string[] cc = e.Split('\"');
            for (int j = 0; j < cc.Length - 1; j++)
            {
                if (j % 2 == 1)
                    v += cc[j];
            }
            string w = v.Replace(";", "\n");
            string x = w.TrimEnd('\n');
            string y = x.Replace("/", "\\");
            return y;
        }

        private void COPYFILES(string a, string b)//复制文件
        {
            string cc = Path.GetDirectoryName(b);
            if (!Directory.Exists(cc))
            {
                Directory.CreateDirectory(cc);
            }
            File.Copy(a, b, true);
        }

        //检测事件---------------------------------------------------------------//
        public MainWindow()
        {
            InitializeComponent();
            button3.Visible = false;
            try//异常检测[检测默认路径]
            {
                int i = 0;
                string[] files = Directory.GetFiles(path + "\\");//加载文件
                int fileNum = files.Length;//获取文件的数量
                richTextBox1.AppendText(TIME() + "正在加载Minecraft资源文件\n");//提示加载文件;
                richTextBox1.ScrollToCaret();
                DirectoryInfo filesroot = new DirectoryInfo(path + "\\");//打开目录信息
                progressBar1.Maximum = fileNum;
                foreach (FileInfo file in filesroot.GetFiles())//遍历此文件夹下的文件
                {
                    string name = file.Name;//获得文件名+拓展名
                    string str = Path.GetFileNameWithoutExtension(name);//去掉文件拓展名
                    comboBox1.Items.Add(str);//将得到的文件名添加入到下拉框中
                    comboBox1.Text = str;//显示最后遍历到的文件名
                    i++;
                }
                if (i == fileNum)
                {
                    richTextBox1.AppendText(TIME() + "加载完成\n");//提示加载成功;
                    richTextBox1.ScrollToCaret();

                    richTextBox1.AppendText("文件路径:" + path + "\\" + "\n" + "by MorinAmy: " + "https://space.bilibili.com/358853848\n");//显示路径和作者链接
                    richTextBox1.ScrollToCaret();
                }
                else if (i == 0)
                {
                    richTextBox1.AppendText(TIME() + "读取失败\n");//提示读取失败
                    richTextBox1.ScrollToCaret();
                }
            }
            catch//发生异常时执行
            {
                richTextBox1.AppendText(TIME() + "请选择Minecraft资源文件夹\n");
                richTextBox1.ScrollToCaret();
                richTextBox1.AppendText("by MorinAmy " + " https://space.bilibili.com/358853848\n");//显示路径和作者链接
                richTextBox1.ScrollToCaret();
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();//打开选择文件夹窗口
            if (folder.ShowDialog() == DialogResult.OK)//如果点击确定执行
            {
                richTextBox1.AppendText(TIME() + "正在加载Minecraft资源文件\n");//提示加载文件;
                richTextBox1.ScrollToCaret();

                path1 = folder.SelectedPath;//将新路径赋值
                DirectoryInfo folderroot = new DirectoryInfo(path1 + "\\");//打开目录信息
                string detect = string.Join("", folderroot);//将文件夹路径转换为字符类型
                bool isContain = detect.Contains("assets\\indexes");//检测路径是否正确
                if (isContain == true)//路径正确执行
                {
                    int i = 0;
                    path = path1;
                    comboBox1.Items.Clear();//清除下拉框之前的内容
                    string[] files = Directory.GetFiles(path1 + "\\");//加载文件
                    int fileNum = files.Length;//获取文件的数量
                    progressBar1.Maximum = fileNum;
                    foreach (FileInfo file in folderroot.GetFiles())//遍历文件夹下所有文件
                    {
                        string name = file.Name;//获得文件名+拓展名
                        string str = Path.GetFileNameWithoutExtension(name);//将拓展名去掉
                        comboBox1.Items.Add(str);//将得到的文件名添加入到下拉框中
                        comboBox1.Text = str;//显示最后遍历到的文件名
                        i++;
                        //richTextBox1.Text = string.Join("", fileNum);
                    }
                    if (i == fileNum)
                    {
                        richTextBox1.AppendText(TIME() + "加载完成\n");//提示加载成功;
                        richTextBox1.ScrollToCaret();
                        richTextBox1.AppendText("文件路径:" + path + "\\" + "\n");//显示路径和作者链接
                        richTextBox1.ScrollToCaret();
                    }
                    else if (i == 0)
                    {
                        richTextBox1.AppendText(TIME() + "读取失败\n");//提示读取失败
                        richTextBox1.ScrollToCaret();
                    }
                }
                else if (isContain == false)
                {
                    richTextBox1.AppendText(TIME() + "请选择indexes文件夹\n");
                    richTextBox1.ScrollToCaret();
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            button3.Text = "正在取消";
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();//打开导出文件夹窗口
            if (folder.ShowDialog() == DialogResult.OK)//如果点击确定执行
            {
                richTextBox1.AppendText(TIME() + "正在导出...\n");
                richTextBox1.ScrollToCaret();

                string foldPath = folder.SelectedPath;
                try
                {
                    DirectoryInfo filesroot = new DirectoryInfo(path + "\\");//打开默认初始化的路径
                    string detect = string.Join("", filesroot);//将文件夹路径转换为字符类型
                    bool isContain = detect.Contains("assets\\indexes");//查询路径是否正确
                    if (isContain == true)//路径正确执行
                    {
                        comboBox1.Enabled = false;
                        button1.Enabled = false;
                        button2.Enabled = false;
                        button3.Visible = true;

                        string fileout = path.Replace("indexes", "objects");
                        string file = comboBox1.Text;
                        string str1 = File.ReadAllText(filesroot + "\\" + file + ".json");
                        string hash = HASH(str1);
                        string outputpath = OUTPUTPATH(str1);
                        string[] Hash = hash.Split('\n');
                        int t = Hash.Length;
                        string[] Outputpath = outputpath.Split('\n');
                        int j = 0;
                        int i = 0;
                        int o = 0;

                        progressBar1.Maximum = t;

                        Task.Run(() =>
                        {
                            try
                            {
                                while (i < Hash.Length)
                                {
                                    if (button3.Text == "正在取消")
                                    {
                                        break;
                                    }
                                    while (j < Outputpath.Length)
                                    {
                                        o++;
                                        string a = Hash[i];
                                        string b = Outputpath[j];
                                        COPYFILES(fileout + "\\" + Regex.Replace(a, @"\w{38}$", "\\") + a, foldPath + "\\" + b);
                                        i++;
                                        j++;
                                        this.Invoke(new Action(() =>
                                        {
                                            progressBar1.Value = o;
                                            if (checkBox1.Checked == true)
                                            {
                                                richTextBox1.AppendText(TIME() + b + "\n");
                                                richTextBox1.ScrollToCaret();
                                            }
                                        }));
                                        break;
                                    }
                                }
                                this.Invoke(new Action(() =>
                                {
                                    if (button3.Text == "正在取消")
                                    {
                                        richTextBox1.AppendText(TIME() + "已取消导出,导出文件不完整\n");
                                        button3.Text = "取消";
                                        button1.Enabled = true;
                                        button2.Enabled = true;
                                        comboBox1.Enabled = true;
                                        progressBar1.Value = t;
                                        button3.Visible = false;
                                    }
                                    else if (i == Hash.Length)
                                    {
                                        button1.Enabled = true;
                                        button2.Enabled = true;
                                        comboBox1.Enabled = true;
                                        button3.Visible = false;

                                        richTextBox1.AppendText(TIME() + "导出完成\n");
                                        richTextBox1.ScrollToCaret();
                                    }
                                }));
                            }
                            catch
                            {
                                richTextBox1.AppendText("已取消|文件不完整\n结束导出");
                                button1.Enabled = true;
                                button2.Enabled = true;
                                comboBox1.Enabled = true;
                                button3.Visible = false;
                            }
                        });
                    }
                }
                catch
                {
                    button1.Enabled = true;
                    button2.Enabled = true;
                    comboBox1.Enabled = true;
                    button3.Visible = false;

                    richTextBox1.AppendText(TIME() + "请选择indexes文件夹\n");
                    richTextBox1.ScrollToCaret();
                    MessageBox.Show("请先选择indexes文件夹");
                }
            }
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                DialogResult dr = MessageBox.Show("显示详细信息会使运行变慢", "注意", MessageBoxButtons.YesNo);
                if (dr == DialogResult.No)
                {
                    checkBox1.Checked = false;
                }
            }
        }

        private int a = 0;

        private void ProgressBar1_Click(object sender, EventArgs e)
        {
            a++;
            if (a == 4)
            {
                richTextBox1.AppendText("\n改善了UI假死现象\n增加了取消导出的选项\n导出速度提高\n代码重构\n程序版本1.1.2\n");
                richTextBox1.ScrollToCaret();
            }
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }
    }
}