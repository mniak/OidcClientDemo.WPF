using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using OidcClientDemo.WPF.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OidcClientDemo.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IEnumerable<IMessageHandler> messageHandlers;

        private interface IMessageHandler
        {
            void Register(object recipient, IMessenger messenger);
            void Unregister(object recipient, IMessenger messenger);
        }
        private class MessageHandler<T> : IMessageHandler
        {
            private readonly Action<T> eventHandler;

            public MessageHandler(Action<T> eventHandler)
            {
                this.eventHandler = eventHandler;
            }

            public void Register(object recipient, IMessenger messenger)
            {
                messenger.Register<T>(recipient, this.eventHandler);
            }

            public void Unregister(object recipient, IMessenger messenger)
            {
                messenger.Unregister<T>(recipient, this.eventHandler);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += this.MainWindow_Loaded;
            Unloaded += this.MainWindow_Unloaded;

            this.messageHandlers = new IMessageHandler[] {
                new MessageHandler<AlertMessage>(HandleAlert),
                new MessageHandler<SelectFileAndSaveMessage>(HandleSelectFileAndSave),
                new MessageHandler<SelectFileAndOpenMessage>(HandleSelectFileAndOpen),
            };
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var handler in messageHandlers)
            {
                handler.Register(this, Messenger.Default);
            }
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (var handler in messageHandlers)
            {
                handler.Unregister(this, Messenger.Default);
            }
        }

        private void HandleAlert(AlertMessage msg)
        {
            Activate();
            MessageBoxImage icon;
            switch (msg.Type)
            {
                case AlertMessage.AlertType.Information:
                    icon = MessageBoxImage.Information;
                    break;
                case AlertMessage.AlertType.Warning:
                    icon = MessageBoxImage.Warning;
                    break;
                case AlertMessage.AlertType.Error:
                    icon = MessageBoxImage.Error;
                    break;

                default:
                case AlertMessage.AlertType.Message:
                    icon = MessageBoxImage.None;
                    break;
            }
            MessageBox.Show(this, msg.Message, msg.Title, MessageBoxButton.OK, icon);
        }
        private void HandleSelectFileAndSave(SelectFileAndSaveMessage msg)
        {
            var dialog = new SaveFileDialog()
            {
                AddExtension = true,
                Filter = "OIDC Configuration|*.oidc",
                DefaultExt = ".oidc",
                Title = "Save OIDC configuration"
            };
            msg.Result = dialog.ShowDialog(this);
            if (!(msg.Result ?? false))
                return;

            using (var stream = dialog.OpenFile())
            {
                if (msg.WriteAction != null)
                    msg.WriteAction(stream);
            }
        }
        private void HandleSelectFileAndOpen(SelectFileAndOpenMessage msg)
        {
             var dialog = new OpenFileDialog()
            {
                AddExtension = true,
                Filter = "OIDC Configuration|*.oidc",
                DefaultExt = ".oidc",
                Title = "Open OIDC configuration"
            };
            msg.Result = dialog.ShowDialog(this);
            if (!(msg.Result ?? false))
                return;

            using (var stream = dialog.OpenFile())
            {
                if (msg.ReadAction != null)
                    msg.ReadAction(stream);
            }
        }
    }
}
