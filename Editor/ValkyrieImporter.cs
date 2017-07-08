using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;
using Valkyrie2Unity.Translator;

namespace Valkyrie2Unity.Editor
{
    /// <summary>
    /// ScriptedImporter 负责监视 .vk 文件的变化，并触发编译流程
    /// </summary>
    [ScriptedImporter(0, new[] { "vk", "valkyrie" })]
    public class ValkyrieImporter : ScriptedImporter
    {
        private const string GeneratedAssembliesPath = "Assets/Plugins/ValkyrieLanguage/";

        public override void OnImportAsset(AssetImportContext ctx)
        {
            Debug.Log($"[VkImporter] 检测到 .vk 文件变化: {ctx.assetPath}");

            var sourceCode = File.ReadAllText(ctx.assetPath);
            var assemblyName = Path.GetFileNameWithoutExtension(ctx.assetPath);

            Directory.CreateDirectory(GeneratedAssembliesPath);
            var finalDllPath = Path.Combine(GeneratedAssembliesPath, "Assembly-Valkyrie.dll");

            var compiler = new ValkyrieTranslator();
            var result = compiler.Compile(sourceCode, assemblyName, finalDllPath);

            if (!result.Success)
            {
                foreach (var error in result.Errors)
                {
                    Debug.LogError($"[VkCompiler] 编译失败 for '{ctx.assetPath}': {error}");
                }

                return;
            }

            Debug.Log($"[VkImporter] 成功编译 '{ctx.assetPath}' 到 '{finalDllPath}'. Unity 将自动加载程序集。");
        }
    }
}