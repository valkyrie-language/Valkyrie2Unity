using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Valkyrie2Unity.Parser;

namespace Valkyrie2Unity.Translator
{
    /// <summary>
    /// VK脚本编译器，负责解析和生成IL
    /// </summary>
    public class ValkyrieTranslator
    {

        
        public CompilationResult Compile(string sourceCode, string assemblyNameStr, string finalDllPath)
        {
            // [关键修正] 重新引入"保存并移动"模式
            var simpleFileName = Path.GetFileName(finalDllPath);
            var tempDllPathInRoot = Path.Combine(Directory.GetCurrentDirectory(), simpleFileName);
            
            try
            {
                var parseResult = ValkyrieParser.Parse(sourceCode);
                if (!parseResult.Success)
                {
                    return new CompilationResult { Success = false, Errors = new[] { parseResult.Error } };
                }

                var assemblyName = new AssemblyName(assemblyNameStr);
                var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Save);
                
                // ModuleBuilder 和 Save() 都必须使用不带路径的文件名
                var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, simpleFileName, true);

                var typeBuilder = moduleBuilder.DefineType(
                    parseResult.ClassName,
                    TypeAttributes.Public | TypeAttributes.Class,
                    typeof(MonoBehaviour)
                );

                var methodBuilder = typeBuilder.DefineMethod(
                    "Start",
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    typeof(void),
                    Type.EmptyTypes
                );

                var il = methodBuilder.GetILGenerator();
                il.Emit(OpCodes.Ldstr, parseResult.LogMessage);
                var logMethod = typeof(Debug).GetMethod("Log", new[] { typeof(object) });
                il.Emit(OpCodes.Call, logMethod);
                il.Emit(OpCodes.Ret);

                typeBuilder.CreateType();
                
                // 步骤 1: 使用纯文件名保存，DLL 会被创建在项目根目录
                assemblyBuilder.Save(simpleFileName);

                // 步骤 2: 验证临时文件是否真的被创建
                if (!File.Exists(tempDllPathInRoot))
                {
                    return new CompilationResult 
                    { 
                        Success = false, 
                        Errors = new[] { $"AssemblyBuilder.Save() 调用完成，但未在项目根目录创建临时文件 '{simpleFileName}'。" } 
                    };
                }

                // 步骤 3: 将根目录的 DLL 移动到最终的目标位置
                if (File.Exists(finalDllPath))
                {
                    File.Delete(finalDllPath);
                }
                File.Move(tempDllPathInRoot, finalDllPath);

                Debug.Log($"[VkCompiler] 已成功将临时 DLL 移动到 '{finalDllPath}'");

                return new CompilationResult { Success = true };
            }
            catch (Exception ex)
            {
                return new CompilationResult { Success = false, Errors = new[] { ex.ToString() } };
            }
            finally
            {
                // 步骤 4 (关键): 无论成功与否，都确保清理掉根目录的临时文件
                if (File.Exists(tempDllPathInRoot))
                {
                    File.Delete(tempDllPathInRoot);
                }
            }
        }
    }
}