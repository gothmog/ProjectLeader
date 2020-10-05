using ProjectLeader.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectLeader.Service.Iface
{
	public interface IHornbachParser
	{
		Resource GetResource(string url);
	}
}