using System.Text;

namespace i18n.Core.Models
{
    /// <summary>
    /// A localized message residing in a PO resource file
    /// </summary>
    public class I18NMessage
    {
        public string MsgId { get; private set; }
        public string MsgStr { get; private set; }

        public I18NMessage(string msgId, string msgStr)
        {
            MsgId = msgId;
            MsgStr = msgStr;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("msgid \"").Append(MsgId).Append("\"").AppendLine();
            sb.Append("msgstr \"").Append(MsgStr).Append("\"").AppendLine();
            return sb.ToString();
        }
    }
}

