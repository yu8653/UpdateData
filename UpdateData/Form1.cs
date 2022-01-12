using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Data.SqlClient;

namespace UpdateData
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        clsLogger clsLogger = new clsLogger();
        ExecTask execTask = new ExecTask();
        FileHandle fileHandle = new FileHandle();

        string path = ConfigurationManager.AppSettings["FileBackupPath"];
        string Date = DateTime.Now.ToString("yyyyMMdd");

        List<string> FileNameList = new List<string>();

        private async void Form1_Load(object sender, EventArgs e)
        {

            string FilePath = path + "\\" + Date;
            if (Directory.Exists(path))
            {
                Get_FileList(FilePath);
            }

            if (FileNameList.Count > 0)
            {
                await ExecuteSQLJobAsync();
                SendMail($"{Date}檔案已更新");
            }

            #region 清除過期檔案
            await Task.Delay(100);
            fileHandle.ClearFile(AppDomain.CurrentDomain.BaseDirectory + "Logs");
            #endregion

            #region 應用程式結束
            this.Close();
            Environment.Exit(Environment.ExitCode);
            #endregion

        }

        /// <summary>
        /// 取得資料夾內的檔案
        /// </summary>
        /// <param name="targetDirectory">資料夾</param>
        private void Get_FileList(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                ProcessFile(fileName);
            }
        }

        /// <summary>
        /// 修改檔案名稱
        /// </summary>
        /// <param name="path">檔案路徑</param>
        private void ProcessFile(string path)
        {
            try
            {
                string fileName = Path.GetFileName(path);
                //Console.WriteLine("檔案：{0}", fileName);
                clsLogger.WriteNLog("\r\n【ProcessFile】\r\n檔案：" + fileName + "\r\n", clsLogger.LogKind.Trace, "");

                if (fileName.IndexOf(".csv") > -1)
                {
                    string NewFileName = "";
                    for (int i = 0; i < fileName.Split('_').Length - 1; i++)
                    {
                        NewFileName += fileName.Split('_')[i] + "_";
                    }
                    NewFileName = NewFileName.Substring(0, NewFileName.Length - 1);

                    FileNameList.Add(NewFileName);

                    //Console.WriteLine("檔案：{0}", NewFileName);
                }

            }
            catch (Exception ex)
            {
                //Console.WriteLine("《ProcessFile》：{0}", ex.Message);
                clsLogger.WriteNLog("\r\n【ProcessFile】\r\nProcessFile error：" + ex.Message + "\r\n", clsLogger.LogKind.DEBUG, "");
            }
        }

        /// <summary>
        /// 啟動資料庫作業
        /// </summary>
        /// <returns></returns>
        private async Task ExecuteSQLJobAsync()
        {
            List<string> ProgramFileList = new List<string> { "FileName1", "FileName2", "FileName3", "FileName4", "FileName5" };

            foreach (string FileName in FileNameList)
            {

                //確認五個活動檔案都有才啟動作業
                if (ProgramFileList.Exists(x => x == FileName))
                {
                    ProgramFileList.Remove(FileName);

                    if (ProgramFileList.Count > 0) continue;
                }

                //啟動檔案相對應作業
                await execTask.ExecuteTaskAsync(FileName);

            }

        }

        /// <summary>
        /// 寄信
        /// </summary>
        /// <param name="content"></param>
        private void SendMail(string content)
        {

            content += "\r\n===== 此為系統信，請勿回覆，謝謝。 =====\r\n";

            string SmtpPort = ConfigurationManager.AppSettings["SmtpPort"];
            string SmtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            string SmtpFrom = ConfigurationManager.AppSettings["SmtpFrom"];

            MailMessage myMail = new MailMessage();

            try
            {
                //寄件者
                myMail.From = new MailAddress(SmtpFrom, "Sender");

                //收件者
                foreach (SettingsProperty Mail in Properties.MailList.Default.Properties)
                {
                    myMail.To.Add(new MailAddress(Mail.DefaultValue.ToString(), Mail.Name));
                }

                myMail.SubjectEncoding = Encoding.UTF8;
                myMail.Subject = "Data Update";
                myMail.IsBodyHtml = false;
                myMail.BodyEncoding = Encoding.UTF8;
                myMail.Body = content;

                SmtpClient mySmtp = new SmtpClient();
                mySmtp.Port = Convert.ToInt32(SmtpPort);
                mySmtp.Host = SmtpHost;
                mySmtp.EnableSsl = false;
                mySmtp.Send(myMail);
            }
            catch (Exception ex)
            {
                //Console.WriteLine("《SendMail》：{0}", ex.Message);
                clsLogger.WriteNLog("\r\n【SendMail】\r\nSendMail error：" + ex.Message + "\r\n", clsLogger.LogKind.DEBUG, "");
            }
        }


    }
}
