using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace コマンドをもう一度 {
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            if (e.Args.Length == 0) {
                //以前のコマンドを再実行する
                String openFilePath = Setting.value.OpenFilePath;
                if (openFilePath == null) {
                    MessageBox.Show("任意のファイルを本アプリにドラッグ＆ドロップして下さい", "ファイル情報なし", MessageBoxButton.OK, MessageBoxImage.Error);
                } else {
                    openFile(openFilePath);
                }
            } else {
                //渡されたファイルを開く
                var val = Setting.value;
                val.OpenFilePath = e.Args[0];
                Setting.value = val;
                openFile(e.Args[0]);
            }
            Environment.Exit(0);
        }
        private void openFile(String filePath) {
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.WorkingDirectory = Path.GetDirectoryName(filePath);
            p.StartInfo.FileName = @"""" + filePath + @"""";
            p.Start();
        }
    }
    class Setting {
        public static SettingClass value {
            get {
                if (_instance == null) {
                    _instance = new Setting();
                    _instance.init();
                }
                return _instance.setting;
            }
            set {
                if (_instance == null) {
                    _instance = new Setting();
                    _instance.init();
                }
                _instance.save(value);
            }
        }
        private static Setting _instance = null;
        private SettingClass setting;
        private String saveFilePath;
        private Setting() { }
        private void init() {
            if (setting != null) {
                return;
            }
            var myPath = Assembly.GetEntryAssembly().Location;
            var myDirectory = Path.GetDirectoryName(myPath);
            var myFilenameWithoutExtension = Path.GetFileNameWithoutExtension(myPath);
            var settingPath = Path.Combine(myDirectory, myFilenameWithoutExtension + ".xml");
            saveFilePath = settingPath;
            if (!File.Exists(saveFilePath)) {
                setting = new SettingClass();
                return;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(SettingClass));
            StreamReader sr = new StreamReader(saveFilePath, new UTF8Encoding(false));
            setting = (SettingClass)serializer.Deserialize(sr);
            sr.Close();
        }
        private void save(SettingClass value) {
            XmlSerializer serializer = new XmlSerializer(typeof(SettingClass));
            StreamWriter sw = new StreamWriter(saveFilePath, false, new UTF8Encoding(false));
            serializer.Serialize(sw, setting);
            sw.Close();
        }
    }
    public class SettingClass {
        public String OpenFilePath { get; set; }
    }
}
