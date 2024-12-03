using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace PropertyGenerator
{
    internal static class ShaderWrapperGenerator
    {
        [MenuItem("Assets/PropertyGenerator/GenerateShaderWrapper", priority = 1)]
        private static void Init()
        {
            if (Selection.activeObject is Shader shader)
            {
                Generate(shader);
            }
            else
            {
                Debug.LogError($"{nameof(Shader)}以外のファイルが選択されています。");
            }
        }

        private static void Generate(Shader shader)
        {
            //シェーダー名だけを抽出する
            string className = $"{shader.name.Split('/')[^1]}Wrapper".Replace(" ", String.Empty);
            CodeBuilder codeBuilder = new CodeBuilder();

            codeBuilder.AddAutoGenerated(nameof(ShaderWrapperGenerator));
            codeBuilder.AddNameSpace("UnityEngine");
            codeBuilder.AddNameSpace("System");

            using (codeBuilder.CreateBlockScope("namespace PropertyGenerator.Generated"))
            {
                codeBuilder.NewLine("[Serializable]");
                using (codeBuilder.CreateBlockScope($"public class {className}"))
                {
                    codeBuilder.NewLine($"[SerializeField] private {nameof(Renderer)} renderer;");
                    codeBuilder.NewLine($"private {nameof(Material)} target;");

                    //Materialのインスタンスを登録するコンストラクタを生成する
                    AddConstructor(className, codeBuilder);

                    AddMaterialCreateMethod(codeBuilder);

                    //Shaderのプロパティを生成する
                    for (int i = 0; i < shader.GetPropertyCount(); i++)
                    {
                        //アンダーバーがついてたら省く
                        string name = shader.GetPropertyName(i).Replace("_", String.Empty);
                        int id = shader.GetPropertyNameId(i);
                        ShaderPropertyType type = shader.GetPropertyType(i);

                        AddParameter(name, id, type, codeBuilder);
                    }
                }
            }

            //ソースファイルとして出力する
            SourceCreator.CreateFile(className, codeBuilder);
        }

        private static void AddConstructor(string className, CodeBuilder builder)
        {
            builder.NewLine();
            builder.NewLine($"public {className}() {{ }}");
            builder.NewLine();
            builder.NewLine($"public {className}(Material target)");
            builder.NewLine("{");
            builder.NewLine("   target = renderer.material;");
            builder.NewLine("}");
        }

        private static void AddMaterialCreateMethod(CodeBuilder builder)
        {
            builder.NewLine();
            builder.NewLine("private void CheckPersistentMaterial()");
            builder.NewLine("{");
            builder.NewLine("   if (target != null)");
            builder.NewLine("   {");
            builder.NewLine("       return;");
            builder.NewLine("   }");
            builder.NewLine();
            builder.NewLine("   target = renderer.material;");
            builder.NewLine("}");
        }

        private static void AddParameter(string name, int id, ShaderPropertyType type, CodeBuilder builder)
        {
            string methodSuffix = GetMethodSuffix(type);
            string propertyType = GetPropertyType(type);

            builder.NewLine();

            using (builder.CreateBlockScope($"public {propertyType} {name}"))
            {
                builder.NewLine("get");
                builder.NewLine("{");
                builder.NewLine("  CheckPersistentMaterial();");
                builder.NewLine($"  return target.Get{methodSuffix}({id});");
                builder.NewLine("}");

                builder.NewLine("set");
                builder.NewLine("{");
                builder.NewLine("  CheckPersistentMaterial();");
                builder.NewLine($"  target.Set{methodSuffix}({id}, value);");
                builder.NewLine("}");
            }
        }

        private static string GetMethodSuffix(ShaderPropertyType type)
        {
            return type switch
            {
                ShaderPropertyType.Color => "Color",
                ShaderPropertyType.Range => "Float",
                ShaderPropertyType.Float => "Float",
                ShaderPropertyType.Int => "Integer",
                ShaderPropertyType.Vector => "Vector",
                ShaderPropertyType.Texture => "Texture",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static string GetPropertyType(ShaderPropertyType type)
        {
            return type switch
            {
                ShaderPropertyType.Color => "Color",
                ShaderPropertyType.Range => "float",
                ShaderPropertyType.Float => "float",
                ShaderPropertyType.Int => "int",
                ShaderPropertyType.Vector => "Vector4",
                ShaderPropertyType.Texture => "Texture",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}