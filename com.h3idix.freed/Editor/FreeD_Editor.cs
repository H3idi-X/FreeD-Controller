using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine.UIElements;

namespace h3idiX
{
    [CustomEditor(typeof(FreeD_Controller))]
    public class FreeD_Editor : Editor
    {
        // connection indicator
        private GUIContent activeIcon;
        private GUIContent deactiveIcon;
        private Image indicatoerImage;
        // Network
        private IntegerField portIntegerField;
        // new button
        private Button calibrationDataButton;
        // no asset is set
        private Label errorLabel;
        
        // camera is not physical
        private Label physicalCameraLabel;
        // position vector3 field
        private Toggle positionCheckBox;
        private Vector3Field positionField;
        private Vector3Field offsetField;
        
        // focal length
        private Toggle flocalLengthCheckBox;
        private FloatField facalLengthField;
        // focus dstance
        private Toggle focusDistanceCheckBox;
        private FloatField focusDsitanceField;
        
        IVisualElementScheduledItem scheculedAction;
        public override VisualElement CreateInspectorGUI()
        {

            var controller = target as FreeD_Controller;
            Debug.Assert(controller != null);
            
            if (activeIcon == null)
            {
                activeIcon = EditorGUIUtility.IconContent( "d_DataMode.Mixed" );     
            }

            if (deactiveIcon == null)
            {
                deactiveIcon = EditorGUIUtility.IconContent( "d_DataMode.Authoring" );            
            }

        
            VisualElement myInspector = new VisualElement();
        
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.h3idix.freed/Editor/FreeD_ControllerInspector.uxml");
            visualTree.CloneTree(myInspector);
            indicatoerImage = myInspector.Q<Image>("Indicator");
            Debug.Assert(indicatoerImage != null);

            calibrationDataButton = myInspector.Q<Button>("CalibrationDataButton");
            Debug.Assert(calibrationDataButton != null);
            calibrationDataButton.clickable.clicked += () =>
            {
                LensData lensData =  LensData.SaveAsset(null, true);
                controller.lensData = lensData;
            };
            // network
            portIntegerField = myInspector.Q<IntegerField>("LocalPortIntegerField");
            Debug.Assert(portIntegerField != null);
            portIntegerField.RegisterValueChangedCallback(arg =>
            {
                controller.UpdatePortNumber();
            });
            
            positionCheckBox = myInspector.Q<Toggle>("PositionEnabled");
            Debug.Assert(positionCheckBox != null);
            positionField = myInspector.Q<Vector3Field>("PositionVector3Field");
            Debug.Assert(positionField != null);
            offsetField = myInspector.Q<Vector3Field>("PositionOffset");
            Debug.Assert(offsetField != null);
            positionField.SetEnabled(positionCheckBox.value);
            offsetField.SetEnabled(positionCheckBox.value);
            positionCheckBox.RegisterValueChangedCallback(arg =>
            {                    
                positionField.SetEnabled(arg.newValue);
                offsetField.SetEnabled(arg.newValue);
            });
            
            // focal length
            flocalLengthCheckBox  = myInspector.Q<Toggle>("FocalLengthEnabled");
            Debug.Assert(flocalLengthCheckBox != null);
            facalLengthField = myInspector.Q<FloatField>("FocalLengthField");
            Debug.Assert(facalLengthField != null);
            facalLengthField.SetEnabled(flocalLengthCheckBox.value);
            flocalLengthCheckBox.RegisterValueChangedCallback(arg =>
            {                    
                facalLengthField.SetEnabled(arg.newValue);
            });
            
            // focus distance
            focusDistanceCheckBox  = myInspector.Q<Toggle>("FocusDistanceEnabled");
            Debug.Assert(focusDistanceCheckBox != null);
            focusDsitanceField = myInspector.Q<FloatField>("FocusDistanceField");
            Debug.Assert(facalLengthField != null);
            focusDsitanceField.SetEnabled(focusDistanceCheckBox.value);
            focusDistanceCheckBox.RegisterValueChangedCallback(arg =>
            {                    
                focusDsitanceField.SetEnabled(arg.newValue);
            });

            

            physicalCameraLabel = myInspector.Q<Label>("LabelPhysicalCamera");
            Debug.Assert(physicalCameraLabel != null);
            
            
            errorLabel =  myInspector.Q<Label>("LabelNoLensDataAssigned");
            Debug.Assert(errorLabel != null);
            
            indicatoerImage.image = activeIcon.image;
            scheculedAction = indicatoerImage.schedule.Execute(() =>
            {
                if (controller.isConnected)
                {
                    indicatoerImage.image =
                        ((indicatoerImage.image == activeIcon.image) ? deactiveIcon.image : activeIcon.image);
                }
                else
                {
                    indicatoerImage.image = deactiveIcon.image;
                }

                if (controller.lensData != null)
                {
                    errorLabel.visible = false;
                }
                else
                {
                    errorLabel.visible = true;            
                }
                if (controller.targetCamera == null || controller.targetCamera.usePhysicalProperties == false)
                {
                    physicalCameraLabel.visible = true;      

                }
                else
                {
                    physicalCameraLabel.visible = false;    
                }
                
            });
            scheculedAction.Every(300); // ms   // TODO. when destroyed?
            return myInspector;
        }
        

        static void UpdateButton(Button button, Object obj, int buttonWidth) {
            if ( obj == null )
            {
                button.text = "New";
            }
            else {
                button.text = "Edit";
            }

            button.style.width = buttonWidth;
        }
    }


    
}
