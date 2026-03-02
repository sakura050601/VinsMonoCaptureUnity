#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace VinsMonoCapture.Editor.iOS
{
    public static class IosFrameworkAutoLinker
    {
        [PostProcessBuild(100)]
        public static void OnPostProcessBuild(BuildTarget target, string buildPath)
        {
            if (target != BuildTarget.iOS)
            {
                return;
            }

            var projectPath = PBXProject.GetPBXProjectPath(buildPath);
            var project = new PBXProject();
            project.ReadFromFile(projectPath);

            var mainTargetGuid = project.GetUnityMainTargetGuid();
            var frameworkTargetGuid = project.GetUnityFrameworkTargetGuid();

            AddRequiredFrameworks(project, mainTargetGuid);
            AddRequiredFrameworks(project, frameworkTargetGuid);

            project.SetBuildProperty(mainTargetGuid, "ENABLE_BITCODE", "NO");
            project.SetBuildProperty(frameworkTargetGuid, "ENABLE_BITCODE", "NO");

            project.WriteToFile(projectPath);
        }

        private static void AddRequiredFrameworks(PBXProject project, string targetGuid)
        {
            project.AddFrameworkToProject(targetGuid, "AVFoundation.framework", false);
            project.AddFrameworkToProject(targetGuid, "CoreMotion.framework", false);
            project.AddFrameworkToProject(targetGuid, "CoreMedia.framework", false);
            project.AddFrameworkToProject(targetGuid, "CoreVideo.framework", false);
            project.AddFrameworkToProject(targetGuid, "CoreImage.framework", false);
            project.AddFrameworkToProject(targetGuid, "ImageIO.framework", false);
            project.AddFrameworkToProject(targetGuid, "UIKit.framework", false);
            project.AddFrameworkToProject(targetGuid, "Foundation.framework", false);
        }
    }
}
#endif
