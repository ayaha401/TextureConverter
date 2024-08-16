using UnityEngine;
using UnityEditor;
using System.IO;

namespace AyahaGraphicDevelopTools.TextureConverter
{
    public class TextureConverterWindow : EditorWindow
    {
        /// <summary>
        /// ��������e�N�X�`���̃T�C�Y�ݒ�
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
        /// �C���|�[�g����Texuter��\������
        /// </summary>
        /// <param name="textureProp">textureProp</param>
        /// <param name="importTexture">�C���|�[�g����Texuter</param>
        /// <param name="selectIndex">�I�������v���r���[���[�h</param>
        private void DrawImportTexture(SerializedProperty textureProp, Texture2D importTexture, ref int selectIndex)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.PropertyField(textureProp);
                if (textureProp != null)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        // �}�e���A����ݒ肷��
                        importTexturePreviewMaterial.SetTexture("_MainTex", importTexture);
                        importTexturePreviewMaterial.SetInt("_DivedMode", selectIndex);

                        // �e�N�X�`�����v���r���[�\������
                        Texture2D importTexturePreview = GetPreviewTexture(importTexturePreviewMaterial, importTexture, ImportTexturePreviewSize, ImportTexturePreviewSize);
                        GUILayout.Label(importTexturePreview, GUILayout.Width(200), GUILayout.Height(200));

                        // RGBA�AAll�ǂ̕\������̂��ݒ肷��
                        using (new EditorGUILayout.VerticalScope())
                        {
                            selectIndex = GUILayout.SelectionGrid(selectIndex, previewKinds, 1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// �R���o�[�g�������ʂ�Texuter��\������
        /// </summary>
        /// <param name="selectIndex">�I�������v���r���[���[�h</param>
        private void DrawConvertedTexture(ref int selectIndex)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    // �}�e���A����ݒ肷��
                    convertedTexturePreviewMaterial.SetTexture("_SourceTexture1", importTexture1);
                    convertedTexturePreviewMaterial.SetInt("_DivedMode1", selectImportTexture1PreviewIndex);

                    convertedTexturePreviewMaterial.SetTexture("_SourceTexture2", importTexture2);
                    convertedTexturePreviewMaterial.SetInt("_DivedMode2", selectImportTexture2PreviewIndex);

                    convertedTexturePreviewMaterial.SetTexture("_SourceTexture3", importTexture3);
                    convertedTexturePreviewMaterial.SetInt("_DivedMode3", selectImportTexture3PreviewIndex);

                    convertedTexturePreviewMaterial.SetTexture("_SourceTexture4", importTexture4);
                    convertedTexturePreviewMaterial.SetInt("_DivedMode4", selectImportTexture4PreviewIndex);

                    // �e�N�X�`�����v���r���[�\������
                    convertedTexturePreview = GetPreviewTexture(convertedTexturePreviewMaterial, Texture2D.whiteTexture, textureSize.x, textureSize.y);
                    int previewAll = 0;
                    if (selectIndex == previewAll)
                    {
                        GUILayout.Label(convertedTexturePreview, GUILayout.Width(200), GUILayout.Height(200));
                    }
                    else
                    {
                        // �}�e���A����ݒ肷��
                        importTexturePreviewMaterial.SetTexture("_MainTex", convertedTexturePreview);
                        importTexturePreviewMaterial.SetInt("_DivedMode", selectIndex);

                        var convertedTextureDivedPreview = GetPreviewTexture(importTexturePreviewMaterial, null, ImportTexturePreviewSize, ImportTexturePreviewSize);
                        GUILayout.Label(convertedTextureDivedPreview, GUILayout.Width(200), GUILayout.Height(200));
                    }

                    // RGBA�AAll�ǂ̕\������̂��ݒ肷��
                    using (new EditorGUILayout.VerticalScope())
                    {
                        selectIndex = GUILayout.SelectionGrid(selectIndex, previewKinds, 1);
                    }
                    GUILayout.FlexibleSpace();
                }

                DrawGenerateTextureSize();
                if (GUILayout.Button("�ۑ�"))
                {
                    DrawSaveTexture(convertedTexturePreview);
                }
            }
        }


        /// <summary>
        /// �}�e���A������Texture�v���r���[�̃e�N�X�`�����쐬����
        /// </summary>
        /// <param name="material">�}�e���A��</param>
        /// <param name="importTexture">�C���|�[�g����Texuter</param>
        /// <param name="width">��������e�N�X�`���̃T�C�Y</param>
        /// <param name="height">��������e�N�X�`���̃T�C�Y</param>
        private Texture2D GetPreviewTexture(Material material, Texture2D importTexture, int width, int height)
        {
            width = Mathf.Max(width, 2);
            height = Mathf.Max(height, 2);

            // RenderTexture�̍쐬
            RenderTexture renderTexture = RenderTexture.GetTemporary(width, height);

            // �������_�����O�^�[�Q�b�g�̕ۑ�
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTexture;

            // RenderTexture�Ƀe�N�X�`����`��
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, width, height, 0);
            Graphics.Blit(importTexture, renderTexture, material);
            GL.PopMatrix();

            // Texture2D�ɃR�s�[
            Texture2D previewTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            previewTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            previewTexture.Apply();

            // ���̃����_�����O�^�[�Q�b�g�ɖ߂�
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTexture);

            return previewTexture;
        }

        /// <summary>
        /// �e�N�X�`���̐����T�C�Y
        /// </summary>
        private void DrawGenerateTextureSize()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                using (new EditorGUILayout.VerticalScope())
                {
                    sizeKind = (GenerateSizeKinds)EditorGUILayout.EnumPopup("�e�N�X�`���T�C�Y", sizeKind);

                    // �J�X�^���T�C�Y��I�����͐�p�̓��̓G���A���o��
                    if (sizeKind == GenerateSizeKinds.Custom)
                    {
                        textureSize = EditorGUILayout.Vector2IntField("�J�X�^���T�C�Y", textureSize);
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
        /// �e�N�X�`����ۑ�����
        /// </summary>
        /// <param name="texture">���������e�N�X�`��</param>
        private void DrawSaveTexture(Texture2D texture)
        {
            var path = EditorUtility.SaveFilePanel("���O��t���ĕۑ�", "", "Texture", "png");
            if (string.IsNullOrEmpty(path))
            {
                // ���[�U�[���L�����Z�������ꍇ
                return;
            }

            // �e�N�X�`���f�[�^��PNG�ɃG���R�[�h
            byte[] pngData = texture.EncodeToPNG();

            if (pngData != null)
            {
                File.WriteAllBytes(path, pngData);
                Debug.Log("�e�N�X�`����ۑ�: " + path);
            }
            else
            {
                Debug.LogError("PNG�ւ̃G���R�[�h�Ɏ��s");
            }
        }
    }
}
