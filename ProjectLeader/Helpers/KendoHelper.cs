using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using Kendo.Mvc.UI.Html;
using System.Xml;
using System.Reflection;
using System.ComponentModel;


namespace ProjectLeader.Helpers
{
    public static class GridForEntity
    {
        /// <summary>
        /// Metoda vytvářející automaticky gridy do View
        /// </summary>
        /// <typeparam name="T">Typ objektu, jehož kolekci grid zobrazuje</typeparam>
        /// <param name="helper">HtmlHelper</param>
        /// <param name="name">Jméno gridu - je třeba dodržovat unikátnost</param>
        /// <param name="excel">Jméno javasriptové funkce realizující export do excelu</param>
        /// <param name="excelAction">Jméno akce na controlleru realizující export do excelu</param>
        /// <param name="readAction">Jméno akce, která tahá data, na controlleru </param>
        /// <param name="controller">Jméno controlleru</param>
        /// <param name="additionalParmsFunct">Jméno javascriptové funkce pro doplňující parametry při tahání dat</param>
        /// <param name="preColumns">Eventuelní sloupce, které jsou na prvních místech - pouze zobrazovací</param>
        /// <param name="AddButton">htmlstring s tlačítkem přidat...</param>
        /// <returns></returns>
        public static Kendo.Mvc.UI.Fluent.GridBuilder<T> GetStandartGrid<T>(this HtmlHelper helper, string name, string excel, string excelAction, string readAction, string controller, string additionalParmsFunct = "", IList<ColumnForGrid> preColumns = null, string AddButton = "") where T : class
        {

            GridBuilder<T> builder = helper.Kendo().Grid<T>();
            builder.Columns(cols =>
            {
                if (preColumns != null)
                {
                    foreach (ColumnForGrid cfg in preColumns)
                    {
                        cols.Bound(cfg.Property).ClientTemplate(cfg.Template).Title(cfg.Name).Width(cfg.Width);
                    }
                }
                foreach (PropertyInfo pi in typeof(T).GetProperties())
                {
                    if (preColumns == null || !preColumns.Select(x => x.Property).Contains(pi.Name))
                    {
                        DescriptionAttribute desAtt = IsForeignProperty(pi);
                        DisplayNameAttribute nameAtt = IsBoundProperty(pi);
                        WidthAttribute widthAtt = IsWidthProperty(pi);
                        if (desAtt != null)
                        {
                            if (nameAtt != null)
                            {
                              if (widthAtt != null) cols.ForeignKey(pi.Name, CustomHtmlHelpers.GetModelAsSelectListItems(helper, null, desAtt.Description), "Value", "Text").Title(nameAtt.DisplayName).Width(widthAtt.Width);
                              else cols.ForeignKey(pi.Name, CustomHtmlHelpers.GetModelAsSelectListItems(helper, null, desAtt.Description), "Value", "Text").Title(nameAtt.DisplayName);
                            }
                            else
                            {
                                if (widthAtt != null) cols.ForeignKey(pi.Name, CustomHtmlHelpers.GetModelAsSelectListItems(helper, null, desAtt.Description), "Value", "Text").Width(widthAtt.Width);
                                else cols.ForeignKey(pi.Name, CustomHtmlHelpers.GetModelAsSelectListItems(helper, null, desAtt.Description), "Value", "Text");
                            }
                        }
                        else if (nameAtt != null)
                        {
                            if (pi.PropertyType == typeof(DateTime) || pi.PropertyType == typeof(DateTime?))
                            {
                                //pokud ma polozka atribut, ze se ma zobrazit pouze datum bez casu
                                System.ComponentModel.DataAnnotations.UIHintAttribute showDateAtt = IsDateProperty(pi);
                                if (showDateAtt != null)
                                {
                                    if (widthAtt != null) cols.Bound(pi.Name).Title(nameAtt.DisplayName).Format("yyyy-MM-dd").Width(widthAtt.Width);
                                    else cols.Bound(pi.Name).Title(nameAtt.DisplayName).Format("yyyy-MM-dd");
                                }
                                else
                                {
                                  if (widthAtt != null) cols.Bound(pi.Name).Title(nameAtt.DisplayName).Format("yyyy-MM-dd").Width(widthAtt.Width);
                                  else cols.Bound(pi.Name).Title(nameAtt.DisplayName).Format("yyyy-MM-dd");
                                }
                            }
                            else
                            {
                                if (widthAtt != null) cols.Bound(pi.Name).Title(nameAtt.DisplayName).Width(widthAtt.Width);
                                else cols.Bound(pi.Name).Title(nameAtt.DisplayName);
                            }
                        }
                    }
                }
            });
            
            builder.Name(name);
            builder.ToolBar(bar =>
            {
              if (!String.IsNullOrEmpty(excel))
              {
                bar.Template(AddButton + "<div class=\"left\"><input type='button' value='Export do excelu' onClick='" +
                                               excel + "(); return true;' class='k-button' disabled='disabled' /></div>");
              }
              else
              {
                bar.Template(AddButton);
              }
            });


            builder.DataSource(source =>
            {
                source.Ajax().Read(read => read.Action(readAction, controller).Data(additionalParmsFunct)).PageSize(10);
            });

            builder.Selectable(selectable => selectable
            .Mode(GridSelectionMode.Single)
            .Type(GridSelectionType.Row));

            builder.Sortable();
            builder.Pageable(page => page.PageSizes(true).Refresh(true).Enabled(true).Input(true).Messages(mess =>
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
                        x.Extra(false);
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
                                    })
                                    .ForString(fd =>
                                    {
                                        fd.Clear().Contains("Obsahuje");
                                        fd.IsEqualTo("Rovná se");
                                    fd.IsNotEqualTo("Nerovná se");
                                    fd.DoesNotContain("Neobsahuje");
                                    fd.EndsWith("Končí na");
                                    fd.StartsWith("Začíná na");;
                                    });
                            });
                            });

            return builder;
        }

        private static DisplayNameAttribute IsBoundProperty(PropertyInfo prop)
        {
            DisplayNameAttribute attr =  (DisplayNameAttribute)System.Attribute.GetCustomAttribute(prop, typeof(DisplayNameAttribute));
            return attr;
        }

        private static WidthAttribute IsWidthProperty(PropertyInfo prop)
        {
            return (WidthAttribute)System.Attribute.GetCustomAttribute(prop, typeof(WidthAttribute));
        }

        private static DescriptionAttribute IsForeignProperty(PropertyInfo prop)
        {
            DescriptionAttribute attr = (DescriptionAttribute)System.Attribute.GetCustomAttribute(prop, typeof(DescriptionAttribute));
            return attr;
        }

        /// <summary>
        /// Vrati nalezeny atribut, zda se ma zobrazit polozka jen s datumeme bez casu
        /// </summary>
        /// <param name="prop">Property</param>
        /// <returns>Atribute</returns>
        private static System.ComponentModel.DataAnnotations.UIHintAttribute IsDateProperty(PropertyInfo prop)
        {
            System.ComponentModel.DataAnnotations.UIHintAttribute attr = (System.ComponentModel.DataAnnotations.UIHintAttribute)System.Attribute.GetCustomAttribute(prop, typeof(System.ComponentModel.DataAnnotations.UIHintAttribute));
            if(attr == null) return null;
            if(attr.UIHint == "Date") return attr;
            return null;
        }

        /// <summary>
        /// Metoda zjištující práva uživatele na akci, může se zavolat z view ale standartně jí volají Metody GetStandartGrid, GetMenuItems a GetAuthorizedSubmit
        /// </summary>
        /// <param name="html">Html controller</param>
        /// <param name="controller">jméno controlleru</param>
        /// <param name="action">jméno akce</param>
        /// <returns>TRUE - ma pravo</returns>
        //public static bool CheckUserPermission(this HtmlHelper html, string controller, string action)
        //{
        //    var identity = html.ViewContext.HttpContext.User.Identity;

        //    if (identity != null)
        //    {
        //        var cache = DependencyResolver.Current.GetService<ICacheService>();
        //        IList<AuthorizationModel> permissions = cache.LoadPermissionsForUser(html.ViewContext.HttpContext.User.Identity.Name);
        //        if (!controller.Contains("Controller")) controller += "Controller";
        //        if (permissions.FirstOrDefault(x => x.Name == controller) != null)
        //        {
        //            AuthorizationModel methodModel = permissions.FirstOrDefault(x => x.Name == controller).Childrens.FirstOrDefault(x => x.Name == action);
        //            if (methodModel != null)
        //            {
        //                return methodModel.Permission == Permission.Write;
        //            }
        //            else
        //            {
        //                foreach (AuthorizationModel subMethodModels in permissions.FirstOrDefault(x => x.Name == controller).Childrens)
        //                {
        //                    if (subMethodModels.Childrens != null)
        //                    {
        //                        AuthorizationModel subMethodModel = subMethodModels.Childrens.FirstOrDefault(x => x.Name == action);
        //                        if (subMethodModel != null) return subMethodModel.Permission == Permission.Write;
        //                    }
        //                }
        //            }
        //        }
        //        else return true;
        //    }
        //    return false;
        //}
    }
}