using System;
using System.Configuration;
using System.IO;

namespace UpdateData
{
    class FileHandle
    {
        public FileHandle()
        {


        }

        clsLogger clsLogger = new clsLogger();

        /// <summary>
        /// 清除路徑下超過保存期限的檔案
        /// </summary>
        /// <param name="TargetPath"></param>
        public void ClearFile(string TargetPath)
        {
            if (!Directory.Exists(TargetPath))
            {
                return;
            }
            DateTime DeleteTime = DateTime.Now.AddDays(int.Parse(ConfigurationManager.AppSettings["ExpiredDate"]) * -1);
            DirectoryInfo di = new DirectoryInfo(TargetPath);
            foreach (FileInfo item in di.GetFiles())
            {
                try
                {
                    if (DeleteTime > item.CreationTime)
                    {
                        item.Delete();
                    }
                }
                catch (Exception ex)
                {
                    clsLogger.WriteNLog($"\r\n【FileHandle】ClearFile Error\r\n{ex.Message}", clsLogger.LogKind.DEBUG, "");
                }

            }


            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(TargetPath);
            foreach (string subdirectory in subdirectoryEntries)
            {
                ClearFile(subdirectory);
            }

        }
    }
}
