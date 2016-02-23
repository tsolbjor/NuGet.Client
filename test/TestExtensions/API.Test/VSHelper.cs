using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using NuGet.PackageManagement.VisualStudio;
using Task = System.Threading.Tasks.Task;

namespace API.Test
{
    public static class VSHelper
    {
        public static string GetNewGUID()
        {
            return Guid.NewGuid().ToString("d").Substring(0, 4).Replace("-", "");
        }

        public static void CreateSolution(string solutionDirectory, string name)
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await CreateSolutionAsync(solutionDirectory, name);
            });
        }

        private static async Task CreateSolutionAsync(string solutionDirectory, string name)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = ServiceLocator.GetInstance<DTE>();
            dte.Solution.Create(solutionDirectory, name);

            var solutionPath = Path.Combine(solutionDirectory, name);
            dte.Solution.SaveAs(solutionPath);
        }

        private static string SolutionNameFormat = "Solution_{0}";
        public static void CreateNewSolution(string outputPath, string solutionName)
        {
            var solutionDir = Path.Combine(outputPath, solutionName);

            Directory.CreateDirectory(solutionDir);

            CreateSolution(solutionDir, solutionName);
        }
        
        public static void CreateNewSolution(string outputPath)
        {
            var id = GetNewGUID();
            var solutionName = string.Format(SolutionNameFormat, id);

            CreateNewSolution(outputPath, solutionName);
        }

        public static void EnsureSolution(string outputPath)
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await EnsureSolutionAsync(outputPath);
            });
        }

        private static async Task EnsureSolutionAsync(string outputPath)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var dte = ServiceLocator.GetInstance<DTE>();
            if (dte.Solution == null || !dte.Solution.IsOpen)
            {
                CreateNewSolution(outputPath);
            }
        }
        
        // HACK: Make this handle nested solution folders
        private static Project GetSolutionFolderProject(DTE2 dte2, string solutionFolderName)
        {
            var solution2 = (Solution2)dte2.Solution;
            Project solutionFolderProject = null;

            foreach (var item in solution2.Projects)
            {
                var project = item as Project;
                if (project != null)
                {
                    if (project.UniqueName.StartsWith(solutionFolderName, StringComparison.OrdinalIgnoreCase))
                    {
                        solutionFolderProject = project;                        
                    }
                }
            }

            return solutionFolderProject;
        }

        public static object NewProject(
            string templatePath,
            string outputPath,
            string templateName,
            string projectName,
            string solutionFolderName)
        {
            if (string.IsNullOrEmpty(templateName))
            {
                throw new ArgumentException(
                    "Argument cannot be null or empty",
                    nameof(templateName));
            }

            var name = projectName;
            if (string.IsNullOrEmpty(name))
            {
                var id = GetNewGUID();
                name = templateName + "_" + id;
            }

            return ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                return await NewProjectAsync(
                    templatePath,
                    outputPath,
                    templateName,
                    name,
                    solutionFolderName);
            });
        }

        private static async Task<object> NewProjectAsync(
            string templatePath,
            string outputPath,
            string templateName,
            string projectName,
            string solutionFolderName)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            await EnsureSolutionAsync(outputPath);

            string projectTemplatePath = null;
            string projectTemplateFilePath = null;

            var dte = ServiceLocator.GetInstance<DTE>();
            var dte2 = (DTE2)dte;
            var solution2 = dte2.Solution as Solution2;
            Project solutionFolderProject = null;

            dynamic newProject = null;

            if (templateName.Equals("DNXClassLibrary", StringComparison.Ordinal)
                || templateName.Equals("DNXConsoleApp", StringComparison.Ordinal))
            {
                projectTemplatePath = templateName + ".vstemplate|FrameworkVersion=4.5";
                var lang = "CSharp/Web";

                projectTemplateFilePath = solution2.GetProjectItemTemplate(projectTemplatePath, lang);
            }
            else
            {
                projectTemplatePath = Path.Combine(templatePath, templateName + ".zip");

                var projectTemplateFiles = Directory.GetFiles(projectTemplatePath, "*.vstemplate");
                Debug.Assert(projectTemplateFiles.Length > 0);
                projectTemplateFilePath = projectTemplateFiles[0];
            }

            var solutionDir = Path.GetDirectoryName(solution2.FullName);

            string destPath = null;
            if (string.IsNullOrEmpty(solutionFolderName))
            {
                destPath = Path.Combine(solutionDir, projectName);
            }
            else
            {
                destPath = Path.Combine(solutionDir, Path.Combine(solutionFolderName, projectName));
            }

            var window = dte2.ActiveWindow as Window2;

            if (string.IsNullOrEmpty(solutionFolderName))
            {
                newProject = solution2.AddFromTemplate(projectTemplateFilePath, destPath, projectName, Exclusive: false);
            }
            else
            {
                solutionFolderProject = GetSolutionFolderProject(dte2, solutionFolderName);                
                if (solutionFolderProject == null)
                {
                    throw new ArgumentException("No corresponding solution folder exists", nameof(solutionFolderName));
                }

                var solutionFolder = (SolutionFolder)solutionFolderProject.Object;
                newProject = solutionFolder.AddFromTemplate(projectTemplateFilePath, destPath, projectName);
            }

            foreach (var document in dte2.Documents)
            {
                try
                {
                    ((Document)document).Close();
                }
                catch { }
            }

            foreach(var config in dte2.Solution.SolutionBuild.SolutionConfigurations)
            {
                var solutionConfiguration = config as SolutionConfiguration2;
                if (solutionConfiguration.PlatformName.Equals("x86", StringComparison.Ordinal))
                {
                    solutionConfiguration.Activate();
                }
            }

            window.SetFocus();

            // HACK: May need to be fixed
            if (newProject == null)
            {
                if (solutionFolderProject != null)
                {
                    foreach (ProjectItem project in solutionFolderProject.ProjectItems)
                    {
                        newProject = project.Object as Project;
                        if (project.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                        else
                        {
                            newProject = null;
                        }
                    }
                }
                else
                {
                    foreach(Project project in solution2.Projects)
                    {
                        if (project.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase))
                        {
                            newProject = project;
                            break;
                        }
                    }
                }
            }

            if (newProject == null)
            {
                throw new InvalidOperationException("Could not create new project or could not locate newly created project");
            }

            return newProject;
        }
    }
}
