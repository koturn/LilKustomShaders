using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Koturn.lilToon.Windows
{
    /// <summary>
    /// A window class for creating Texture2DArray.
    /// </summary>
    public class TextureCombineWindow : EditorWindow
    {
        /// <summary>
        /// Texture list.
        /// </summary>
        [SerializeField]
        private List<Texture2D> _textures;
        /// <summary>
        /// Last saved Asset Path.
        /// </summary>
        [SerializeField]
        private string _assetPath;
        /// <summary>
        /// Scroll position.
        /// </summary>
        [SerializeField]
        private Vector2 _scrollPosition;

        /// <summary>
        /// Initialize members.
        /// </summary>
        public TextureCombineWindow()
        {
            _textures = new List<Texture2D>();
            _textures.Add(null);
            _assetPath = "Assets/Texture2DArray.asset";
            _scrollPosition = new Vector2();
        }

        /// <Summary>
        /// Draw window components.
        /// </Summary>
        private void OnGUI()
        {
            var textures = _textures;

            using (var svScope = new EditorGUILayout.ScrollViewScope(_scrollPosition))
            {
                _scrollPosition = svScope.scrollPosition;

                int removeIndex = -1;
                for (int i = 0; i < textures.Count; i++)
                {
                    using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
                    using (var ccScope = new EditorGUI.ChangeCheckScope())
                    {
                        var result = (Texture2D)EditorGUILayout.ObjectField(textures[i], typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
                        if (ccScope.changed)
                        {
                            textures[i] = result;
                        }
                        if (GUILayout.Button("Remove Texture"))
                        {
                            removeIndex = i;
                        }
                    }
                }
                if (removeIndex != -1)
                {
                    textures.RemoveAt(removeIndex);
                }
            }

            if (GUILayout.Button("Add Texture"))
            {
                textures.Add(null);
            }
            if (GUILayout.Button("Create Texture2DArray"))
            {
                OnCreateTexture2DArrayClicked();
            }
        }

        /// <summary>
        /// An action when button of "Create Texture2DArray" is clicked.
        /// </summary>
        private void OnCreateTexture2DArrayClicked()
        {
            var textures = _textures.Where(texture => texture != null).ToList();
            if (textures.Count == 0)
            {
                EditorUtility.DisplayDialog("Caution", "No available texture is added", "OK");
                return;
            }
            var assetPath = EditorUtility.SaveFilePanelInProject(
                "Save mesh",
                Path.GetFileName(_assetPath),
                "asset",
                "Enter a file name to save the Texture2DArray to",
                Path.GetDirectoryName(_assetPath));
            if (assetPath.Length == 0)
            {
                return;
            }
            _assetPath = assetPath;

            var tex2dArray = new Texture2DArray(
                textures[0].width,
                textures[0].height,
                textures.Count,
                TextureFormat.ARGB32,
                true)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            for (var i = 0; i < textures.Count; i++)
            {
                if (textures[i] == null)
                {
                    continue;
                }
                var texture = CreateReadableTexture2D(textures[i]);
                tex2dArray.SetPixels(texture.GetPixels(0), i, 0);
                DestroyImmediate(texture);
            }
            tex2dArray.Apply();

            AssetDatabase.CreateAsset(tex2dArray, assetPath);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Copy and create readable texture.
        /// </summary>
        /// <param name="texture">Source texture.</param>
        /// <returns>Created readable texture.</returns>
        private static Texture2D CreateReadableTexture2D(Texture2D texture)
        {
            var renderTexture = RenderTexture.GetTemporary(
                texture.width,
                texture.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);
            Graphics.Blit(texture, renderTexture);

            var previousTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;

            var readableTexture = new Texture2D(texture.width, texture.height);
            readableTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            readableTexture.Apply();

            RenderTexture.active = previousTexture;
            RenderTexture.ReleaseTemporary(renderTexture);
            return readableTexture;
        }

        /// <summary>
        /// Open window.
        /// </summary>
        [MenuItem("Assets/koturn/LilCrossFade/Texture Combine Tool", false, 1200)]
        public static void Open()
        {
            EditorWindow.GetWindow<TextureCombineWindow>("Texture Combine Tool");
        }
    }
}

