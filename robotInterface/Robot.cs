using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace robotInterface_barthelemy
{
    class Robot
    {
        public byte[] UART_ReceivedBytes;
        public Queue<byte> byteListReceived = new Queue<byte>();
    }
}
