using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;


public class clsLogger
{
    public string UserIP = "";//User IP

    public clsLogger()
    {
        UserIP = GetIP4Address();//取得 User IP
    }

    /// <summary>
    /// 取得IP
    /// </summary>
    /// <returns>IP</returns>
    public string GetIP4Address()
    {
        string IP4Address = String.Empty;
        foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
        {
            if (IPA.AddressFamily.ToString() == "InterNetwork")
            {
                IP4Address = IPA.ToString();
                break;
            }
        }
        return IP4Address;
    }

    /// <summary>紀錄 NLog</summary>
    /// <param name="logkind">訊息類型</param>
    /// <param name="sMessage">訊息內容</param>
    /// <param name="fileName">檔案名稱</param>
    public void WriteNLog(string sMessage, LogKind logkind, string fileName)
    {
        string LogFileName = "";
        if (fileName == null || fileName == "")
        {
            LogFileName = "";
        }
        else
        {
            LogFileName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
        }

        Logger logger = LogManager.GetLogger(LogFileName);
        switch (logkind)
        {
            case LogKind.DEBUG:
                logger.Debug("IP：" + UserIP + sMessage);
                break;
            case LogKind.ERROR:
                logger.Error("IP：" + UserIP + sMessage);
                break;
            case LogKind.INFO:
                logger.Info("IP：" + UserIP + sMessage);
                break;
            case LogKind.WARN:
                logger.Warn("IP：" + UserIP + sMessage);
                break;
            case LogKind.Trace:
                logger.Trace("IP：" + UserIP + sMessage);
                break;
        }
    }

    public enum LogKind
    {
        DEBUG = 1, //針對在開發測試階段紀錄相關偵錯訊息
        INFO = 2, 
        WARN = 3,  //紀錄相關警告訊息,此訊息不影響系統運作
        ERROR = 4, //紀錄相關嚴重的錯誤訊息此錯誤會影響系統運作
        Trace = 5  //紀錄提醒用的訊息，此類訊息可作為系統 Trace 依據
    }
}