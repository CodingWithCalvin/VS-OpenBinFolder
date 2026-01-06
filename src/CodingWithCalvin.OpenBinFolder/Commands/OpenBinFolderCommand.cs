using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using CodingWithCalvin.Otel4Vsix;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Project = EnvDTE.Project;

namespace CodingWithCalvin.OpenBinFolder.Commands
{
    internal class OpenBinFolderCommand
    {
        private readonly Package _package;

        private OpenBinFolderCommand(Package package)
        {
            _package = package;

            var commandService = (OleMenuCommandService)
                ServiceProvider.GetService(typeof(IMenuCommandService));

            if (commandService == null)
            {
                return;
            }

            var menuCommandId = new CommandID(
                PackageGuids.CommandSetGuid,
                PackageIds.OpenBinCommandId
            );
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

            using var activity = VsixTelemetry.StartCommandActivity("OpenBinFolder.OpenPath");

            try
            {
                if (!(ServiceProvider.GetService(typeof(DTE)) is DTE2 dte))
                {
                    throw new ArgumentNullException(nameof(dte));
                }

                foreach (
                    UIHierarchyItem selectedItem in (Array)
                        dte.ToolWindows.SolutionExplorer.SelectedItems
                )
                {
                    switch (selectedItem.Object)
                    {
                        case Project project:
                            OpenProjectBinFolder(project);
                            break;
                    }
                }

                VsixTelemetry.LogInformation("Bin folder opened successfully");
            }
            catch (Exception ex)
            {
                activity?.RecordError(ex);
                VsixTelemetry.TrackException(ex, new Dictionary<string, object>
                {
                    { "operation.name", "OpenPath" }
                });
                throw;
            }
        }

        private void OpenProjectBinFolder(Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            using var activity = VsixTelemetry.StartCommandActivity("OpenBinFolder.OpenProjectBinFolder");

            try
            {
                var projectPath =
                    Path.GetDirectoryName(project.FullName)
                    ?? throw new InvalidOperationException();

                var projectOutputPath = project
                    .ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath")
                    .Value.ToString();

                var projectBinPath = Path.Combine(projectPath, projectOutputPath);

                                System.Diagnostics.Process.Start(
                    Directory.Exists(projectBinPath) ? projectBinPath : projectPath
                );

                VsixTelemetry.LogInformation("Opened bin folder for project");
            }
            catch (Exception ex)
            {
                activity?.RecordError(ex);
                VsixTelemetry.TrackException(ex, new Dictionary<string, object>
                {
                    { "operation.name", "OpenProjectBinFolder" },
                    });

                MessageBox.Show(
                    $@"
                    Unable to determine output path for selected project
                    {Environment.NewLine}
                    {Environment.NewLine}
                    Exception: {ex.Message}"
                );
            }
        }
    }
}
