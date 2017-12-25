using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace QlKe4._0
{
    public partial class Form1 : Form
    {
        bool flag = false;
        string statue1 = null;
        string statue2 = null;
        string m_cookie = "JSESSIONID=abcOfvL2nJm0SfErA0dcw";

        Thread thread_check = null;
        List<Thread> thread_list = null;

        public Form1()
        {
            InitializeComponent();
            initComboBox();
            initWuliao();
            flag = false;
            m_cookie = "JSESSIONID=abcOfvL2nJm0SfErA0dcw";
            thread_list = new List<Thread>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            getYZM1();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            postLogin();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            postLogout();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            initStatue();

            if (comboBox1.SelectedIndex == 0)
            {
                turnTo0();
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                turnTo1();
            }
            else
            {
                turnTo2();
            }

            try
            {
                if (thread_check.IsAlive == true)
                {
                    thread_check.Abort();
                }
            }
            catch(Exception e2)
            {
                Console.WriteLine(e2.ToString());
            }

            thread_check = new Thread(checkResult);
            thread_check.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            getYZM2();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            flag = true;
            toolStripStatusLabel3.Text = "工作状态：正常";
            Thread thread_temp = new Thread(qiangke);
            thread_list.Add(thread_temp);
            thread_temp.Start();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            flag = false;
            toolStripStatusLabel3.Text = "工作状态：停止";
            foreach (Thread t in thread_list)
            {
                t.Abort();
            }
            thread_list.Clear();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void initComboBox() {
            comboBox1.Items.Add("本学期课程");
            comboBox1.Items.Add("重修课程");
            comboBox1.Items.Add("跨年级课程");
        }





        private void getYZM1()
        {
            HttpWebRequest request = null;
            string url = "http://121.194.57.131/validateCodeAction.do?random=0.9718519976163162";
            try
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                request.Timeout = 5000;
                request.Method = "GET";

                request.Accept = "image/png, image/svg+xml, image/jxr, image/*;q=0.8, */*;q=0.5";
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("Accept-Language", "zh-CN");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";

                request.KeepAlive = true;
                request.Host = "121.194.57.131";

                request.Referer = "http://121.194.57.131/logout.do";
                request.Headers.Add("Cookie", m_cookie);

                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream stream_response = response.GetResponseStream();
                    pictureBox1.Image = Image.FromStream(stream_response);
                }
                catch (Exception e2)
                {
                    Console.WriteLine(e2.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                request.Abort();
            }
        }

        private void postLogin()
        {
            int count = 1;

            string zjh = textBox1.Text;
            string mm = textBox2.Text;
            string yzm = textBox3.Text;

            HttpWebRequest request = null;
            string url = "http://121.194.57.131/loginAction.do";
            string paramData = "zjh1=&tips=&lx=&evalue=&eflag=&fs=&dzslh=&zjh=" + zjh + "&mm=" + mm + "&v_yzm=" + yzm;
            byte[] byteArray = Encoding.Default.GetBytes(paramData);
            try
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                request.Timeout = 5000;
                request.Method = "POST";

                request.Accept = "text/html, application/xhtml+xml, image/jxr, */*";
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("Accept-Language", "zh-CN");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";

                request.KeepAlive = true;
                request.Host = "121.194.57.131";

                request.Referer = "http://121.194.57.131/loginAction.do";
                request.Headers.Add("Cookie", m_cookie);
                request.ContentLength = byteArray.Length;
                request.ContentType = "application/x-www-form-urlencoded";

                Stream newStream = request.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();

                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream htmlStream = response.GetResponseStream();
                    StreamReader weatherStreamReader = new StreamReader(htmlStream);

                    Char[] readBuff = new Char[256];
                    int iflag = weatherStreamReader.Read(readBuff, 0, 256);
                    while (iflag > 0)
                    {
                        iflag = weatherStreamReader.Read(readBuff, 0, 256);
                        count++;
                    }

                    if (count == 3)
                    {
                        toolStripStatusLabel1.Text = "登陆状态：已登陆";
                    }
                    weatherStreamReader.Close();
                    htmlStream.Close();
                    response.Close();
                }
                catch (Exception e2)
                {
                    count++;
                    if (count == 3)
                    {
                        toolStripStatusLabel1.Text = "登陆状态：已登陆";
                    }
                    Console.WriteLine(e2.ToString());
                }

            }
            catch (Exception e)
            {
                count++;
                label1.Text = count.ToString();
                Console.WriteLine(e.ToString());
            }
        }

        private void postLogout()
        {
            int count = 1;

            HttpWebRequest request = null;
            string url = "http://121.194.57.131/logout.do";
            string paramData = "loginType=platformLogin";
            byte[] byteArray = Encoding.Default.GetBytes(paramData);
            try
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                request.Timeout = 5000;
                request.Method = "POST";

                request.Accept = "text/html, application/xhtml+xml, image/jxr, */*";
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("Accept-Language", "zh-CN");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";

                request.KeepAlive = true;
                request.Host = "121.194.57.131";

                request.Referer = "http://121.194.57.131/menu/s_top.jsp";
                request.Headers.Add("Cookie", m_cookie);
                request.ContentLength = byteArray.Length;
                request.ContentType = "application/x-www-form-urlencoded";

                Stream newStream = request.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();

                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream htmlStream = response.GetResponseStream();
                    StreamReader weatherStreamReader = new StreamReader(htmlStream);

                    Char[] readBuff = new Char[256];
                    int iflag = weatherStreamReader.Read(readBuff, 0, 256);
                    while (iflag > 0)
                    {
                        iflag = weatherStreamReader.Read(readBuff, 0, 256);
                        count++;
                    }

                    if (count == 6)
                    {
                        toolStripStatusLabel1.Text = "登陆状态：已登出";
                    }
                    weatherStreamReader.Close();
                    htmlStream.Close();
                    response.Close();
                }
                catch (Exception e2)
                {
                    count++;
                    if (count == 6)
                    {
                        toolStripStatusLabel1.Text = "登陆状态：已登出";
                    }
                    Console.WriteLine(e2.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void turnTo0() {

        }

        private void turnTo1() {

        }

        private void turnTo2()
        {
            int count = 1;

            HttpWebRequest request = null;
            string url = "http://121.194.57.131/bxXxBtxAction.do?actionType=5";
            try
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                request.Timeout = 5000;
                request.Method = "GET";

                request.Accept = "text/html, application/xhtml+xml, image/jxr, */*";
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("Accept-Language", "zh-CN");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";

                request.KeepAlive = true;
                request.Host = "121.194.57.131";

                request.Referer = "http://121.194.57.131/menu/s_top.jsp";
                request.Headers.Add("Cookie", m_cookie);

                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream htmlStream = response.GetResponseStream();
                    StreamReader weatherStreamReader = new StreamReader(htmlStream);

                    Char[] readBuff = new Char[256];
                    int iflag = weatherStreamReader.Read(readBuff, 0, 256);
                    while (iflag > 0)
                    {
                        iflag = weatherStreamReader.Read(readBuff, 0, 256);
                        count++;
                    }

                    if (count > 400)
                    {
                        toolStripStatusLabel2.Text = "选课范围：跨年级课程";
                    }
                    weatherStreamReader.Close();
                    htmlStream.Close();
                    response.Close();
                }
                catch (Exception e2)
                {
                    count++;
                    if (count > 400)
                    {
                        toolStripStatusLabel2.Text = "选课范围：跨年级课程";
                    }
                    Console.WriteLine(e2.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void getYZM2()
        {
            HttpWebRequest request = null;
            string url = "http://121.194.57.131/validateCodeAction.do?random=0.9718519976163162";
            try
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                request.Timeout = 5000;
                request.Method = "GET";

                request.Accept = "image/png, image/svg+xml, image/jxr, image/*;q=0.8, */*;q=0.5";
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("Accept-Language", "zh-CN");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";

                request.KeepAlive = true;
                request.Host = "121.194.57.131";

                request.Referer = "http://121.194.57.131/bxXxBtxAction.do?actionType=2";
                request.Headers.Add("Cookie", m_cookie);

                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream stream_response = response.GetResponseStream();
                    pictureBox2.Image = System.Drawing.Image.FromStream(stream_response);
                }
                catch (Exception e2)
                {
                    Console.WriteLine(e2.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                request.Abort();
            }
        }

        public void my_request()
        {
            string kch = textBox4.Text;
            string yzm = textBox5.Text;
            HttpWebRequest request = null;
            string url = "http://121.194.57.131/bxXxBtxAction.do?actionType=3";
            string paramData = "ifraType=knj&v_yzm=" + yzm + "&jhxn=&jhxq=&kcId=" + kch;
            byte[] byteArray = Encoding.Default.GetBytes(paramData);
            try
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                request.Timeout = 5000;
                request.Method = "POST";
                request.Accept = "image/gif, image/jpeg, image/pjpeg, application/x-ms-application, application/xaml+xml, application/x-ms-xbap, */*";
                request.Referer = " http://121.194.57.131/bxXxBtxAction.do?actionType=5";
                request.Headers.Add("Accept-Language", "zh-cn");
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 10.0; WOW64; Trident/7.0; .NET4.0C; .NET4.0E; .NET CLR 2.0.50727; .NET CLR 3.0.30729; .NET CLR 3.5.30729)";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.ContentLength = byteArray.Length;
                request.Host = "121.194.57.131";
                request.Headers.Add("Pragma", "no-cache");
                request.Headers.Add("Cookie", m_cookie);
                Stream newStream = request.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                request.Abort();
            }
        }

        private void qiangke()
        {
            while (flag) {
                my_request();
                Thread.Sleep(1000 * (int.Parse(textBox6.Text)));
            }
        }

        private string getStatue() {
            HttpWebRequest request = null;
            string url = "http://121.194.57.131/ctkcAction.do?actionType=4";
            try
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                request.Timeout = 5000;
                request.Method = "GET";

                request.Accept = "*/*";
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("Accept-Language", "zh-CN");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                request.Headers.Add("X-Prototype-Version", "1.4.0");
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = 0;
                request.Headers.Add("Pragma", "no-cache");

                request.KeepAlive = true;
                request.Host = "121.194.57.131";

                request.Referer = "http://121.194.57.131/ctkcAction.do?actionType=1&xkjd=bxbtx";
                request.Headers.Add("Cookie", m_cookie);

                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream response_stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(response_stream);
                    String recive = reader.ReadToEnd();
                    reader.Close();
                    response_stream.Close();
                    response.Close();
                    return recive;
                }
                catch (Exception e2)
                {
                    Console.WriteLine(e2.ToString());
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        private void initStatue() {
            String statue = getStatue();
            try
            {
                statue1 = statue.Substring(7, 4);
                statue2 = statue.Substring(20, 3);
                button7.Text = "必修：" + statue1;
                button8.Text = "限选：" + statue2;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
        }

        private void checkStatue() {
            String statue = getStatue();
            try
            {
                string statue1_temp = statue.Substring(7, 4);
                string statue2_temp = statue.Substring(20, 3);
                if (statue1 != statue1_temp || statue2 != statue2_temp)
                {
                    flag = false;
                    toolStripStatusLabel3.Text = "工作状态：停止";
                    foreach (Thread t in thread_list)
                    {
                        t.Abort();
                    }
                    thread_list.Clear();
                    initStatue();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void checkResult() {
            while (true)
            {
                checkStatue();
                Thread.Sleep(10000);
            }
        }

        private void initWuliao() {
            FileStream fileReader = new FileStream("C:\\Users\\LHP\\Pictures\\ll.jpg", FileMode.Open);
            pictureBox3.Image = Image.FromStream(fileReader);
        }
    }
}
