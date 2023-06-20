using System.Net;
using System.Text;
using UnityEngine.Networking;

namespace CP_SDK.Network
{
    /// <summary>
    /// Web Response class
    /// </summary>
    public sealed class WebResponse
    {
        /// <summary>
        /// Response bytes
        /// </summary>
        private byte[] m_BodyBytes;
        /// <summary>
        /// Body string
        /// </summary>
        private string m_BodyString = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Result code
        /// </summary>
        public readonly HttpStatusCode StatusCode;
        /// <summary>
        /// Reason phrase
        /// </summary>
        public readonly string ReasonPhrase;
        /// <summary>
        /// Is success
        /// </summary>
        public readonly bool IsSuccessStatusCode;
        /// <summary>
        /// Should retry
        /// </summary>
        public readonly bool ShouldRetry;
        /// <summary>
        /// Response bytes
        /// </summary>
        public byte[] BodyBytes => m_BodyBytes;
        /// <summary>
        /// Response string
        /// </summary>
        public string BodyString
        {
            get
            {
                if (m_BodyString == null)
                    m_BodyString = m_BodyBytes?.Length > 0 ? Encoding.UTF8.GetString(m_BodyBytes) : string.Empty;

                return m_BodyString;
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="p_Request">Reply status</param>
        public WebResponse(UnityWebRequest p_Request)
        {
            StatusCode          = (HttpStatusCode)p_Request.responseCode;
            ReasonPhrase        = p_Request.error;
            IsSuccessStatusCode = !(p_Request.isHttpError || p_Request.isNetworkError);
            ShouldRetry         = IsSuccessStatusCode ? false : (p_Request.responseCode < 400 || p_Request.responseCode >= 500);

            m_BodyBytes         = p_Request.downloadHandler.data;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Populate data
        /// </summary>
        /// <param name="p_BodyBytes">Body bytes</param>
        internal void Populate(byte[] p_BodyBytes)
            => m_BodyBytes = p_BodyBytes;
    }
}
