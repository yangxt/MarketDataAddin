using System.Text;

namespace MarketDataAddin
{
    public class Utilities
    {
        public static readonly Utilities instance = new Utilities();

        public byte[] GetBytes(string m_string)
        {
            if (null == m_string)
            {
                return null;
            }
            return Encoding.UTF8.GetBytes(m_string);
        }
        public string GetString(byte[] m_byte)
        {
            if (null == m_byte)
            {
                return null;
            }
            return Encoding.UTF8.GetString(m_byte);
        }

    }
}
