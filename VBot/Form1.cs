using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace VBot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Load the document using HTMLAgilityPack as normal
            var html = new HtmlDocument();
            html.LoadHtml(@"
  <html>
      <head></head>
      <body>
        <div>
          <p class='content'>Fizzler</p>
          <p>CSS Selector Engine</p></div>
      </body>
  </html>");

            // Fizzler for HtmlAgilityPack is implemented as the 
            // QuerySelectorAll extension method on HtmlNode

            var document = html.DocumentNode;

            // yields: [<p class="content">Fizzler</p>]
            document.QuerySelectorAll(".content");

            // yields: [<p class="content">Fizzler</p>,<p>CSS Selector Engine</p>]
            document.QuerySelectorAll("p");

            // yields empty sequence
            document.QuerySelectorAll("body>p");

            // yields [<p class="content">Fizzler</p>,<p>CSS Selector Engine</p>]
            document.QuerySelectorAll("body p");

            // yields [<p class="content">Fizzler</p>]
            document.QuerySelectorAll("p:first-child");
        }
    }
}
