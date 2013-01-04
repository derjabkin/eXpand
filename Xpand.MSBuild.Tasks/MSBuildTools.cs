using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.BuildEngine;
using System.IO;
using Microsoft.Build.Framework;

#pragma warning disable 618
namespace Xpand.MSBuild.Tasks
{
    public static class MSBuildTools
    {
        [ThreadStatic]
        private static bool inGetLibraryVersionFromProject;

        public static Version GetLibraryVersionFromProject(string projectFileName)
        {
            if (inGetLibraryVersionFromProject || !File.Exists(projectFileName)) return null;
            inGetLibraryVersionFromProject = true;
            Project project = new Project();
            try
            {

                project.Load(projectFileName);
            }
            catch (InvalidProjectFileException)
            {
                return null;
            }
            finally
            {
                inGetLibraryVersionFromProject = false;
            }
            
            foreach (BuildPropertyGroup pg in project.PropertyGroups)
            {

                BuildProperty property  = (from BuildProperty p in pg where p.Name == "LibraryVersion" select p).FirstOrDefault();
                if (property != null)
                    return new Version(property.Value);
            }

            return null;
            
        }

        public static string[] GetProjectFileNames(Project solution)
        {
            List<string> list = new List<string>();

            BuildItemGroup group = solution.GetEvaluatedItemsByName("_SolutionProjectProjects");
            {
                foreach (BuildItem item in group)
                {
                    list.Add(item.Include);
                }
            }

            return list.ToArray();
        }

        public static string[] GetProjectFileNames(string solutionFileName)
        {
            Project project = new Project();
            project.Load(solutionFileName);
            return GetProjectFileNames(project);
        }

        public static bool IsSolutionFileName(this string fileName)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");

            return Path.GetExtension(fileName).Equals(".sln", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsSolutionItem(this ITaskItem item)
        {
            return item.ItemSpec.IsSolutionFileName();
        }

        public static IEnumerable<string> GetProjectFileNames(this IEnumerable<ITaskItem> taskItems)
        {
            if (taskItems == null) throw new ArgumentNullException("taskItems");
            foreach (ITaskItem item in taskItems)
            {
                if (item.IsSolutionItem())
                {
                    string solutionDirectory = Path.GetDirectoryName(item.ItemSpec);
                    if (string.IsNullOrEmpty(solutionDirectory)) solutionDirectory = Directory.GetCurrentDirectory();
                    string solutionPath = Path.GetFullPath(solutionDirectory);
                    foreach (string fileName in GetProjectFileNames(item.ItemSpec))
                    {
                        string path = Path.IsPathRooted(fileName) ? fileName : Path.Combine(solutionPath, fileName);
                        yield return path;
                    }
                }
                else
                    yield return item.ItemSpec;
            }
        }
    }
}
