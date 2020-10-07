using Khepri.AssetDelivery;
using Khepri.PlayAssetDelivery.Editor;
using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UDebug = UnityEngine.Debug;

public class PlayAssetPackBundlesPreprocessor : IPreprocessBuildWithReport {
    public int callbackOrder => 2;

    public void OnPreprocessBuild ( BuildReport report ) {
        var isAabBuild = report.summary.outputPath.EndsWith ( ".aab", StringComparison.InvariantCultureIgnoreCase );
        var buildCfg = GetOrCreateConfig ();
        buildCfg.IsAabBuild = isAabBuild;
        EditorUtility.SetDirty ( buildCfg );
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
        if ( !isAabBuild || report.summary.platform != BuildTarget.Android )
            return;

        UDebug.Log (
            $"[{nameof ( PlayAssetPackBundlesPreprocessor )}.{nameof ( OnPreprocessBuild )}]" +
            " This is aab-build. Remove addressable asset bundles from StreamingAssets folder."
        );
        foreach (var bundle in AssetPackBuilder.GetBundles(Addressables.PlayerBuildDataPath))
        {
            bundle.DeleteFile();
        };
    }

    private static AndroidBuildTypeConfig GetOrCreateConfig () {
		var config = AssetDatabase.LoadAssetAtPath <AndroidBuildTypeConfig> ( AndroidBuildTypeConfig.Path );
		if ( config == null ) {
			config = ScriptableObject.CreateInstance <AndroidBuildTypeConfig> ();
			AssetDatabase.CreateAsset ( config, AndroidBuildTypeConfig.Path );
		}
		return config;
	}
}