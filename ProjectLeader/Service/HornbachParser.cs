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
			WebRequest objRequest = HttpWebRequest.Create(url);
			WebResponse objResponse = objRequest.GetResponse();
			using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
			{
				strResult = sr.ReadToEnd();
				sr.Close();
			}
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(strResult);
			IList<HtmlNode> nodes = new List<HtmlNode>();
			GetBodyNodeByTag(doc.DocumentNode, nodes, "table");
			if (nodes.Count > 0)
			{
				IList<HtmlNode> trNodes = new List<HtmlNode>();
				GetBodyNodeByTag(nodes[0], trNodes, "tr");
				foreach (HtmlNode node in trNodes)
				{
					list.Add(node.ChildNodes[1].InnerHtml.Replace("<em>", "").Replace("</em>", ""), node.ChildNodes[0].InnerHtml);
				}
			}
			return new Resource();
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