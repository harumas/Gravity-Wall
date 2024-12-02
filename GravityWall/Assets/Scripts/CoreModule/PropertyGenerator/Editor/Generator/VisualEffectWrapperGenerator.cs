﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

namespace PropertyGenerator
{
    public class VisualEffectWrapperGenerator
    {
        private static readonly Dictionary<Type, string> propertyTypeMap = new Dictionary<Type, string>()
        {
            { typeof(AnimationCurve), "AnimationCurve" },
            { typeof(bool), "Bool" },
            { typeof(float), "Float" },
            { typeof(Gradient), "Gradient" },
            { typeof(int), "Int" },
            { typeof(Matrix4x4), "Matrix4x4" },
            { typeof(Mesh), "Mesh" },
            { typeof(Texture), "Texture" },
            { typeof(uint), "UInt" },
            { typeof(Vector2), "Vector2" },
            { typeof(Vector3), "Vector3" },
            { typeof(Vector4), "Vector4" },
        };

        [MenuItem("Assets/PropertyGenerator/GenerateVisualEffectWrapper", priority = 1)]
        private static void Init()
        {
            if (Selection.activeObject is VisualEffectAsset vfxAsset)
            {
                Generate(vfxAsset);
            }
            else
            {
                Debug.LogError($"{nameof(Shader)}以外のファイルが選択されています。");
            }
        }

        private static void Generate(VisualEffectAsset vfxAsset)
        {
            //シェーダー名だけを抽出する
            string className = $"{vfxAsset.name.Split('/')[^1]}Wrapper".Replace(" ", String.Empty);
            CodeBuilder codeBuilder = new CodeBuilder();

            codeBuilder.AddAutoGenerated(nameof(VisualEffectWrapperGenerator));
            codeBuilder.AddNameSpace("System");
            codeBuilder.AddNameSpace("UnityEngine");
            codeBuilder.AddNameSpace("UnityEngine.VFX");

            using (codeBuilder.CreateBlockScope("namespace PropertyGenerator.Generated"))
            {
                codeBuilder.NewLine("[Serializable]");
                using (codeBuilder.CreateBlockScope("public class " + className))
                {
                    codeBuilder.NewLine($"[SerializeField] private {nameof(VisualEffect)} target;");
                    codeBuilder.NewLine($"public {nameof(VisualEffect)} Effect => target;");

                    var exposedProperties = new List<VFXExposedProperty>();
                    vfxAsset.GetExposedProperties(exposedProperties);

                    codeBuilder.NewLine();
                    foreach (VFXExposedProperty property in exposedProperties)
                    {
                        AddProperty(property.name, codeBuilder);
                    }

                    foreach (VFXExposedProperty property in exposedProperties)
                    {
                        AddParameter(property.name, property.type, codeBuilder);
                    }
                }
            }

            //ソースファイルとして出力する
            SourceCreator.CreateFile(className, codeBuilder);
        }

        private static void AddProperty(string name, CodeBuilder builder)
        {
            builder.NewLine($"private static readonly int {name}Property = Shader.PropertyToID(\"{name}\");");
        }

        private static void AddParameter(string name, Type type, CodeBuilder builder)
        {
            string suffix = propertyTypeMap[type];

            builder.NewLine();
            using (builder.CreateBlockScope($"public {type} {name}"))
            {
                builder.NewLine($"get => target.Get{suffix}({name}Property);");
                builder.NewLine($"set => target.Set{suffix}({name}Property, value);");
            }
        }
    }
}