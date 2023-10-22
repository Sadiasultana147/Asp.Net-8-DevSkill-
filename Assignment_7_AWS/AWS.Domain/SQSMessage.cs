using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS.Domain
{
    public class SQSMessage
    {
        public string Content { get; set; }
        public string ReceiptHandle { get; set; }
    }
}
