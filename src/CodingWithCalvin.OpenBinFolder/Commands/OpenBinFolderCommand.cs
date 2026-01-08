using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using CodingWithCalvin.Otel4Vsix;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.VCProjectEngine;
using Project = EnvDTE.Project;

namespace CodingWithCalvin.OpenBinFolder.Commands
{
    internal class OpenBinFolderCommand
    {
        // Project type GUIDs
        private const string CSharpProjectKind = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
        private const string VbNetProjectKind = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";
        private const string FSharpProjectKind = "{F2A71F9B-5D33-465A-A702-920D77279786}";
        private const string CppProjectKind = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";

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

                string projectBinPath;

                if (IsCppProject(project.Kind) && project.Object is VCProject vcProject)
                {
                    projectBinPath = GetCppOutputPath(vcProject, projectPath);
                }
                else
                {
                    var projectOutputPath = project
                        .ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath")
                        .Value.ToString();
                    projectBinPath = Path.Combine(projectPath, projectOutputPath);
                }

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

            bool IsCppProject(string projectKind)
            {
                return string.Equals(projectKind, CppProjectKind, StringComparison.OrdinalIgnoreCase);
            }

            string GetCppOutputPath(VCProject vcProject, string projectPath)
            {
                var activeConfig = vcProject.ActiveConfiguration as VCConfiguration;
                if (activeConfig == null)
                {
                    throw new InvalidOperationException("Unable to get active configuration for C++ project");
                }

                // Evaluate expands macros like $(OutDir), $(Configuration), $(Platform), etc.
                var outDir = activeConfig.Evaluate("$(OutDir)");

                if (Path.IsPathRooted(outDir))
                {
                    return outDir;
                }

                return Path.Combine(projectPath, outDir);
            }
        }
    }
}
