using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZkhwAnalyApp
{
    public class DbHelperMySQL
    {
        public static string connectionStringYpt = "database=zkhw;Password=zkhw123;User ID=zkhw;server=47.92.74.93;port=3307;character set=utf8";
        public static int ExecuteSqlYpt(string SQLString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionStringYpt))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (MySqlException e)
                    {
                        connection.Close();
                        if (e.Message.IndexOf("timecodeUnique") > 0)
                        {
                            return 1;
                        }
                        else
                        {
                            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Application.StartupPath + "/log.txt", true))
                            {
                                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + e.Message + "\r\n" + SQLString);
                            }
                            return 0;
                        }
                    }
                }
            }
        }

        public static int ExecuteSqlTranYpt(List<String> SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionStringYpt))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                int a = 0;
                try
                {
                    int count = 0;
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        MySqlTransaction tx = conn.BeginTransaction();
                        cmd.Transaction = tx;
                        try
                        {
                            a = n;
                            string strsql = SQLStringList[n];
                            cmd.CommandText = strsql;
                            count += cmd.ExecuteNonQuery();
                            tx.Commit();
                        }
                        catch (MySqlException e)
                        { 
                            tx.Rollback();
                        }
                    }

                    return count;
                }
                catch (MySqlException e)
                {
                    conn.Close();
                    return 0;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static DataSet QueryYpt(string SQLString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionStringYpt))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        public static object GetSingleYpt(string SQLString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionStringYpt))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (MySqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }


        /// <summary>
        /// 临时测试使用
        /// </summary>
        /// <param name="SQLString"></param>
        /// <returns></returns>
        public static DataSet QueryLocal(string SQLString)
        {
            string connectionString = "database=zkhwdf;Password=zkhw123;User ID=zkhw;server=127.0.0.1;port=3307;character set=utf8";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }
        public static int ExecuteSqlLocal(string SQLString)
        {
            string connectionString = "database=zkhwdf;Password=zkhw123;User ID=zkhw;server=127.0.0.1;port=3307;character set=utf8";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (MySqlException e)
                    {
                        connection.Close(); 
                        return 0;
                    }
                }
            }
        }

        public static object GetSingle(string SQLString)
        {
            string connectionString = "database=zkhw;Password=zkhw123;User ID=zkhw;server=sx.bestzkhw.com;port=3306;character set=utf8";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (MySqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

    }
}
