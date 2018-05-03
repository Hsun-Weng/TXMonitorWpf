using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using FutureApplication.Utility;


namespace FutureApplication
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon systemNotifyIcon;
        RestUtility restUtil;
        Thread backThread;
        ListBoxItem priceItem;
        private Boolean isUpdatePrice;
        ManualResetEvent _mre = new ManualResetEvent(false);
        int prePrice = 0;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.systemNotifyIcon = new System.Windows.Forms.NotifyIcon();
            //this.systemNotifyIcon.Icon = new System.Drawing.Icon(((Bitmap)Properties.Resources.ResourceManager.GetObject("tray")).GetHicon());
            this.systemNotifyIcon.Text = "Hello";
            this.systemNotifyIcon.Visible = true;

            restUtil = new RestUtility();

            isUpdatePrice = true;
            
            startUpdatePriceProcess();
        }

        private void backProcess()
        {
            String curPrice;
            int curPriceInt;

            while(true){
                curPrice = restUtil.getTXCurPrice();

                if (curPrice.Length == 0)
                {
                    curPriceInt = 0;
                }
                else
                {
                    curPriceInt = Int16.Parse(curPrice);
                }

                try
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action<int>(updateCurPriceLabel), curPriceInt);
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action<int>(appendPrice), curPriceInt);
                    prePrice = Int16.Parse(curPrice);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                Thread.Sleep(5 * 1000);//單位毫秒
            }
        }

        private void appendPrice(int curPrice){
            priceItem = new ListBoxItem();
            priceItem.Content = curPrice + "  " + DateTime.Now.ToString("HH:mm:ss");
            priceList.Items.Add(priceItem);
        }

        private void updateCurPriceLabel(int curPrice)
        {
            if (curPrice > prePrice)
            {
                curPriceText.Foreground = Brushes.Red;
            }
            else if (curPrice < prePrice)
            {
                curPriceText.Foreground = Brushes.Green;
            }
            else
            {
                curPriceText.Foreground = Brushes.Black;
            }
            
            curPriceText.Content = curPrice;
        }

        private void stopUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Button updateButton = (Button)sender;
            isUpdatePrice = !isUpdatePrice;
            if (isUpdatePrice)
            {
                updateButton.Content = "停止取價";
                startUpdatePriceProcess();
            }
            else
            {
                updateButton.Content = "開始取價";
                backThread.Abort();
            }
        }

        public void startUpdatePriceProcess()
        {
            ThreadStart backThreadStart = new ThreadStart(backProcess);

            backThread = new Thread(backThreadStart);
            backThread.Name = "updatePriceThread";
            backThread.IsBackground = true;
            backThread.Start();
        }
    }
}
