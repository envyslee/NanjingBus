using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
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
                                try
                                {
                                    bindList[int.Parse(item.currentLevel) - 1].BusId = item.busId;
                                    bindList[int.Parse(item.currentLevel) - 1].BusSpeed = item.busSpeed.Split(',').FirstOrDefault();
                                    bindList[int.Parse(item.currentLevel) - 1].Visibility = Visibility.Visible;
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
            try
            {
                CurrStation = NavigationContext.QueryString["tb"];
                lineName = NavigationContext.QueryString["lineName"];
                lastStation = NavigationContext.QueryString["lastStation"];

                stationCollection = (App.Current as App).stationList;
                foreach (var item in stationCollection)
                {
                    bindList.Add(new BindList(){StationName = item});
                }

            }
            catch (Exception)
            {
                MessageHelper.Show("传参错误");
            }


        }

        private void RefreshIconButton_OnClick(object sender, EventArgs e)
        {
            BindList.Clear();
            stationCollection = (App.Current as App).stationList;
            foreach (var item in stationCollection)
            {
                bindList.Add(new BindList() { StationName = item });
            }
            DetailPage_OnLoaded(null,null);
        }
    }
}