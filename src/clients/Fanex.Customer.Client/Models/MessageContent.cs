using System.Collections.Generic;

namespace Fanex.Customer.Client.Models
{
    public class MessageContent
    {
        public string MainMessage { get; set; }

        public IEnumerable<SubMessageContent> SubMessages { get; set; }
    }

    public class SubMessageContent
    {
        public string Message { get; set; }

        public bool IsLive { get; set; }
    }
}