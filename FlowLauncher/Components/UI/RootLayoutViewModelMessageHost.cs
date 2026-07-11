using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Metadata;
using CommunityToolkit.Mvvm.Input;

namespace FlowLauncher.Components.UI;

public enum MessageType
{
    Info,
    Success,
    Warning,
    Error,
}

public class MessageClickable : AvaloniaObject
{
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<MessageClickable, string>(nameof(Text));

    [Content]
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<MessageClickable, ICommand?>(nameof(Command));

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<MessageClickable, object?>(nameof(CommandParameter));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
}

public class ToastMessage : MessageClickable
{
    public static readonly StyledProperty<MessageType> TypeProperty =
        AvaloniaProperty.Register<ToastMessage, MessageType>(nameof(Type));

    public MessageType Type
    {
        get => GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }
}

public class NotificationButton : MessageClickable;

public class NotificationMessage : MessageClickable
{
    public static readonly StyledProperty<MessageType> TypeProperty =
        AvaloniaProperty.Register<NotificationMessage, MessageType>(nameof(Type));

    public MessageType Type
    {
        get => GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    public static readonly DirectProperty<NotificationMessage, List<NotificationButton>> ButtonsProperty =
        AvaloniaProperty.RegisterDirect<NotificationMessage, List<NotificationButton>>(
            nameof(Buttons),
            o => o.Buttons,
            (o, v) => o.Buttons = v
        );

    public List<NotificationButton> Buttons
    {
        get => field ??= [];
        set => SetAndRaise(ButtonsProperty, ref field!, value);
    }
}

partial class RootLayoutViewModel
{
    public partial class MessageHostModel(RootLayoutViewModel RootLayout)
    {
        public AvaloniaList<ToastMessage> ToastList { get; } = [];

        /// <summary>
        /// 发送 Toast 提示
        /// </summary>
        /// <param name="message">提示内容</param>
        public void SendToast(ToastMessage message)
        {
        }

        /// <summary>
        /// 发送通知消息
        /// </summary>
        /// <param name="message">消息内容</param>
        public void SendNotification(NotificationMessage message)
        {
        }
    }
}
