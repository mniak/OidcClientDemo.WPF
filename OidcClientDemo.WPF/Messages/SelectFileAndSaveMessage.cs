using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OidcClientDemo.WPF.Messages
{
    class SelectFileAndSaveMessage
    {
        public bool? Result { get; internal set; }

        public Action<Stream> WriteAction { get; internal set; }

    }
}
