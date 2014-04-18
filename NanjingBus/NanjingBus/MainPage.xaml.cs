using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using NanjingBus.Method;
using NanjingBus.Resources;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace NanjingBus
{
    public partial class MainPage : PhoneApplicationPage
    {
        private XElement StationXml;
        private XElement LineXml;
        //线路名称
        private string lineName;
        private string lastStation;
        //站点名称
        private string stationName;
        //当前所在站点名称
        private string currStation;

        private string wholeLineName;

        ObservableCollection<string> directionCollection = new ObservableCollection<string>();

        ObservableCollection<string> stationCollection = new ObservableCollection<string>();

        public ObservableCollection<string> DirectionCollection
        {
            get
            {
                return directionCollection;
            }
            set
            {
                directionCollection = value;
            }
        }

        public ObservableCollection<string> StationCollection
        {
            get
            {
                return stationCollection;
            }
            set
            {
                stationCollection = value;

            }
        }


        ObservableCollection<string> matchingStation = new ObservableCollection<string>();
        //输入站点名称返回的所有站点
        public ObservableCollection<string> MatchingStation
        {
            get
            {
                return matchingStation;
            }
            set
            {
                matchingStation = value;
            }
        }

        ObservableCollection<string> lineCollection = new ObservableCollection<string>();

        public ObservableCollection<string> LineCollection
        {
            get
            {
                return lineCollection;
            }
            set
            {
                lineCollection = value;
            }
        }

      
        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
        }


        /// <summary>
        /// 获取某站点的所有线路
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Station_OnTap(object sender, GestureEventArgs e)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageHelper.Show("手机没有联网噜~");
                return;
            }
            if (string.IsNullOrEmpty(StationTextBox.Text))
            {
                MessageHelper.Show("您还没输入线路哦~");
                return;
            }

            MatchingStation.Clear();
            ProgressBar.Visibility = Visibility.Visible;


            string uri = "http://openapi.aibang.com/bus/stats?" + "app_key=" + CommonParam.appkey + "&city=" + "南京" +
                         "&q=" + StationTextBox.Text;
            WebRequest request = HttpWebRequest.Create(uri);
            IAsyncResult result = request.BeginGetResponse(StationCallback, request);

        }

        private void StationCallback(IAsyncResult result)
        {

            HttpWebRequest request = (HttpWebRequest)result.AsyncState;
            WebResponse response;
            try
            {
                response = request.EndGetResponse(result);
            }
            catch (Exception)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    ProgressBar.Visibility = Visibility.Collapsed;
                    MessageHelper.Show("网络出错噜~");
                });

                return;
            }
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    XElement xml = XElement.Load(reader);
                    LineXml = xml;
                    if (xml.Element("result_num").Value == "0")
                    {
                        //currStation = xml.Descendants("name").FirstOrDefault().Value;
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            MessageHelper.Show("没有匹配的公交站点噜~");
                            ProgressBar.Visibility = Visibility.Collapsed;
                        });
                        return;
                    }
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        foreach (var descendant in xml.Descendants("name"))
                        {
                            matchingStation.Add(descendant.Value);
                        }
                        ProgressBar.Visibility = Visibility.Collapsed;
                    });

                }
            }
        }

        /// <summary>
        /// 获取某条线路的正反方向和所有站点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Line_OnTap(object sender, GestureEventArgs e)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageHelper.Show("手机没有联网噜~");
                return;
            }
            if (string.IsNullOrEmpty(LineTextBox.Text))
            {
                MessageHelper.Show("您还没输入线路哦~");
                return;
            }
            DirectionCollection.Clear();
            ProgressBar.Visibility = Visibility.Visible;

            string uri = "http://openapi.aibang.com/bus/lines?" + "app_key=" + CommonParam.appkey + "&city=" + "南京" + "&q=" +
                        LineTextBox.Text;
            WebRequest request = HttpWebRequest.Create(uri);
            IAsyncResult result = request.BeginGetResponse(ResponseCallback, request);
        }



        private void ResponseCallback(IAsyncResult result)
        {
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;
            WebResponse response;
            try
            {
                 response = request.EndGetResponse(result);
            }
            catch (Exception)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    ProgressBar.Visibility = Visibility.Collapsed;
                    MessageHelper.Show("网络出错噜~");
                });
            
                return;
            }
 
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    XElement xml = XElement.Load(reader);
                    StationXml = xml;
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        foreach (var descendant in xml.Descendants("name"))
                        {
                            directionCollection.Add(descendant.Value);
                        }
                        ProgressBar.Visibility = Visibility.Collapsed;
                    });
                }
            }
        }

        /// <summary>
        /// 选择某一方向线路
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Direction_OnTap(object sender, GestureEventArgs e)
        {
            //StationPopup.Visibility = Visibility.Visible;
            OpenTitleStoryboard.Begin();
            TextBlock tb = sender as TextBlock;
            StationCollection.Clear();
            lineName = tb.Text.Substring(0, tb.Text.IndexOf('('));
            foreach (var trigger in StationXml.Descendants("name"))
            {
                if (trigger.Value == tb.Text)
                {
                    var tmpXml = trigger.AncestorsAndSelf();
                    string[] allStation = tmpXml.Descendants("stats").FirstOrDefault().Value.Split(';');
                    foreach (var s in allStation)
                    {
                        stationCollection.Add(s);
                    }
                    lastStation = allStation.Last();
                    break;
                }
            }
        }

        /// <summary>
        /// 选择当前所在站点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrStation_OnTap(object sender, GestureEventArgs e)
        {
            TextBlock tb = sender as TextBlock;

            if (tb != null)
            {
                (App.Current as App).stationList = stationCollection;
             
                NavigationService.Navigate(new Uri("/DetailPage.xaml?tb=" + tb.Text + "&lineName=" + lineName + "&lastStation=" + lastStation, UriKind.Relative));

            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            StationPopup.Visibility = Visibility.Collapsed;
            LineGrid.Visibility = Visibility.Collapsed;
        }


        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (StationPopup.Visibility == Visibility.Visible)
            {
                StationPopup.Visibility = Visibility.Collapsed;
                e.Cancel = true;
            }
            if (LineGrid.Visibility == Visibility.Visible)
            {
                LineGrid.Visibility = Visibility.Collapsed;
                e.Cancel = true;
            }
        }

        private void MatchingStation_OnTap(object sender, GestureEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            if (tb != null)
            {
                currStation = tb.Text;
                OpenLineStoryboard.Begin();
                LineCollection.Clear();
                foreach (var item in LineXml.Descendants("name"))
                {
                    if (item.Value == currStation)
                    {
                        var tmpXml = item.AncestorsAndSelf();
                        string[] lines = tmpXml.Descendants("line_names").FirstOrDefault().Value.Split(';');
                        foreach (var line in lines)
                        {
                            lineCollection.Add(line);
                        }
                        //lastStation = lines.Last();
                        break;
                    }

                }
            }
        }

        /// <summary>
        /// 获取站点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrLine_OnTap(object sender, GestureEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            if (tb != null)
            {
                string str = tb.Text;
                wholeLineName = str;
                int i = str.IndexOf('(');
                lineName = str.Substring(0, i);
                //int j = str.IndexOf('-');
                //string destination = str.Substring(j + 1, str.Length - j - 2);

                string uri = "http://openapi.aibang.com/bus/lines?" + "app_key=" + CommonParam.appkey + "&city=" + "南京" + "&q=" + lineName;
                WebRequest request = HttpWebRequest.Create(uri);
                IAsyncResult result = request.BeginGetResponse(NavCallback, request);

            }
        }

       
        private void NavCallback(IAsyncResult result)
        {
            string destination = string.Empty;
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;
            WebResponse response;
            try
            {
                response = request.EndGetResponse(result);
            }
            catch (Exception)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    ProgressBar.Visibility = Visibility.Collapsed;
                    MessageHelper.Show("网络出错噜~");
                });
                return;
            }
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    XElement xml = XElement.Load(reader);

                    foreach (var descendant in xml.Descendants("name"))
                    {
                        if (descendant.Value == wholeLineName)
                        {
                            var tmpXml = descendant.AncestorsAndSelf();
                            string[] allStation = tmpXml.Descendants("stats").FirstOrDefault().Value.Split(';');
                            destination = allStation.Last();
                            (App.Current as App).stationList = new ObservableCollection<string>();
                            foreach (var item in allStation)
                            {
                                (App.Current as App).stationList.Add(item);
                            }
                            break;
                        }
                    }
                }
            }
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(
                    new Uri(
                        "/DetailPage.xaml?tb=" + currStation + "&lineName=" + lineName + "&lastStation=" + destination,
                        UriKind.Relative));
            });

        }


        private void AboutBarMenuItem_OnClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }
    }
}