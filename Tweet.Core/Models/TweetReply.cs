using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet.Core.Models
{
    public class TweetReply
    {
        public string TweetContent { get; set; }

        public DateTime Date { get; set; }

        public Profile User { get; set; }
    }
}
