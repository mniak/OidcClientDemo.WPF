using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OidcClientDemo.WPF.Messages
{
    class AlertMessage
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public AlertType Type { get; set; }

        public enum AlertType { 
            Message,
            Information,
            Warning,
            Error,
        }
    }
}
