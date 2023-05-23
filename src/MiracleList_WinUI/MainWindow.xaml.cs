// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using MiracleList;
using MiracleList_WinUI.Events;
using MiracleList_WinUI.Views;
using MvvmGen.Events;

namespace MiracleList_WinUI;

public sealed partial class MainWindow : Window,
    IEventSubscriber<UserLoggedInEvent,
        UserLoggedOutEvent,
        ShowViewEvent>
{
    private readonly IAppState appState;
    private Dictionary<Type, object> _viewCache = new();

    public MainWindow(IEventAggregator eventAggregator,IAppState appState)
    {
        this.InitializeComponent();
        ShowView(typeof(LoginView));
        eventAggregator.RegisterSubscriber(this);
        MaximizeWindow();
        this.appState = appState;
    }

    public void MaximizeWindow()
    {
        var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);

        PInvoke.User32.ShowWindow(windowHandle, PInvoke.User32.WindowShowStyle.SW_MAXIMIZE);
    }

    public void OnEvent(UserLoggedInEvent eventData)
    {
        footerControl.UpdateStatusText();
        ShowView(typeof(TaskManagementView));
    }
    
    public void OnEvent(UserLoggedOutEvent eventData)
    {
        appState.Username = "";
        _viewCache.Clear();
        footerControl.UpdateStatusText();
        ShowView(typeof(LoginView));
    }

    public void OnEvent(ShowViewEvent eventData)
    {
        ShowView(eventData.ViewType);
    }

    private void ShowView(Type viewType)
    {
        if (!_viewCache.ContainsKey(viewType))
        {
            var view = ((App)App.Current).ServiceProvider.GetService(viewType)
                ?? throw new Exception($"View not registered {viewType}");
            _viewCache.Add(viewType, view);
        }

        contentArea.Content = _viewCache[viewType];
    }
}

