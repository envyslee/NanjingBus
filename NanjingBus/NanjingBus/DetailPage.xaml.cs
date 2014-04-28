using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using NanjingBus.Entity;
using NanjingBus.Method;
using Newtonsoft.Json;

namespace NanjingBus
{
    public partial class DetailPage : PhoneApplicationPage
    {
        private string CurrStation;
        private string lineName;
        private string lastStation;
        private int currStationId;
        private int busNums=0;
        private int nearestStationId;

        ObservableCollection<BindList> bindList = new ObservableCollection<BindList>();


      //当前公交信息
      public ObservableCollection<BindList> BindList
        {
            get
            {
                return bindList;
            }
            set
            {
                bindList = value;
            }
        }

        //所有站台名字
        private ObservableCollection<string> stationCollection;


        public DetailPage()
        {
            InitializeComponent();
        }

        private void DetailPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            busNums = 0;
            nearestStationId = 0;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                ProgressBar.Visibility = Visibility.Visible;
            });
            string uri =
                  "http://trafficomm.jstv.com/smartBus/Module=BusHelper/Controller=BusInfo/Action=GetCurrentBus/LineName=/StationName=" +
                  CurrStation + "/LineName=" + lineName + "/Destination=" + lastStation + "/key=07e1e5b97fc0f50b1bd842ceb1666973";
            WebRequest webRequest = HttpWebRequest.Create(uri);
            IAsyncResult result = webRequest.BeginGetResponse(CurrCallBack, webRequest);
           
        }
        private void CurrCallBack(IAsyncResult result)
        {
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;
            WebResponse response = request.EndGetResponse(result);
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    var resultData = reader.ReadToEnd();
                    CurrentData currentData = JsonConvert.DeserializeObject<CurrentData>(resultData);
                    if (currentData.data.Count!=0)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            foreach (var item in currentData.data)
                            {
                                //有公交
                                try
                                {
                                    bindList[int.Parse(item.currentLevel) - 1].BusId = item.busId;
                                    bindList[int.Parse(item.currentLevel) - 1].BusSpeed = item.busSpeed.Split(',').FirstOrDefault();
                                    bindList[int.Parse(item.currentLevel) - 1].Visibility = Visibility.Visible;
                                    if (int.Parse(item.currentLevel)<=currStationId)
                                    {
                                        busNums++;
                                        //第一次给值
                                        if (nearestStationId==0)
                                        {
                                            nearestStationId = currStationId - int.Parse(item.currentLevel) + 1;
                                        }
                                        //发现更小值即距离更近的公交，则重赋
                                        if (currStationId- int.Parse(item.currentLevel)+1<nearestStationId)
                                        {
                                            nearestStationId = currStationId - int.Parse(item.currentLevel) + 1;
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    MessageHelper.Show("数据有误");
                                }
                            }
                            ProgressBar.Visibility = Visibility.Collapsed;
                        });
                    }
                    else
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            MessageHelper.Show("该线路没有实时信息哦~");
                            ProgressBar.Visibility = Visibility.Collapsed;
                        });
                    }
                }
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode==NavigationMode.Back)
            {
                return;
            }
            CurrStation = NavigationContext.QueryString["tb"];
            lineName = NavigationContext.QueryString["lineName"];
            lastStation = NavigationContext.QueryString["lastStation"];
            stationCollection = (App.Current as App).stationList;
            currStationId = stationCollection.IndexOf(CurrStation)+1;

            Color themeColor = (Color)Application.Current.Resources["PhoneForegroundColor"];

            if (themeColor.ToString() == "#FFFFFFFF")
            {
                // 暗色主题
                try
                {
                    foreach (var item in stationCollection)
                    {
                        if (item == CurrStation)
                        {
                            bindList.Add(new BindList() { StationName = item, ColorBrush = new SolidColorBrush(Colors.Red) });
                        }
                        else
                        {
                            bindList.Add(new BindList() { StationName = item, ColorBrush = new SolidColorBrush(Colors.White) });
                        }
                    }

                }
                catch (Exception)
                {
                    MessageHelper.Show("传参错误");
                }
            }
            else
            {
                // 亮色主题   
                try
                {

                    foreach (var item in stationCollection)
                    {
                        if (item == CurrStation)
                        {
                            bindList.Add(new BindList() { StationName = item, ColorBrush = new SolidColorBrush(Colors.Red) });
                        }
                        else
                        {
                            bindList.Add(new BindList() { StationName = item, ColorBrush = new SolidColorBrush(Colors.Black) });
                        }
                    }

                }
                catch (Exception)
                {
                    MessageHelper.Show("传参错误");
                }
            } 
          

        }


        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshIconButton_OnClick(object sender, EventArgs e)
        {
            BindList.Clear();
            //stationCollection = (App.Current as App).stationList;
            Color themeColor = (Color)Application.Current.Resources["PhoneForegroundColor"];

            if (themeColor.ToString() == "#FFFFFFFF")
            {
                // 暗色主题
                try
                {
                    foreach (var item in stationCollection)
                    {
                        if (item == CurrStation)
                        {
                            bindList.Add(new BindList() { StationName = item, ColorBrush = new SolidColorBrush(Colors.Red) });
                        }
                        else
                        {
                            bindList.Add(new BindList() { StationName = item, ColorBrush = new SolidColorBrush(Colors.White) });
                        }
                    }

                }
                catch (Exception)
                {
                    MessageHelper.Show("传参错误");
                }
            }
            else
            {
                // 亮色主题   
                try
                {

                    foreach (var item in stationCollection)
                    {
                        if (item == CurrStation)
                        {
                            bindList.Add(new BindList() { StationName = item, ColorBrush = new SolidColorBrush(Colors.Red) });
                        }
                        else
                        {
                            bindList.Add(new BindList() { StationName = item, ColorBrush = new SolidColorBrush(Colors.Black) });
                        }
                    }

                }
                catch (Exception)
                {
                    MessageHelper.Show("传参错误");
                }
            } 
            DetailPage_OnLoaded(null,null);
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgIconButton_OnClick(object sender, EventArgs e)
        {
            SmsComposeTask smsTask=new SmsComposeTask();
            smsTask.Body = "当前共有" + busNums + "辆公交开往【" + CurrStation + "】，其中最近一辆距离该站还有" + nearestStationId + "站路。" + "\n" + "——来自【南京掌上公交】";
         
            smsTask.Show();
        }
    }
}