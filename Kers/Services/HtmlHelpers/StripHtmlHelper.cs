using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Kers.HtmlHelpers
{
    public static class StripHtmlHelper
    {
        /// <summary>
        /// Renders an HTML string and returns plain text
        /// </summary>
        public static string StripHtml(this IHtmlHelper helper, string input)
        {
            return StripHtml(input);
        }




        public static string StripHtml(this string input)
        {
            string result = System.Text.RegularExpressions.Regex.Replace(input, "<[^>]*>|&nbsp;|&rsquo;", string.Empty);
    
            return result;
        }






    }
}