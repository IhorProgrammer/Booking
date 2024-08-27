using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLibrary.Services.MessageSender
{
    public interface IMessageSender
    {
        public abstract void Send( string jwt, MessageSenderTypes massageSender );
    }
}
