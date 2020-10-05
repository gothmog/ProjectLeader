using HtmlAgilityPack;
using ProjectLeader.Classes;
using ProjectLeader.Service.Iface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace ProjectLeader.Service
{
	public class HornbachParser : IHornbachParser
	{
		public Resource GetResource(string url)
		{
			Resource resource = new Resource();
			WebRequest objRequest = HttpWebRequest.Create(url);
			WebResponse objResponse = objRequest.GetResponse();
			using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
			{
				HtmlDocument doc = new HtmlDocument();
				doc.LoadHtml(sr.ReadToEnd());
				IList<HtmlNode> nodes = new List<HtmlNode>();
				GetBodyNodeById(doc.DocumentNode, nodes, "product-information");
				if (nodes.Count > 0)
				{
					IList<HtmlNode> nameNodes = new List<HtmlNode>();
					GetBodyNodeByTag(nodes[0], nameNodes, "h1");
					if (nameNodes.Count > 0)
					{
						resource.Name = nameNodes[0].InnerText;
					}
					IList<HtmlNode> priceNodes = new List<HtmlNode>();
					GetBodyNodeByTag(nodes[0], priceNodes, "span");
					if (priceNodes.Count > 0)
					{
						string price = priceNodes[0].InnerText.Split(new string[] { "CZK" }, StringSplitOptions.RemoveEmptyEntries)[0];
						decimal priceDec = 0;
						if (Decimal.TryParse(price, out priceDec))
						{
							resource.Price = priceDec;
						}
					}

				}
				nodes = new List<HtmlNode>();
				GetBodyNodeById(doc.DocumentNode, nodes, "awssld__container");
				if (nodes.Count > 0)
				{
					IList<HtmlNode> picNodes = new List<HtmlNode>();
					GetBodyNodeByTag(nodes[0], picNodes, "img");
					if(picNodes.Count > 0)
					{
						resource.ImageUrl = picNodes[0].Attributes.FirstOrDefault(x => x.Name == "src").Value;
					}
				}
				sr.Close();
				return resource;

			}
		}

		private void GetBodyNodeById(HtmlNode html, IList<HtmlNode> nodes, string nameNode)
		{
			if (html.Id == nameNode) nodes.Add(html);
			foreach (HtmlNode nod in html.ChildNodes) GetBodyNodeById(nod, nodes, nameNode);
		}

		private void GetBodyNodeByClass(HtmlNode html, IList<HtmlNode> nodes, string className)
		{
			if (html.HasAttributes)
			{
				var attrClass = html.Attributes.FirstOrDefault(x => x.Name == "class");
				if (attrClass != null)
				{
					if (attrClass.Value == className) nodes.Add(html);
				}
			}
			foreach (HtmlNode nod in html.ChildNodes) GetBodyNodeByClass(nod, nodes, className);
		}

		private void GetBodyNodeByTag(HtmlNode html, IList<HtmlNode> nodes, string nameNode)
		{
			if (html.Name == nameNode) nodes.Add(html);
			foreach (HtmlNode nod in html.ChildNodes) GetBodyNodeByTag(nod, nodes, nameNode);
		}
	}
}