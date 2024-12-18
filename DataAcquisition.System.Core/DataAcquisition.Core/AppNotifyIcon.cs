using System;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace DataAcquisition.Core;

/// <summary>
/// @author Xioa
/// @date  2024��12��17��
/// </summary>
public partial class App: Application
{
    private NotifyIcon? _notifyIcon;
    
    private void InitializeNotifyIcon()
    {
        _notifyIcon = new NotifyIcon
        {
            Icon = new System.Drawing.Icon("Assets/png/logo_32x32.ico"), 
            Visible = true,
            Text = "Xioa-Admin"
        };

        _notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        
        _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
        _notifyIcon.ContextMenuStrip.Items.Add("��", null, Open_Click);
        _notifyIcon.ContextMenuStrip.Items.Add("�˳�", null, Exit_Click);
    }
    
    private void NotifyIcon_DoubleClick(object? sender, EventArgs e)
    {
        MainWindowShow?.Show();
        MainWindowShow?.Activate();
    }

    private void Open_Click(object? sender, EventArgs e)
    {
        MainWindowShow?.Show();
        MainWindowShow?.Activate();
    }

    private void Exit_Click(object? sender, EventArgs e)
    {
        _notifyIcon?.Dispose();
        Application.Current.Shutdown();
        Environment.Exit(0);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _notifyIcon.Dispose(); // ��������ͼ��
        base.OnExit(e);
    }
    
}