/*
 * Updated Code from https://github.com/GDSSecurity/DotNET-MVC-Enumerator
 */
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcRoutesFinder
{
    class ControllerChecker
    {
        List<string> results = new List<string>();

        public bool inheritsFromController(SyntaxNode root)//, string args TODO Custom
        {
            var controllerBaseClasses = new string[] { "Controller", "ApiController", "ControllerBase" };
            bool isValid = false;

            try
            {
               var baseTypes = root.DescendantNodes().OfType<BaseTypeSyntax>().ToList();
                if (baseTypes.Any())
                {
                    foreach (var baseClass in controllerBaseClasses)
                    {
                        if (baseTypes.Any(c => c.ToString().Contains(baseClass, StringComparison.OrdinalIgnoreCase)))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    isValid = false;
                }
            }
            catch (InvalidOperationException)
            {
                isValid = false;
            }

            return isValid;
        }


        public void enumerateEntrypoints(SyntaxNode root,// string attributeToSearch, string negativeSearch,
            string path, Dictionary<string, List<Result>> resultList)
        {
            try
            {
                ClassDeclarationSyntax controller =
                    root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();

                //Get all the public methods in this class
                IEnumerable<MethodDeclarationSyntax> methods =
                    from m in root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                    where m.Modifiers.ToString().Contains("public")
                    select m;

                List<string> controllerAttrs = CheckAttribute.getControllerAttributes(controller);

                List<Result> resultsForController = new List<Result>();

                foreach (var method in methods)
                {
                    Result result = new Result();

                    // Return all the attributes to list it only in the CSV output
                    List<string> methodAttributes = CheckAttribute.getMethodAttributes(method, controller);

                    // Set attributes set at Controller level
                    CheckAttribute.setMethodAttributesFromController(methodAttributes, controllerAttrs);

                    bool addAttributeFlag = true;

                    /*if (!string.IsNullOrEmpty(attributeToSearch))
                    {
                        string attributeMatched = methodAttributes.FirstOrDefault(s => s.StartsWith(attributeToSearch));

                        // Only add attributes that start with the value 'searched' for - via command line switch
                        if (string.IsNullOrEmpty(attributeMatched))
                        {
                            addAttributeFlag = false;
                        }
                    }


                    // Only add attributes that are missing the 'negative search' passed via command line switch
                    if (!string.IsNullOrEmpty(negativeSearch))
                    {
                        string attributeMatched = methodAttributes.FirstOrDefault(s => s.StartsWith(negativeSearch));

                        if (!string.IsNullOrEmpty(attributeMatched))
                        {
                            addAttributeFlag = false;
                        }
                    }*/


                    if (addAttributeFlag)
                    {
                        result.MethodName = method.Identifier.ValueText;

                        result.Attributes = methodAttributes;

                        result.SetSupportedHTTPMethods(methodAttributes);

                        result.SetRoute(methodAttributes);

                        resultsForController.Add(result);
                    }

                }

                if (resultsForController.ToArray().Length > 0)
                {
                    resultList.Add(path, resultsForController);
                }
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Failed to parse: \"" + path + "\", Skipping..");
            }
            catch (Exception e)
            {
                Console.WriteLine("Unhandled Exception Occurred.");
                Console.WriteLine(e.ToString());
            }
        }
    }
}
