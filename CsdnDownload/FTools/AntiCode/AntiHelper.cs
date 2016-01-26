using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FTools.AntiCode
{
    public class AntiHelper
    {
        private string dllPath = string.Empty;
        public AntiHelper()
        {
            dllPath = System.Environment.CurrentDirectory + "\\AntiVC.dll";
            if (!File.Exists(dllPath))
            {
                ExportAntiDll();
            }
            
        }
        [DllImport("AntiVC.dll")]
        public static extern int LoadCdsFromFile(string FilePath, string Password);

        [DllImport("AntiVC.dll")]
        public static extern int LoadCdsFromBuffer(byte[] FileBuffer, int FileBufLen, string Password);

        [DllImport("AntiVC.dll")]
        public static extern bool GetVcodeFromFile(int CdsFileIndex, string FilePath, StringBuilder Vcode);

        [DllImport("AntiVC.dll")]
        public static extern bool GetVcodeFromBuffer(int CdsFileIndex, byte[] FileBuffer, int ImgBufLen, StringBuilder Vcode);

        [DllImport("AntiVC.dll")]
        public static extern bool GetVcodeFromURL(int CdsFileIndex, string ImgURL, StringBuilder Vcode);

        [DllImport("urlmon.dll", EntryPoint = "URLDownloadToFileA")]
        public static extern int URLDownloadToFile(int pCaller, string szURL, string szFileName, int dwReserved, int lpfnCB);

        private void ExportAntiDll()
        {
            byte[] res = new byte[Resource1.AntiVC.Length];
            Resource1.AntiVC.CopyTo(res,0);
            FileStream sw = new FileStream(dllPath, FileMode.Create,FileAccess.Write);
            try
            {
                sw.Write(res,0,res.Length);
            }
            catch (Exception ex)
            {
                Log.Log.WriteLog(this.GetType(), ex);
                throw;
            }
            finally
            {
                sw.Close();//写入
            }
        }
    }
}
