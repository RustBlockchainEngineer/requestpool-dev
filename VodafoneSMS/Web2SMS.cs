using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Vodafone
{
    public class Web2SMS
    {
        private static string Url = "https://e3len.vodafone.com.eg/web2sms/sms/submit/";
        private static string AccountId = "";
        private static string Password = "";
        private static string ClientSecret = "";
        private static string DefaultSenderName = "IDSC";


        public static void Configure(string url, string accountId, string password, string clientSecret, string defaultSenderName = "IDSC")
        {
            AccountId = accountId;
            Password = password;
            ClientSecret = clientSecret;
            Url = url;
            DefaultSenderName = defaultSenderName;
        }


        public static SMSResponse Send(List<Recepient> recepients)
        {
            string xmlns = "http://www.edafa.com/web2sms/sms/model/";
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration dec = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(dec);
            XmlElement rootNode = xmlDoc.CreateElement("SubmitSMSRequest", xmlns);

            XmlAttribute xsi = xmlDoc.CreateAttribute("xmlns", "xsi", "http://www.w3.org/2000/xmlns/");
            xsi.Value = "http://www.w3.org/2001/XMLSchema-instance";
            rootNode.Attributes.Append(xsi);

            XmlAttribute schemaLocation = xmlDoc.CreateAttribute("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance");
            schemaLocation.Value = "http://www.edafa.com/web2sms/sms/model/SMSAPI.xsd";
            rootNode.Attributes.Append(schemaLocation);

            XmlAttribute xsiType = xmlDoc.CreateAttribute("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance");
            xsiType.Value = "SubmitSMSRequest";
            rootNode.Attributes.Append(xsiType);

            xmlDoc.AppendChild(rootNode);

            XmlNode accountNode = xmlDoc.CreateElement("AccountId", xmlns);
            accountNode.InnerText = AccountId;
            rootNode.AppendChild(accountNode);

            XmlNode passwordNode = xmlDoc.CreateElement("Password", xmlns);
            passwordNode.InnerText = Password;
            rootNode.AppendChild(passwordNode);

            StringBuilder secureHashInput = new StringBuilder();
            secureHashInput.Append("AccountId=");
            secureHashInput.Append(AccountId);
            secureHashInput.Append("&");
            secureHashInput.Append("Password=");
            secureHashInput.Append(Password);
            foreach (Recepient r in recepients)
            {
                secureHashInput.Append("&");
                secureHashInput.Append("SenderName=");
                secureHashInput.Append(String.IsNullOrEmpty(r.SenderName) ? DefaultSenderName : r.SenderName);
                secureHashInput.Append("&");
                secureHashInput.Append("ReceiverMSISDN=");
                secureHashInput.Append(r.ReceiverMSISDN);
                secureHashInput.Append("&");
                secureHashInput.Append("SMSText=");
                secureHashInput.Append(r.SMSText);
            }
            XmlNode hashNode = xmlDoc.CreateElement("SecureHash", xmlns);
            HMACSHA secureHashEncoder = new HMACSHA();
            hashNode.InnerText = secureHashEncoder.ConvertStringToHash(secureHashInput.ToString(), ClientSecret);
            //hashNode.InnerText = GetSecureHash(HexStringToByteArray(ClientSecret), secureHashInput.ToString());
            rootNode.AppendChild(hashNode);

            foreach (Recepient r in recepients)
            {
                XmlNode smsNode = xmlDoc.CreateElement("SMSList", xmlns);
                XmlNode senderNode = xmlDoc.CreateElement("SenderName", xmlns);
                XmlNode receiverNode = xmlDoc.CreateElement("ReceiverMSISDN", xmlns);
                XmlNode textNode = xmlDoc.CreateElement("SMSText", xmlns);

                senderNode.InnerText = String.IsNullOrEmpty(r.SenderName) ? DefaultSenderName : r.SenderName;
                receiverNode.InnerText = r.ReceiverMSISDN;
                textNode.InnerText = r.SMSText;

                smsNode.AppendChild(senderNode);
                smsNode.AppendChild(receiverNode);
                smsNode.AppendChild(textNode);

                rootNode.AppendChild(smsNode);
            }
            //xmlDoc.Save(HttpContext.Current.Server.MapPath("~/sms.xml"));
            string xmlString = "";
            using (var stringWriter = new StringWriter())
            {
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    xmlDoc.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    xmlString = stringWriter.GetStringBuilder().ToString();
                }
            }
            var requestData = Encoding.UTF8.GetBytes(xmlString);

            //ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);
            WebRequest webRequest = WebRequest.Create(Url);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/xml";
            webRequest.ContentLength = requestData.Length;
            using (var stream = webRequest.GetRequestStream())
            {
                stream.Write(requestData, 0, requestData.Length);
            }
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();
                string responseText = new StreamReader(response.GetResponseStream()).ReadToEnd();

                List<string> headers = new List<string>();
                foreach (var headerName in response.Headers.AllKeys)
                {
                    headers.Add(headerName + " : " + response.Headers[headerName]);
                }
                return new SMSResponse()
                {
                    FinalStatus = true,
                    ResponseText = responseText,
                    StatusCode = response.StatusCode.ToString(),
                    StatusDescription = response.StatusDescription,
                    Headers = headers
                };
            }
            catch (WebException ex)
            {
                List<string> requestHeaders = new List<string>();
                foreach (var headerName in webRequest.Headers.AllKeys)
                {
                    requestHeaders.Add(headerName + " : " + webRequest.Headers[headerName]);
                }
                List<string> responseHeaders = new List<string>();
                foreach (var headerName in ex.Response.Headers.AllKeys)
                {
                    responseHeaders.Add(headerName + " : " + ex.Response.Headers[headerName]);
                }

                return new SMSResponse()
                {
                    FinalStatus = false,
                    ResponseText = ex.Message,
                    StatusCode = ex.Status.ToString(),
                    Headers = responseHeaders,
                    RequestHeaders = requestHeaders,
                    RequestBody = xmlString
                };
            }
        }

        public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }

    public class Recepient
    {
        public string SenderName { get; set; }
        public string ReceiverMSISDN { get; set; }
        public string SMSText { get; set; }
    }

    public class SMSResponse
    {
        public bool FinalStatus { get; set; }
        public string ResponseText { get; set; }
        public List<string> Headers { get; set; }
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string RequestBody { get; set; }
        public List<string> RequestHeaders { get; set; }
    }

    /*
       public static string GetSecureHash(byte[] key, String source)
       {
           String secureHash = "";
           using (HMACSHA256 hmac = new HMACSHA256(key))
           {
               byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(source));
               secureHash = BitConverter.ToString(hashValue).Replace("-", "");
           }
           return secureHash;
       }

       public static byte[] HexStringToByteArray(String hex)
       {
           long NumberChars = hex.Length;
           byte[] bytes = new byte[NumberChars / 2];
           for (long i = 0; i < NumberChars; i += 2)
               bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
           return bytes;
       }
       */

}
