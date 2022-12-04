using Newtonsoft.Json;
using SimpleTimeTracker.Core;
using SimpleTimeTracker.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;

namespace SimpleTimeTracker.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            Instance = this;
            this.DataContext = new MainWindowViewModel();
            InitializeComponent();

            // ウィンドウのサイズを復元
            RecoverWindowBounds(TemporaryFilePath());
        }

        public static MainWindow Instance { get; private set; }


        protected override void OnClosing(CancelEventArgs e)
        {
            // ウィンドウのサイズを保存
            SaveWindowBounds(TemporaryFilePath());
            base.OnClosing(e);
        }

        private string TemporaryFilePath()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "temporary.json");
        }

        /// <summary>
        /// ウィンドウの位置・サイズを保存します。
        /// </summary>
        void SaveWindowBounds(string path)
        {
            var settings = new Temporary();
            settings.WindowMaximized = WindowState == WindowState.Maximized;
            WindowState = WindowState.Normal; // 最大化解除
            settings.WindowLeft = Left;
            settings.WindowTop = Top;
            settings.WindowWidth = Width;
            settings.WindowHeight = Height;

            var str = JsonConvert.SerializeObject(settings);
            File.WriteAllText(path, str);
        }

        /// <summary>
        /// ウィンドウの位置・サイズを復元します。
        /// </summary>
        void RecoverWindowBounds(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            var str = File.ReadAllText(path);

            if (string.IsNullOrWhiteSpace(str))
            {
                return;
            }

            var settings = JsonConvert.DeserializeObject<Temporary>(str);

            if (settings == null)
            {
                return;
            }

            // 左
            if (settings.WindowLeft >= 0 && (settings.WindowLeft + settings.WindowWidth) < SystemParameters.VirtualScreenWidth)
            {
                Left = settings.WindowLeft; 
            }
            // 上
            if (settings.WindowTop >= 0 && (settings.WindowTop + settings.WindowHeight) < SystemParameters.VirtualScreenHeight)
            { 
                Top = settings.WindowTop; 
            }
            // 幅
            if (settings.WindowWidth > 0 && settings.WindowWidth <= SystemParameters.WorkArea.Width)
            { 
                Width = settings.WindowWidth;
            }
            // 高さ
            if (settings.WindowHeight > 0 && settings.WindowHeight <= SystemParameters.WorkArea.Height)
            { 
                Height = settings.WindowHeight; 
            }
            // 最大化
            if (settings.WindowMaximized)
            {
                // ロード後に最大化
                Loaded += (o, e) => WindowState = WindowState.Maximized;
            }
        }
    }

    public class Temporary
    {
        public double WindowLeft { get; set;}
        public double WindowTop { get; set;}
        public double WindowWidth { get; set;}
        public double WindowHeight { get; set;}
        public bool WindowMaximized { get; set;}
    }

}
