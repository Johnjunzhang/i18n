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

        protected bool Equals(I18NMessage other)
        {
            return string.Equals(MsgId, other.MsgId) && string.Equals(MsgStr, other.MsgStr);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((I18NMessage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((MsgId != null ? MsgId.GetHashCode() : 0)*397) ^ (MsgStr != null ? MsgStr.GetHashCode() : 0);
            }
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

