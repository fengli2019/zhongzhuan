using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ZkhwAnalyApp
{
    public class LogicHandler
    {
        public static bool UpLoadDatashDal(UpLoadDataForSH obj,out string err)
        {
            err = "";
            bool flag = false;
            try
            {
                string id = Common.GetNewId();
                string timeCodeUnique = obj.bar_code + "_" + obj.createtime;
                string sql0 = string.Format("select count(ID) from zkhw_temp_sh where timecodeUnique='{0}'", timeCodeUnique);
                object o = DbHelperMySQL.GetSingleYpt(sql0);
                bool isinsert = false;
                if(o==null)
                {
                    isinsert = true;
                }
                else
                {
                    if(int.Parse(o.ToString())<=0)
                    {
                        isinsert = true;
                    }
                }
                string sql = "";
                if (isinsert==true)
                {
                    sql = string.Format(@"insert into zkhw_temp_sh(ID,bar_code,ALT,AST,TBIL,DBIL,CREA,UREA,GLU,TG,CHO,HDLC,LDLC,ALB,UA,HCY,AFP,
                                      CEA,Ka,Na,TP,ALP,GGT,CHE,TBA,APOA1,APOB,CK,CKMB,LDHL,HBDH,aAMY,createtime,status,timecodeUnique)values('{0}','{1}','{2}'
                                      ,'{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}'
                                      ,'{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}',0,'{33}')",
                                          id, obj.bar_code, obj.ALT, obj.AST, obj.TBIL, obj.DBIL, obj.CREA, obj.UREA, obj.GLU, obj.TG, obj.CHO, obj.HDL_C,
                                          obj.LDL_C, obj.ALB, obj.UA, obj.HCY, obj.AFP, obj.CEA, obj.Ka, obj.Na, obj.TP, obj.ALP, obj.GGT, obj.CHE, obj.TBA,
                                          obj.APOA1, obj.APOB, obj.CK, obj.CKMB, obj.LDHL, obj.HBDH, obj.aAMY, obj.createtime, timeCodeUnique);
                }
                else
                {
                    sql = @"update zkhw_temp_sh set ALB='" + obj.ALB + "',ALP='" + obj.ALP + "',ALT='" + obj.ALT + "',AST='" + obj.AST + 
                        "',CHO='" + obj.CHO + "',Crea='" + obj.Crea + "',DBIL='" + obj.DBIL + "',GGT='" + obj.GGT + 
                        "',GLU='" + obj.GLU + "',HDLC='" + obj.HDL_C + "',LDLC='" + obj.LDL_C + "',TBIL='" + obj.TBIL +
                        "',TG='" + obj.TG + "',TP='" + obj.TP + "',UA='" + obj.UA + "',UREA='" + obj.UREA +
                        "' where timecodeUnique = '" + timeCodeUnique + "'";

                }
                int ret = DbHelperMySQL.ExecuteSqlYpt(sql);
                if (ret > 0)
                {
                    flag = true;
                }
            }
            catch(Exception e)
            {
                err = e.Message;
            }
            return flag;
        }

        public static bool UpLoadDataxcgDal(UpLoadDataForXCG obj, out string err)
        {
            err = "";
            bool flag = false;
            try
            {
                string id = Common.GetNewId();
                string timeCodeUnique = obj.bar_code + "_" + obj.createtime;
                string sql0 = string.Format("select count(ID) from zkhw_temp_xcg where timecodeUnique='{0}'", timeCodeUnique);
                object o = DbHelperMySQL.GetSingleYpt(sql0);
                bool isinsert = false;
                if (o == null)
                {
                    isinsert = true;
                }
                else
                {
                    if (int.Parse(o.ToString()) <= 0)
                    {
                        isinsert = true;
                    }
                }
                string sql = "";
                if (isinsert==true)
                {
                    sql = string.Format(@"insert into zkhw_temp_xcg(ID,bar_code,WBC,RBC,PCT,PLT,HGB,HCT,MCV,MCH,MCHC,RDWCV,RDWSD,
                                             MONO,MONOP,GRAN,GRANP,NEUT,NEUTP,EO,EOP,BASO,BASOP,LYM,
                                            LYMP,MPV,PDW,MXD,MXDP,PLCR,OTHERS,createtime,status,timecodeUnique)values('{0}','{1}','{2}','{3}','{4}'
                                            ,'{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}'
                                            ,'{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}',0,'{32}')",
                                            id, obj.bar_code, obj.WBC, obj.RBC, obj.PCT, obj.PLT, obj.HGB, obj.HCT, obj.MCV, obj.MCH, obj.MCHC,
                                            obj.RDW_CV, obj.RDW_SD, obj.MONO, obj.MONOP, obj.GRAN, obj.GRANP, obj.NEUT, obj.NEUTP, obj.EO, obj.EOP,
                                            obj.BASO, obj.BASOP, obj.LYM, obj.LYMP, obj.MPV, obj.PDW, obj.MXD, obj.MXDP, obj.PLCR,
                                            obj.OTHERS, obj.createtime, timeCodeUnique);
                }
                else
                {
                    sql = @"update zkhw_temp_xcg set HCT='" + obj.HCT + "',HGB='" + obj.HGB + "',LYM='" + obj.LYM + 
                        "',LYMP='" + obj.LYMP + "',MCH='" + obj.MCH + "',MCHC='" + obj.MCHC + "',MCV='" + obj.MCV + 
                        "',MPV='" + obj.MPV + "',MXD='" + obj.MXD + "',MXDP='" + obj.MXDP + "',NEUT='" + obj.NEUT +
                        "',NEUTP='" + obj.NEUTP + "',PCT='" + obj.PCT + "',PDW='" + obj.PDW + "',PLT='" + obj.PLT + 
                        "',RBC='" + obj.RBC + "',RDWCV='" + obj.RDW_CV + "',RDWSD='" + obj.RDW_SD + "',WBC='" + obj.WBC +
                        "',MONO='" + obj.MONO + "',MONOP='" + obj.MONOP + "',GRAN='" + obj.GRAN + "',GRANP='" + obj.GRANP +
                        "',PLCR='" + obj.PLCR + "' where timecodeUnique = '" + timeCodeUnique + "'";
                }
                int ret = DbHelperMySQL.ExecuteSqlYpt(sql);
                if (ret > 0)
                {
                    flag = true;
                }
            }
            catch (Exception e)
            {
                err = e.Message;
            }
            return flag;
        }

        public static DataSet GetDataForMySql(string s,string top,string tdb,out string err)
        {
            err = "";
            DataSet ds = null;
            try
            {
                string sql = "select * from "+ tdb+ s;
                if(top !="")
                {
                    sql = "select * from " + tdb + s + " LIMIT "+top;
                }
                ds = DbHelperMySQL.QueryLocal(sql);
            }
            catch(Exception e)
            {
                err = e.Message;
            }
            return ds;
        }
        public static bool DeleteMySqlSh(string id)
        {
            bool flag = false;
            try
            {
                string sql = string.Format("Delete From zkhw_tj_sh where id='{0}'", id);
                int ret = DbHelperMySQL.ExecuteSqlLocal(sql);
                if (ret > 0)
                {
                    flag = true;
                }
            }
            catch
            {

            }
            return flag;
        }

        public static bool DeleteMySqlXcg(string id)
        {
            bool flag = false;
            try
            {
                string sql = string.Format("Delete From zkhw_tj_xcg where id='{0}'", id);
                int ret = DbHelperMySQL.ExecuteSqlLocal(sql);
                if (ret > 0)
                {
                    flag = true;
                }
            }
            catch
            {

            }
            return flag;
        }

        public static int GetFailNumShMySql()
        {
            int num = 0;
            string sql = "select count(*) from zkhw_tj_sh ";
            object obj = DbHelperMySQL.GetSingle(sql);
            if (obj != null)
            {
                num = int.Parse(obj.ToString());
            }
            return num;
        }

        public static int GetFailNumXcgMySql()
        {
            int num = 0;
            string sql = "select count(*) from zkhw_tj_xcg ";
            object obj = DbHelperMySQL.GetSingle(sql);
            if (obj != null)
            {
                num = int.Parse(obj.ToString());
            }
            return num;
        }


        public static bool InsertAccessSh(UpLoadDataForSH obj, out string err)
        {
            bool flag = false;
            err = "";
            try
            { 
                string sql = string.Format(@"insert into zkhw_tj_sh(bar_code,ALT,AST,TBIL,DBIL,CREA,UREA,GLU,TG,CHO,HDLC,LDLC,ALB,UA,HCY,AFP,
                                      CEA,Ka,Na,TP,ALP,GGT,CHE,TBA,APOA1,APOB,CK,CKMB,LDHL,HBDH,aAMY,createtime)values('{0}','{1}','{2}'
                                      ,'{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}'
                                      ,'{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}')",
                                          obj.bar_code, obj.ALT, obj.AST, obj.TBIL, obj.DBIL, obj.CREA, obj.UREA, obj.GLU, obj.TG, obj.CHO, obj.HDL_C,
                                          obj.LDL_C, obj.ALB, obj.UA, obj.HCY, obj.AFP, obj.CEA, obj.Ka, obj.Na, obj.TP, obj.ALP, obj.GGT, obj.CHE, obj.TBA,
                                          obj.APOA1, obj.APOB, obj.CK, obj.CKMB, obj.LDHL, obj.HBDH, obj.aAMY, obj.createtime);
                int ret = DbHelperAccess.ExecuteSql(sql);
                if (ret > 0)
                {
                    flag = true;
                }
            }
            catch (Exception e)
            {
                err = e.Message;
            }
            return flag;
        }

        public static bool InsertAccessXCG(UpLoadDataForXCG obj, out string err)
        {
            err = "";
            bool flag = false;
            try
            { 
                string sql = string.Format(@"insert into zkhw_tj_xcg(bar_code,WBC,RBC,PCT,PLT,HGB,HCT,MCV,MCH,MCHC,RDWCV,RDWSD,
                                             MONO,MONOP,GRAN,GRANP,NEUT,NEUTP,EO,EOP,BASO,BASOP,LYM,
                                            LYMP,MPV,PDW,MXD,MXDP,PLCR,OTHERS,createtime)values('{0}','{1}','{2}','{3}','{4}'
                                            ,'{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}'
                                            ,'{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}')",
                                            obj.bar_code, obj.WBC, obj.RBC, obj.PCT, obj.PLT, obj.HGB, obj.HCT, obj.MCV, obj.MCH, obj.MCHC,
                                            obj.RDW_CV, obj.RDW_SD, obj.MONO, obj.MONOP, obj.GRAN, obj.GRANP, obj.NEUT, obj.NEUTP, obj.EO, obj.EOP,
                                            obj.BASO, obj.BASOP, obj.LYM, obj.LYMP, obj.MPV, obj.PDW, obj.MXD, obj.MXDP, obj.PLCR,
                                            obj.OTHERS, obj.createtime);
                int ret = DbHelperAccess.ExecuteSql(sql);
                if (ret > 0)
                {
                    flag = true;
                }
            }
            catch (Exception e)
            {
                err = e.Message;
            }
            return flag;
        }

        public static bool DeleteAccessSh(string id)
        {
            bool flag = false;
            try
            {
                string sql = string.Format("Delete From zkhw_tj_sh where id={0}", id);
                int ret = DbHelperAccess.ExecuteSql(sql);
                if (ret > 0)
                {
                    flag = true;
                }
            }
            catch
            {

            }
            return flag;
        } 

        public static bool DeleteAccessXcg(string id)
        {
            bool flag = false;
            try
            {
                string sql = string.Format("Delete From zkhw_tj_xcg where id={0}", id);
                int ret = DbHelperAccess.ExecuteSql(sql);
                if(ret >0)
                {
                    flag = true;
                }
            }
            catch
            {

            }
            return flag;
        }
        public static DataSet GetDataForAccess(string s,string top,string tdb,out string err)
        {
            err = "";
            DataSet ds = null;
            try
            {
                string sql = "select * from " + tdb + s;
                if (top != "")
                {
                    sql = "select top " + top + " * from " + tdb + s;
                }
                ds = DbHelperAccess.Query(sql);
            }
            catch(Exception e)
            {
                err = e.Message;
            }
            return ds;
        }

        public static int GetFailNumSh()
        {
            int num = 0;
            string sql = "select count(*) from zkhw_tj_sh ";
            object obj= DbHelperAccess.GetSingle(sql);
            if(obj !=null)
            {
                num = int.Parse(obj.ToString());
            }
            return num;
        }

        public static int GetFailNumXcg()
        {
            int num = 0;
            string sql = "select count(*) from zkhw_tj_xcg ";
            object obj = DbHelperAccess.GetSingle(sql);
            if (obj != null)
            {
                num = int.Parse(obj.ToString());
            }
            return num;
        }


        public static DataSet getShenghuaXueChangGuiForYLH(string strSQL, string shpath)
        { 
            try
            {
                DataSet ds = new DataSet();
                string strcon = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source =" + shpath + "";
                ds = DbHelperAccess.Query(strSQL, strcon);
                return ds;
            }
            catch(Exception r)
            {
                return null;
            }
        }


    }
}
