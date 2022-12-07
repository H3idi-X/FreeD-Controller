using UnityEngine;
using UnityEditor;
using System.Collections;

namespace h3idiX
{
    [CustomEditor(typeof(FreeD_Controller))]
    public class FreeD_Editor : Editor
    {

        public override void OnInspectorGUI()
        {
            EditorUtility.SetDirty(target); // to update inspector 
            FreeD_Controller controller = target as FreeD_Controller;

            float orgLabelWidth = EditorGUIUtility.labelWidth;
            float orgFieldWiddth = EditorGUIUtility.fieldWidth;
            Misc(controller);
            EditorGUIUtility.labelWidth = orgLabelWidth;
            EditorGUIUtility.fieldWidth = orgFieldWiddth;
            Yaw(controller);
            EditorGUIUtility.labelWidth = orgLabelWidth;
            EditorGUIUtility.fieldWidth = orgFieldWiddth;
            Pitch(controller);
            EditorGUIUtility.labelWidth = orgLabelWidth;
            EditorGUIUtility.fieldWidth = orgFieldWiddth;
            Roll(controller);
            EditorGUIUtility.labelWidth = orgLabelWidth;
            EditorGUIUtility.fieldWidth = orgFieldWiddth;
            PosX(controller);
            EditorGUIUtility.labelWidth = orgLabelWidth;
            EditorGUIUtility.fieldWidth = orgFieldWiddth;
            PosY(controller);
            EditorGUIUtility.labelWidth = orgLabelWidth;
            EditorGUIUtility.fieldWidth = orgFieldWiddth;
            PosZ(controller);
            EditorGUIUtility.labelWidth = orgLabelWidth;
            EditorGUIUtility.fieldWidth = orgFieldWiddth;
            // Focal Distance.
            FocalLength(controller);
            EditorGUIUtility.labelWidth = orgLabelWidth;
            EditorGUIUtility.fieldWidth = orgFieldWiddth;
            // Focus Distance.
            FocusDistance(controller);
            EditorGUIUtility.labelWidth = orgLabelWidth;
            EditorGUIUtility.fieldWidth = orgFieldWiddth;



        }

        private void Misc(FreeD_Controller controller)
        {
            controller.LocalPort = EditorGUILayout.IntField("Local Port", controller.LocalPort);
            EditorGUILayout.IntField("Vendor ID", controller.vendorID);
            EditorGUILayout.IntField("Camera ID", controller.cameraID);

        }

        private void Yaw(FreeD_Controller controller)
        {
            controller.useYaw = EditorGUILayout.BeginToggleGroup("Yaw", controller.useYaw);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;
            var val = controller.yaw / controller.yawDivider;
            EditorGUILayout.FloatField(val.ToString(), controller.yaw);
            controller.yawDivider = EditorGUILayout.FloatField("/", controller.yawDivider);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndToggleGroup();

        }

        private void Pitch(FreeD_Controller controller)
        {
            controller.usePitch = EditorGUILayout.BeginToggleGroup("Pitch", controller.usePitch);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;
            var val = controller.pitch / controller.pitchDivider;
            EditorGUILayout.FloatField(val.ToString(), controller.pitch);
            controller.pitchDivider = EditorGUILayout.FloatField("/", controller.pitchDivider);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndToggleGroup();

        }

        private void PosX(FreeD_Controller controller)
        {
            controller.useXpos = EditorGUILayout.BeginToggleGroup("X Pos", controller.useXpos);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;
            var val = controller.posX / controller.posXDivider;
            EditorGUILayout.FloatField(val.ToString(), controller.posX);
            controller.posXDivider = EditorGUILayout.FloatField("/", controller.posXDivider);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndToggleGroup();
        }

        private void PosY(FreeD_Controller controller)
        {
            controller.useYpos = EditorGUILayout.BeginToggleGroup("Y Pos", controller.useYpos);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;
            var val = controller.posY / controller.posYDivider;
            EditorGUILayout.FloatField(val.ToString(), controller.posY);
            controller.posYDivider = EditorGUILayout.FloatField("/", controller.posYDivider);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndToggleGroup();
        }

        private void PosZ(FreeD_Controller controller)
        {
            controller.useZpos = EditorGUILayout.BeginToggleGroup("Z Pos", controller.useZpos);

            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;
            var val = controller.posZ / controller.posZDivider;
            EditorGUILayout.FloatField(val.ToString(), controller.posZ);
            controller.posZDivider = EditorGUILayout.FloatField("/", controller.posZDivider);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndToggleGroup();
        }

        private void Roll(FreeD_Controller controller)
        {
            controller.useRoll = EditorGUILayout.BeginToggleGroup("Roll", controller.useRoll);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;
            var val = controller.roll / controller.rollDivider;
            EditorGUILayout.FloatField(val.ToString(), controller.roll);
            controller.rollDivider = EditorGUILayout.FloatField("/", controller.rollDivider);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndToggleGroup();
        }

        private void FocusDistance(FreeD_Controller controller)
        {
            controller.useFocusDistance =
                EditorGUILayout.BeginToggleGroup("Focus Distance", controller.useFocusDistance);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;

            controller.minFocusDistance = EditorGUILayout.FloatField(controller.targetCamera.focusDistance.ToString(),
                controller.minFocusDistance);
            EditorGUIUtility.labelWidth = 48;
            controller.maxFocusDistance =
                EditorGUILayout.FloatField("+ ((", controller.maxFocusDistance, GUILayout.Width(80));

            EditorGUILayout.LabelField("- " + controller.minFocusDistance + " ) *", GUILayout.Width(56));
            EditorGUIUtility.labelWidth = 24;
            EditorGUILayout.IntField((int)controller.focus, GUILayout.Width(72));

            controller.focusDistanceDivider =
                EditorGUILayout.FloatField("/", controller.focusDistanceDivider, GUILayout.Width(96));
            EditorGUILayout.LabelField(")");
            EditorGUI.indentLevel--;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndToggleGroup();
        }

        private void FocalLength(FreeD_Controller controller)
        {
            controller.useFocalLength = EditorGUILayout.BeginToggleGroup("Focal Distance", controller.useFocalLength);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;

            controller.minFocalLength = EditorGUILayout.FloatField(controller.targetCamera.focalLength.ToString(),
                controller.minFocalLength);
            EditorGUIUtility.labelWidth = 48;
            controller.maxFocalLength =
                EditorGUILayout.FloatField("+ ((", controller.maxFocalLength, GUILayout.Width(80));

            EditorGUILayout.LabelField("- " + controller.minFocalLength + " ) *", GUILayout.Width(56));
            EditorGUIUtility.labelWidth = 24;
            EditorGUILayout.IntField((int)controller.zoom, GUILayout.Width(72));

            controller.focalLengthDivider =
                EditorGUILayout.FloatField("/", controller.focalLengthDivider, GUILayout.Width(96));
            EditorGUILayout.LabelField(")");

            EditorGUI.indentLevel--;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndToggleGroup();

        }
    }
}
