/*
 * Updated Code from https://github.com/GDSSecurity/DotNET-MVC-Enumerator
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcRoutesFinder
{
    public class Result
    {
        public string MethodName { get; set; }
        public List<string> HttpMethods { get; set; } = new List<string>();
        public string Route { get; set; }
        public List<string> Attributes { get; set; } = new List<string>();


        private bool ContainsMethod(List<string> methodAttributes, string httpMethod)
        {
            string attrname = methodAttributes.FirstOrDefault(s => s.Contains(httpMethod));
            return !string.IsNullOrEmpty(attrname);
        }

        public void SetRoute(List<string> methodAttributes)
        {
            string attribute = methodAttributes.FirstOrDefault(s => s.Contains("Route"));

            if (attribute != null)
            {
                Route = attribute;
            }
        }

        public void SetSupportedHTTPMethods(List<string> methodAttributes)
        {
            if (ContainsMethod(methodAttributes, "HttpPost") ||
                    ContainsMethod(methodAttributes, "Http.Post"))
            {
                HttpMethods.Add("POST");
            }
            if (ContainsMethod(methodAttributes, "HttpGet") ||
                    ContainsMethod(methodAttributes, "Http.Get"))
            {
                HttpMethods.Add("GET");
            }
            if (ContainsMethod(methodAttributes, "HttpPut") ||
                    ContainsMethod(methodAttributes, "Http.Put"))
            {
                HttpMethods.Add("PUT");
            }
            if (ContainsMethod(methodAttributes, "HttpPatch") ||
                    ContainsMethod(methodAttributes, "Http.Patch"))
            {
                HttpMethods.Add("PATCH");
            }
            if (ContainsMethod(methodAttributes, "HttpHead") ||
                    ContainsMethod(methodAttributes, "Http.Head"))
            {
                HttpMethods.Add("HEAD");
            }
            if (ContainsMethod(methodAttributes, "HttpDelete") ||
                    ContainsMethod(methodAttributes, "Http.Delete"))
            {
                HttpMethods.Add("DELETE");
            }
            if (ContainsMethod(methodAttributes, "HttpOptions") ||
                    ContainsMethod(methodAttributes, "Http.Options"))
            {
                HttpMethods.Add("OPTIONS");
            }
        }
    }

}
