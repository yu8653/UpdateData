using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ExecTask
{
    clsLogger clsLogger = new clsLogger();
    SqlConn SqlConn = new SqlConn();

    public ExecTask()
    {
        
    }

    /// <summary>
    /// 執行SQL Server Agent作業
    /// </summary>
    /// <param name="FileName">檔案名稱</param>
    public async Task ExecuteTaskAsync(string FileName)
    {

        string TaskName = FileNameToTask(FileName);
        if (TaskName == "") return;

        //sp執行SQL Server Agent作業
        string SQL = $"EXEC msdb.dbo.sp_start_job N'{TaskName}';";

        SqlConn.DB_Select("", SQL, new List<SqlParameter>(), "", "", "");

        await Task.Delay(1000);//延遲1秒秒查詢狀態
        //sp查詢作業狀態
        string SQL_Status = $"EXEC msdb.dbo.sp_help_job @job_name= N'{TaskName}', @job_aspect='JOB';";
        int i = 0;
        while (true)
        {
            DataTable dt = SqlConn.DB_Select("", SQL_Status, new List<SqlParameter>(), "", "", "");
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["current_execution_status"].ToString() == "4")
                { //取得目前的作業狀態 4 = 閒置
                    if (dt.Rows[0]["last_run_outcome"].ToString() == "1")
                    {
                        clsLogger.WriteNLog($"\r\n【execTask】\r\nExecuteTaskAsync： 作業－{TaskName} 執行成功\r\n", clsLogger.LogKind.DEBUG, "");
                    }
                    else if (dt.Rows[0]["last_run_outcome"].ToString() == "0")
                    {
                        clsLogger.WriteNLog($"\r\n【execTask】\r\nExecuteTaskAsync： 作業－{TaskName} 執行失敗\r\n", clsLogger.LogKind.DEBUG, "");
                    }
                    break;
                }

            }

            i++;
            if (i > 50)
                break;

            await Task.Delay(5000);//間隔5秒再去查詢狀態
        }

        return;
    }

    /// <summary>
    /// 取得檔案對應之作業名稱
    /// </summary>
    /// <param name="FileName">檔案名稱</param>
    /// <returns></returns>
    private string FileNameToTask(string FileName)
    {
        string TaskName;
        switch (FileName)
        {
            case "fileName":
                TaskName = "taskName";
                break;

            default:
                TaskName = "";
                break;
        }

        return TaskName;


    }


}




