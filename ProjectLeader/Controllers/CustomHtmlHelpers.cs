using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Reflection;
using System.Web;
using System.Xml;
using MongoBaseRepository;
using MongoBaseRepository.Classes;
using MongoDB.Bson;
using ProjectLeader.Service.Iface;

namespace ProjectLeader.Helpers
{
    /// <summary>
    /// Vlastní pomocné HtmlHelpery
    /// </summary>
    public static class CustomHtmlHelpers
    {
        //public static IEnumerable<SelectListItem> GetModelAsSelectListItems(this HtmlHelper htmlHelper, ObjectId selectedValue, string type, int? ciselnik = 0)
        //{
        //    var db = DependencyResolver.Current.GetService<IMongoRepository>();
        //    return db.GetEntityAsSelectListItems<>()
        //}

      public static IEnumerable<MongoListItem> GetModelAsSelectListItems(this HtmlHelper htmlHelper, string type, string selectedValue)
      {
        ObjectId objId = new ObjectId();
        if (ObjectId.TryParse(selectedValue, out objId))
        {
          var db = DependencyResolver.Current.GetService<IMongoRepository>();
          switch(type)
          {
            
          }
        }
        return new List<MongoListItem>();
      }

      public static MvcHtmlString DropDownListDynamicMethod(this HtmlHelper helper, string controller, string method, string ddlName, string userProp)
        {
            IEnumerable<SelectListItem> data = new List<SelectListItem>();
            IList<SelectListItem> helpData = new List<SelectListItem>();
            Type[] types = typeof(CustomHtmlHelpers).Assembly.GetTypes();
            Type type = types.FirstOrDefault(x => x.Name == controller);
            if (type != null)
            {
                MethodInfo[] methodsToList = type.GetMethods();
                MethodInfo mInfo = methodsToList.FirstOrDefault(x => x.Name == method.TrimStart('_'));
                if (mInfo != null)
                {
                    data = (IEnumerable<SelectListItem>)mInfo.Invoke(null, null);
                    foreach (SelectListItem sli in data) helpData.Add(new SelectListItem() { Text = sli.Text, Value = sli.Value, Selected = sli.Value == userProp ? true : false });
                }
            }
            return helper.DropDownList(ddlName, helpData);
        }
    }
}