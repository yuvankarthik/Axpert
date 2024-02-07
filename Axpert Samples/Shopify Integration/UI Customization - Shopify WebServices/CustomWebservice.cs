using ASBExt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Net.Configuration;
using System.Net;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace ASBCustom
{

    /// <summary>
    /// Summary description for customwebservice
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class CustomWebservice : System.Web.Services.WebService
    {
        Util.Util utilObj = new Util.Util();
        ASBExt.WebServiceExt asbExt = new ASBExt.WebServiceExt();
        LogFile.Log logobj = new LogFile.Log();

        [WebMethod(EnableSession = true)]
        public string CustomFunction()
        {
            string result = string.Empty;
            result = Session["project"].ToString();
            return result;
        }
        [WebMethod(EnableSession = true)]
        public string SetIvparams(string param)
        {
            string result = "done";
            Session["iviewcustomparams"] = param;
            return result;
        }
        [WebMethod(EnableSession = true)]
        public string GetChoices(string transid, string sqlQuery)
        {
            if (HttpContext.Current.Session["project"] == null)
                return utilObj.SESSTIMEOUT;
            string errorLog = logobj.CreateLog("Call GetChoices", Session["nsessionid"].ToString(), "CallGetChoices-" + transid + "", "new");
            string inputXML = string.Empty;
            inputXML = "<sqlresultset axpapp='" + Session["project"].ToString() + "' sessionid='" + Session["nsessionid"].ToString() + "' trace='" + errorLog + "' appsessionkey='" + HttpContext.Current.Session["AppSessionKey"].ToString() + "' username='" + HttpContext.Current.Session["username"].ToString() + "' ><sql>" + sqlQuery + "</sql>";
            inputXML += HttpContext.Current.Session["axApps"].ToString() + HttpContext.Current.Application["axProps"].ToString() + HttpContext.Current.Session["axGlobalVars"].ToString() + HttpContext.Current.Session["axUserVars"].ToString() + "</sqlresultset>";
            string result = asbExt.CallGetChoiceWS(transid, inputXML);
            return result;
        }



        [WebMethod(EnableSession = true)]
        public string CreateFastReportPDF(ArrayList fldArray, ArrayList fldDbRowNo, ArrayList fldValueArray, ArrayList fldDeletedArray, string s, string key)
        {
            if (HttpContext.Current.Session["project"] == null)
                return utilObj.SESSTIMEOUT;
            TStructData tstData = (TStructData)Session[key];
            tstData.GetFieldValueXml(fldArray, fldDbRowNo, fldValueArray, fldDeletedArray, "-1", "false", "ALL", "");
            s += "<varlist><row>" + tstData.fldValueXml + tstData.memVarsData + "</row></varlist>";
            s += HttpContext.Current.Session["axApps"].ToString() + HttpContext.Current.Application["axProps"].ToString() + HttpContext.Current.Session["axGlobalVars"].ToString() + HttpContext.Current.Session["axUserVars"].ToString() + "</root>";
            string result = string.Empty;
            result = tstData.CallCreateFastPDFWS(s);
            return result;
        }

        public string CheckDMSIntegration(string structid)
        {
            string dmsSql = string.Empty;
            string res = string.Empty;
            string errorLog = string.Empty;

            ASBExt.WebServiceExt objWebServiceExt = new ASBExt.WebServiceExt();
            /*  dmsSql = "<sqlresultset axpapp='" + HttpContext.Current.Session["project"].ToString() + "' sessionid='" + HttpContext.Current.Session["nsessionid"].ToString() + "' trace='" + errorLog + "'><sql>select  url,add_view,dc, fieldnames from tomni  where dmsid = '" + structid + "' </sql>";
              dmsSql += HttpContext.Current.Session["axApps"].ToString() + HttpContext.Current.Application["axProps"].ToString() + "</sqlresultset>";
              res = objWebServiceExt.CallGetChoiceWS("", dmsSql);
              if (res.Contains(Constants.ERROR) == true)
              {
                  res = res.Replace(Constants.ERROR, "");
                  res = res.Replace("</error>", "");
                  res = res.Replace("\n", "");
                  throw (new Exception(res));
              }*/
            return res;


        }

        [WebMethod(EnableSession = true)]
        public string SendMail(string toAddress, string msgBody, string subject, string userMail)
        {

            string status = "Failure";
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                NetworkCredential credential = (NetworkCredential)smtp.Credentials;

                string from = credential.UserName;
                string host = smtp.Host;
                int port = smtp.Port;
                bool enableSsl = smtp.EnableSsl;
                string user = credential.UserName;
                string password = credential.Password;
                
                message.From = new MailAddress(from);
                message.To.Add(new MailAddress(toAddress));
                message.CC.Add(from + "," + userMail);
                message.Subject = subject;
                message.IsBodyHtml = false; //to make message body as html  
                message.Body = msgBody;
                smtp.Port = port;
                smtp.Host = host; //for gmail host  
                smtp.EnableSsl = enableSsl;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(user, password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                status = "Success";
            }
            catch (Exception ex)
            {
               LogFile.Log logObj = new LogFile.Log();
                logObj.CreateLog("Send Mail -\n\tError - " + ex.Message, HttpContext.Current.Session.SessionID, "Send Mail", "");
            }
            return status;
        }

        [WebMethod(EnableSession = true)]
        public string GetShopifyClientDetails()
        {
            string responseResult = "{\"status\":\"failed\",\"errormessage\":\"could not connect\"}";
            string InterSSOToken = string.Empty;
            {
                try
                {
                    string response = string.Empty;
                    string URL = "https://b843cb-2.myshopify.com//admin/api/2024-01/customers.json";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                    request.Method = "GET";
                    request.Headers.Add("X-Shopify-Access-Token", "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");

                    try
                    {
                        WebResponse webResponse = request.GetResponse();
                        Stream webStream = webResponse.GetResponseStream();
                        StreamReader responseReader = new StreamReader(webStream);
                        response = responseReader.ReadToEnd();
                        responseReader.Close();
                        return response;
                    }
                    catch (Exception e)
                    {
                        responseResult = "{\"status\":\"failed\",\"errormessage\":\"" + e.Message + "\"}";
                    }
                }
                catch (Exception ex)
                {
                    responseResult = "{\"status\":\"failed\",\"errormessage\":\"" + ex.Message + "\"}";
                }
            }
            return responseResult;
        }

        [WebMethod(EnableSession = true)]
        public string GetShopifyCountDetails()
        {
            string responseResult = "{\"status\":\"failed\",\"errormessage\":\"could not connect\"}";
            string InterSSOToken = string.Empty;
            {
                try
                {
                    string response = string.Empty;
                    string URL = "https://b843cb-2.myshopify.com//admin/api/2024-01/customers/count.json";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                    request.Method = "GET";
                    request.Headers.Add("X-Shopify-Access-Token", "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");

                    try
                    {
                        WebResponse webResponse = request.GetResponse();
                        Stream webStream = webResponse.GetResponseStream();
                        StreamReader responseReader = new StreamReader(webStream);
                        response = responseReader.ReadToEnd();
                        responseReader.Close();
                        return response;
                    }
                    catch (Exception e)
                    {
                        responseResult = "{\"status\":\"failed\",\"errormessage\":\"" + e.Message + "\"}";
                    }
                }
                catch (Exception ex)
                {
                    responseResult = "{\"status\":\"failed\",\"errormessage\":\"" + ex.Message + "\"}";
                }
            }
            return responseResult;
        }

    }
}
