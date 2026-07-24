#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace ChemistryLab.Editor
{
    public static class BuildWindows
    {
        public static void Build()
        {
            var outputPath = "D:/Codex-Workplace/artifacts/ChemistryLabMVP/chemistry-lab.exe";
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/SampleScene.unity" },
                locationPathName = outputPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            });

            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException("Windows build failed: " + report.summary.result);
            }

            UnityEngine.Debug.Log("Windows build succeeded: " + outputPath);
        }
    }
}
#endif
