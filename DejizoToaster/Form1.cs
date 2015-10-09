using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DejizoToaster
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private WebResponse HttpGet(string uri)
        {
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.Method = "GET";

            var res = req.GetResponse();

            return res;
        }
    }
}
