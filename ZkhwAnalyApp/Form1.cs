/*
 * timer1：显示当前时间
 * timer2：重新发送生化、血常规中的数据 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Net.Sockets;
using System.Text.RegularExpressions; 

namespace ZkhwAnalyApp
{
    public partial class Form1 : Form
    {
        private string comname = "";
        private bool timerFlag = false;
        private string myDeviceFactory = "";
        string xcglasttime = "";
        string shlasttime = "";
        string shenghuaPath = "";
        string xuechangguiPath = "";
        string strIP = "";
        string strPort = "";

        List<DisplayData> _existShList = new List<DisplayData>();
        List<DisplayData> _existXcgList = new List<DisplayData>();
        public Form1()
        {
            InitializeComponent();
        }
       
        private void SetListViewColWidth()
        {
            listView1.Columns[3].Width = listView1.ClientSize.Width - (listView1.Columns[0].Width + listView1.Columns[1].Width + listView1.Columns[2].Width);
        }
        private void Form1_Load(object sender, EventArgs e)
        { 
            SetListViewColWidth();
            listView1.BackColor = Color.Azure;
            GetDeviceInfo();
            bool flag=GetDataForDevice();
            //if (flag == false) return; //先不要这句
            Common.WriteConfigTime();

            timer1.Start();    //显示时间
            timer2.Start();   //补发数据
            timer3.Start();  //失败个数
        }
        private void GetDeviceInfo()
        { 
            string shxqAgreement = "";
            string com = "";
            Common.GetConfigValues(out shenghuaPath, out xuechangguiPath, out shxqAgreement, out com,out shlasttime,out xcglasttime,out strIP,out strPort);
            comboBox1.Text = shxqAgreement;
            textBox1.Text = shenghuaPath;
            textBox2.Text = xuechangguiPath;
            comboBox2.Text = com;
            comname = com;

            myDeviceFactory = shxqAgreement;
        }
       
        private void timer1_Tick(object sender, EventArgs e)
        {
            lblDate.Text = "时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            SetListViewColWidth();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("是否确认退出？", "操作提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    timer1.Stop();
                    timer2.Stop();
                    timer3.Stop();
                    timer4.Stop();
                    Application.DoEvents();
                    Environment.Exit(0);
                }
                catch
                {

                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        #region 配置信息
        
        private void ControlVisible(bool a)
        {
            label14.Visible = a;
            comboBox2.Visible = a;
            label3.Visible = a;
            label1.Visible = a;
            textBox1.Visible = a;
            label5.Visible = a;
            label2.Visible = a;
            textBox2.Visible = a; 
        }

        private void IPPortControlVisible(bool a)
        {
            label10.Visible = a;
            textBox4.Visible = a;
            label11.Visible = a;
            textBox5.Visible = a;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0) return;
            ControlVisible(false);
            IPPortControlVisible(true);
            textBox4.Text = strIP;
            textBox5.Text = strPort;
            if (this.comboBox1.Text == "库贝尔")
            {
                 
                label14.Visible = true;
                comboBox2.Visible = true;
                string[] ArryPort = System.IO.Ports.SerialPort.GetPortNames();
                this.comboBox2.Items.Clear();
                if (ArryPort.Length > 0)
                {
                    for (int i = 0; i < ArryPort.Length; i++)
                    {
                        this.comboBox2.Items.Add(ArryPort[i]);
                    }
                    comboBox2.Text = comname;
                }
                
            }
            else if(this.comboBox1.Text == "英诺华")
            {
                IPPortControlVisible(false);
                ControlVisible(true);
                label14.Visible = false;
                comboBox2.Visible = false;
            } 
        }
       

        private void button1_Click(object sender, EventArgs e)
        { 
            if (this.comboBox1.Text == "库贝尔")
            {
                if (this.comboBox2.Text == "")
                { 
                    MessageBox.Show("库贝尔血球串口号不能为空!");
                    return;
                }
            }
            if(this.comboBox1.Text == "英诺华")
            {
                if(textBox1.Text.Trim()=="")
                {
                    MessageBox.Show("英诺华生化中间库地址不能为空!");
                    return;
                }

                if (textBox2.Text.Trim() == "")
                {
                    MessageBox.Show("英诺华血球中间库地址不能为空!");
                    return;
                }
            }
            string com = comboBox2.Text;
            string shxqAgreement= comboBox1.Text;
            string shenghuaPath1= textBox1.Text;
            string xuechangguiPath1= textBox2.Text;
            string err = "";
            string ip = textBox4.Text;
            string port = textBox5.Text;
            bool flag = Common.WriteConfigValues(shenghuaPath1, xuechangguiPath1, shxqAgreement, com,ip,port,out err);
            if(flag==true)
            {
                MessageBox.Show("保存成功,重启程序。");
            }
            else
            {
                string tmp = "出错：" + err;
                MessageBox.Show(tmp);
            } 
        }

        #endregion

        #region  补发信息

        #region 异步补发发送
        delegate bool RePushDataShHandler(UpLoadDataForSH obj);   //定义委托函数 
        private bool RePushDataForSh(UpLoadDataForSH obj)
        {
            bool flag = false; 
            try
            { 
                string err1 = "";
                flag = LogicHandler.UpLoadDatashDal(obj, out err1);
                if(flag==true)
                {
                    LogicHandler.DeleteAccessSh(obj.ID);//删除对应的记录
                    //LogicHandler.DeleteMySqlSh(obj.ID);
                }
            }
            catch
            {
            }
            return flag;
        }

        private void CallBackPushShDataReFunc(IAsyncResult result)
        {
            try
            {
                RePushDataShHandler handler = (RePushDataShHandler)((AsyncResult)result).AsyncDelegate;
                bool flag = handler.EndInvoke(result);
                UpLoadDataForSH obj = (UpLoadDataForSH)result.AsyncState;
                if (flag == true)
                { 
                    DisplayData obj1 = Common.GetobjDisplaySh(obj);
                    ShowMessage(listView1, obj1);   //显示在listview中
                    ShowMessage1(label7, 1);    //成功的显示发送成功个数
                }
            }
            catch
            {}
        }

        delegate bool RePushDataXcgHandler(UpLoadDataForXCG obj);
        private bool RePushDataForXcg(UpLoadDataForXCG obj)
        {
            bool flag = false;
            try
            {
                string err1 = "";
                flag = LogicHandler.UpLoadDataxcgDal(obj, out err1);
                if(flag==true)
                {
                    LogicHandler.DeleteAccessXcg(obj.ID);//删除对应的记录
                    //LogicHandler.DeleteMySqlXcg(obj.ID);
                }
            }
            catch
            {
            }
            return flag;
        }

        private void CallBackPushXcgDataReFunc(IAsyncResult result)
        {
            try
            {
                RePushDataXcgHandler handler = (RePushDataXcgHandler)((AsyncResult)result).AsyncDelegate;
                bool flag = handler.EndInvoke(result);
                UpLoadDataForXCG obj = (UpLoadDataForXCG)result.AsyncState;
                if (flag == true)
                {
                    DisplayData obj1 = Common.GetobjDisplayXCG(obj);
                    ShowMessage(listView1, obj1);
                    ShowMessage1(label7, 1);
                }
            }
            catch
            {

            }
        }

        #endregion
        private void AgainSendSh()
        {
            try
            {
                RePushDataShHandler handler = new RePushDataShHandler(RePushDataForSh);
                string err = "";
                //DataSet ds = LogicHandler.GetDataForMySql("", "10", "zkhw_tj_sh", out err); // 
                DataSet ds = LogicHandler.GetDataForAccess("", "10", "zkhw_tj_sh", out err);
                if (ds != null)
                {
                    DataTable dt = ds.Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        UpLoadDataForSH obj = Common.GetObjshByDR(dr);
                        IAsyncResult result = handler.BeginInvoke(obj, new AsyncCallback(CallBackPushShDataReFunc), obj); 
                    }
                }
            }
            catch(Exception er)
            {
                lblerr.Visible = true;
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Application.StartupPath + "/log.txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + er.Message);
                }
            } 
        }

        private void AgainSendXcg()
        {
            try
            {
                RePushDataXcgHandler handler = new RePushDataXcgHandler(RePushDataForXcg);
                string err = "";
                //DataSet ds = LogicHandler.GetDataForMySql("", "10", "zkhw_tj_xcg", out err);
                DataSet ds = LogicHandler.GetDataForAccess("", "10", "zkhw_tj_xcg", out err);
                if (ds != null)
                {
                    DataTable dt = ds.Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];  
                        UpLoadDataForXCG obj = Common.GetObjxcgByDR(dr);
                        IAsyncResult result = handler.BeginInvoke(obj, new AsyncCallback(CallBackPushXcgDataReFunc), obj); 
                    }
                }
            }
            catch (Exception er)
            {
                lblerr.Visible = true;
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Application.StartupPath + "/log.txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + er.Message);
                }
            }
        } 
        /// <summary>
        /// 这里重新发送失败的信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (timerFlag == true) return;
            timerFlag = true;
            lblerr.Visible = false;
            try
            {
                AgainSendSh(); //读取zkhw_tj_sh
                AgainSendXcg(); //血常规  zkhw_tj_xcg
            }
            catch 
            {
                
            }
            finally
            {
                timerFlag = false;
            }
        }

        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox3.Text.Trim() == "") return;
            listView1.SelectedItems.Clear();
            for (int i=0;i< listView1.Items.Count;i++)
            {
                ListViewItem lv = (ListViewItem)listView1.Items[i];
                string str = lv.Text;
                if(str== textBox3.Text)
                {
                    lv.Selected = true; 
                    lv.EnsureVisible();
                    break;
                }
            }
        }

        #region 写listview
        delegate void ShowMessageDelegate(ListView lvw, DisplayData obj);
        private void ShowMessage(ListView lvw, DisplayData obj)
        {
            if (lvw.InvokeRequired)
            {
                ShowMessageDelegate myDelegate = new ShowMessageDelegate(ShowMessage); //方法2
                lvw.Invoke(myDelegate, new object[] { lvw, obj });
            }
            else
            {
                int index = 0;
                bool flag = false;
                for(int i=0;i<lvw.Items.Count;i++)
                {
                    ListViewItem lv = (ListViewItem)lvw.Items[i];
                    string str = lv.Text;
                    string sType = lv.SubItems[1].Text;
                    if (str==obj.BarCode && sType==obj.Type)
                    {
                        lv.SubItems[2].Text = obj.UploadDate;
                        lv.SubItems[3].Text = obj.Content;
                        index = i;
                        flag = true;
                        break;
                    }
                }
                if(flag==false)
                {
                    ListViewItem lv = new ListViewItem();
                    lv.Text = obj.BarCode;
                    lv.SubItems.Add(obj.Type);
                    lv.SubItems.Add(obj.UploadDate);
                    lv.SubItems.Add(obj.Content);
                    lvw.BeginUpdate();
                    lvw.Items.Add(lv);
                    index = lvw.Items.Count - 1;
                    lvw.EndUpdate();
                }
                //lvw.Items[index].Selected = true;
                //lvw.SelectedItems[0].BackColor = Color.FromArgb(49, 106, 197);
                //lvw.Items[index].EnsureVisible();
            }
        }

        delegate void ShowMessageDelegate1(Label lb, int num);
        private void ShowMessage1(Label lb, int num)
        {
            if (lb.InvokeRequired)
            {
                ShowMessageDelegate1 myDelegate1 = new ShowMessageDelegate1(ShowMessage1);  
                lb.Invoke(myDelegate1, new object[] { lb, num });
            }
            else
            {
                string tmp = lb.Text;
                if (tmp == "") tmp = "0";
                tmp=(int.Parse(tmp) + num).ToString();
                lb.Text = tmp; 
            }
        }

        private void ShowMessageFail(Label lb, int num)
        {
            if (lb.InvokeRequired)
            {
                ShowMessageDelegate1 myDelegate1 = new ShowMessageDelegate1(ShowMessageFail);
                lb.Invoke(myDelegate1, new object[] { lb, num });
            }
            else
            { 
                lb.Text = num.ToString();
            }
        }
        #endregion

        #region 失败个数
        bool failflag = false;
        private void timer3_Tick(object sender, EventArgs e)
        {
            try
            {
                if (failflag == true) return;
                failflag = true;
                //int numsh = LogicHandler.GetFailNumShMySql();
                //int numxcg = LogicHandler.GetFailNumXcgMySql();
                int numsh = LogicHandler.GetFailNumSh();
                int numxcg = LogicHandler.GetFailNumXcg();
                int num = numsh + numxcg;
                ShowMessageFail(lblfail, num);
            }
            catch
            {}
            finally
            {
                failflag = false;
            }
        }

        #endregion

        private bool GetDataForDevice()
        {
            bool flag = true;
            switch(myDeviceFactory)
            {
                case "英诺华":
                    timer4.Start();
                    break;
                case "库贝尔":
                    socketTcpKbe();
                    flag = initPort();
                    port.DataReceived += new SerialDataReceivedEventHandler(this.mySerialPort_DataReceived);
                    break;
                case "雷杜":
                    socketTcp();
                    break;
                case "迈瑞":
                    socketTcpMr();
                    break;
            }
            return flag;
        }

        #region 迈瑞
        private void socketTcpMr()
        {
            try
            {
                string hostName = Dns.GetHostName();   //获取本机名
                IPHostEntry localhost = Dns.GetHostByName(hostName);//方法已过期，可以获取IPv4的地址
                                                                    //IPAddress ip = localhost.AddressList[0];
                IPAddress ip = IPAddress.Parse(strIP);
                Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint point = new IPEndPoint(ip, int.Parse(strPort));
                //socket绑定监听地址
                serverSocket.Bind(point);
                //设置同时连接个数
                serverSocket.Listen(10);

                //利用线程后台执行监听,否则程序会假死
                Thread thread = new Thread(ListenMr);
                thread.IsBackground = true;
                thread.Start(serverSocket);
            }
            catch(Exception dd)
            {
                MessageBox.Show(dd.Message);
            }
            
        }
        private void ListenMr(object o)
        {
            var serverSocket = o as Socket;
            while (true)
            {
                //等待连接并且创建一个负责通讯的socket
                var send = serverSocket.Accept();
                //获取链接的IP地址
                //var sendIpoint = send.RemoteEndPoint.ToString();
                //开启一个新线程不停接收消息
                Thread thread = new Thread(ReciveMr);
                thread.IsBackground = true;
                thread.Start(send);
            }
        }
        private void ReciveMr(object o)
        {
            var send = o as Socket; 
            totalByteRead = new Byte[0];
            while (true)
            {
                //获取发送过来的消息容器
                byte[] buffer = new byte[1024 * 5];
                var effective = 0;
                try
                {
                    effective = send.Receive(buffer);
                }
                catch { break; }
                //有效字节为0则跳过
                if (effective == 0)
                {
                    break;
                }
                byte[] buffernew = buffer.Skip(0).Take(effective).ToArray();
                totalByteRead = totalByteRead.Concat(buffernew).ToArray();
                if (totalByteRead.Length < 100) { continue; }
                string sHL7 = Encoding.Default.GetString(totalByteRead).Trim();
                string sendHL7new = "";
                string sendHL7 = "MSH|^~\\&|LIS||||20361231235956||ACK^R01|1|P|2.3.1||||||UNICODE"
                                + "MSA|AA|1";
                string[] sendArray = sendHL7.Split('|');
                string sendHL7sh = "MSH|^~\\&|||||20120508094823||ACK^R01|1|P|2.3.1||||2||ASCII|||"
                                  + "MSA|AA|1|Message accepted|||0|";
                string[] sendArraysh = sendHL7sh.Split('|');
                if (sHL7.IndexOf("ASCII") > 0)
                {//解析生化协议报文数据
                    if (sHL7.Substring(0, 3) != "MSH" || sHL7.Substring(sHL7.Length - 1, 1) != "|")
                    {
                        continue;
                    }
                    UpLoadDataForSH sh = new UpLoadDataForSH(); 
                    string[] sHL7Pids = Regex.Split(sHL7, "PID", RegexOptions.IgnoreCase);
                    if (sHL7Pids.Length == 0) { return; };
                    string[] MSHArray = sHL7Pids[0].Split('|');
                    sendArraysh[6] = MSHArray[6];
                    sendArraysh[9] = MSHArray[9];
                    sendArraysh[22] = MSHArray[9];
                    string[] sHL7PArray = sHL7Pids[1].Split('|');
                    sh.bar_code = sHL7PArray[33]; 
                    //把HL7分成段
                    string[] sHL7Lines = Regex.Split(sHL7, "OBX", RegexOptions.IgnoreCase);
                    if (sHL7Lines.Length == 0) { return; };
                    for (int i = 1; i < sHL7Lines.Length; i++)
                    {
                        string[] sHL7Array = sHL7Lines[i].Split('|');
                        switch (sHL7Array[4])
                        {
                            case "ALB": sh.ALB = sHL7Array[5]; break;
                            case "ALP": sh.ALP = sHL7Array[5]; break;
                            case "ALT": sh.ALT = sHL7Array[5]; break;
                            case "AST": sh.AST = sHL7Array[5]; break;
                            case "TC": sh.CHO = sHL7Array[5]; break;
                            case "CREA-S": sh.CREA = sHL7Array[5]; break;
                            case "D-Bil-V": sh.DBIL = sHL7Array[5]; break;
                            case "Glu-G": sh.GLU = sHL7Array[5]; break;
                            case "HDL-C": sh.HDL_C = sHL7Array[5]; break;
                            case "LDL-C": sh.LDL_C = sHL7Array[5]; break;
                            case "T-Bil-V": sh.TBIL = sHL7Array[5]; break;
                            case "TG": sh.TG = sHL7Array[5]; break;
                            case "TP": sh.TP = sHL7Array[5]; break;
                            case "UA": sh.UA = sHL7Array[5]; break;
                            case "UREA": sh.UREA = sHL7Array[5]; break;
                            default: break;
                        }
                    }
                    sh.createtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //插入
                    RePushDataShHandler handler = new RePushDataShHandler(PushDataForSh);
                    DisplayData obj = new DisplayData();
                    obj.BarCode = sh.bar_code;
                    obj.UploadDate = sh.createtime;
                    var q = (from l in _existShList where l.BarCode == sh.bar_code && l.UploadDate == sh.createtime select l).ToList();
                    if (q.Count <= 0)
                    { 
                        _existShList.Add(obj); 
                    }
                    else
                    {
                        for (int i = 0; i < _existShList.Count; i++)
                        {
                            if (obj.BarCode == _existShList[i].BarCode)
                            {
                                _existShList.RemoveAt(i);
                                break;
                            }
                        }
                        _existShList.Add(obj);
                    }
                    IAsyncResult result = handler.BeginInvoke(sh, new AsyncCallback(CallBackPushShDataFunc), sh);
                    //返回生化的确认数据报文
                    for (int j = 0; j < sendArraysh.Length; j++)
                    {
                        sendHL7sh += "|" + sendArraysh[j];
                    }
                    byte[] sendBytes = Encoding.Unicode.GetBytes(sendHL7sh.Substring(1));
                    byte[] sendBytes1 = { 0x0B };
                    byte[] sendBytes2 = { 0x1C, 0x0D };
                    sendBytes = sendBytes1.Concat(sendBytes).Concat(sendBytes2).ToArray();
                    send.Send(sendBytes);
                }
                else if (sHL7.IndexOf("UNICODE") > 1)
                {//解析血球协议报文数据
                    try
                    {
                        if (sHL7.Substring(0, 3) != "MSH" || sHL7.Substring(sHL7.Length - 1, 1) != "F")
                        {
                            continue;
                        }
                        UpLoadDataForXCG xcg = new UpLoadDataForXCG(); 
                        string[] sHL7Pids = Regex.Split(sHL7, "PID", RegexOptions.IgnoreCase);
                        if (sHL7Pids.Length == 0) { return; };
                        string[] MSHArray = sHL7Pids[0].Split('|');
                        sendArray[6] = MSHArray[6];
                        sendArray[9] = MSHArray[9];
                        sendArray[19] = MSHArray[9];
                        string[] sHL7PArray = sHL7Pids[1].Split('|');
                        xcg.bar_code = sHL7PArray[14]; 
                        xcg.createtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        //把HL7分成段
                        string[] sHL7Lines = Regex.Split(sHL7, "OBX", RegexOptions.IgnoreCase);
                        if (sHL7Lines.Length == 0) { return; };
                        for (int i = 1; i < sHL7Lines.Length; i++)
                        {
                            string[] sHL7Array = sHL7Lines[i].Split('|');
                            if (sHL7Array[2].IndexOf("NM") == -1)
                            {
                                continue;
                            }
                            if (sHL7Array[3].IndexOf("WBC^LN") > -1)
                            {
                                xcg.WBC = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("LYM#") > -1)
                            {
                                xcg.LYM = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("LYM%") > -1)
                            {
                                xcg.LYMP = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("RBC^LN") > -1)
                            {
                                xcg.RBC = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("HGB^LN") > -1)
                            {
                                xcg.HGB = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("PCT") > -1)
                            {
                                xcg.PCT = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("PLT^LN") > -1)
                            {
                                xcg.PLT = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("HCT") > -1)
                            {
                                xcg.HCT = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("MCV") > -1)
                            {
                                xcg.MCV = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("MCH^LN") > -1)
                            {
                                xcg.MCH = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("MCHC^LN") > -1)
                            {
                                xcg.MCHC = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("RDW-CV") > -1)
                            {
                                xcg.RDW_CV = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("RDW-SD") > -1)
                            {
                                xcg.RDW_SD = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("NEU#") > -1)
                            {
                                xcg.NEUT = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("NEU%") > -1)
                            {
                                xcg.NEUTP = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("MPV") > -1)
                            {
                                xcg.MPV = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("PDW^LN") > -1)
                            {
                                xcg.PDW = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("MID#") > -1)
                            {
                                xcg.MXD = sHL7Array[5]; continue;
                            }
                            else if (sHL7Array[3].IndexOf("MID%") > -1)
                            {
                                xcg.MXDP = sHL7Array[5]; continue;
                            }
                            else
                            {
                                continue;
                            }
                        } 
                        RePushDataXcgHandler handler = new RePushDataXcgHandler(PushDataForXcg);
                        DisplayData obj = new DisplayData();
                        obj.BarCode = xcg.bar_code;
                        obj.UploadDate = xcg.createtime;
                        var q = (from l in _existXcgList where l.BarCode == xcg.bar_code && l.UploadDate == xcg.createtime select l).ToList();
                        if (q.Count <= 0)
                        { 
                            _existXcgList.Add(obj); 
                        }
                        else
                        {
                            for (int i = 0; i < _existXcgList.Count; i++)
                            {
                                if (obj.BarCode == _existXcgList[i].BarCode)
                                {
                                    _existXcgList.RemoveAt(i);
                                    break;
                                }
                            }
                            _existXcgList.Add(obj);
                        }
                        IAsyncResult result = handler.BeginInvoke(xcg, new AsyncCallback(CallBackPushXcgDataFunc), xcg);
                        //返回血球的确认数据报文
                        for (int j = 0; j < sendArray.Length; j++)
                        {
                            sendHL7new += "|" + sendArray[j];
                        }
                        byte[] sendBytes = Encoding.Unicode.GetBytes(sendHL7new.Substring(1));
                        byte[] sendBytes1 = { 0x0B };
                        byte[] sendBytes2 = { 0x1C, 0x0D };
                        sendBytes = sendBytes1.Concat(sendBytes).Concat(sendBytes2).ToArray();
                        send.Send(sendBytes);
                    }
                    catch (Exception ex)
                    {
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Application.StartupPath + "/log.txt", true))
                        {
                            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "\n" + ex.Message + "\n" + ex.StackTrace);
                        }
                    }
                }
                totalByteRead = new Byte[0];
            }
        }

        #endregion

        #region 英诺华解析
        bool ylhFlag = false;
        private void timer4_Tick(object sender, EventArgs e)
        {
            try
            {
                if (ylhFlag == true) return;
                lblerror1.Visible = false;
                ylhFlag = true;
                getShengHuaForYLH();   //生化
                getXuechangguiForYLH();//血常规 
            }
            finally
            {
                ylhFlag = false;
            } 
        }

        
        private void  getShengHuaForYLH()
        {
            lblerrinfo.Text = ""; 
            if (shenghuaPath == "" || !File.Exists(shenghuaPath))
            {
                lblerrinfo.Text="未获取到生化中间库地址，请检查是否设置地址！";
                return;
            }
            else
            {
                bool bl = shenghuaPath.IndexOf("Lis_DB.mdb") > -1 ? true : false;
                if (bl == false)
                {
                    lblerrinfo.Text="生化中间库地址不正确，请检查是否设置地址！";
                    return ;
                }
            }
            string sql1 = "select sample_id,patient_id,send_time from LisOutput where send_time > cdate('" + shlasttime + "') order by send_time asc";
            DataSet ds = LogicHandler.getShenghuaXueChangGuiForYLH(sql1, shenghuaPath);
            if (ds == null || ds.Tables.Count < 1)
            { 
                return ;
            }
            RePushDataShHandler handler = new RePushDataShHandler(PushDataForSh);
            DataTable arr_dt1 = ds.Tables[0];
            if (arr_dt1.Rows.Count > 0)
            {
                for (int j = 0; j < arr_dt1.Rows.Count; j++)
                {
                    string sql2 = "select lop.patient_id,lop.send_time,lopr.* from LisOutput lop, LisOutputResult lopr where lop.sample_id=lopr.sample_id and lop.sample_id='" + arr_dt1.Rows[j]["sample_id"].ToString() + "'";
                    DataSet ds1 = LogicHandler.getShenghuaXueChangGuiForYLH(sql2, shenghuaPath);
                    DataTable arr_dt2 = ds1.Tables[0];
                    if (arr_dt2.Rows.Count > 0)
                    {
                        UpLoadDataForSH sh = new UpLoadDataForSH(); 
                        sh.bar_code = arr_dt1.Rows[j]["patient_id"].ToString();
                        sh.createtime = Convert.ToDateTime(arr_dt1.Rows[j]["send_time"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"); 
                        for (int i = 0; i < arr_dt2.Rows.Count; i++)
                        {
                            string item = arr_dt2.Rows[i]["item"].ToString();
                            switch (item)
                            {
                                case "ALB": sh.ALB = arr_dt2.Rows[i]["result"].ToString(); break;
                                case "ALP": sh.ALP = arr_dt2.Rows[i]["result"].ToString(); break;
                                case "ALT": sh.ALT = arr_dt2.Rows[i]["result"].ToString(); break;
                                case "AST": sh.AST = arr_dt2.Rows[i]["result"].ToString(); break;
                                case "CHO": sh.CHO = arr_dt2.Rows[i]["result"].ToString(); break;
                                case "Crea": sh.CREA = arr_dt2.Rows[i]["result"].ToString(); break;
                                case "DBIL": sh.DBIL = arr_dt2.Rows[i]["result"].ToString(); break;
                                case "GGT": sh.GGT = arr_dt2.Rows[i]["result"].ToString(); break;
                                case "GLU": sh.GLU = arr_dt2.Rows[i]["result"].ToString(); break;
                                case "HDL_C": sh.HDL_C = arr_dt2.Rows[i]["result"].ToString(); break;
                                case "LDL_C": sh.LDL_C = arr_dt2.Rows[i]["result"].ToString(); break;
                                case "TBIL": sh.TBIL = arr_dt2.Rows[i]["result"].ToString(); break;
                                case "TG": sh.TG = arr_dt2.Rows[i]["result"].ToString(); break;
                                case "TP": sh.TP = arr_dt2.Rows[i]["result"].ToString(); break;
                                case "UA": sh.UA = arr_dt2.Rows[i]["result"].ToString(); break;
                                case "UREA": sh.UREA = arr_dt2.Rows[i]["result"].ToString(); break;
                                default: break;
                            }
                        }
                        if (sh.ALT == "N/A") { sh.ALT = "0"; };
                        if (sh.AST == "N/A") { sh.AST = "0"; };
                        if (sh.TBIL == "N/A") { sh.TBIL = "0"; };
                        if (sh.CREA == "N/A") { sh.CREA = "0"; };
                        if (sh.UREA == "N/A") { sh.UREA = "0"; };
                        if (sh.GLU == "N/A") { sh.GLU = "0"; };
                        if (sh.TG == "N/A") { sh.TG = "0"; };
                        if (sh.CHO == "N/A") { sh.CHO = "0"; };
                        if (sh.HDL_C == "N/A") { sh.HDL_C = "0"; };
                        if (sh.LDL_C == "N/A") { sh.LDL_C = "0"; };
                        DisplayData obj = new DisplayData();
                        obj.BarCode = sh.bar_code;
                        obj.UploadDate = sh.createtime;
                        var q = (from l in _existShList where l.BarCode == sh.bar_code && l.UploadDate == sh.createtime select l).ToList();
                        if(q.Count<=0)
                        { 
                            _existShList.Add(obj); 
                        }
                        else
                        {
                            for (int i = 0; i < _existShList.Count; i++)
                            {
                                if (obj.BarCode == _existShList[i].BarCode)
                                {
                                    _existShList.RemoveAt(i);
                                    break;
                                }
                            }
                            _existShList.Add(obj);
                        }
                        IAsyncResult result = handler.BeginInvoke(sh, new AsyncCallback(CallBackPushShDataFunc), sh);
                    }
                }
            } 
        }

        private void getXuechangguiForYLH()
        { 
            lblerrinfo.Text = "";
            if (xuechangguiPath == "" || !File.Exists(xuechangguiPath))
            {
                lblerrinfo.Text = "未获取到血球中间库地址，请检查是否设置地址！";
                return ;
            }
            else
            {
                bool bl = xuechangguiPath.IndexOf("Lis_DB.mdb") > -1 ? true : false;
                if (bl == false) { lblerrinfo.Text = "血球中间库地址不正确，请检查是否设置地址！"; return ; }
                string sql1 = "select sample_id,patient_id,send_time from LisOutput where send_time > cdate('" + xcglasttime + "') order by send_time asc";
                DataSet ds = LogicHandler.getShenghuaXueChangGuiForYLH(sql1, xuechangguiPath);
                if (ds == null || ds.Tables.Count < 1) { return ; }
                RePushDataXcgHandler handler = new RePushDataXcgHandler(PushDataForXcg);
                DataTable arr_dt1 = ds.Tables[0];
                if (arr_dt1 != null && arr_dt1.Rows.Count > 0)
                {
                    for (int j = 0; j < arr_dt1.Rows.Count; j++)
                    {
                        string sql2 = "select lop.patient_id,lop.send_time,lopr.* from LisOutput lop, LisOutputResult lopr where lop.sample_id=lopr.sample_id and lop.sample_id='" + arr_dt1.Rows[j]["sample_id"].ToString() + "'";
                        DataTable arr_dt2 = LogicHandler.getShenghuaXueChangGuiForYLH(sql2, xuechangguiPath).Tables[0];
                        if (arr_dt2.Rows.Count > 0)
                        {
                            UpLoadDataForXCG xcg = new UpLoadDataForXCG(); 
                            xcg.bar_code = arr_dt1.Rows[j]["patient_id"].ToString();  
                            DateTime newtime = Convert.ToDateTime(arr_dt1.Rows[j]["send_time"].ToString());
                            DateTime oldtime = Convert.ToDateTime(xcglasttime);
                            if (newtime <= oldtime)
                            {
                                continue;
                            }
                            xcg.createtime = newtime.ToString("yyyy-MM-dd HH:mm:ss");
                            for (int i = 0; i < arr_dt2.Rows.Count; i++)
                            {
                                string item = arr_dt2.Rows[i]["item"].ToString();
                                switch (item)
                                {
                                    case "HCT": xcg.HCT = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "HGB": xcg.HGB = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "LYM#": xcg.LYM = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "LYM%": xcg.LYMP = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "MCH": xcg.MCH = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "MCHC": xcg.MCHC = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "MCV": xcg.MCV = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "MPV": xcg.MPV = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "MXD#": xcg.MXD = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "MXD%": xcg.MXDP = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "NEUT#": xcg.NEUT = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "NEUT%": xcg.NEUTP = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "PCT": xcg.PCT = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "PDW": xcg.PDW = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "PLT": xcg.PLT = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "RBC": xcg.RBC = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "RDW_CV": xcg.RDW_CV = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "RDW_SD": xcg.RDW_SD = arr_dt2.Rows[i]["result"].ToString(); break;
                                    case "WBC": xcg.WBC = arr_dt2.Rows[i]["result"].ToString(); break;
                                    default: break;
                                }
                            }
                            DisplayData obj = new DisplayData();
                            obj.BarCode = xcg.bar_code;
                            obj.UploadDate = xcg.createtime;
                            var q = (from l in _existXcgList where l.BarCode == xcg.bar_code && l.UploadDate == xcg.createtime select l).ToList();
                            if (q.Count <= 0)
                            { 
                                _existXcgList.Add(obj); 
                            }
                            else
                            {
                                for (int i = 0; i < _existXcgList.Count; i++)
                                {
                                    if (obj.BarCode == _existXcgList[i].BarCode)
                                    {
                                        _existXcgList.RemoveAt(i);
                                        break;
                                    }
                                }
                                _existXcgList.Add(obj);
                            }
                            IAsyncResult result = handler.BeginInvoke(xcg, new AsyncCallback(CallBackPushXcgDataFunc), xcg);
                        }
                    }
                }
            }
        }
        #endregion

        #region 正常发送显示界面
        private bool PushDataForSh(UpLoadDataForSH obj)
        {
            bool flag = false;
            try
            {
                string err1 = "";
                flag = LogicHandler.UpLoadDatashDal(obj, out err1); 
                if (flag == false) //插入本地表中以便下次补发
                { 
                    LogicHandler.InsertAccessSh(obj, out err1);
                }
            }
            catch
            {

            }
            return flag;
        }
        private void CallBackPushShDataFunc(IAsyncResult result)
        {
            try
            {
                RePushDataShHandler handler = (RePushDataShHandler)((AsyncResult)result).AsyncDelegate;
                bool flag = handler.EndInvoke(result);
                UpLoadDataForSH obj = (UpLoadDataForSH)result.AsyncState;
                if (flag == true)
                {
                    DisplayData obj1 = Common.GetobjDisplaySh(obj);
                    ShowMessage(listView1, obj1);   //显示在listview中
                    ShowMessage1(lblsuccess, 1);    //成功的显示发送成功个数
                }
            }
            catch
            { }
        }
        private bool PushDataForXcg(UpLoadDataForXCG obj)
        {
            bool flag = false;
            try
            {
                string err1 = "";
                flag = LogicHandler.UpLoadDataxcgDal(obj, out err1);
                if (flag == false) //插入本地表中以便下次补发
                {
                    LogicHandler.InsertAccessXCG(obj, out err1);
                }
            }
            catch
            {
            }
            return flag;
        }
        private void CallBackPushXcgDataFunc(IAsyncResult result)
        {
            try
            {
                RePushDataXcgHandler handler = (RePushDataXcgHandler)((AsyncResult)result).AsyncDelegate;
                bool flag = handler.EndInvoke(result);
                UpLoadDataForXCG obj = (UpLoadDataForXCG)result.AsyncState;
                if (flag == true)
                {
                    DisplayData obj1 = Common.GetobjDisplayXCG(obj);
                    ShowMessage(listView1, obj1);
                    ShowMessage1(lblsuccess, 1);
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 库贝尔
        public SerialPort port = new SerialPort();
        Byte[] totalByteRead = new Byte[0];
        private bool initPort()
        {
            string com = comname;
            try
            {
                if (!port.IsOpen)
                {
                    string portName = com;
                    int baudRate = 115200;
                    port.PortName = portName;
                    port.BaudRate = baudRate;
                    port.DtrEnable = true;
                    port.ReceivedBytesThreshold = 1;
                    port.Open();
                    return true;
                }
                else
                {
                    lblerrinfo.Text = "库贝尔血球串口连接失败,请联系运维人员!";
                    return false;
                }
            }
            catch (Exception ee)
            {
                lblerrinfo.Text = "库贝尔血球串口连接失败,请联系运维人员!";
                return false;
            }
        }

        private void mySerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            bool isCRC = false;
            Thread.Sleep(12000);
            try
            {
                SerialPort sp = (SerialPort)sender;
                string text = string.Empty;
                Byte[] byteRead = new Byte[sp.BytesToRead];
                if (byteRead.Length == 0)
                {
                    return;
                }
                sp.Read(byteRead, 0, byteRead.Length);
                //sp.DiscardInBuffer();
                //sp.DiscardOutBuffer();
                totalByteRead = totalByteRead.Concat(byteRead).ToArray();
                text =Common.ToHexString(totalByteRead);
                if (totalByteRead.Length > 1000)
                {
                    string beginText = text.Substring(0, 16);
                    string endText = text.Substring(text.Length - 18, 18);
                    if (beginText == "3C73616D706C653E" && endText == "3C2F73616D706C653E")
                    {
                        text = Encoding.ASCII.GetString(totalByteRead);
                        isCRC = true;
                    }
                    string endText1 = text.Substring(text.Length - 22, 22);
                    if (beginText == "3C73616D706C653E" && endText1 == "3C2F73616D706C653E0D0A")
                    {
                        text = Encoding.ASCII.GetString(totalByteRead);
                        isCRC = true;
                    }
                }

                if (isCRC)
                {
                    //using (StreamWriter sw = new StreamWriter(Application.StartupPath + "/log.txt", true))
                    //{
                    //    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "接收报文2：" + text );
                    //}
                    Thread multiAdd = new Thread(parsingTextData);
                    multiAdd.IsBackground = true;
                    multiAdd.Start(text);
                    totalByteRead = new Byte[0];
                }
            }
            catch (Exception ee)
            {
                using (StreamWriter sw = new StreamWriter(Application.StartupPath + "/log.txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "异常报文1：" +Common.ToHexString(totalByteRead));
                }
            }
        }

      

        public void parsingTextData(object parameter)
        {
            UpLoadDataForXCG xcg = new UpLoadDataForXCG();
            try
            {
                string xmlStr = @parameter.ToString(); 
                var doc = new XmlDocument();
                doc.LoadXml(xmlStr);
                var rowNoteList = doc.SelectNodes("/sample/smpinfo/p");
                var fieldNodeID = rowNoteList[0].ChildNodes;
                string barcode = fieldNodeID[1].InnerText;
                string[] barcodes = barcode.Split('/');
                xcg.bar_code = barcodes[0].Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Trim();
                var fieldNodeTime = rowNoteList[2].ChildNodes;
                string timeNow = fieldNodeTime[1].InnerText;
                timeNow = timeNow.Replace("T", " ") + ":00";
                xcg.createtime = timeNow; //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var smpresultsList = doc.SelectNodes("/sample/smpresults/p");
                foreach (XmlNode rowNode in smpresultsList)
                {
                    var fieldNodeList = rowNode.ChildNodes;
                    string type = fieldNodeList[0].InnerText;
                    switch (type)
                    {
                        case "HCT": xcg.HCT = fieldNodeList[1].InnerText; break;
                        case "HGB": xcg.HGB = fieldNodeList[1].InnerText; break;
                        case "LYMHC": xcg.LYM = fieldNodeList[1].InnerText; break;
                        case "LYMHR": xcg.LYMP = fieldNodeList[1].InnerText; break;
                        case "MCH": xcg.MCH = fieldNodeList[1].InnerText; break;
                        case "MCHC": xcg.MCHC = fieldNodeList[1].InnerText; break;
                        case "MCV": xcg.MCV = fieldNodeList[1].InnerText; break;
                        case "MPV": xcg.MPV = fieldNodeList[1].InnerText; break;
                        case "MIDC": xcg.MXD = fieldNodeList[1].InnerText; break;
                        case "MIDR": xcg.MXDP = fieldNodeList[1].InnerText; break;
                        case "NEUTC": xcg.NEUT = fieldNodeList[1].InnerText; break;
                        case "NEUTR": xcg.NEUTP = fieldNodeList[1].InnerText; break;
                        case "PCT": xcg.PCT = fieldNodeList[1].InnerText; break;
                        case "PDW": xcg.PDW = fieldNodeList[1].InnerText; break;
                        case "PLT": xcg.PLT = fieldNodeList[1].InnerText; break;
                        case "RBC": xcg.RBC = fieldNodeList[1].InnerText; break;
                        case "RDW-CV": xcg.RDW_CV = fieldNodeList[1].InnerText; break;
                        case "RDW-SD": xcg.RDW_SD = fieldNodeList[1].InnerText; break;
                        case "WBC": xcg.WBC = fieldNodeList[1].InnerText; break;
                        case "MONC": xcg.MONO = fieldNodeList[1].InnerText; break;
                        case "MONP": xcg.MONOP = fieldNodeList[1].InnerText; break;
                        case "GRAC": xcg.GRAN = fieldNodeList[1].InnerText; break;
                        case "GRAP": xcg.GRANP = fieldNodeList[1].InnerText; break;
                        case "P-LCR": xcg.PLCR = fieldNodeList[1].InnerText; break;
                        default: break; 
                    }
                }
            }
            catch (Exception ee)
            {
                using (StreamWriter sw = new StreamWriter(Application.StartupPath + "/log.txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "异常报文解析：" + ee.Message + "--" + ee.StackTrace);
                }
                return;
            }
            RePushDataXcgHandler handler = new RePushDataXcgHandler(PushDataForXcg);
            DisplayData obj = new DisplayData();
            obj.BarCode = xcg.bar_code;
            obj.UploadDate = xcg.createtime;
            var q = (from l in _existXcgList where l.BarCode == xcg.bar_code && l.UploadDate == xcg.createtime select l).ToList();
            if (q.Count <= 0)
            { 
                _existXcgList.Add(obj);  
            }
            else
            {
                for (int i = 0; i < _existXcgList.Count; i++)
                {
                    if (obj.BarCode == _existXcgList[i].BarCode)
                    {
                        _existXcgList.RemoveAt(i);
                        break;
                    }
                }
                _existXcgList.Add(obj);
            }
            IAsyncResult result = handler.BeginInvoke(xcg, new AsyncCallback(CallBackPushXcgDataFunc), xcg);
        }

        private void socketTcpKbe()
        {
            try
            {
                string hostName = Dns.GetHostName();   //获取本机名
                IPHostEntry localhost = Dns.GetHostByName(hostName);//方法已过期，可以获取IPv4的地址
                                                                    //IPAddress ip = localhost.AddressList[0];
                IPAddress ip = IPAddress.Parse(strIP);
                Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint point = new IPEndPoint(ip, int.Parse(strPort));
                //socket绑定监听地址
                serverSocket.Bind(point);
                //设置同时连接个数
                serverSocket.Listen(10);

                //利用线程后台执行监听,否则程序会假死
                Thread thread = new Thread(ListenKbe);
                thread.IsBackground = true;
                thread.Start(serverSocket);
            }
            catch(Exception d)
            {
                MessageBox.Show(d.Message);
            }
        }
        private void ListenKbe(object o)
        {
            var serverSocket = o as Socket;
            while (true)
            {
                //等待连接并且创建一个负责通讯的socket
                var send = serverSocket.Accept();
                //获取链接的IP地址
                //var sendIpoint = send.RemoteEndPoint.ToString();
                //开启一个新线程不停接收消息
                Thread thread = new Thread(ReciveKbe);
                thread.IsBackground = true;
                thread.Start(send);
            }
        }
        private byte[] AckKbe(string str)
        {
            string[] astr = Regex.Split(str, "MSA", RegexOptions.IgnoreCase);
            
            string a = astr[0];
            string b = "MSA"+astr[1];
            int num = a.Length + b.Length + 5;
            byte[] c = new byte[num];
            byte[] a1 = Encoding.ASCII.GetBytes(a);
            Array.Copy(a1, 0, c, 1, a1.Length);
            byte[] b1 = Encoding.ASCII.GetBytes(b);
            Array.Copy(b1, 0, c, a1.Length+2, b1.Length);
            //特殊处理的几个值
            c[0] = 0x0B;
            c[a1.Length + 1] = 0x0D;
            c[num - 3] = 0x0D;
            c[num - 2] = 0x1C;
            c[num - 1] = 0x0D;
            return c;
        }
        private void ReciveKbe(object o)
        {
            try
            {
                var send = o as Socket;
                RePushDataShHandler handler = new RePushDataShHandler(PushDataForSh);
                while (true)
                {
                    //获取发送过来的消息容器
                    byte[] buffer = new byte[1024 * 2];
                    var effective = 0;
                    try
                    {
                        effective = send.Receive(buffer);
                    }
                    catch { break; }
                    //有效字节为0则跳过
                    if (effective == 0)
                    {
                        break;
                    }
                    string sendHL7new = "";
                    //string sendHL7 = "MSH|^~\\&|||ICUBIO|740|20190708162020||ACK^R01|4|P|2.3.1||||0||ASCII|||MSA|AA|4|Message accepted|||0|";
                    string sendHL7 = "MSH|^~\\&|||ICUBIO|740|20190821110721||ACK^R01|1|P|2.3.1||||0||ASCII|||MSA|AA|1|Message accepted|||0|";
                    string[] sendArray = sendHL7.Split('|');
                     
                    byte[] buffernew = buffer.Skip(0).Take(effective).ToArray();
                    /**************调试用**************/
                    //string str = "";
                    //for(int i=0;i< buffernew.Length;i++)
                    //{
                    //    str = str + buffernew[i].ToString("X2");
                    //}
                    /**************end**************/
                    string sHL7 = Encoding.Default.GetString(buffernew).Trim();
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Application.StartupPath + "/log.txt", true))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "\n" + sHL7);
                    }
                    if (sHL7.IndexOf("ICUBIO") > 0)
                    {//解析生化协议报文数据                   
                        UpLoadDataForSH sh = new UpLoadDataForSH(); 
                        string[] sHL7Pids = Regex.Split(sHL7, "PID", RegexOptions.IgnoreCase);
                        if (sHL7Pids.Length == 0) { return; };
                        string[] MSHArray = sHL7Pids[0].Split('|');
                        //sendArray[3] = MSHArray[3];
                        sendArray[6] = MSHArray[6];
                        sendArray[9] = MSHArray[9];
                        sendArray[11] = MSHArray[11];
                        sendArray[22] = MSHArray[9];
                        string[] sHL7PArray = sHL7Pids[1].Split('|');
                        sh.bar_code = sHL7PArray[33]; 
                        //把HL7分成段
                        string[] sHL7Lines = Regex.Split(sHL7, "OBX", RegexOptions.IgnoreCase);
                        if (sHL7Lines.Length == 0) { return; };
                        for (int i = 1; i < sHL7Lines.Length; i++)
                        {
                            string[] sHL7Array = sHL7Lines[i].Split('|');
                            if (sHL7Array[5] == "" || "".Equals(sHL7Array[5]))
                            {
                                continue;
                            }
                            switch (sHL7Array[4])
                            {
                                case "ALB": sh.ALB = sHL7Array[5].Substring(0, sHL7Array[5].IndexOf('.') + 3); break;
                                case "ALP": sh.ALP = sHL7Array[5].Substring(0, sHL7Array[5].IndexOf('.') + 3); break;
                                case "ALT": sh.ALT = sHL7Array[5].Substring(0, sHL7Array[5].IndexOf('.')); break;
                                case "AST": sh.AST = sHL7Array[5].Substring(0, sHL7Array[5].IndexOf('.')); break;
                                case "CHO": sh.CHO = sHL7Array[5].Substring(0, sHL7Array[5].IndexOf('.') + 3); break;
                                case "CREA": sh.CREA = sHL7Array[5].Substring(0, sHL7Array[5].IndexOf('.') + 2); break;
                                case "D-BIL": sh.DBIL = sHL7Array[5].Substring(0, sHL7Array[5].IndexOf('.') + 3); break;
                                case "GGT": sh.GGT = sHL7Array[5].Substring(0, sHL7Array[5].IndexOf('.') + 3); break;
                                case "GLU": sh.GLU = sHL7Array[5].Substring(0, sHL7Array[5].IndexOf('.') + 3); break;
                                case "HDL": sh.HDL_C = sHL7Array[5].Substring(0, sHL7Array[5].IndexOf('.') + 3); break;
                                case "LDL": sh.LDL_C = sHL7Array[5].Substring(0, sHL7Array[5].IndexOf('.') + 3); break;
                                case "T-BIL": sh.TBIL = sHL7Array[5].Substring(0, sHL7Array[5].IndexOf('.') + 2); break;
                                case "TG": sh.TG = sHL7Array[5].Substring(0, sHL7Array[5].IndexOf('.') + 3); break;
                                case "TP": sh.TP = sHL7Array[5].Substring(0, sHL7Array[5].IndexOf('.') + 3); break;
                                case "UA": sh.UA = sHL7Array[5].Substring(0, sHL7Array[5].IndexOf('.') + 3); break;
                                case "UREA": sh.UREA = sHL7Array[5].Substring(0, sHL7Array[5].IndexOf('.') + 3); break;
                                default: break;
                            }
                        }
                        //返回生化的确认数据报文
                        for (int j = 0; j < sendArray.Length; j++)
                        {
                            sendHL7new += "|" + sendArray[j];
                        }
                        //byte[] sendBytes = Encoding.Unicode.GetBytes(sendHL7new.Substring(1));
                        //byte[] sendBytes = Encoding.ASCII.GetBytes(sendHL7new.Substring(1));
                        byte[] sendBytes = AckKbe(sendHL7new.Substring(1));
                        /**************调试用**************/
                        //string str1 = "";
                        //for (int i = 0; i < sendBytes.Length; i++)
                        //{
                        //    str1 = str1 + sendBytes[i].ToString("X2");
                        //}
                        //send.Send(sendBytes);
                        /**************end**************/
                        sh.createtime = DateTime.ParseExact(sHL7PArray[38], "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture).ToString("yyyy-MM-dd HH:mm:ss");
                        //插入
                        DisplayData obj = new DisplayData();
                        obj.BarCode = sh.bar_code;
                        obj.UploadDate = sh.createtime;
                        var q = (from l in _existShList where l.BarCode == sh.bar_code && l.UploadDate == sh.createtime select l).ToList();
                        if (q.Count <= 0)
                        {
                            _existShList.Add(obj);
                        }
                        else
                        {
                            for(int i=0;i< _existShList.Count;i++)
                            {
                                if(obj.BarCode== _existShList[i].BarCode)
                                {
                                    _existShList.RemoveAt(i);
                                    break;
                                }
                            }
                            _existShList.Add(obj);
                        }    
                        IAsyncResult result = handler.BeginInvoke(sh, new AsyncCallback(CallBackPushShDataFunc), sh);
                         
                    }
                }
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Application.StartupPath + "/log.txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "\n" + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
        #endregion

        #region 雷杜
        private void socketTcp()
        {
            try
            {
                string hostName = Dns.GetHostName();   //获取本机名
                IPHostEntry localhost = Dns.GetHostByName(hostName);//方法已过期，可以获取IPv4的地址
                                                                    //IPAddress ip = localhost.AddressList[0];
                IPAddress ip = IPAddress.Parse(strIP);
                Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint point = new IPEndPoint(ip, int.Parse(strPort));
                //socket绑定监听地址
                serverSocket.Bind(point);
                //设置同时连接个数
                serverSocket.Listen(10);

                //利用线程后台执行监听,否则程序会假死
                Thread thread = new Thread(Listen);
                thread.IsBackground = true;
                thread.Start(serverSocket);
            }
            catch(Exception d)
            {
                MessageBox.Show(d.Message);
            }
        }
        private void Listen(object o)
        {
            var serverSocket = o as Socket;
            while (true)
            {
                //等待连接并且创建一个负责通讯的socket
                var send = serverSocket.Accept();
                //获取链接的IP地址
                //var sendIpoint = send.RemoteEndPoint.ToString();
                //开启一个新线程不停接收消息
                Thread thread = new Thread(Recive);
                thread.IsBackground = true;
                thread.Start(send);
            }
        }

        private void Recive(object o)
        {
            var send = o as Socket;
            while (true)
            {
                //获取发送过来的消息容器
                byte[] buffer = new byte[1024 * 2];
                var effective = 0;
                try
                {
                    effective = send.Receive(buffer);
                }
                catch { break; }
                //有效字节为0则跳过
                if (effective == 0)
                {
                    break;
                }
                string sendHL7new = "";
                string sendHL7 = "MSH|^~\\&|||Rayto||1||ACK^R01|1|P|2.3.1||||S||UNICODE|||MSA|AA|1|||||";
                string[] sendArray = sendHL7.Split('|');
                byte[] buffernew = buffer.Skip(0).Take(effective).ToArray();
                string sHL7 = Encoding.Default.GetString(buffernew).Trim();
                if (sHL7.IndexOf("CHEMRAY420") > 0)
                {//解析生化协议报文数据                   
                    UpLoadDataForSH sh = new UpLoadDataForSH(); 
                    string[] sHL7Pids = Regex.Split(sHL7, "PID", RegexOptions.IgnoreCase);
                    if (sHL7Pids.Length == 0) { return; };
                    string[] MSHArray = sHL7Pids[0].Split('|');
                    sendArray[6] = MSHArray[6];
                    sendArray[9] = MSHArray[9];
                    sendArray[17] = "ASCII";
                    sendArray[22] = MSHArray[9];
                    string[] sHL7PArray = sHL7Pids[1].Split('|');
                    sh.bar_code = sHL7PArray[34]; 
                    //把HL7分成段
                    string[] sHL7Lines = Regex.Split(sHL7, "OBX", RegexOptions.IgnoreCase);
                    if (sHL7Lines.Length == 0) { return; };
                    for (int i = 1; i < sHL7Lines.Length; i++)
                    {
                        string[] sHL7Array = sHL7Lines[i].Split('|');
                        switch (sHL7Array[4])
                        {
                            case "ALB": sh.ALB = sHL7Array[5]; break;
                            case "ALP": sh.ALP = sHL7Array[5]; break;
                            case "ALT": sh.ALT = sHL7Array[5]; break;
                            case "AST": sh.AST = sHL7Array[5]; break;
                            case "CHO": sh.CHO = sHL7Array[5]; break;
                            case "CREA": sh.CREA = sHL7Array[5]; break;
                            case "DBIL": sh.DBIL = sHL7Array[5]; break;
                            case "GGT": sh.GGT = sHL7Array[5]; break;
                            case "GLU": sh.GLU = sHL7Array[5]; break;
                            case "HDL": sh.HDL_C = sHL7Array[5]; break;
                            case "LDL": sh.LDL_C = sHL7Array[5]; break;
                            case "TBIL": sh.TBIL = sHL7Array[5]; break;
                            case "TG": sh.TG = sHL7Array[5]; break;
                            case "TP": sh.TP = sHL7Array[5]; break;
                            case "UA": sh.UA = sHL7Array[5]; break;
                            case "UREA": sh.UREA = sHL7Array[5]; break;
                            default: break;
                        }
                    }
                    sh.createtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //插入
                    RePushDataShHandler handler = new RePushDataShHandler(PushDataForSh);
                    DisplayData obj = new DisplayData();
                    obj.BarCode = sh.bar_code;
                    obj.UploadDate = sh.createtime;
                    var q = (from l in _existShList where l.BarCode == sh.bar_code && l.UploadDate == sh.createtime select l).ToList();
                    if (q.Count <= 0)
                    { 
                        _existShList.Add(obj);  
                    }
                    else
                    {
                        for (int i = 0; i < _existShList.Count; i++)
                        {
                            if (obj.BarCode == _existShList[i].BarCode)
                            {
                                _existShList.RemoveAt(i);
                                break;
                            }
                        }
                        _existShList.Add(obj);
                    }
                    IAsyncResult result = handler.BeginInvoke(sh, new AsyncCallback(CallBackPushShDataFunc), sh);
                    //返回生化的确认数据报文
                    for (int j = 0; j < sendArray.Length; j++)
                    {
                        sendHL7new += "|" + sendArray[j];
                    }
                    byte[] sendBytes = Encoding.Unicode.GetBytes(sendHL7new.Substring(1));
                    send.Send(sendBytes);
                }
                else
                {//解析血球协议报文数据
                    try
                    {
                        UpLoadDataForXCG xcg = new UpLoadDataForXCG(); 
                        string[] sHL7Pids = Regex.Split(sHL7, "PID", RegexOptions.IgnoreCase);
                        if (sHL7Pids.Length == 0) { return; };
                        string[] MSHArray = sHL7Pids[0].Split('|');
                        sendArray[6] = MSHArray[6];
                        sendArray[9] = MSHArray[9];
                        sendArray[22] = MSHArray[9];
                        string[] sHL7PArray = sHL7Pids[1].Split('|');
                        xcg.bar_code = sHL7PArray[2]; 
                        xcg.createtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        //把HL7分成段
                        string[] sHL7Lines = Regex.Split(sHL7, "OBX", RegexOptions.IgnoreCase);
                        if (sHL7Lines.Length == 0) { return; };
                        for (int i = 1; i < sHL7Lines.Length; i++)
                        {
                            string[] sHL7Array = sHL7Lines[i].Split('|');
                            switch (sHL7Array[3])
                            {
                                case "HCT": xcg.HCT = sHL7Array[5]; break;
                                case "HGB": xcg.HGB = sHL7Array[5]; break;
                                case "LYM#": xcg.LYM = sHL7Array[5]; break;
                                case "LYM%": xcg.LYMP = sHL7Array[5]; break;
                                case "MCH": xcg.MCH = sHL7Array[5]; break;
                                case "MCHC": xcg.MCHC = sHL7Array[5]; break;
                                case "MCV": xcg.MCV = sHL7Array[5]; break;
                                case "MPV": xcg.MPV = sHL7Array[5]; break;
                                case "MID#": xcg.MXD = sHL7Array[5]; break;
                                case "MID%": xcg.MXDP = sHL7Array[5]; break;
                                case "NEUT#": xcg.NEUT = sHL7Array[5]; break;
                                case "NEUT%": xcg.NEUTP = sHL7Array[5]; break;
                                case "PCT": xcg.PCT = sHL7Array[5]; break;
                                case "PDW": xcg.PDW = sHL7Array[5]; break;
                                case "PLT": xcg.PLT = sHL7Array[5]; break;
                                case "RBC": xcg.RBC = sHL7Array[5]; break;
                                case "RDW-CV": xcg.RDW_CV = sHL7Array[5]; break;
                                case "RDW-SD": xcg.RDW_SD = sHL7Array[5]; break;
                                case "WBC": xcg.WBC = sHL7Array[5]; break;
                                case "MON#": xcg.MONO = sHL7Array[5]; break;
                                case "MON%": xcg.MONOP = sHL7Array[5]; break;
                                case "GRA#": xcg.GRAN = sHL7Array[5]; break;
                                case "GRA%": xcg.GRANP = sHL7Array[5]; break;
                                case "P-LCR": xcg.PLCR = sHL7Array[5]; break;
                                default: break;
                            }
                        }
                        RePushDataXcgHandler handler = new RePushDataXcgHandler(PushDataForXcg);
                        DisplayData obj = new DisplayData();
                        obj.BarCode = xcg.bar_code;
                        obj.UploadDate = xcg.createtime;
                        var q = (from l in _existXcgList where l.BarCode == xcg.bar_code && l.UploadDate == xcg.createtime select l).ToList();
                        if (q.Count <= 0)
                        { 
                            _existXcgList.Add(obj); 
                        }
                        else
                        {
                            for (int i = 0; i < _existXcgList.Count; i++)
                            {
                                if (obj.BarCode == _existXcgList[i].BarCode)
                                {
                                    _existXcgList.RemoveAt(i);
                                    break;
                                }
                            }
                            _existXcgList.Add(obj);
                        }
                        IAsyncResult result = handler.BeginInvoke(xcg, new AsyncCallback(CallBackPushXcgDataFunc), xcg);
                        //返回血球的确认数据报文
                        for (int j = 0; j < sendArray.Length; j++)
                        {
                            sendHL7new += "|" + sendArray[j];
                        }
                        byte[] sendBytes = Encoding.Unicode.GetBytes(sendHL7new.Substring(1));
                        send.Send(sendBytes);
                    }
                    catch (Exception ex)
                    {
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Application.StartupPath + "/log.txt", true))
                        {
                            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "\n" + ex.Message + "\n" + ex.StackTrace);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
