using UnityEngine;
using UnityEditor;
using System.Collections;

public class ProcessModel : AssetPostprocessor {

    // 预处理模型回调函数
    void OnPreprocessModel()
    {
        // 如果模型名称包括@
        if (assetPath.Contains("@"))
        {
            ModelImporter model = assetImporter as ModelImporter;
            model.animationType = ModelImporterAnimationType.Generic;
        }
    }
}
