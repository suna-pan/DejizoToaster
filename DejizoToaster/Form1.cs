using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
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

            var id = LoopUp("toast");
            Debug.WriteLine(id);
        }


        private int LoopUp(string word)
        {
            var xml = DejizoSearch(word);
            var ns = xml.Name.Namespace;

            var itemCount = xml.Descendants(ns + "ItemCount").First().Value;
            if (Int32.Parse(itemCount) < 1)
                return -1;

            var itemId = xml.Descendants(ns + "ItemID").First().Value;
            return Int32.Parse(itemId);
        }


        private WebResponse HttpGet(string uri)
        {
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.Method = "GET";

            var res = req.GetResponse();

            return res;
        }


        private XElement DejizoSearch(string word)
        {
            var encWord = HttpUtility.UrlEncode(word);
            var queryUri = "http://public.dejizo.jp/NetDicV09.asmx/SearchDicItemLite?Dic=EJdict&Word="
                           + encWord + "&Scope=HEADWORD&Match=STARTWITH&Merge=AND&Prof=XHTML&PageSize=1&PageIndex=0";
            Debug.WriteLine(queryUri);

            var res = HttpGet(queryUri);
            var resReader = XmlReader.Create(res.GetResponseStream());
            var docs = XElement.Load(resReader);

            return docs;
        }
    }
}
