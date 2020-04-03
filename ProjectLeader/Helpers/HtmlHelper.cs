using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Resources;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using ProjectLeader.Controllers;
using MongoBaseRepository;
using MongoBaseRepository.Classes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace ProjectLeader.Helpers
{
  public static class HtmlHelperExtensions
  {
    public static IList<SelectListItem> GetUsersAsSelectListItems(this HtmlHelper htmlHelper, string selectedValue)
    {
      var db = DependencyResolver.Current.GetService<IMongoRepository>();
      ObjectId id = ObjectId.Empty;
      ObjectId.TryParse(selectedValue, out id);
      return db.GetCollection<MongoUser>(x => true).
        Select(x => new SelectListItem() { Value = x._id.ToString(), Text = x.UserName, Selected = x._id.ToString() == selectedValue }).ToList();
    }

    public static string GetDisplayName(this Enum enumValue)
    {
      var type = enumValue.GetType();
      var member = type.GetMember(Enum.GetName(type, enumValue));
      var attributes = member[0].GetCustomAttributes(typeof(DisplayAttribute), false);
      return ((DisplayAttribute)attributes[0]).Name;
    }

    public static IList<SelectListItem> GetEnumSelectListItem(this Enum enumValue)
    {
      IList<SelectListItem> items = new List<SelectListItem>();
      var type = enumValue.GetType();
      foreach (var v in Enum.GetValues(type))
      {
        items.Add(new SelectListItem()
        {
          Value = ((Enum)v).ToString(),
          Text = ((Enum)v).GetDisplayName(),
          Selected = ((Enum)v).ToString() == enumValue.ToString()
        });
      }
      return items;
    }

    /// <summary>
    /// Vrati label pro vlastnost modelu vstupující do editor template.
    /// </summary>
    public static MvcHtmlString GetPropertyLabel<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
    {
      string lang = "cs-CZ";
      if (HttpContext.Current.Request.Cookies["lang"] != null)
      {
        lang = HttpContext.Current.Request.Cookies["lang"].Value;
      }
      Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
      ResourceManager rm = new ResourceManager("ProjectLeader.Resources.Entity", typeof(BaseController).Assembly);
      string returnedString = html.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
      string hlpString = rm.GetString(returnedString, Thread.CurrentThread.CurrentUICulture);
      return html.Label(hlpString != null ? hlpString : returnedString);
    }

    /// <summary>
    /// Vrati lokalizovaný string pro vlastnost modelu vstupující do editor template.
    /// </summary>
    public static string GetPropertyName<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
    {
      string lang = "cs-CZ";
      if (HttpContext.Current.Request.Cookies["lang"] != null)
      {
        lang = HttpContext.Current.Request.Cookies["lang"].Value;
      }
      Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
      ResourceManager rm = new ResourceManager("TestCRM.Resources.Entity", typeof(BaseController).Assembly);
      string returnedString = html.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
      string hlpString = rm.GetString(returnedString, Thread.CurrentThread.CurrentUICulture);
      return hlpString != null ? hlpString : returnedString;
    }

    /// <summary>
    /// Přeloží řetězec
    /// </summary>
    public static string GetResourceName<TModel>(this HtmlHelper<TModel> html, string s)
    {
      string lang = "cs-CZ";
      if (HttpContext.Current.Request.Cookies["lang"] != null)
      {
        lang = HttpContext.Current.Request.Cookies["lang"].Value;
      }
      Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
      ResourceManager rm = new ResourceManager("ProjectLeader.Resources.Entity", typeof(BaseController).Assembly);
      string hlpString = rm.GetString(s, Thread.CurrentThread.CurrentUICulture);
      return hlpString != null ? hlpString : s;
    }

    public static Kendo.Mvc.UI.Fluent.GridBuilder<T> GetStandardGrid<T>(this HtmlHelper helper, string name) where T : class
    {
      GridBuilder<T> builder = helper.Kendo().Grid<T>();
      builder.Name(name);
      builder.Scrollable();
      builder.Selectable();
      builder.Pageable(page => page.PageSizes(true).Enabled(true).Input(true).Messages(mess =>
      {
        mess.Display("{0} - {1} z celkem {2}");
        mess.Empty("žádné položky nevyhovují kritériím");
        mess.First("Přejít na první stránku");
        mess.ItemsPerPage("Položek na stránce");
        mess.Last("Přejít na poslední stránku");
        mess.Of("z {0}");
        mess.Page("Stránka");
        mess.Previous("Přejít na předcházející stránku");
        mess.Next("Přejít na následující stránku");
        mess.Refresh("Obnovit");
      }));
      builder.Filterable(x =>
      {
        x.Operators(oper =>
        {
          oper.ForString(fs => fs.Clear().Contains("Obsahuje"));

        });
        x.Messages(mess =>
        {
          mess.Info("Filtrování");
          mess.Filter("Filtrovat");
          mess.Clear("Vyčistit");
          mess.SelectValue("Vyberte");
          mess.IsTrue("Ano");
          mess.IsFalse("Ne");
          mess.And("A");
          mess.Or("Nebo");
        })
            .Operators(oper =>
            {
              oper.ForDate(fd =>
              {
                fd.IsEqualTo("Rovná se");
                fd.IsNotEqualTo("Nerovná se");
                fd.IsGreaterThan("Je větší než");
                fd.IsGreaterThanOrEqualTo("Je větší než nebo rovno");
                fd.IsLessThan("Je menší než");
                fd.IsLessThanOrEqualTo("Je menší než nebo rovno");
              }).
                  ForEnums(fe =>
                  {
                    fe.IsEqualTo("Rovná se");
                    fe.IsNotEqualTo("Nerovná se");
                  }).ForNumber(fd =>
                  {
                    fd.IsEqualTo("Rovná se");
                    fd.IsNotEqualTo("Nerovná se");
                    fd.IsGreaterThan("Je větší než");
                    fd.IsGreaterThanOrEqualTo("Je větší než nebo rovno");
                    fd.IsLessThan("Je menší než");
                    fd.IsLessThanOrEqualTo("Je menší než nebo rovno");
                  }).ForString(fd =>
                  {
                    fd.IsEqualTo("Rovná se");
                    fd.IsNotEqualTo("Nerovná se");
                    fd.Contains("Obsahuje");
                    fd.DoesNotContain("Neobsahuje");
                    fd.EndsWith("Končí na");
                    fd.StartsWith("Začíná na");
                  });
            });
      });

      return builder;
    }
  }
}