using System.Collections.Generic;
using System.Text;

namespace i18n
{
    /// <summary>
    /// A localized message residing in a PO resource file
    /// </summary>
    public class I18NMessage
    {
        private readonly string msgId;
        private readonly string msgStr;

        public I18NMessage(string msgId, string msgStr)
        {
            this.msgId = msgId;
            this.msgStr = msgStr;
        }

        public string MsgId
        {
            get { return msgId; }
        }

        public string MsgStr
        {
            get { return msgStr; }
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

