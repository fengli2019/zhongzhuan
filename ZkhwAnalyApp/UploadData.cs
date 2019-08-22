using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZkhwAnalyApp
{
    public class DisplayData
    {
        public  string BarCode { get; set; }
        public  string Type { get; set; }
        public  string UploadDate { get; set; }
        public  string Content { get; set; }
    }
    public class UpLOadDataBase
    {
        public string ID { get; set; }
        public string bar_code { get; set; }
        public string createtime { get; set; }
        public int status { get; set; }
    }
    public class UpLoadDataForSH: UpLOadDataBase
    { 
        public string ALT { get; set; }
        public string AST { get; set; }
        public string TBIL { get; set; }
        public string DBIL { get; set; }
        public string CREA { get; set; }
        public string UREA { get; set; }
        public string GLU { get; set; }
        public string TG { get; set; }
        public string CHO { get; set; }
        public string HDLC { get; set; }
        public string LDLC { get; set; }
        public string ALB { get; set; }
        public string UA { get; set; }
        public string HCY { get; set; }
        public string AFP { get; set; }
        public string CEA { get; set; }
        public string Ka { get; set; }
        public string Na { get; set; }
        public string TP { get; set; }
        public string ALP { get; set; }
        public string GGT { get; set; }
        public string CHE { get; set; }
        public string TBA { get; set; }
        public string APOA1 { get; set; }
        public string APOB { get; set; }
        public string CK { get; set; }
        public string CKMB { get; set; }
        public string LDHL { get; set; }
        public string HBDH { get; set; }
        public string aAMY { get; set; } 
      
        public string HDL_C { get; set; }
        public string LDL_C { get; set; }
    }

    public class UpLoadDataForXCG:UpLOadDataBase
    { 
        public string WBC { get; set; }
        public string RBC { get; set; }
        public string PCT { get; set; }
        public string PLT { get; set; }
        public string HGB { get; set; }
        public string HCT { get; set; }
        public string MCV { get; set; }
        public string MCH { get; set; }
        public string MCHC { get; set; }
        public string RDWCV { get; set; }
        public string RDWSD { get; set; }
        public string MONO { get; set; }
        public string MONOP { get; set; }
        public string GRAN { get; set; }
        public string GRANP { get; set; }
        public string NEUT { get; set; }
        public string NEUTP { get; set; }
        public string EO { get; set; }
        public string EOP { get; set; }
        public string BASO { get; set; }
        public string BASOP { get; set; }
        public string LYM { get; set; }
        public string LYMP { get; set; }
        public string MPV { get; set; }
        public string PDW { get; set; }
        public string MXD { get; set; }
        public string MXDP { get; set; }
        public string PLCR { get; set; }
        public string OTHERS { get; set; } 
        public string RDW_CV { get; set; }
        public string RDW_SD { get; set; }
    }
}
