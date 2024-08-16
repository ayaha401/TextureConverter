using UnityEngine;
using UnityEditor;
using System.IO;

namespace AyahaGraphicDevelopTools.TextureConverter
{
    public class TextureConverterWindow : EditorWindow
    {
        /// <summary>
        /// 生成するテクスチャのサイズ設定
        /// </summary>
        private enum GenerateSizeKinds
        {
            Half = 512,
            Normal = 1024,
            Large = 2048,
            Custom = -1,
        }

        private const int ImportTexturePreviewSize = 200;
        private string[] previewKinds = new string[] { "ALL", "R", "G", "B", "A" };

        [SerializeField]
        private Texture2D importTexture1;
        private SerializedProperty importTexture1Prop;
        private int selectImportTexture1PreviewIndex = 0;


        [SerializeField]
        private Texture2D importTexture2;
        private SerializedProperty importTexture2Prop;
        private int selectImportTexture2PreviewIndex = 0;

        [SerializeField]
        private Texture2D importTexture3;
        private SerializedProperty importTexture3Prop;
        private int selectImportTexture3PreviewIndex = 0;

        [SerializeField]
        private Texture2D importTexture4;
        private SerializedProperty importTexture4Prop;
        private int selectImportTexture4PreviewIndex = 0;

        private int selectConvertedTexturePreviewIndex = 0;

        private SerializedObject so;

        private Material importTexturePreviewMaterial;
        private Shader colorDivedShader;

        private Material convertedTexturePreviewMaterial;
        private Shader textureConvertShader;

        Texture2D convertedTexturePreview;
        private GenerateSizeKinds sizeKind = GenerateSizeKinds.Normal;
        private Vector2Int textureSize = new Vector2Int(1024, 1024);

        [MenuItem("AyahaGraphicDevelopTools/TextureConverter")]
        private static void ShowWindow()
        {
            var window = GetWindow<TextureConverterWindow>("TextureConverterWindow");
            window.titleContent = new GUIContent("TextureConverter");
        }

        private void OnEnable()
        {
            so = new SerializedObject(this);

            colorDivedShader = Shader.Find("Unlit/ColorDived");
            importTexturePreviewMaterial = new Material(colorDivedShader);

            importTexture1Prop = so.FindProperty("importTexture1");
            importTexture2Prop = so.FindProperty("importTexture2");
            importTexture3Prop = so.FindProperty("importTexture3");
            importTexture4Prop = so.FindProperty("importTexture4");

            textureConvertShader = Shader.Find("Unlit/TextureConvert");
            convertedTexturePreviewMaterial = new Material(textureConvertShader);
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {

                    DrawImportTexture(importTexture1Prop, importTexture1, ref selectImportTexture1PreviewIndex);
                    DrawImportTexture(importTexture2Prop, importTexture2, ref selectImportTexture2PreviewIndex);
                    DrawImportTexture(importTexture3Prop, importTexture3, ref selectImportTexture3PreviewIndex);
                    DrawImportTexture(importTexture4Prop, importTexture4, ref selectImportTexture4PreviewIndex);
                }

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                DrawConvertedTexture(ref selectConvertedTexturePreviewIndex);
            }

            so.ApplyModifiedProperties();
        }

        /// <summary>
        /// インポートしたTexuterを表示する
        /// </summary>
        /// <param name="textureProp">textureProp</param>
        /// <param name="importTexture">インポートしたTexuter</param>
        /// <param name="selectIndex">選択したプレビューモード</param>
        private void DrawImportTexture(SerializedProperty textureProp, Texture2D importTexture, ref int selectIndex)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.PropertyField(textureProp);
                if (textureProp != null)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        // マテリアルを設定する
                        importTexturePreviewMaterial.SetTexture("_MainTex", importTexture);
                        importTexturePreviewMaterial.SetInt("_DivedMode", selectIndex);

                        // テクスチャをプレビュー表示する
                        Texture2D importTexturePreview = GetPreviewTexture(importTexturePreviewMaterial, importTexture, ImportTexturePreviewSize, ImportTexturePreviewSize);
                        GUILayout.Label(importTexturePreview, GUILayout.Width(200), GUILayout.Height(200));

                        // RGBA、Allどの表示するのか設定する
                        using (new EditorGUILayout.VerticalScope())
                        {
                            selectIndex = GUILayout.SelectionGrid(selectIndex, previewKinds, 1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// コンバートした結果のTexuterを表示する
        /// </summary>
        /// <param name="selectIndex">選択したプレビューモード</param>
        private void DrawConvertedTexture(ref int selectIndex)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    // マテリアルを設定する
                    convertedTexturePreviewMaterial.SetTexture("_SourceTexture1", importTexture1);
                    convertedTexturePreviewMaterial.SetInt("_DivedMode1", selectImportTexture1PreviewIndex);

                    convertedTexturePreviewMaterial.SetTexture("_SourceTexture2", importTexture2);
                    convertedTexturePreviewMaterial.SetInt("_DivedMode2", selectImportTexture2PreviewIndex);

                    convertedTexturePreviewMaterial.SetTexture("_SourceTexture3", importTexture3);
                    convertedTexturePreviewMaterial.SetInt("_DivedMode3", selectImportTexture3PreviewIndex);

                    convertedTexturePreviewMaterial.SetTexture("_SourceTexture4", importTexture4);
                    convertedTexturePreviewMaterial.SetInt("_DivedMode4", selectImportTexture4PreviewIndex);

                    // テクスチャをプレビュー表示する
                    convertedTexturePreview = GetPreviewTexture(convertedTexturePreviewMaterial, Texture2D.whiteTexture, textureSize.x, textureSize.y);
                    int previewAll = 0;
                    if (selectIndex == previewAll)
                    {
                        GUILayout.Label(convertedTexturePreview, GUILayout.Width(200), GUILayout.Height(200));
                    }
                    else
                    {
                        // マテリアルを設定する
                        importTexturePreviewMaterial.SetTexture("_MainTex", convertedTexturePreview);
                        importTexturePreviewMaterial.SetInt("_DivedMode", selectIndex);

                        var convertedTextureDivedPreview = GetPreviewTexture(importTexturePreviewMaterial, null, ImportTexturePreviewSize, ImportTexturePreviewSize);
                        GUILayout.Label(convertedTextureDivedPreview, GUILayout.Width(200), GUILayout.Height(200));
                    }

                    // RGBA、Allどの表示するのか設定する
                    using (new EditorGUILayout.VerticalScope())
                    {
                        selectIndex = GUILayout.SelectionGrid(selectIndex, previewKinds, 1);
                    }
                    GUILayout.FlexibleSpace();
                }

                DrawGenerateTextureSize();
                if (GUILayout.Button("保存"))
                {
                    DrawSaveTexture(convertedTexturePreview);
                }
            }
        }


        /// <summary>
        /// マテリアルからTextureプレビューのテクスチャを作成する
        /// </summary>
        /// <param name="material">マテリアル</param>
        /// <param name="importTexture">インポートしたTexuter</param>
        /// <param name="width">生成するテクスチャのサイズ</param>
        /// <param name="height">生成するテクスチャのサイズ</param>
        private Texture2D GetPreviewTexture(Material material, Texture2D importTexture, int width, int height)
        {
            width = Mathf.Max(width, 2);
            height = Mathf.Max(height, 2);

            // RenderTextureの作成
            RenderTexture renderTexture = RenderTexture.GetTemporary(width, height);

            // 旧レンダリングターゲットの保存
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTexture;

            // RenderTextureにテクスチャを描画
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, width, height, 0);
            Graphics.Blit(importTexture, renderTexture, material);
            GL.PopMatrix();

            // Texture2Dにコピー
            Texture2D previewTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            previewTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            previewTexture.Apply();

            // 元のレンダリングターゲットに戻す
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTexture);

            return previewTexture;
        }

        /// <summary>
        /// テクスチャの生成サイズ
        /// </summary>
        private void DrawGenerateTextureSize()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                using (new EditorGUILayout.VerticalScope())
                {
                    sizeKind = (GenerateSizeKinds)EditorGUILayout.EnumPopup("テクスチャサイズ", sizeKind);

                    // カスタムサイズを選択時は専用の入力エリアを出す
                    if (sizeKind == GenerateSizeKinds.Custom)
                    {
                        textureSize = EditorGUILayout.Vector2IntField("カスタムサイズ", textureSize);
                        textureSize = Vector2Int.Max(textureSize, new Vector2Int(2, 2));
                    }
                    else
                    {
                        textureSize.x = (int)sizeKind;
                        textureSize.y = (int)sizeKind;
                    }
                }
                GUILayout.FlexibleSpace();
            }
        }

        /// <summary>
        /// テクスチャを保存する
        /// </summary>
        /// <param name="texture">生成したテクスチャ</param>
        private void DrawSaveTexture(Texture2D texture)
        {
            var path = EditorUtility.SaveFilePanel("名前を付けて保存", "", "Texture", "png");
            if (string.IsNullOrEmpty(path))
            {
                // ユーザーがキャンセルした場合
                return;
            }

            // テクスチャデータをPNGにエンコード
            byte[] pngData = texture.EncodeToPNG();

            if (pngData != null)
            {
                File.WriteAllBytes(path, pngData);
                Debug.Log("テクスチャを保存: " + path);
            }
            else
            {
                Debug.LogError("PNGへのエンコードに失敗");
            }
        }
    }
}
