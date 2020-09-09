using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace UpdateVersionFileTool {
   public partial class Form1 : Form {
      public Form1() {
         InitializeComponent();
      }

      private void button1_Click(object sender, EventArgs e) {
         this.label1.Text = "更新中...";

         var doc = new XmlDocument();
         doc.Load("settings.xml");

         var nodes = doc.SelectNodes("/root/system_list");

         foreach (XmlNode item in nodes) {
            var versionFilePath = "";
            var targetFolder = "";

            foreach (XmlNode child in item.ChildNodes) {
               switch (child.Name) {
                  case "version_file_path":
                  versionFilePath = child.InnerText;
                  break;
                  case "target_folder":
                  targetFolder = child.InnerText;
                  break;
               }
            }

            if (versionFilePath == "" || targetFolder == "") {
               label1.Text = "settings.xmlを見直してください";
               break;
            }

            XmlDocument wdoc = new XmlDocument();
            wdoc.Load(versionFilePath);
            var wnodes = wdoc.SelectNodes("/root/file_list");

            var err = false;

            foreach (XmlNode witem in wnodes) {
               var fileName = "";
               var size = "";
               var date = "";
               var time = "";

               foreach (XmlNode child in witem.ChildNodes) {
                  if (child.Name == "file_name") {
                     fileName = child.InnerText;
                     if (!File.Exists(targetFolder + "\\" + fileName)) {
                        label1.Text = "ファイルがない";
                        err = true;
                        break;
                     }

                     FileInfo file = new FileInfo(targetFolder + "\\" + fileName);
                     size = file.Length.ToString();
                     date = file.LastWriteTime.ToString("yyyy/MM/dd");
                     time = file.LastWriteTime.ToString("HH:mm:ss");
                  }
               }

               if (size == "" || date == "" || time == "") {
                  label1.Text = "ファイル属性取得エラー";
                  err = true;
                  break;
               }

               foreach (XmlNode child in witem.ChildNodes) {
                  switch (child.Name) {
                  case "size":
                     child.InnerText = size;
                     break;
                  case "date":
                     child.InnerText = date;
                     break;
                  case "time":
                     child.InnerText = time;
                     break;
                  }
               }
            }

            if (err) {
               break;
            }

            wdoc.Save(versionFilePath);
         }
         this.label1.Text = "終了!!";
      }

      private void Form1_Shown(object sender, EventArgs e) {
         this.label1.Text = "";
      }
   }
}
