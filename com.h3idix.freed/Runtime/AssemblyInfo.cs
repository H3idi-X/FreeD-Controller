#if UNITY_EDITOR
using System.Runtime.CompilerServices;


[assembly: InternalsVisibleTo("com.h3idiX.freeD.editor")] 
[assembly: InternalsVisibleTo("Unity.UIElements")]
[assembly: InternalsVisibleTo("Unity.UIElements.Editor")]
[assembly: InternalsVisibleTo("UnityEditor.UIElementsModule")]
[assembly: InternalsVisibleTo("UnityEditor.UIElementsGameObjectsModule")]
#endif