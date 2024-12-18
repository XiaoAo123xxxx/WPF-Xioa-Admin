using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAcquisition.Core.Views.DialogView.Model;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools.Extension;
using HandyControl.Tools.Extension;

namespace DataAcquisition.Core.Views.DialogView.ViewModel;

/// <summary>
/// @author Xioa
/// @date  2024年12月17日
/// </summary>
public partial class ProgressDialogViewModel : ObservableObject, IDialogResultable<string>
{
    [ObservableProperty] private string message;
    public Dialog? Dialog;

    public ProgressDialogViewModel()
    {
        this.Message = "BaseMessage";
        this.CloseAction = CloseAction_Method;
    }

    [RelayCommand]
    public void CloseDialog()
    {
        Result = this.Message;
        this.Dialog?.Close();
        //Dialog.Close(MessageToken.DialogPageToken);
    }

    private void CloseAction_Method()
    {
    }

    public string Result { get; set; }
    public Action CloseAction { get; set; }
}