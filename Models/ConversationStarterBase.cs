using System.Threading.Tasks;

namespace AxonPartners.Models
{
    public abstract class ConversationStarterBase : IConversationStarterBase
    {
        public string ToId { get; set; }
        public string ToName { get; set; }
        public string FromId { get; set; }
        public string FromName { get; set; }
        public string ServiceUrl { get; set; }
        public string ChannelId { get; set; }
        public string ConversationId { get; set; }

        public abstract Task Resume(string conversationId, string channelId, string text, string locale);
    }

    public interface IConversationStarterBase
    {
        Task Resume(string conversationId, string channelId, string text, string locale);
    }
}
