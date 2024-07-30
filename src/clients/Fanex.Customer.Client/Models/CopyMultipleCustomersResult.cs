using System.Collections.Generic;
using System.Linq;

namespace Fanex.Customer.Client.Models
{
    public class CopyMultipleCustomersResult : Result
    {
        public IEnumerable<string> Usernames { get; set; } = Enumerable.Empty<string>();
        
        public IEnumerable<string> Domains { get; set; } = Enumerable.Empty<string>();

        public MessageContent MessageContent { get; set; }

        public int MsgType { get; set; }
    }
}
