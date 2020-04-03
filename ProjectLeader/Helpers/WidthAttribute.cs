using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProjectLeader.Helpers
{
    /// <summary>
    /// Atribut, který reprezentuje šířku sloupce v gridu (Kendo)
    /// </summary>
    public class WidthAttribute : Attribute
    {
        public string Width { get; set; }

        public WidthAttribute(string width)
        {
            this.Width = width;
        }
    }
}