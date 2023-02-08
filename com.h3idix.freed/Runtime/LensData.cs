using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace h3idiX
{
    [CreateAssetMenu(menuName = "FreeD module/LensData")]
    internal class LensData : ScriptableObject
    {
        [SerializeField] internal List<LensItemData> lensFocalData;
        [SerializeField] internal List<LensItemData> lensFocusData;
        [SerializeField] internal List<LensOffsetItem> lensOffsetData;
        [SerializeField] internal AnimationCurve focalLengthCurve;
        [SerializeField] internal AnimationCurve focusDistanceCurve;
        [SerializeField] internal AnimationCurve[] lensOffsetCurves = new AnimationCurve[3];
        public void Reset()
        {
            lensFocalData = new()
            {
                new() { input =1024, length = 24.0f, isActive = false },
                new() { input =2048, length = 35.0f, isActive = true },

            };
            lensFocusData = new()
            {
                new() { input =100, length = 10.0f, isActive = false },
                new() { input =200, length = 20.0f, isActive = true },
                new() { input =300, length = 30.0f, isActive = true },
            };
            lensOffsetData = new List<LensOffsetItem>()
            {
                new LensOffsetItem() {input = 0, offset = Vector3.zero, isActive = true}
            };

        }


        
#if UNITY_EDITOR
        internal static LensData SaveAsset(LensData lensData, bool createAsset)
        {
            const string kDefaultAssetPath = "assets/FreeD_LensData";
            if (createAsset)
            {
                lensData = ScriptableObject.CreateInstance<LensData>();
                lensData.lensFocalData = new List<LensItemData>();
                lensData.lensFocusData = new List<LensItemData>();
                lensData.lensOffsetData = new List<LensOffsetItem>();
            }


            var savePath = kDefaultAssetPath;
            savePath = EditorUtility.SaveFilePanelInProject("Choose file to save:",
                System.IO.Path.GetFileName(savePath), "asset", "Choose file to save.",
                System.IO.Path.GetDirectoryName(savePath));
            if (string.IsNullOrEmpty(savePath))
            {
                return lensData;
            }

            if (createAsset)
            {
                AssetDatabase.CreateAsset(lensData, savePath);
            }

            AssetDatabase.SaveAssetIfDirty(lensData);
            AssetDatabase.Refresh();

            return lensData;
        }
#endif //UNITY_EDITOR
    }
}