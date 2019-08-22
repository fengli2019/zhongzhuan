using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

namespace ZkhwAnalyApp
{
    public class Common
    {
        public static DisplayData GetobjDisplayXCG(UpLoadDataForXCG a)
        {
            a.RDWSD = a.RDW_SD;
            a.RDWCV= a.RDW_CV;
            DisplayData obj = new DisplayData();
            obj.BarCode = a.bar_code;
            obj.UploadDate = a.createtime;
            obj.Type = "血常规";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"WBC:{0},RBC:{1},PCT:{2},PLT:{3},HGB:{4},HCT:{5},MCV:{6},MCH:{7},MCHC:{8},RDWCV:{9},RDWSD:{10}",
                a.WBC, a.RBC, a.PCT, a.PLT, a.HGB, a.HCT, a.MCV, a.MCH, a.MCHC, a.RDWCV, a.RDWSD);
            sb.AppendFormat(@",MONO:{0},MONOP:{1},GRAN:{2},GRANP:{3},NEUT:{4},NEUTP:{5},EO:{6},EOP:{7},BASO:{8},BASOP:{9},LYM:{10}",
                a.MONO, a.MONOP, a.GRAN, a.GRANP, a.NEUT, a.NEUTP, a.EO, a.EOP, a.BASO, a.BASOP, a.LYM);
            sb.AppendFormat(@",LYMP:{0},MPV:{1},PDW:{2},MXD:{3},MXDP:{4},PLCR:{5},OTHERS:{6}", a.LYMP, a.MPV, a.PDW, a.MXD, a.MXDP, a.PLCR, a.OTHERS);
            obj.Content = sb.ToString();
            return obj;
        }
        public static DisplayData GetobjDisplaySh(UpLoadDataForSH a)
        {
            a.LDLC= a.LDL_C;
            a.HDLC= a.HDL_C;
            DisplayData obj = new DisplayData();
            obj.BarCode = a.bar_code;
            obj.UploadDate = a.createtime;
            obj.Type = "生化";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"ALT:{0},AST:{1},TBIL:{2},DBIL:{3},CREA:{4},UREA:{5},GLU:{6},TG:{7},CHO:{8},HDLC:{9},LDLC:{10},ALB:{11},UA:{12},HCY:{13},AFP:{14},",
                a.ALT, a.AST, a.TBIL, a.DBIL, a.CREA, a.UREA, a.GLU, a.TG, a.CHO, a.HDLC, a.LDLC, a.ALB, a.UA, a.HCY, a.AFP);
            sb.AppendFormat(@"CEA:{0}, Ka:{1}, Na:{2}, TP:{3}, ALP:{4}, GGT:{5}, CHE:{6}, TBA:{7}, APOA1:{8}, APOB:{9}, CK:{10}, CKMB:{11}, LDHL:{12}, HBDH:{13}, aAMY:{14}",
                a.CEA, a.Ka, a.Na, a.TP, a.ALP, a.GGT, a.CHE, a.TBA, a.APOA1, a.APOB, a.CK, a.CKMB, a.LDHL, a.HBDH, a.aAMY);
            obj.Content = sb.ToString();
            return obj;
        }
        public static UpLoadDataForSH GetObjshByDR(DataRow dr)
        {
            UpLoadDataForSH obj = new UpLoadDataForSH();
            if(dr["ID"] !=null)
            {
                obj.ID = dr["ID"].ToString();
            }
            if (dr["bar_code"] != null)
            {
                obj.bar_code = dr["bar_code"].ToString();
            }
            if (dr["ALT"] != null)
            {
                obj.ALT = dr["ALT"].ToString();
            }
            if (dr["AST"] != null)
            {
                obj.AST = dr["AST"].ToString();
            }
            if (dr["TBIL"] != null)
            {
                obj.TBIL = dr["TBIL"].ToString();
            }
            if (dr["DBIL"] != null)
            {
                obj.DBIL = dr["DBIL"].ToString();
            }
            if (dr["CREA"] != null)
            {
                obj.CREA = dr["CREA"].ToString();
            }
            if (dr["UREA"] != null)
            {
                obj.UREA = dr["UREA"].ToString();
            }
            if (dr["GLU"] != null)
            {
                obj.GLU = dr["GLU"].ToString();
            }
            if (dr["TG"] != null)
            {
                obj.TG = dr["TG"].ToString();
            }
            if (dr["CHO"] != null)
            {
                obj.CHO = dr["CHO"].ToString();
            }
            if (dr["HDLC"] != null)
            {
                obj.HDLC = dr["HDLC"].ToString();
                obj.HDL_C = obj.HDLC;
            }
            if (dr["LDLC"] != null)
            {
                obj.LDLC = dr["LDLC"].ToString();
                obj.LDL_C = obj.LDLC;
            }
            if (dr["ALB"] != null)
            {
                obj.ALB = dr["ALB"].ToString();
            }
            if (dr["UA"] != null)
            {
                obj.UA = dr["UA"].ToString();
            }
            if (dr["HCY"] != null)
            {
                obj.HCY = dr["HCY"].ToString();
            }
            if (dr["AFP"] != null)
            {
                obj.AFP = dr["AFP"].ToString();
            }
            if (dr["CEA"] != null)
            {
                obj.CEA = dr["CEA"].ToString();
            }
            if (dr["Ka"] != null)
            {
                obj.Ka= dr["Ka"].ToString();
            }
            if (dr["Na"] != null)
            {
                obj.Na = dr["Na"].ToString();
            }
            if (dr["TP"] != null)
            {
                obj.TP = dr["TP"].ToString();
            }
            if (dr["ALP"] != null)
            {
                obj.ALP = dr["ALP"].ToString();
            }
            if (dr["GGT"] != null)
            {
                obj.GGT = dr["GGT"].ToString();
            }
            if (dr["CHE"] != null)
            {
                obj.CHE = dr["CHE"].ToString();
            }
            if (dr["TBA"] != null)
            {
                obj.TBA = dr["TBA"].ToString();
            }
            if (dr["APOA1"] != null)
            {
                obj.APOA1 = dr["APOA1"].ToString();
            }
            if (dr["APOB"] != null)
            {
                obj.APOB = dr["APOB"].ToString();
            }
            if (dr["CK"] != null)
            {
                obj.CK = dr["CK"].ToString();
            }
            if (dr["CKMB"] != null)
            {
                obj.CKMB = dr["CKMB"].ToString();
            }
            if (dr["LDHL"] != null)
            {
                obj.LDHL = dr["LDHL"].ToString();
            }
            if (dr["HBDH"] != null)
            {
                obj.HBDH = dr["HBDH"].ToString();
            }
            if (dr["aAMY"] != null)
            {
                obj.aAMY = dr["aAMY"].ToString();
            }
            if (dr["createtime"] != null)
            {
                obj.createtime = dr["createtime"].ToString();
            }  
            return obj;
        }
        public static UpLoadDataForXCG GetObjxcgByDR(DataRow dr)
        {
            UpLoadDataForXCG obj = new UpLoadDataForXCG();
            if (dr["ID"] != null)
            {
                obj.ID = dr["ID"].ToString();
            }
            if (dr["bar_code"] != null)
            {
                obj.bar_code = dr["bar_code"].ToString();
            }
            if (dr["WBC"] != null)
            {
                obj.WBC = dr["WBC"].ToString();
            }
            if (dr["RBC"] != null)
            {
                obj.RBC = dr["RBC"].ToString();
            }
            if (dr["PCT"] != null)
            {
                obj.PCT = dr["PCT"].ToString();
            }
            if (dr["PLT"] != null)
            {
                obj.PLT = dr["PLT"].ToString();
            }
            if (dr["HGB"] != null)
            {
                obj.HGB = dr["HGB"].ToString();
            }
            if (dr["HCT"] != null)
            {
                obj.HCT = dr["HCT"].ToString();
            }
            if (dr["MCV"] != null)
            {
                obj.MCV = dr["MCV"].ToString();
            }
            if (dr["MCH"] != null)
            {
                obj.MCH = dr["MCH"].ToString();
            }
            if (dr["MCHC"] != null)
            {
                obj.MCHC = dr["MCHC"].ToString();
            }
            if (dr["RDWCV"] != null)
            {
                obj.RDWCV = dr["RDWCV"].ToString();
                obj.RDW_CV = obj.RDWCV;
            }
            if (dr["RDWSD"] != null)
            {
                obj.RDWSD = dr["RDWSD"].ToString();
                obj.RDW_SD = obj.RDWSD;
            }
            if (dr["MONO"] != null)
            {
                obj.MONO = dr["MONO"].ToString();
            }
            if (dr["MONOP"] != null)
            {
                obj.MONOP = dr["MONOP"].ToString();
            }
            if (dr["GRAN"] != null)
            {
                obj.GRAN = dr["GRAN"].ToString();
            }
            if (dr["GRANP"] != null)
            {
                obj.GRANP = dr["GRANP"].ToString();
            }
            if (dr["NEUT"] != null)
            {
                obj.NEUT = dr["NEUT"].ToString();
            }
            if (dr["NEUTP"] != null)
            {
                obj.NEUTP = dr["NEUTP"].ToString();
            }
            if (dr["EO"] != null)
            {
                obj.EO = dr["EO"].ToString();
            }
            if (dr["EOP"] != null)
            {
                obj.EOP = dr["EOP"].ToString();
            }
            if (dr["BASO"] != null)
            {
                obj.BASO = dr["BASO"].ToString();
            }
            if (dr["BASOP"] != null)
            {
                obj.BASOP = dr["BASOP"].ToString();
            }
            if (dr["LYM"] != null)
            {
                obj.LYM = dr["LYM"].ToString();
            }
            if (dr["LYMP"] != null)
            {
                obj.LYMP = dr["LYMP"].ToString();
            }
            if (dr["MPV"] != null)
            {
                obj.MPV = dr["MPV"].ToString();
            }
            if (dr["PDW"] != null)
            {
                obj.PDW = dr["PDW"].ToString();
            }
            if (dr["MXD"] != null)
            {
                obj.MXD = dr["MXD"].ToString();
            }
            if (dr["MXDP"] != null)
            {
                obj.MXDP = dr["MXDP"].ToString();
            }
            if (dr["PLCR"] != null)
            {
                obj.PLCR = dr["PLCR"].ToString();
            }
            if (dr["OTHERS"] != null)
            {
                obj.OTHERS = dr["OTHERS"].ToString();
            }
            if (dr["createtime"] != null)
            {
                obj.createtime = dr["createtime"].ToString();
            }
            return obj;
        }

        public static  void GetConfigValues(out string shenghuaPath, out string xuechangguiPath, out string shxqAgreement, out string com,out string shlasttime,out string xcglasttime,out string ip,out string port)
        {
            shenghuaPath = "";
            xuechangguiPath = "";
            shxqAgreement = "";
            com = "";
            shlasttime = "";
            xcglasttime = "";
            ip = "";
            port = "";
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode node;
            string path = @"config.xml";
            xmlDoc.Load(path);
            node = xmlDoc.SelectSingleNode("config/shenghuaPath");
            shenghuaPath = node.InnerText;

            node = xmlDoc.SelectSingleNode("config/xuechangguiPath");
            xuechangguiPath = node.InnerText;

            node = xmlDoc.SelectSingleNode("config/shxqAgreement");
            shxqAgreement = node.InnerText;

            node = xmlDoc.SelectSingleNode("config/com");
            com = node.InnerText;

            node = xmlDoc.SelectSingleNode("config/shlasttime");
            shlasttime = node.InnerText;

            node = xmlDoc.SelectSingleNode("config/xcglasttime");
            xcglasttime = node.InnerText;

            node = xmlDoc.SelectSingleNode("config/ip");
            ip = node.InnerText;

            node = xmlDoc.SelectSingleNode("config/port");
            port = node.InnerText;
        }
        public static void WriteConfigTime()
        { 
            XmlDocument xmlDoc = new XmlDocument();
            string path = @"config.xml";
            xmlDoc.Load(path);

            XmlNode node = xmlDoc.SelectSingleNode("config/shlasttime");
            node.InnerText = DateTime.Now.ToString("yyyy-MM-dd") + " 06:00:00";
             
            node = xmlDoc.SelectSingleNode("config/xcglasttime");
            node.InnerText = DateTime.Now.ToString("yyyy-MM-dd") + " 06:00:00";

            xmlDoc.Save(path);
        }
        public static bool WriteConfigValues(string shenghuaPath, string xuechangguiPath, string shxqAgreement, string com,string ip,string port,out string err)
        {
            bool flag = false;
            err = "";
            try
            {
                XmlDocument xmlDoc = new XmlDocument(); 
                string path = @"config.xml";
                xmlDoc.Load(path);

                XmlNode node = xmlDoc.SelectSingleNode("config/com");
                node.InnerText = com; 

                XmlNode node1 = xmlDoc.SelectSingleNode("config/shxqAgreement");
                node1.InnerText = shxqAgreement;

                XmlNode node2 = xmlDoc.SelectSingleNode("config/shenghuaPath");
                node2.InnerText = shenghuaPath;

                XmlNode node3 = xmlDoc.SelectSingleNode("config/xuechangguiPath");
                node3.InnerText = xuechangguiPath;

                XmlNode node4 = xmlDoc.SelectSingleNode("config/ip");
                node4.InnerText = ip;

                XmlNode node5 = xmlDoc.SelectSingleNode("config/port");
                node5.InnerText = port;

                xmlDoc.Save(path);
                flag = true;
            }
            catch(Exception e)
            {
                err = e.Message;
            }
            return flag;
        }

        public static string GetNewId()
        {
            string id = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
            string guid = Guid.NewGuid().ToString().Replace("-", "");

            id += guid.Substring(0, 15);
            return id;
        }

        public static string ToHexString(byte[] bytes)
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }
    }
}
