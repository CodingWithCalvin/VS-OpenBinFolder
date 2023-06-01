using System;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Project = EnvDTE.Project;

namespace CodingWithCalvin.OpenBinFolder.Vsix.Commands
{
    internal class OpenBinFolderCommand
    {
        private readonly Package _package;

        private OpenBinFolderCommand(Package package)
        {
            _package = package;

            var commandService = (OleMenuCommandService)ServiceProvider.GetService(typeof(IMenuCommandService));

            if (commandService == null)
            {
                return;
            }

            var menuCommandId = new CommandID(PackageGuids.CommandSetGuid, PackageIds.OpenBinCommandId);
            var menuItem = new MenuCommand(OpenPath, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        private IServiceProvider ServiceProvider => _package;

        public static void Initialize(Package package)
        {
            _ = new OpenBinFolderCommand(package);
        }

        private void OpenPath(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!(ServiceProvider.GetService(typeof(DTE)) is DTE2 dte))
            {
                throw new ArgumentNullException(nameof(dte));
            }

            foreach (UIHierarchyItem selectedItem in (Array)dte.ToolWindows.SolutionExplorer.SelectedItems)
            {
                switch (selectedItem.Object)
                {
                    case Project project:
                        try
                        {
                            OpenProjectBinFolder(project);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($@"
                                Unable to determine output path for selected project
                                {Environment.NewLine}
                                {Environment.NewLine}
                                Exception: {ex.Message}");
                        }

                        break;
                }
            }

            void OpenProjectBinFolder(Project project)
            {
                var projectPath = Path.GetDirectoryName(project.FullName)
                                  ?? throw new InvalidOperationException();

                var projectActiveConfigProperties = project.ConfigurationManager.ActiveConfiguration.Properties;
                if (projectActiveConfigProperties != null)
                {
                    var projectOutputPath = projectActiveConfigProperties.Item("OutputPath").Value.ToString();

                    var projectBinPath = Path.Combine(projectPath, projectOutputPath);

                    System.Diagnostics.Process.Start(
                        Directory.Exists(projectBinPath)
                            ? projectBinPath
                            : projectPath
                    );
                }
                else
                {
                    MessageBox.Show($@"Unable to determine output path for selected project");
                }
            }
        }
    }
}
