using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Serializable]
    public class UserDialogState
    {
        public string UserId { get; set; }
        public string ChannelId { get; set; }
        public string ConversationId { get; set; }
        public string DialogId { get; set; }
        public bool IsFinished { get; set; }
        public int LastAskedQuestionId { get; set; }
        public DateTime sysUpdateDateUtc { get; set; }
    }
}
