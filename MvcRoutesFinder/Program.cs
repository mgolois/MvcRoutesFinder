/*
 * Updated Code from https://github.com/GDSSecurity/DotNET-MVC-Enumerator
 */
using CommandLine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MvcRoutesFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Processor)
                .WithNotParsed(c => Environment.Exit(0));
        }

        private static void Processor(Options o)
        {
            try
            {
                var results = new Dictionary<string, List<Result>>();

                //if (string.IsNullOrEmpty(o.CsvOutput))
                //{
                //    DateTime dt = DateTime.Now;
                //    string timestamp = dt.ToString("yyyyMMddHHmmss");
                //    o.CsvOutput = "enumerated_controllers_" + timestamp + ".csv";
                //}

                //var documents = LoadSolution(o.SolutionPath);

                string[] paths = Directory.GetFiles(o.Directory, "*.cs", SearchOption.AllDirectories);

                if (paths.Any())
                {
                    foreach (var path in paths)
                    {
                        using (var stream = File.OpenRead(path))
                        {

                            var tree = CSharpSyntaxTree.ParseText(SourceText.From(stream), path: path);
                            SyntaxNode root = tree.GetRoot();

                            // Check if the Class inherits Apicontroller or Controller and print out all the public entry points
                            ControllerChecker controllerchk = new ControllerChecker();
                            if (controllerchk.inheritsFromController(root))
                            {
                                controllerchk.enumerateEntrypoints(root,  path, results);//o.AttributeSearch, o.NegativeSearch,
                            }
                        }
                    }

                    string[] controllerPaths = results.Keys.ToArray();
                    string pathToTrim = getPathToTrim(controllerPaths);

                    if (!string.IsNullOrEmpty(o.CsvOutput))
                    {
                        printCSVResults(results, o.CsvOutput, pathToTrim);
                    }
                    printCommandLineResults(results, pathToTrim);
                }
                else
                {
                    Console.WriteLine("Unable to find any document from solution line");
                }
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Invalid Path");
            }
            catch (IndexOutOfRangeException)
            {
                //Shoudn't Reach this, but in case
                Console.WriteLine("No Arguments passed");
            }
            catch (UnauthorizedAccessException e)
            {
                e.GetBaseException();
                Console.WriteLine("You do not seem to have appropiate Permissions on this direcctory");
            }
            catch (NotSupportedException)
            {
                Console.WriteLine("The operating system is Windows CE, which does not have current directory functionality.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Illegal characters passed as arguments! ");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected error {e}");
            }
        }
        //private static IEnumerable<Document> LoadSolution(string solutionDir)
        //{
        //    var solutionFilePath = Path.GetFullPath(solutionDir);

        //    MSBuildWorkspace workspace = MSBuildWorkspace.Create();
        //    workspace.WorkspaceFailed += Workspace_WorkspaceFailed;
        //    Solution solution = workspace.OpenSolutionAsync(solutionFilePath).Result;

        //    var documents = new List<Document>();
        //    foreach (var projectId in solution.ProjectIds)
        //    {
        //        var project = solution.GetProject(projectId);
        //        foreach (var documentId in project.DocumentIds)
        //        {
        //            Document document = solution.GetDocument(documentId);
        //            if (document.SupportsSyntaxTree) documents.Add(document);
        //        }
        //    }

        //    return documents;
        //}

        //private static void Workspace_WorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        private static void printCommandLineResults(Dictionary<string, List<Result>> results, string pathToTrim)
        {
            foreach (KeyValuePair<string, List<Result>> pair in results)
            {
                Console.WriteLine("Controller: \n" + pair.Key.Replace(pathToTrim, ""));
                foreach (Result result in pair.Value)
                {
                    Console.WriteLine($"Method={result.MethodName};");
                    Console.WriteLine($"Route={result.Route};");
                    Console.WriteLine($"Http={string.Join(",", result.HttpMethods)};");
                    Console.WriteLine($"Attributes={string.Join(",", result.Attributes)}");
                }

                Console.WriteLine("\n======================\n");
            }
        }

        private static void printCSVResults(Dictionary<string, List<Result>> results, string filename, string pathToTrim)
        {
            var csvExport = new CsvExport();

            foreach (KeyValuePair<string, List<Result>> pair in results)
            {
                foreach (Result result in pair.Value)
                {
                    csvExport.AddRow();
                    csvExport["Controller"] = pair.Key.Replace(pathToTrim, "");
                    csvExport["Method Name"] = result.MethodName;
                    csvExport["Route"] = result.Route;
                    csvExport["HTTP Method"] = string.Join(", ", result.HttpMethods.ToArray());
                    csvExport["Attributes"] = string.Join(", ", result.Attributes.ToArray());
                }
            }

            File.Create(filename).Dispose();
            csvExport.ExportToFile(filename);
            Console.WriteLine("CSV output written to: " + filename);
        }

        private static string getPathToTrim(string[] paths)
        {
            string pathToTrim = "";

            try
            {
                string aPath = paths.First();

                string[] dirsInFilePath = aPath.Split('\\');
                int highestMatchingIndex = 0;

                for (int i = 0; i < dirsInFilePath.Length; i++)
                {

                    for (int j = 0; j < paths.Length; j++)
                    {
                        string[] splitPath = paths[j].Split('\\');
                        if (!dirsInFilePath[i].Equals(splitPath[i]))
                        {
                            highestMatchingIndex = i - 1;
                            break;
                        }
                    }
                }

                if (highestMatchingIndex == 0)
                {
                    pathToTrim = "." + "\\";
                }
                else
                {
                    pathToTrim = string.Join("\\", dirsInFilePath, 0, highestMatchingIndex);
                }

            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("No Entrypoints with the specified search parameters found.");
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return pathToTrim;

        }

    }
}
