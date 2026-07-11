using System.Windows.Input;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;

namespace FlowLauncher.Components.UI;

public enum MessageType
{
    Info,
    Success,
    Warning,
    Error,
}

public readonly record struct ToastMessage(
    string Message,
    MessageType Type = MessageType.Info,
    ICommand? ClickCommand = null
);

public readonly record struct NotificationButton(
    string Text,
    ICommand? Command = null
);

public readonly record struct NotificationMessage(
    string Message,
    MessageType Type = MessageType.Info,
    ICommand? Command = null,
    IReadOnlyList<NotificationButton>? Buttons = null
);

partial class RootLayoutViewModel
{
    public partial class MessageHostModel(RootLayoutViewModel RootLayout)
    {
        public AvaloniaList<ToastMessage> ToastList { get; } = [];

        /// <summary>
        /// 发送 Toast 提示
        /// </summary>
        /// <param name="message">提示内容</param>
        [RelayCommand]
        public void SendToast(ToastMessage message)
        {
        }

        /// <summary>
        /// 发送通知消息
        /// </summary>
        /// <param name="message">消息内容</param>
        [RelayCommand]
        public void SendNotification(NotificationMessage message)
        {
        }
    }
}
