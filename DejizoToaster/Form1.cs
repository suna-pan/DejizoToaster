using DesktopToast;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace DejizoToaster
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Shown(object sender, EventArgs e)
        {
            var id = LoopUp("move");
            Debug.WriteLine(id);
            var res = GetDetail(id);
            var head = (string)res.GetType().GetProperty("Word").GetValue(res, null);
            var body = (string)res.GetType().GetProperty("Body").GetValue(res, null);
            Debug.WriteLine(head);
            Debug.WriteLine(body);
            ShowToastAsync(head, body);
            this.Close();
        }


        private string LoopUp(string word)
        {
            var xml = DejizoSearch(word);
            var ns = xml.Name.Namespace;

            var itemCount = xml.Descendants(ns + "ItemCount").First().Value;
            if (Int32.Parse(itemCount) < 1)
                return "";

            var itemId = xml.Descendants(ns + "ItemID").First().Value;
            return itemId;
        }


        private object GetDetail(string id)
        {
            var xml = DejizoGet(id);
            var ns = xml.Name.Namespace;

            char[] tc= { '\n', '\t', ' ' };
            var word = xml.Descendants(ns + "Head").First().Value;
            var body = xml.Descendants(ns + "Body").First().Value;
            return new
            {
                Word = Regex.Replace(word.Trim(tc), "\\s+", " "), 
                Body = Regex.Replace(body.Trim(tc), "\\s+", " ")
            };
        }


        private WebResponse HttpGet(string uri)
        {
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.Method = "GET";

            var res = req.GetResponse();

            return res;
        }


        private XElement CallAPI(string queryUri)
        {
            var res = HttpGet(queryUri);
            var resReader = XmlReader.Create(res.GetResponseStream());
            var docs = XElement.Load(resReader);

            return docs;
        }


        private XElement DejizoSearch(string word)
        {
            var encWord = HttpUtility.UrlEncode(word);
            var queryUri = "http://public.dejizo.jp/NetDicV09.asmx/SearchDicItemLite?Dic=EJdict&Word="
                           + encWord + "&Scope=HEADWORD&Match=STARTWITH&Merge=AND&Prof=XHTML&PageSize=1&PageIndex=0";
            Debug.WriteLine(queryUri);

            return CallAPI(queryUri);
        }


        private XElement DejizoGet(string id)
        {
            var queryUri = "http://public.dejizo.jp/NetDicV09.asmx/GetDicItemLite?Dic=EJdict&Item=" + id + "&Loc=&Prof=XHTML";
            Debug.WriteLine(queryUri);

            return CallAPI(queryUri);
        }


        private async Task<string> ShowToastAsync(string HeadLine, string Body)
        {
            var request = new ToastRequest
            {
                ToastHeadline = HeadLine,
                ToastBody = Body,
                ShortcutFileName = "DejizoToaster.lnk",
                ShortcutTargetFilePath = Assembly.GetExecutingAssembly().Location,
                AppId = "DejizoToaster.WinForms",
            };

            var result = await ToastManager.ShowAsync(request);

            return result.ToString();
        }

    }
}
