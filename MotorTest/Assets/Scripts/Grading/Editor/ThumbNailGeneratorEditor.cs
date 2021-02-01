using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

[CustomEditor(typeof(ThumbNailGenerator))]
[CanEditMultipleObjects]
public class ThumbNailGeneratorEditor : Editor
{
    ThumbNailGenerator m_ThumbNailSC;
    [SerializeField]
    string PicturePath;
    private void OnEnable()
    {
        m_ThumbNailSC = (ThumbNailGenerator)target;

    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Select Path"))
        {
            SelectPath();
        }
        if (GUILayout.Button("Generate ThumbNails"))
        {
            GenerateThumbNail();
        }
    }
    void SelectPath()
    {
        PicturePath = EditorUtility.OpenFolderPanel("Select Folder","","");
    }
    private void GenerateThumbNail()
    {
        AssetDatabase.Refresh();
        foreach(Transform model in m_ThumbNailSC.MotorModel.transform)
        {
            byte[] ImgBytes;
            if(model.GetComponent<MeshRenderer>() != null)
            {
                Texture2D TempTexture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
                TempTexture = AssetPreview.GetAssetPreview(model.gameObject);

                string fileName = model.name + "_Thumbnail";

                // Commented Code is for alpha layer but currently not working..... NEEDS FIIIXEES IF THERE IS TIME !!!!
                //Color32 c = new Color32(1, 0, 0, 1);
                //if(TempTexture != null)
                //{
                //    Color32[] colors = TempTexture.GetPixels32();
                //    TempTexture.alphaIsTransparency = true;
                //    for (int i = 0; i < colors.Length; i++)
                //    {
                //        //if (colors[i].b == (byte)(1f) || colors[i].g == (byte)(1f) || colors[i].r == (byte)(1f))
                //            colors[i].a = (byte)(1f/i*colors.Length);
                //    }
                //    TempTexture.SetPixels32(colors);
                //    TempTexture.Apply();
                //    Debug.Log("Alpha nostradaa");
                //}

                ImgBytes = TempTexture.EncodeToPNG();
                if (PicturePath != null && TempTexture != null)
                {
                    File.WriteAllBytes(PicturePath + "/" + fileName + ".png", ImgBytes);

                }
                else
                {
                    Debug.Log("No Folder Path");
                }

                    //System.IO.Path.Combine(,fileName + ".png")
                    //, TempTexture.EncodeToPNG());
            }

        }
        AssetDatabase.Refresh();
    }
}