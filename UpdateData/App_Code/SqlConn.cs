using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

/// <summary>
/// SqlConn 的摘要描述
/// </summary>
public class SqlConn
{
    public string ConnStr = "";
    public int CmdTimeout = 1800;//等待命令執行的時間(秒)。 預設為 30 秒。

    clsLogger clsLogger = new clsLogger();

    public SqlConn()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    /// <summary>取得資料庫連結字串</summary>
    /// <returns>資料庫連結字串</returns>
    public string GetConnStr(string DBName)
    {
        string result = "";
        string DBConnStrName = "Local";

        try { DBConnStrName = ConfigurationManager.AppSettings["DBConnStrName"]; } catch { }

        if (ConnStr == "")
        {
            string DataSource = "", InitialCatalog = "", UserID = "", Pxd = "", UserInfo = "";
            try { DataSource = ConfigurationManager.AppSettings["DbServer_" + DBConnStrName]; } catch { }
            try { UserID = ConfigurationManager.AppSettings["DbUser"]; } catch { }
            try { Pxd = ConfigurationManager.AppSettings["DbPxd"]; } catch { }
            try
            {
                if (DBName == "") InitialCatalog = ConfigurationManager.AppSettings["DbName_" + DBConnStrName];
                else InitialCatalog = ConfigurationManager.AppSettings["DbName_" + DBName];
            }
            catch { }

            switch (DBConnStrName)
            {
                case "Local"://本機DB
                default:
                    UserInfo = "Integrated Security=SSPI;";
                    break;
                case "Prd"://正式機DB
                    UserInfo = "User ID=" + UserID + ";Password=" + Pxd + ";";
                    break;
                case "Uat"://測試機DB
                    UserInfo = "User ID=" + UserID + ";Password=" + Pxd + ";";
                    break;
                case "Sit"://測試機DB
                    UserInfo = "User ID=" + UserID + ";Password=" + Pxd + ";";
                    break;
            }
            ConnStr = @"Data Source=" + DataSource + ";Initial Catalog=" + InitialCatalog + ";" + UserInfo;//SQL連結字串
        }
        result = ConnStr;

        return result;
    }//取得資料庫連結字串

    /// <summary>資料庫查詢</summary>
    /// <param name="TSQL">SQL語法</param>
    /// <param name="SqlParams">參數</param>
    /// <param name="ProgramName">程序名稱</param>
    /// <param name="PageName">頁面名稱</param>
    /// <param name="Msg">訊息</param>
    /// <returns>查詢結果資訊</returns>
    public DataTable DB_Select(string DBName, string TSQL, List<SqlParameter> SqlParams, string ProgramName, string PageName, string Msg)
    {
        DataTable SelectDt = new DataTable();

        try
        {
            using (SqlConnection Conn = new SqlConnection())
            {
                Conn.ConnectionString = GetConnStr(DBName);

                using (SqlCommand Cmd = new SqlCommand())
                {
                    Cmd.Connection = Conn;
                    Cmd.CommandTimeout = CmdTimeout;
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = TSQL;

                    Cmd.Parameters.Clear();
                    Cmd.Parameters.AddRange(SqlParams.ToArray());

                    using (SqlDataAdapter Adapter = new SqlDataAdapter())
                    {
                        Adapter.SelectCommand = Cmd;
                        Adapter.Fill(SelectDt);
                        Adapter.Dispose();
                    }

                    Cmd.Parameters.Clear();
                    Cmd.Dispose();
                }

                Conn.Dispose();
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.Message);
            clsLogger.WriteNLog("\r\n【SqlConn】" + "《DB_Select》\r\nSQL：" + Debug_SQL(TSQL, SqlParams) + "\r\nError：" + ex.Message, clsLogger.LogKind.DEBUG, "");
        }

        return SelectDt;
    }//資料庫查詢

    /// <summary>資料庫查詢</summary>
    /// <param name="TSQL">SQL語法</param>
    /// <param name="SqlParams">參數</param>
    /// <param name="ProgramName">程序名稱</param>
    /// <param name="PageName">頁面名稱</param>
    /// <param name="Msg">訊息</param>
    /// <returns>查詢結果資訊</returns>
    public DataSet DB_Select_DS(string DBName, string TSQL, List<SqlParameter> SqlParams, string ProgramName, string PageName, string Msg)
    {
        DataSet SelectDs = new DataSet();

        try
        {
            using (SqlConnection Conn = new SqlConnection())
            {
                Conn.ConnectionString = GetConnStr(DBName);

                using (SqlCommand Cmd = new SqlCommand())
                {
                    Cmd.Connection = Conn;
                    Cmd.CommandTimeout = CmdTimeout;
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = TSQL;

                    Cmd.Parameters.Clear();
                    Cmd.Parameters.AddRange(SqlParams.ToArray());

                    using (SqlDataAdapter Adapter = new SqlDataAdapter())
                    {
                        Adapter.SelectCommand = Cmd;
                        Adapter.Fill(SelectDs);
                        Adapter.Dispose();
                    }

                    Cmd.Parameters.Clear();
                    Cmd.Dispose();
                }

                Conn.Dispose();
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.Message);
            clsLogger.WriteNLog("\r\n【SqlConn】" + "《DB_Select_DS》\r\nSQL：" + Debug_SQL(TSQL, SqlParams) + "\r\nError：" + ex.Message, clsLogger.LogKind.DEBUG, "");
        }

        return SelectDs;
    }//資料庫查詢

    /// <summary>資料庫更新</summary>
    /// <param name="TSQL">SQL語法</param>
    /// <param name="SqlParams">參數</param>
    /// <param name="ProgramName">程序名稱</param>
    /// <param name="PageName">頁面名稱</param>
    /// <param name="Msg">訊息</param>
    /// <returns>更新結果筆數</returns>
    public int DB_Update(string DBName, string TSQL, List<SqlParameter> SqlParams, string ProgramName, string PageName, string Msg)
    {
        int result = 0;//更新資料列數目

        try
        {
            using (SqlConnection Conn = new SqlConnection())
            {
                Conn.ConnectionString = GetConnStr(DBName);

                using (SqlCommand Cmd = new SqlCommand())
                {
                    Cmd.Connection = Conn;
                    Cmd.CommandTimeout = CmdTimeout;
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = TSQL;
                    Cmd.Parameters.Clear();
                    Cmd.Parameters.AddRange(SqlParams.ToArray());

                    if (Conn.State == ConnectionState.Closed)
                    {
                        Conn.Open();
                    }

                    result = Cmd.ExecuteNonQuery();

                    if (Conn.State == ConnectionState.Open)
                    {
                        Conn.Close();
                    }

                    Cmd.Parameters.Clear();
                    Cmd.Dispose();
                }

                Conn.Dispose();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return result;
    }//資料庫更新

    /// <summary>預存程序查詢</summary>
    /// <param name="TSQL">SQL語法</param>
    /// <param name="SqlParams">參數</param>
    /// <param name="ProgramName">程序名稱</param>
    /// <param name="PageName">頁面名稱</param>
    /// <param name="Msg">訊息</param>
    /// <returns>查詢結果資訊</returns>
    public DataTable SP_Select_DT(string DBName, string SP_Name, List<SqlParameter> SqlParams, string ProgramName, string PageName, string Msg)
    {
        DataTable SelectDt = new DataTable();

        try
        {
            using (SqlConnection Conn = new SqlConnection())
            {
                Conn.ConnectionString = GetConnStr(DBName);

                using (SqlCommand Cmd = new SqlCommand())
                {
                    Cmd.Connection = Conn;
                    Cmd.CommandTimeout = CmdTimeout;
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandText = SP_Name;

                    Cmd.Parameters.Clear();
                    Cmd.Parameters.AddRange(SqlParams.ToArray());

                    using (SqlDataAdapter Adapter = new SqlDataAdapter())
                    {
                        Adapter.SelectCommand = Cmd;
                        Adapter.Fill(SelectDt);
                        Adapter.Dispose();
                    }

                    Cmd.Parameters.Clear();
                    Cmd.Dispose();
                }

                Conn.Dispose();
            }
        }
        catch (Exception ex)
        {

            System.Diagnostics.Debug.WriteLine(ex.Message);
            clsLogger.WriteNLog("\r\n【SqlConn】" + "《SP_Select_DT》\r\nError：" + ex.Message, clsLogger.LogKind.DEBUG, "");
        }

        return SelectDt;
    }//預存程序查詢

    /// <summary>
    /// 預存程序執行
    /// </summary>
    /// <param name="DBName"></param>
    /// <param name="SP_Name"></param>
    /// <param name="SqlParams"></param>
    /// <param name="ProgramName"></param>
    /// <param name="PageName"></param>
    /// <param name="Msg"></param>
    /// <returns></returns>
    public Boolean SP_Execute(string DBName, string SP_Name, List<SqlParameter> SqlParams, string ProgramName, string PageName, string Msg)
    {
        Boolean IsSuccess = false;

        try
        {
            using (SqlConnection Conn = new SqlConnection())
            {
                Conn.ConnectionString = GetConnStr(DBName);

                using (SqlCommand Cmd = new SqlCommand())
                {
                    Cmd.Connection = Conn;
                    Cmd.CommandTimeout = CmdTimeout;
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.CommandText = SP_Name;

                    Cmd.Parameters.Clear();
                    Cmd.Parameters.AddRange(SqlParams.ToArray());

                    //using (SqlDataAdapter Adapter = new SqlDataAdapter())
                    //{
                    //    Adapter.SelectCommand = Cmd;
                    //    Adapter.Fill(SelectDt);
                    //    Adapter.Dispose();
                    //}

                    if (Conn.State == ConnectionState.Closed)
                    {
                        Conn.Open();
                    }

                    int ExecuteAmount = Cmd.ExecuteNonQuery();

                    if (Conn.State == ConnectionState.Open)
                    {
                        Conn.Close();
                    }

                    IsSuccess = true;

                    Cmd.Parameters.Clear();
                    Cmd.Dispose();
                }

                Conn.Dispose();
            }
        }
        catch (Exception ex)
        {

            System.Diagnostics.Debug.WriteLine(ex.Message);
            clsLogger.WriteNLog("\r\n【SqlConn】" + "《SP_Select_DT》\r\nError：" + ex.Message, clsLogger.LogKind.DEBUG, "");
        }

        return IsSuccess;
    }//預存程序查詢

    /// <summary>SQL Debug用</summary>
    /// <param name="SQL_String">SQL語法</param>
    /// <param name="SQL_Parameters">參數</param>
    /// <returns></returns>
    public string Debug_SQL(string SQL_String, List<SqlParameter> SQL_Parameters)
    {
        string TSQL = "";

        foreach (SqlParameter SQL_Parameter in SQL_Parameters)
        {
            int ParameterSize = 1;
            if (SQL_Parameter.Size > 0) { ParameterSize = SQL_Parameter.Size * 2; }
            switch (SQL_Parameter.SqlDbType.ToString())
            {
                case "NVarChar":
                    TSQL += "DECLARE " + SQL_Parameter.ParameterName.ToString() + " " + SQL_Parameter.SqlDbType.ToString() + "(" + ParameterSize.ToString() + ")" + " = '" + SQL_Parameter.Value.ToString().Replace("'", "''") + "';";
                    break;
                case "VarChar":
                    TSQL += "DECLARE " + SQL_Parameter.ParameterName.ToString() + " " + SQL_Parameter.SqlDbType.ToString() + "(" + ParameterSize.ToString() + ")" + " = '" + SQL_Parameter.Value.ToString().Replace("'", "''") + "';";
                    break;
                case "Int":
                    TSQL += "DECLARE " + SQL_Parameter.ParameterName.ToString() + " " + SQL_Parameter.SqlDbType.ToString() + " = " + SQL_Parameter.Value.ToString().Replace("'", "''") + ";";
                    break;
                default:
                    TSQL += "DECLARE " + SQL_Parameter.ParameterName.ToString() + " " + SQL_Parameter.SqlDbType.ToString() + " = '" + SQL_Parameter.Value.ToString().Replace("'", "''") + "';";
                    break;
            }
            TSQL += "\n";
        }

        TSQL += SQL_String;

        return TSQL;
    }//SQL Debug用

    /// <summary>
    /// Stored Procedure Debug用
    /// </summary>
    /// <param name="SP_Name"></param>
    /// <param name="SQL_Parameters"></param>
    /// <returns></returns>
    public string Debug_SP(string SP_Name, List<SqlParameter> SQL_Parameters)
    {
        string TSQL = "";

        TSQL += "EXEC [" + SP_Name + "]";

        foreach (SqlParameter SQL_Parameter in SQL_Parameters)
        {
            TSQL += SQL_Parameter.ParameterName.ToString() + " = N'" + SQL_Parameter.Value.ToString() + "',";
            TSQL += "\n";
        }

        if (TSQL != "" && TSQL.Length > 3) TSQL.Substring(0, TSQL.Length - 3);

        return TSQL;
    }//預存程序 Debug用


    /// <summary>讀取CSV檔</summary>
    /// <param name="FileEncoding">設定檔案的編碼(65001：UTF8)</param>
    /// <param name="FileName">檔案名稱含路徑</param>
    /// <param name="SaveFileName">檔案名稱</param>
    /// <param name="ProgramName">程序名稱</param>
    /// <param name="PageName">頁面名稱</param>
    /// <param name="Msg">訊息</param>
    /// <returns></returns>
    public DataTable TextFile_Select(string FileEncoding, string FileName, string SaveFileName, string ProgramName, string PageName, string Msg)
    {
        DataTable result = new DataTable();

        try
        {
            //宣告檔案連結字串
            string FileConStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FileName.Replace(SaveFileName, "") + ";Extended Properties='Text;CharacterSet=" + FileEncoding + ";HDR=Yes;IMEX=1'";

            using (System.Data.OleDb.OleDbConnection FileConn = new System.Data.OleDb.OleDbConnection(FileConStr))
            {
                string FileSQLStr = "SELECT * FROM [" + SaveFileName + "]";
                using (System.Data.OleDb.OleDbDataAdapter FileAdapter = new System.Data.OleDb.OleDbDataAdapter(FileSQLStr, FileConn))
                {
                    FileAdapter.Fill(result);

                    //設置空值
                    foreach (DataRow row in result.Rows)
                    {
                        foreach (DataColumn col in result.Columns)
                        {
                            if (row[col] == DBNull.Value)
                            {
                                switch (col.DataType.Name)
                                {
                                    case "String":
                                        row[col] = "";
                                        break;
                                    case "Double":
                                        row[col] = 0;
                                        break;
                                    default:
                                        Console.WriteLine(col.DataType.Name);
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            result.Dispose();//資料表釋放資源
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }


        return result;
    }//讀取CSV檔

    /// <summary>寫入DB Log</summary>
    /// <param name="Action">執行動作</param>
    /// <param name="FileName">檔案名稱</param>
    /// <param name="TableName">資料表名稱</param>
    /// <param name="Note">訊息</param>
    public void WriteDBLog(string Action, string FileName, string TableName, string Note)
    {
        int UpdateAmount = 0;
        string SQL = "";
        List<SqlParameter> SqlParams = new List<SqlParameter>();

        SQL = "INSERT INTO [JTI_DataImport_Log] ([Account],[Action],[FileName],[TableName],[TerminalID],[TerminalIP],[Note]) VALUES(@Account,@Action,@FileName,@TableName,@TerminalID,@TerminalIP,@Note); ";

        try
        {
            SqlParams.Add(new SqlParameter("@Account", SqlDbType.NVarChar) { Value = Application.ProductName });
            SqlParams.Add(new SqlParameter("@Action", SqlDbType.NVarChar) { Value = Action });
            SqlParams.Add(new SqlParameter("@FileName", SqlDbType.NVarChar) { Value = FileName });
            SqlParams.Add(new SqlParameter("@TableName", SqlDbType.NVarChar) { Value = TableName });
            SqlParams.Add(new SqlParameter("@TerminalID", SqlDbType.NVarChar) { Value = Environment.MachineName });
            SqlParams.Add(new SqlParameter("@TerminalIP", SqlDbType.NVarChar) { Value = "127.0.0.1" });
            SqlParams.Add(new SqlParameter("@Note", SqlDbType.NVarChar) { Value = Note });

            UpdateAmount = DB_Update("", SQL, SqlParams, "GIS PORTAL", "", Action);
        }
        catch { }
    }//寫入DB Log


}