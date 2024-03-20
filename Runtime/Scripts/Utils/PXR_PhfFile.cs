using System.Collections;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.AssetImporters;
using UnityEditor.Experimental.AssetImporters;

[ScriptedImporter(1, ".phf")]
public class PXR_PhfFile : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        var phfTxt = File.ReadAllText(ctx.assetPath);
        var assetText = new TextAsset(phfTxt);
        ctx.AddObjectToAsset("main obj", assetText);
        ctx.SetMainObject(assetText);
    }
}
#endif
