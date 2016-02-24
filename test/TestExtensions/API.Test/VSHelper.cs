using System;
using System.Collections;
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
        private static void ThrowStringArgException(string value, string paramName)
        {
            if (string.IsNullOrEmpty(paramName))
            {
                throw new ArgumentException("string cannot be null or empty", nameof(paramName));
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("string cannot be null or empty", paramName);
            }
        }

        private static string GetNewGUID()
        {
            return Guid.NewGuid().ToString("d").Substring(0, 4).Replace("-", "");
        }

        private static async Task<Solution2> GetSolution2Async()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = ServiceLocator.GetInstance<DTE>();
            var dte2 = (DTE2)dte;
            var solution2 = dte2.Solution as Solution2;

            return solution2;
        }

        public static string GetVSVersion()
        {
            return ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                var version = await GetVSVersionAsync();
                return version;
            });
        }

        private static async Task<string> GetVSVersionAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = ServiceLocator.GetInstance<DTE>();
            var version = dte.Version;

            return version;
        }

        public static string GetSolutionFullName()
        {
            return ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                var solutionFullName = await GetSolutionFullNameAsync();
                return solutionFullName;
            });
        }

        private static async Task<string> GetSolutionFullNameAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var solution2 = await GetSolution2Async();
            var solutionFullName = solution2.FullName;
            return solutionFullName;
        }

        public static void CreateSolution(string solutionDirectory, string name)
        {
            ThrowStringArgException(solutionDirectory, nameof(solutionDirectory));
            ThrowStringArgException(name, nameof(name));

            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await CreateSolutionAsync(solutionDirectory, name);
            });
        }

        private static async Task CreateSolutionAsync(string solutionDirectory, string name)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var solution2 = await GetSolution2Async();
            solution2.Create(solutionDirectory, name);

            var solutionPath = Path.Combine(solutionDirectory, name);
            solution2.SaveAs(solutionPath);
        }

        private static string SolutionNameFormat = "Solution_{0}";
        public static void CreateNewSolution(string outputPath, string solutionName)
        {
            ThrowStringArgException(outputPath, nameof(outputPath));
            ThrowStringArgException(solutionName, nameof(solutionName));

            var solutionDir = Path.Combine(outputPath, solutionName);

            Directory.CreateDirectory(solutionDir);

            CreateSolution(solutionDir, solutionName);
        }
        
        public static void CreateNewSolution(string outputPath)
        {
            ThrowStringArgException(outputPath, nameof(outputPath));

            var id = GetNewGUID();
            var solutionName = string.Format(SolutionNameFormat, id);

            CreateNewSolution(outputPath, solutionName);
        }

        public static void EnsureSolution(string outputPath)
        {
            ThrowStringArgException(outputPath, nameof(outputPath));

            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await EnsureSolutionAsync(outputPath);
            });
        }

        private static async Task EnsureSolutionAsync(string outputPath)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var isSolutionAvailable = await IsSolutionAvailableAsync();
            if (!isSolutionAvailable)
            {
                CreateNewSolution(outputPath);
            }
        }

        public static bool IsSolutionAvailable()
        {
            return ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                var isSolutionAvailable = await IsSolutionAvailableAsync();
                return isSolutionAvailable;
            });
        }

        private static async Task<bool> IsSolutionAvailableAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var solution2 = await GetSolution2Async();
            return await IsSolutionAvailableAsync(solution2);
        }

        private static async Task<bool> IsSolutionAvailableAsync(Solution2 solution2)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return solution2 != null && solution2.IsOpen;
        }
        
        public static void CloseSolution()
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await CloseSolutionAsync();
            });
        }

        private static async Task CloseSolutionAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var solution2 = await GetSolution2Async();
            var isSolutionAvailable = await IsSolutionAvailableAsync(solution2);
            if (isSolutionAvailable)
            {
                solution2.Close();
            }
        }

        public static void OpenSolution(string solutionFile)
        {
            ThrowStringArgException(solutionFile, nameof(solutionFile));

            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await OpenSolutionAsync(solutionFile);
            });
        }

        private static async Task OpenSolutionAsync(string solutionFile)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var solution2 = await GetSolution2Async();
            solution2.Open(solutionFile);
        }

        public static void SaveAsSolution(string solutionFile)
        {
            ThrowStringArgException(solutionFile, nameof(solutionFile));

            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await SaveAsSolutionAsync(solutionFile);
            });
        }

        private static async Task SaveAsSolutionAsync(string solutionFile)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var solution2 = await GetSolution2Async();
            solution2.SaveAs(solutionFile);
        }

        public static string GetBuildOutput()
        {
            return ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                var text = await GetBuildOutputAsync();
                return text;
            });
        }

        private static string BuildOutputPaneName = "Build";
        private static async Task<string> GetBuildOutputAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = ServiceLocator.GetInstance<DTE>();
            var dte2 = (DTE2)dte;
            var buildPane = dte2.ToolWindows.OutputWindow.OutputWindowPanes.Item(BuildOutputPaneName);
            var doc = buildPane.TextDocument;
            var sel = doc.Selection;
            sel.StartOfDocument(Extend: false);
            sel.EndOfDocument(Extend: true);
            var text = sel.Text;
            return text;
        }
        
        private static async Task<Project> GetSolutionFolderProjectAsync(Solution2 solution2, string solutionFolderName)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var solutionFolderParts = solutionFolderName.Split('\\');
            var solutionFolderProject = await GetSolutionFolderProjectAsync(solution2.Projects, solutionFolderParts, 0);

            if (solutionFolderProject == null)
            {
                throw new ArgumentException("No corresponding solution folder exists", nameof(solutionFolderName));
            }

            var solutionFolder = solutionFolderProject.Object as SolutionFolder;
            if (solutionFolder == null)
            {
                throw new ArgumentException("Not a valid solution folder", nameof(solutionFolderName));
            }

            return solutionFolderProject;
        }

        private static async Task<Project> GetSolutionFolderProjectAsync(
            IEnumerable projectItems,
            string[] solutionFolderParts,
            int level)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (solutionFolderParts == null)
            {
                throw new ArgumentNullException(nameof(solutionFolderParts));
            }

            if (solutionFolderParts.Length == 0)
            {
                throw new ArgumentException("solution folder parts cannot be null", nameof(solutionFolderParts));
            }

            if (projectItems == null || level >= solutionFolderParts.Length)
            {
                return null;
            }

            var solutionFolderName = solutionFolderParts[level];
            Project solutionFolderProject = null;

            foreach (var item in projectItems)
            {
                // Item could be a project or a projectItem
                Project project = item as Project;

                if (project == null)
                {
                    var projectItem = item as ProjectItem;
                    if (projectItem != null)
                    {
                        project = projectItem.SubProject;
                    }
                }

                if (project != null)
                {
                    if (project.UniqueName.StartsWith(solutionFolderName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (solutionFolderParts.Length == level + 1)
                        {
                            solutionFolderProject = project;
                            break;
                        }
                        else
                        {
                            solutionFolderProject
                                = await GetSolutionFolderProjectAsync(project.ProjectItems, solutionFolderParts, level + 1);
                        }
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
            ThrowStringArgException(templatePath, nameof(templatePath));
            ThrowStringArgException(outputPath, nameof(outputPath));
            ThrowStringArgException(templateName, nameof(templateName));
            // projectName can be null or empty
            // solutionFolderName can be null or empty

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
                solutionFolderProject = await GetSolutionFolderProjectAsync(solution2, solutionFolderName);

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

        public static void NewSolutionFolder(string outputPath, string folderPath)
        {
            ThrowStringArgException(outputPath, nameof(outputPath));
            ThrowStringArgException(folderPath, nameof(folderPath));

            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await NewSolutionFolderAsync(outputPath, folderPath);
            });
        }

        private static async Task NewSolutionFolderAsync(string outputPath, string folderPath)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var solution2 = await GetSolution2Async();

            

            var newSolutionFolderIndex = folderPath.LastIndexOf('\\');

            if (newSolutionFolderIndex == -1)
            {
                // Create solution folder at solution level
                await EnsureSolutionAsync(outputPath);
                solution2.AddSolutionFolder(folderPath);
                return;
            }
            else
            {
                // Get solution folder project object for parent
                var parentName = folderPath.Substring(0, newSolutionFolderIndex);
                var solutionFolderName = folderPath.Substring(newSolutionFolderIndex + 1);

                var parentProject = await GetSolutionFolderProjectAsync(solution2, parentName);
                var parentSolutionFolder = (SolutionFolder)parentProject.Object;
                parentSolutionFolder.AddSolutionFolder(solutionFolderName);
            }
        }

        public static void RenameSolutionFolder(string folderPath, string newName)
        {
            ThrowStringArgException(folderPath, nameof(folderPath));
            ThrowStringArgException(newName, nameof(newName));

            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await RenameSolutionFolderAsync(folderPath, newName);
            });
        }

        #region Private Methods

        private static async Task RenameSolutionFolderAsync(string folderPath, string newName)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var solution2 = await GetSolution2Async();

            var solutionFolderProject = await GetSolutionFolderProjectAsync(solution2, folderPath);
            solutionFolderProject.Name = newName;
        }

        #endregion Private Methods
    }
}
