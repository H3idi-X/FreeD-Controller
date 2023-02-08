using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using UnityEngine;


namespace h3idiX
{
    [CustomEditor(typeof(LensData))]
    internal class LensDataEditor : Editor
    {

        public VisualTreeAsset editorWindowAsset;
        public VisualTreeAsset itemAssetFocalLength;
        public VisualTreeAsset itemAssetFocusDistance;
        public VisualTreeAsset itemAssetOffset;
        
        public List<int> focalLengthItemsSelected = new List<int>();
        public List<int> focusDistanceItemsSelected = new List<int>();
        public List<int> offsetItemsSelected = new List<int>();

        public override VisualElement CreateInspectorGUI()
        {
            var lensData = target as LensData;
            
            var root = editorWindowAsset.CloneTree();

            if (lensData == null)
            {
                Debug.Log("Lens Data is null"); // can't happen.
                return root;
            }
            // ---------------------------------------------------
            var focalLengthCurveField = root.Q<CurveField>("FocalLengthCurve");          
            var listView1 = root.Q<ListView>("FocalLengthListView");
            listView1.makeItem = () => 
                {
                    var item = itemAssetFocalLength.CloneTree();
                    
                    item.Q<IntegerField>().RegisterValueChangedCallback(evt => {lensData.focalLengthCurve =  UpdateCurve(lensData.lensFocalData, focalLengthCurveField);});
                    item.Q<FloatField>().RegisterValueChangedCallback(evt => {lensData.focalLengthCurve =  UpdateCurve(lensData.lensFocalData, focalLengthCurveField); });
                    item.Q<Toggle>().RegisterValueChangedCallback(evt => {lensData.focalLengthCurve =  UpdateCurve(lensData.lensFocalData, focalLengthCurveField); });

                    return item;
                };
            listView1.itemsAdded += (ints) =>
                {
                    lensData.focalLengthCurve =  UpdateCurve(lensData.lensFocalData, focalLengthCurveField);
                };
            listView1.itemsRemoved += (ints) =>
                {
                    lensData.focalLengthCurve =  UpdateCurve(lensData.lensFocalData, focalLengthCurveField);
                };
            listView1.selectedIndicesChanged += (ints) =>
                {
                    List<int> focalLengthItemsSelectedBefore = focalLengthItemsSelected;
                    foreach (var unselected in focalLengthItemsSelectedBefore)
                    {

                    }
                    focalLengthItemsSelected = new List<int>();
                    foreach (var selected in ints)
                    {
                        focalLengthItemsSelected.Add(selected);
                    }
                    lensData.focalLengthCurve =  UpdateCurve(lensData.lensFocalData, focalLengthCurveField);
                };
                
            // ---------------------------------------------------
            var focusDistanceCurveField = root.Q<CurveField>("FocusDistanceCurve");
            var listView2 = root.Q<ListView>("FocusDistanceListView");
            listView2.makeItem = () => 
                {
                    var item = itemAssetFocusDistance.CloneTree();
                    item.Q<IntegerField>().RegisterValueChangedCallback(evt => {lensData.focusDistanceCurve = UpdateCurve(lensData.lensFocusData, focusDistanceCurveField);});
                    item.Q<FloatField>().RegisterValueChangedCallback(evt => {lensData.focusDistanceCurve = UpdateCurve(lensData.lensFocusData, focusDistanceCurveField); });

                    item.Q<Toggle>().RegisterValueChangedCallback(evt => {lensData.focusDistanceCurve = UpdateCurve(lensData.lensFocusData, focusDistanceCurveField); });
                    return item;
                };
            listView2.itemsAdded += (ints) =>
                {
                    lensData.focusDistanceCurve = UpdateCurve(lensData.lensFocusData, focusDistanceCurveField);
                };
            listView2.itemsRemoved += (ints) =>
                {
                    lensData.focusDistanceCurve = UpdateCurve(lensData.lensFocusData, focusDistanceCurveField);
                };
            listView2.selectedIndicesChanged += (ints) =>
                {
                    List<int> focusDistanceItemsBefore= focusDistanceItemsSelected;
                    foreach (var unselected in focusDistanceItemsBefore)
                    {

                    }
                    focusDistanceItemsSelected = new List<int>();
                    foreach (var selected in ints)
                    {
                        focusDistanceItemsSelected.Add(selected);
                        
                    }
                    lensData.focusDistanceCurve = UpdateCurve(lensData.lensFocusData, focusDistanceCurveField);
                };

            // ---------------------------------------------------
            var listView3 = root.Q<ListView>("OffsetListView");
            listView3.makeItem = () => 
            {
                var item = itemAssetOffset.CloneTree();
                item.Q<IntegerField>().RegisterValueChangedCallback(evt => { UpdateCurve3(lensData);});
                item.Q<FloatField>().RegisterValueChangedCallback(evt => { UpdateCurve3(lensData);});

                item.Q<Toggle>().RegisterValueChangedCallback(evt => { UpdateCurve3(lensData); });
                return item;
            };
            listView3.itemsAdded += (ints) =>
            {
                UpdateCurve3(lensData);
            };
            listView3.itemsRemoved += (ints) =>
            {
                UpdateCurve3(lensData);
            };
            listView3.selectedIndicesChanged += (ints) =>
            {
                List<int> offsetItemsSelectedBefore = offsetItemsSelected;
                foreach (var unselected in offsetItemsSelectedBefore)
                {

                }
                offsetItemsSelected = new List<int>();
                foreach (var selected in ints)
                {
                    offsetItemsSelected.Add(selected);
                        
                }
                UpdateCurve3(lensData);
            };            
            
            return root;
        }

        void UpdateCurve3(LensData lensData)
        {
            int numKeys = 0;
            for (int ii = 0; ii < lensData.lensOffsetData.Count; ii++)
            {
                if (lensData.lensOffsetData[ii].isActive)
                {
                    numKeys++;
                }
            }
            for (int ii = 0; ii < 3; ii++)
            {
                var ks = new Keyframe[numKeys];
                float valEnd = 0.0f;
                float valStart = 4096.0f;
                int keyIndex = 0;
                for (var dataIndex = 0; dataIndex < numKeys; dataIndex++)
                {
                    if (!lensData.lensOffsetData[dataIndex].isActive)
                    {
                        continue;
                    }
                    float val = (float)lensData.lensOffsetData[dataIndex].input ;
                    if (valEnd < val)
                        valEnd = val;
                    if (valStart > val)
                    {
                        valStart = val;
                    }
                    var offset = lensData.lensOffsetData[dataIndex].offset;
                    ks[keyIndex] = new Keyframe(val, offset[ii]);
                    keyIndex++;
                }
                var curve = new AnimationCurve();
                for (var i = 0; i < ks.Length; i++)
                {

                    curve.AddKey(ks[i]);
                }

                for (var i = 0; i < curve.keys.Length; i++)
                {
                    AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
                    AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
                }
 
                lensData.lensOffsetCurves[ii] = curve;
            }

        }

        AnimationCurve UpdateCurve(List<LensItemData> lensItemData, CurveField curveField)
        {

            int numKeys = 0;
            for (int ii = 0; ii < lensItemData.Count; ii++)
            {
                if (lensItemData[ii].isActive)
                {
                    numKeys++;
                }
            }

            if (numKeys == 0)
            {
                curveField.value = null;
                return null;
            }
            
            var ks = new Keyframe[numKeys];
            float valEnd = 0.0f;
            float valStart = 4096.0f;
            int keyIndex = 0;
            for (var dataIndex = 0; dataIndex < numKeys; dataIndex++)
            {
                if (!lensItemData[dataIndex].isActive)
                {
                    continue;
                }
                float val = (float)lensItemData[dataIndex].input ;
                if (valEnd < val)
                    valEnd = val;
                if (valStart > val)
                {
                    valStart = val;
                }
                var focalLength = lensItemData[dataIndex].length;
                ks[keyIndex] = new Keyframe(val, focalLength);
                keyIndex++;
            }

            AnimationCurve curve = new AnimationCurve(ks);
            /*
            AnimationCurve curve = AnimationCurve.Linear(valStart, 0f, valEnd, 0f);
            if (ks.Length > 0)
            {
                curve = AnimationCurve.Linear(valStart, 0f, valEnd, lensData.lensFocalData[ks.Length-1].length);
            }
            */
            for (var i = 0; i < ks.Length; i++)
            {

                curve.AddKey(ks[i]);
            }

            for (var i = 0; i < curve.keys.Length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
                AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
            }
            
 
            curveField.value = curve;
            return curve;
        }
    
        
    }
}