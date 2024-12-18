﻿using CommunityToolkit.Mvvm.Messaging;
using DataAcquisition.Core.Views.ContentPage;
using DataAcquisition.Core.Views.MainView.Model;
using DataAcquisition.Core.Views.WindowBase;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;


namespace DataAcquisition.Core.Views.MainView.Components
{
    /// <summary>
    /// BreadCrumbBar.xaml 的交互逻辑
    /// </summary>
    public partial class BreadCrumbBar : UserControl
    {
        public static Dictionary<TreeItemModel, System.Windows.Window> items { get; set; } = new Dictionary<TreeItemModel, System.Windows.Window>();

        private Page _basepage = new BasePage();

        private ObservableCollection<TreeItemModel>? _baseItem;
        public ObservableCollection<TreeItemModel> BaseList
        {
            get => _baseItem;
            set
            {
                _baseItem = value;
            }
        }

        private TreeItemModel BaselistRemove(TreeItemModel value)
        {
            var index = Array.IndexOf(BaseList.ToArray(), value);
            TreeItemModel? page = new TreeItemModel();

            if (index > 0 && BaseList[index].IsChecked)
            {
                BaseList[index - 1].IsChecked = true;
                page = BaseList[index - 1];
            }
            else if (index == 0 && BaseList.Count > 1 && BaseList[index].IsChecked)
            {
                BaseList[1].IsChecked = true;
                page = BaseList[1];
            }
            value.IsChecked = false;
            this.BaseList.Remove(value);
            //items.Remove(value);
            if (this.BaseList.Count < 1)
            {
                this.HeaderBorder.Visibility = Visibility.Collapsed;
                page.Page = _basepage;

            }
            return page;
        }

        private void BaselistAdd(TreeItemModel value)
        {
            var r = this.BaseList.FirstOrDefault(x => x == value);
            if (r is not null)
            {
                r.IsChecked = true;
            }
            else
            {
                this.BaseList.Add(value);
                //items.Add(value);
            }



            if (this.HeaderBorder.Visibility == Visibility.Collapsed)
            {
                this.HeaderBorder.Visibility = Visibility.Visible;
            }
        }

        public BreadCrumbBar()
        {
            InitializeComponent();
            BaseList = new ObservableCollection<TreeItemModel>();
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath(nameof(BaseList));
            binding.Mode = BindingMode.TwoWay;
            navButton.SetBinding(ItemsControl.ItemsSourceProperty, binding);

            WeakReferenceMessenger.Default.Register<TreeItemModel>(this, PageAddItem);
            this.frame.Navigate(_basepage);
        }

        private void PageAddItem(object recipient, TreeItemModel message)
        {
            this.Dispatcher.Invoke(() =>
            {

                if (message.Page is not null)
                {
                    var windowOpen = items.FirstOrDefault(e => e.Key == message);
                    if (windowOpen.Value is not null)
                    {
                        windowOpen.Value.Activate();
                        windowOpen.Value.Focusable = true;
                        return;
                    }

                    if (NaviControl.olditemModel is not null 
                    && message == NaviControl.olditemModel && message.PageStatus == PageStatus.Page) 
                    {
                        Growl.Warning($"页面正在显示！");
                        return;
                    }


                    if (message.IsPersistence)
                        this.frame.Navigate(message.Page);
                    else
                    {
                        var temp = Activator.CreateInstance(message.Page.GetType());
                        this.frame.Navigate(temp);

                        message.Page = temp as Page;
                    }
                    BaselistAdd(message);
                    NaviControl.olditemModel = message;

                    if(App.MainWindowShow.WindowState == WindowState.Minimized)
                    {
                        App.MainWindowShow.WindowState = WindowState.Normal;
                    }

                    App.MainWindowShow.Activate();
                }
            });
            WeakReferenceMessenger.Default.Send<NaviSendMessenger<TreeItemModel>>(
                           new NaviSendMessenger<TreeItemModel>(message)
                           );
            NaviControl.olditemModel = message;
        }

        private async void Close_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is TreeItemModel value)
            {
                var nav = value.IsChecked;

                var page = BaselistRemove(value);
                if (nav&& page is not null)
                {
                    await Dispatcher.InvokeAsync(() =>
                     {
                         try
                         {
                             this.frame.RemoveBackEntry();
                         }
                         catch (InvalidOperationException)
                         {


                         }
                         this.frame.Navigate(null);
                         this.frame.Navigate(page.Page);
                     });




                    WeakReferenceMessenger.Default.Send<NaviSendMessenger<TreeItemModel>>(
                        new NaviSendMessenger<TreeItemModel>(page)
                        );



                }

            }
        }

        private void GotoView_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).Tag is TreeItemModel value)
            {
                if (value.Page is not null)
                {
                    this.frame.Navigate(value.Page);
                }
                WeakReferenceMessenger.Default.Send<NaviSendMessenger<TreeItemModel>>(
                          new NaviSendMessenger<TreeItemModel>(value)
                          );
            }
        }

        private void frame_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void Eject_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is TreeItemModel value)
            {
                var nav = value.IsChecked;


                PageWindow pageWindow = new PageWindow(value);
                items.Add(value, pageWindow);
                pageWindow.Show();

                var page = BaselistRemove(value);
                if (nav && page is not null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            this.frame.RemoveBackEntry();
                        }
                        catch (InvalidOperationException)
                        {


                        }
                        this.frame.Navigate(null);

                    });

                    Task.Run(() =>
                    {
                        Thread.Sleep(5);
                        Dispatcher.Invoke(() =>
                        {
                            this.frame.Navigate(page.Page);
                        });

                        WeakReferenceMessenger.Default.Send<NaviSendMessenger<TreeItemModel>>(
                           new NaviSendMessenger<TreeItemModel>(page)
                           );

                    });

                }

            }
        }
    }
}