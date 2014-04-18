using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace NanjingBus
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Email_OnClick(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask=new EmailComposeTask();
            emailComposeTask.Subject = "南京掌上公交";
            emailComposeTask.To = "leebin20@sina.cn";
            emailComposeTask.Show();
        }
        /// <summary>
        /// 评价
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Evaluate_OnClick(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask reviewTask=new MarketplaceReviewTask();
            reviewTask.Show();
        }
        /// <summary>
        /// 推荐应用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Recommend_OnClick(object sender, RoutedEventArgs e)
        {
            MarketplaceSearchTask searchTask=new MarketplaceSearchTask();
            searchTask.SearchTerms = "Potato Studio";
            searchTask.Show();
        }
    }
}