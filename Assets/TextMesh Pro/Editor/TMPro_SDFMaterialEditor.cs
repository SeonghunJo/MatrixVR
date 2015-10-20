// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using TMPro;
using TMPro.EditorUtilities;



public class TMPro_SDFMaterialEditor : MaterialEditor
{
    private struct m_foldout
    { // Track Inspector foldout panel states, globally.
        public static bool face = true;
        public static bool outline = true;
        public static bool underlay = false;
        public static bool bevel = false;
        public static bool light = false;
        public static bool bump = false;
        public static bool env = false;
        public static bool glow = false;
        public static bool debug = false;
    }

    private enum FoldoutType { face, outline, underlay, bevel, light, bump, env, glow, debug };

    //private static PropertyModification m_modifiedProperties;
    private static int m_eventID;


    // Face Properties
    private MaterialProperty m_faceColor;
    private MaterialProperty m_faceTex;
    private MaterialProperty m_faceDilate;
    private MaterialProperty m_faceShininess;
    //private MaterialProperty m_faceSoftness;

    // Outline Properties
    private MaterialProperty m_outlineColor;
    private MaterialProperty m_outlineTex;
    private MaterialProperty m_outlineThickness;
    private MaterialProperty m_outlineSoftness;
    private MaterialProperty m_outlineShininess;

    // Properties Related to Bevel Options
    private MaterialProperty m_bevel;
    private MaterialProperty m_bevelOffset;
    private MaterialProperty m_bevelWidth;
    private MaterialProperty m_bevelClamp;
    private MaterialProperty m_bevelRoundness;

    // Properties for the Underlay Options
    private MaterialProperty m_underlayColor;
    private MaterialProperty m_underlayOffsetX;
    private MaterialProperty m_underlayOffsetY;
    private MaterialProperty m_underlayDilate;
    private MaterialProperty m_underlaySoftness;

    // Properties for Simulated Lighting
    private MaterialProperty m_lightAngle;
    private MaterialProperty m_specularColor;
    private MaterialProperty m_specularPower;
    private MaterialProperty m_reflectivity;
    private MaterialProperty m_diffuse;
    private MaterialProperty m_ambientLight;


    // Bump Mapping Options
    private MaterialProperty m_bumpMap;
    private MaterialProperty m_bumpFace;
    private MaterialProperty m_bumpOutline;

    // Properties for Environmental Mapping 
    private MaterialProperty m_reflectColor;
    private MaterialProperty m_reflectTex;
    private MaterialProperty m_envTiltX;
    private MaterialProperty m_envTiltY;

    private MaterialProperty m_specColor;

    // Properties for Glow Options
    private MaterialProperty m_glowColor;
    private MaterialProperty m_glowInner;
    private MaterialProperty m_glowOffset;
    private MaterialProperty m_glowPower;
    private MaterialProperty m_glowOuter;

    // Hidden properties used for debug
    private MaterialProperty m_mainTex;
    private MaterialProperty m_texSampleWidth;
    private MaterialProperty m_texSampleHeight;
    private MaterialProperty m_gradientScale;

    private MaterialProperty m_scaleX;
    private MaterialProperty m_scaleY;

    private MaterialProperty m_PerspectiveFilter;
    
    private MaterialProperty m_vertexOffsetX;
    private MaterialProperty m_vertexOffsetY;
    private MaterialProperty m_maskCoord;
    private MaterialProperty m_maskSoftnessX;
    private MaterialProperty m_maskSoftnessY;
     
    //private MaterialProperty m_weightNormal;
    //private MaterialProperty m_weightBold;
     
   

    private MaterialProperty m_shaderFlags; // _ShaderFlag useed to determine bevel type.
    private MaterialProperty m_scaleRatio_A;
    private MaterialProperty m_scaleRatio_B;
    private MaterialProperty m_scaleRatio_C;


    // Custom Material Editor Skin Options
    private GUISkin mySkin;
    //private GUIStyle Section_Label;
    private GUIStyle Group_Label;
    private GUIStyle Group_Label_Left;


    // Private Fields  
    private enum Bevel_Types { OuterBevel = 0, InnerBevel = 1 };
    private enum Mask_Type { MaskOff = 0, MaskHard = 1, MaskSoft = 2 };

    private string[] m_bevelOptions = { "Outer Bevel", "Inner Bevel", "--" };
    private int m_bevelSelection;
    private Mask_Type m_mask;

    private enum Underlay_Types { Normal = 0, Inner = 1};
    private Underlay_Types m_underlaySelection = Underlay_Types.Normal;

    private string[] m_Keywords;

    private bool isRatiosEnabled;
    private bool isBevelEnabled;
    private bool isGlowEnabled;
    private bool isBumpEnabled;
    private bool isEnvEnabled;
    private bool isUnderlayEnabled;
    private bool havePropertiesChanged = false;
  


    public override void OnEnable()
    {
        base.OnEnable();

        // Find to location of the TextMesh Pro Asset Folder (as users may have moved it)
        string tmproAssetFolderPath = TMPro_EditorUtility.GetAssetLocation(); 

        // Initialized instance of Material Editor State Manager
        if (EditorGUIUtility.isProSkin)
            mySkin = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/TMPro_DarkSkin.guiskin", typeof(GUISkin)) as GUISkin;
        else
            mySkin = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/TMPro_LightSkin.guiskin", typeof(GUISkin)) as GUISkin;

        if (mySkin != null)
        {
            Group_Label = mySkin.FindStyle("Group Label");
            Group_Label_Left = mySkin.FindStyle("Group Label - Left Half");
        }

        

        // Initialize the Event Listener for Undo Events.     
        Undo.undoRedoPerformed += OnUndoRedo;
        Undo.postprocessModifications += OnUndoRedoEvent;
    }


    public override void OnDisable()
    {
        // Remove Undo / Redo Event Listeners.
        base.OnDisable();
        Undo.undoRedoPerformed -= OnUndoRedo;
        Undo.postprocessModifications -= OnUndoRedoEvent;
    }


    public override void OnInspectorGUI()
    {
        // render the default inspector
        //base.OnInspectorGUI();
        //return;

        serializedObject.Update();

        // if we are not visible... return
        if (!isVisible)
            return;


        ReadMaterialProperties();

        Material targetMaterial = target as Material;

        // If multiple materials have been selected and are not using the same shader, we simply return.
        if (targets.Length > 1)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                Material mat = targets[i] as Material;

                if (targetMaterial.shader != mat.shader)
                {
                    return;
                }
            }
        }


        // Retrieve Shader Multi_Compile Keywords
        m_Keywords = targetMaterial.shaderKeywords;
        isBevelEnabled = m_Keywords.Contains("BEVEL_ON");
        isGlowEnabled = m_Keywords.Contains("GLOW_ON");
        //isUnderlayEnabled = m_Keywords.Contains("UNDERLAY_ON") | m_Keywords.Contains("UNDERLAY_INNER");
        isRatiosEnabled = !m_Keywords.Contains("RATIOS_OFF");

        if (m_Keywords.Contains("UNDERLAY_ON"))
        {
            isUnderlayEnabled = true;
            m_underlaySelection = Underlay_Types.Normal;
        }
        else if (m_Keywords.Contains("UNDERLAY_INNER"))
        {
            isUnderlayEnabled = true;
            m_underlaySelection = Underlay_Types.Inner;
        }
        else
            isUnderlayEnabled = false;


        if (m_Keywords.Contains("MASK_HARD")) m_mask = Mask_Type.MaskHard;
        else if (m_Keywords.Contains("MASK_SOFT")) m_mask = Mask_Type.MaskSoft;
        else m_mask = Mask_Type.MaskOff;


        if (m_shaderFlags.hasMixedValue)
            m_bevelSelection = 2;
        else
            m_bevelSelection = (int)m_shaderFlags.floatValue & 1;


        // Check if Shader selection is compatible with Font Asset



        EditorGUIUtility.LookLikeControls(130, 50);

        // FACE PANEL
        EditorGUI.indentLevel = 0;
        if (GUILayout.Button("<b>Face</b> - <i>Settings</i> -", Group_Label))
            m_foldout.face = !m_foldout.face;

        if (m_foldout.face)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.indentLevel = 1;
            ColorProperty(m_faceColor, "Color");

            if (targetMaterial.HasProperty("_FaceTex")) DrawTextureProperty(m_faceTex, "Texture");
            DrawSliderProperty(m_outlineSoftness, "Softness");
            DrawSliderProperty(m_faceDilate, "Dilate");
            if (targetMaterial.HasProperty("_FaceShininess")) DrawSliderProperty(m_faceShininess, "Gloss");

            if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
        }


        // OUTLINE PANEL
        EditorGUI.indentLevel = 0;
        if (GUILayout.Button("<b>Outline</b> - <i>Settings</i> -", Group_Label))
            m_foldout.outline = !m_foldout.outline;

        if (m_foldout.outline)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.indentLevel = 1;
            ColorProperty(m_outlineColor, "Color");

            if (targetMaterial.HasProperty("_OutlineTex")) DrawTextureProperty(m_outlineTex, "Texture");
            DrawSliderProperty(m_outlineThickness, "Thickness");

            if (targetMaterial.HasProperty("_OutlineShininess")) DrawSliderProperty(m_outlineShininess, "Gloss");

            if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
        }


        // UNDERLAY PANEL
        if (targetMaterial.HasProperty("_UnderlayColor"))
        {
            string underlayKeyword = m_underlaySelection == Underlay_Types.Normal ? "UNDERLAY_ON" : "UNDERLAY_INNER";
            isUnderlayEnabled = DrawTogglePanel(FoldoutType.underlay, "<b>Underlay</b> - <i>Settings</i> -", isUnderlayEnabled, underlayKeyword);


            if (m_foldout.underlay)
            {
                EditorGUI.BeginChangeCheck();

                EditorGUI.indentLevel = 1;

                m_underlaySelection = (Underlay_Types)EditorGUILayout.EnumPopup("Underlay Type", m_underlaySelection);
                if (GUI.changed) SetUnderlayKeywords();

                ColorProperty(m_underlayColor, "Color");
                DrawSliderProperty(m_underlayOffsetX, "OffsetX");
                DrawSliderProperty(m_underlayOffsetY, "OffsetY");
                DrawSliderProperty(m_underlayDilate, "Dilate");
                DrawSliderProperty(m_underlaySoftness, "Softness");

                if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
            }
        }

        // BEVEL PANEL
        if (targetMaterial.HasProperty("_Bevel"))
        {
            isBevelEnabled = DrawTogglePanel(FoldoutType.bevel, "<b>Bevel</b> - <i>Settings</i> -", isBevelEnabled, "BEVEL_ON");

            if (m_foldout.bevel)
            {
                EditorGUI.indentLevel = 1;
                GUI.changed = false;
                m_bevelSelection = EditorGUILayout.Popup("Type", m_bevelSelection, m_bevelOptions) & 1;
                if (GUI.changed)
                {
                    havePropertiesChanged = true;
                    m_shaderFlags.floatValue = m_bevelSelection;
                }

                EditorGUI.BeginChangeCheck();

                DrawSliderProperty(m_bevel, "Amount");
                DrawSliderProperty(m_bevelOffset, "Offset");
                DrawSliderProperty(m_bevelWidth, "Width");
                DrawSliderProperty(m_bevelRoundness, "Roundness");
                DrawSliderProperty(m_bevelClamp, "Clamp");

                if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
            }
        }


        // LIGHTING PANEL
        if (targetMaterial.HasProperty("_SpecularColor") || targetMaterial.HasProperty("_SpecColor"))
        {
            isBevelEnabled = DrawTogglePanel(FoldoutType.light, "<b>Lighting</b> - <i>Settings</i> -", isBevelEnabled, "BEVEL_ON");

            if (m_foldout.light)
            {
                EditorGUI.BeginChangeCheck();

                EditorGUI.indentLevel = 1;
                if (targetMaterial.HasProperty("_LightAngle"))
                { // Non Surface Shader
                    DrawSliderProperty(m_lightAngle, "Light Angle");
                    ColorProperty(m_specularColor, "Specular Color");
                    DrawSliderProperty(m_specularPower, "Specular Power");
                    DrawSliderProperty(m_reflectivity, "Reflectivity Power");
                    DrawSliderProperty(m_diffuse, "Diffuse Shadow");
                    DrawSliderProperty(m_ambientLight, "Ambient Shadow");
                }
                else
                    ColorProperty(m_specColor, "Specular Color");

                if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
            }
        }


        // BUMPMAP PANEL
        if (targetMaterial.HasProperty("_BumpMap"))
        {
            isBevelEnabled = DrawTogglePanel(FoldoutType.bump, "<b>BumpMap</b> - <i>Settings</i> -", isBevelEnabled, "BEVEL_ON");

            if (m_foldout.bump)
            {
                EditorGUI.BeginChangeCheck();

                DrawTextureProperty(m_bumpMap, "Texture");
                DrawSliderProperty(m_bumpFace, "Face");
                DrawSliderProperty(m_bumpOutline, "Outline");

                if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
            }
        }


        // ENVMAP PANEL
        if (targetMaterial.HasProperty("_Cube"))
        {
            isBevelEnabled = DrawTogglePanel(FoldoutType.env, "<b>EnvMap</b> - <i>Settings</i> -", isBevelEnabled, "BEVEL_ON");

            if (m_foldout.env)
            {
                EditorGUI.BeginChangeCheck();

                EditorGUI.indentLevel = 1;
                ColorProperty(m_reflectColor, "Color");
                DrawTextureProperty(m_reflectTex, "Texture");
                if (targetMaterial.HasProperty("_EnvTiltX"))
                {
                    DrawSliderProperty(m_envTiltX, "Tilt X");
                    DrawSliderProperty(m_envTiltY, "Tilt Y");
                }

                if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
            }
        }


        // GLOW PANEL
        if (targetMaterial.HasProperty("_GlowColor"))
        {
            isGlowEnabled = DrawTogglePanel(FoldoutType.glow, "<b>Glow</b> - <i>Settings</i> -", isGlowEnabled, "GLOW_ON");

            if (m_foldout.glow)
            {
                EditorGUI.BeginChangeCheck();

                EditorGUI.indentLevel = 1;
                ColorProperty(m_glowColor, "Color");
                DrawSliderProperty(m_glowOffset, "Offset");
                DrawSliderProperty(m_glowInner, "Inner");
                DrawSliderProperty(m_glowOuter, "Outer");
                DrawSliderProperty(m_glowPower, "Power");

                if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
            }
        }


        // DEBUG PANEL

        if (targetMaterial.HasProperty("_GradientScale"))
        {
            EditorGUI.indentLevel = 0;
            if (GUILayout.Button("<b>Debug</b> - <i>Settings</i> -", Group_Label))
                m_foldout.debug = !m_foldout.debug;

            if (m_foldout.debug)
            {
                EditorGUI.indentLevel = 1;

                EditorGUI.BeginChangeCheck();
       
                DrawTextureProperty(m_mainTex, "Font Atlas");
                DrawFloatProperty(m_gradientScale, "Gradient Scale");
                DrawFloatProperty(m_texSampleWidth, "Texture Width");
                DrawFloatProperty(m_texSampleHeight, "Texture Height");
                GUILayout.Space(20);

                DrawSliderProperty(m_scaleX, "Scale X");
                DrawSliderProperty(m_scaleY, "Scale Y");
                DrawSliderProperty(m_PerspectiveFilter, "Perspective Filter");

                GUILayout.Space(20);

                DrawFloatProperty(m_vertexOffsetX, "Offset X");
                DrawFloatProperty(m_vertexOffsetY, "Offset Y");

                if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;

                // Mask
                
                if (targetMaterial.HasProperty("_MaskCoord"))
                {
                    GUILayout.Space(15);
                    m_mask = (Mask_Type)EditorGUILayout.EnumPopup("Mask", m_mask);
                    if (GUI.changed)
                    {
                        havePropertiesChanged = true;
                        SetMaskKeywords(m_mask);
                    }

                    
                    if (m_mask != Mask_Type.MaskOff)
                    {
                        EditorGUI.BeginChangeCheck();

                        Draw2DBoundsProperty(m_maskCoord, "Mask Bounds");
                        DrawFloatProperty(m_maskSoftnessX, "Softness X");
                        DrawFloatProperty(m_maskSoftnessY, "Softness Y");
           
                        if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
                    }
                    
                    GUILayout.Space(15);
                }
                
                GUILayout.Space(20);

                // Ratios
                GUI.changed = false;
                isRatiosEnabled = EditorGUILayout.Toggle("Enable Ratios?", isRatiosEnabled);
                if (GUI.changed)
                {
                    SetKeyword(!isRatiosEnabled, "RATIOS_OFF");
                }

                EditorGUI.BeginChangeCheck();

                DrawFloatProperty(m_scaleRatio_A, "Scale Ratio A");
                DrawFloatProperty(m_scaleRatio_B, "Scale Ratio B");
                DrawFloatProperty(m_scaleRatio_C, "Scale Ratio C");

                if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
            }
        }


        if (havePropertiesChanged)
        {
            //Debug.Log("Material Editor properties have changed.");
            havePropertiesChanged = false;

            PropertiesChanged();
            EditorUtility.SetDirty(target);
            //TMPro_EditorUtility.RepaintAll(); // Look into using SetDirty.          
            TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, target as Material);
        }
    }


    // Special Handling of Undo / Redo Events.
    private void OnUndoRedo()
    {      
        int UndoEventID = Undo.GetCurrentGroup();
        int LastUndoEventID = m_eventID;

        if (UndoEventID != LastUndoEventID)
        {
            //Debug.Log("Undo Redo Event Performed in Material Editor. Event ID:" + UndoEventID + ".  Target ID: " + m_modifiedProperties.target.GetInstanceID() + "  Current Material: " + m_modifiedProperties.objectReference + "  New Material: " + (m_modifiedProperties.target as Renderer).sharedMaterial);
            TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, target as Material);
            m_eventID = UndoEventID;
        }
    }

    private UndoPropertyModification[] OnUndoRedoEvent(UndoPropertyModification[] modifications)
    {      
        PropertyModification modifiedProperties = modifications[0].propertyModification;
        System.Type objType = modifiedProperties.target.GetType();

        if (objType == typeof(MeshRenderer) || objType == typeof(Material)) // && UndoEventID != LastUndoEventID)
        {          
            //Debug.Log("OnUndoRedoEvent() received in Material Editor. Event ID:" + UndoEventID + ".  Target ID: " + m_modifiedProperties.target.GetInstanceID() + "  Current Material: " + m_modifiedProperties.objectReference + "  New Material: " + (m_modifiedProperties.target as Renderer).sharedMaterial);
            TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, target as Material);
        }

        if (target != null)
            EditorUtility.SetDirty(target);

        return modifications;
    }



    // Function to draw title + enable toggle options as well as setting keyword.
    private bool DrawTogglePanel(FoldoutType type, string label, bool toggle, string keyword)
    {
        float old_LabelWidth = EditorGUIUtility.labelWidth;
        float old_FieldWidth = EditorGUIUtility.fieldWidth;

        EditorGUI.indentLevel = 0;
        Rect rect = EditorGUILayout.GetControlRect(false, 22);
        GUI.Label(rect, GUIContent.none, Group_Label);
        if (GUI.Button(new Rect(rect.x, rect.y, 250, rect.height), label, Group_Label_Left))
        {
            switch (type)
            {
                case FoldoutType.underlay:
                    m_foldout.underlay = !m_foldout.underlay;
                    break;
                case FoldoutType.bevel:
                    m_foldout.bevel = !m_foldout.bevel;
                    break;
                case FoldoutType.light:
                    m_foldout.light = !m_foldout.light;
                    break;
                case FoldoutType.bump:
                    m_foldout.bump = !m_foldout.bump;
                    break;
                case FoldoutType.env:
                    m_foldout.env = !m_foldout.env;
                    break;
                case FoldoutType.glow:
                    m_foldout.glow = !m_foldout.glow;
                    break;
            }
        }

        EditorGUIUtility.labelWidth = 70;
      
        EditorGUI.BeginChangeCheck();

        Material mat = target as Material;

        if (mat.HasProperty("_FaceShininess") == false || keyword != "BEVEL_ON") // Show Enable Toggle only if material is not Surface Shader.
        {
            toggle = EditorGUI.Toggle(new Rect(rect.width - 90, rect.y + 3, 90, 22), new GUIContent("Enable ->"), toggle);
            if (EditorGUI.EndChangeCheck())
            {               
                SetKeyword(toggle, keyword);
                havePropertiesChanged = true;
            }
        }      

        EditorGUIUtility.labelWidth = old_LabelWidth;
        EditorGUIUtility.fieldWidth = old_FieldWidth;

        return toggle;
    }


    // Function to Draw Material Property and make it look like a Slider with numericalf field.
    private void DrawSliderProperty(MaterialProperty property, string label)
    {
        float old_LabelWidth = EditorGUIUtility.labelWidth;
        float old_FieldWidth = EditorGUIUtility.fieldWidth;

        // Draw Slider
        //EditorGUIUtility.labelWidth = 160;
        Rect rect = EditorGUILayout.GetControlRect(false, 20);
        Rect pos0 = new Rect(rect.x, rect.y, rect.width - 55, 20);
        Rect pos1 = new Rect(rect.width - 46, rect.y, 60, 18);

        // Draw Numerical Field
        //EditorGUIUtility.labelWidth = 160;
        RangeProperty(pos0, property, label);
        EditorGUIUtility.labelWidth = 10;
        FloatProperty(new Rect(pos1), property, null);
        if (!property.hasMixedValue)
            property.floatValue = Mathf.Round(property.floatValue * 1000) / 1000;

        EditorGUIUtility.labelWidth = old_LabelWidth;
        EditorGUIUtility.fieldWidth = old_FieldWidth;
    }


    private void DrawTextureProperty(MaterialProperty property, string label)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 75);
        GUI.Label(new Rect(rect.x + 15, rect.y + 5, 100, rect.height), label);
        TextureProperty(new Rect(rect.x - 5, rect.y + 5, 200, rect.height), property, string.Empty, false);
    }


    private void DrawFloatProperty(MaterialProperty property, string label)
    {
        float old_LabelWidth = EditorGUIUtility.labelWidth;
        float old_FieldWidth = EditorGUIUtility.fieldWidth;

        //EditorGUIUtility.labelWidth = 160;
        Rect rect = EditorGUILayout.GetControlRect(false, 20);
        Rect pos0 = new Rect(rect.x, rect.y, 225, 18);

        //EditorGUIUtility.fieldWidth = 60;
        FloatProperty(pos0, property, label);

        EditorGUIUtility.labelWidth = old_LabelWidth;
        EditorGUIUtility.fieldWidth = old_FieldWidth;
    }


    private void DrawVectorProperty(MaterialProperty property, string label)
    {
        float old_LabelWidth = EditorGUIUtility.labelWidth;
        float old_FieldWidth = EditorGUIUtility.fieldWidth;

        EditorGUIUtility.labelWidth = 160;
        Rect rect = EditorGUILayout.GetControlRect(false, 20);
        Rect pos0 = new Rect(rect.x + 15, rect.y + 2, rect.width - 120, 18);
        Rect pos1 = new Rect(175, rect.y - 14, rect.width - 160, 18);

        GUI.Label(pos0, label);     
        VectorProperty(pos1, property, "");

        EditorGUIUtility.labelWidth = old_LabelWidth;
        EditorGUIUtility.fieldWidth = old_FieldWidth;
    }


    private void Draw2DBoundsProperty(MaterialProperty property, string label)
    {
        float old_LabelWidth = EditorGUIUtility.labelWidth;
        float old_FieldWidth = EditorGUIUtility.fieldWidth;

        //EditorGUIUtility.labelWidth = 100;
        Rect rect = EditorGUILayout.GetControlRect(false, 22);
        Rect pos0 = new Rect(rect.x + 15, rect.y + 2, rect.width - 15, 18);
        //Rect pos1 = new Rect(175, rect.y - 14, rect.width - 160, 18);

        GUI.Label(pos0, label);
        EditorGUIUtility.labelWidth = 30;

        float width = (pos0.width - 15) / 5;      
        pos0.x += old_LabelWidth - 30;
        
        Vector4 vec = property.vectorValue;
        pos0.width = width;
        vec.x = EditorGUI.FloatField(pos0, "X", vec.x);
        
        pos0.x += width - 14;
        vec.y = EditorGUI.FloatField(pos0, "Y", vec.y);

        pos0.x += width - 14;
        vec.z = EditorGUI.FloatField(pos0, "W", vec.z);
        
        pos0.x += width - 14;
        vec.w = EditorGUI.FloatField(pos0, "H", vec.w);

        pos0.x = rect.width - 11;
        pos0.width = 25;

        property.vectorValue = vec;

        if (GUI.Button(pos0, "X"))
        {
            Renderer _renderer = Selection.activeGameObject.renderer;
            if (_renderer != null)
            {
                property.vectorValue = new Vector4(0, 0, Mathf.Round(_renderer.bounds.extents.x * 1000) / 1000, Mathf.Round(_renderer.bounds.extents.y * 1000) / 1000);
            }
        }

        EditorGUIUtility.labelWidth = old_LabelWidth;
        EditorGUIUtility.fieldWidth = old_FieldWidth;
    }


    // Function to set keyword for each selected material.
    private void SetKeyword(bool state, string keyword)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            Material mat = targets[i] as Material;

            if (state)
            {                         
                switch (keyword)
                {
                    case "UNDERLAY_ON":
                        mat.EnableKeyword("UNDERLAY_ON");
                        mat.DisableKeyword("UNDERLAY_INNER");
                        break;
                    case "UNDERLAY_INNER":
                        mat.EnableKeyword("UNDERLAY_INNER");
                        mat.DisableKeyword("UNDERLAY_ON");
                        break;
                    default:
                        mat.EnableKeyword(keyword);
                        break;
                }              
            }
            else
            {
                switch (keyword)
                {
                    case "UNDERLAY_ON":
                        mat.DisableKeyword("UNDERLAY_ON");
                        mat.DisableKeyword("UNDERLAY_INNER");
                        break;
                    case "UNDERLAY_INNER":
                        mat.DisableKeyword("UNDERLAY_INNER");
                        mat.DisableKeyword("UNDERLAY_ON");
                        break;
                    default:
                        mat.DisableKeyword(keyword);
                        break;
                }                                   
            }             
        }
    }


    private void SetUnderlayKeywords()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            Material mat = targets[i] as Material;

            if (m_underlaySelection == Underlay_Types.Normal)
            {
                mat.EnableKeyword("UNDERLAY_ON");
                mat.DisableKeyword("UNDERLAY_INNER");
            }
            else if (m_underlaySelection == Underlay_Types.Inner)
            {
                mat.EnableKeyword("UNDERLAY_INNER");
                mat.DisableKeyword("UNDERLAY_ON");
            }
        }
    }


    private void SetMaskKeywords(Mask_Type mask)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            Material mat = targets[i] as Material;

            switch (mask)
            {
                case Mask_Type.MaskHard:
                    mat.EnableKeyword("MASK_HARD");
                    mat.DisableKeyword("MASK_SOFT");
                    mat.DisableKeyword("MASK_OFF");
                    break;
                case Mask_Type.MaskSoft:
                    mat.EnableKeyword("MASK_SOFT");
                    mat.DisableKeyword("MASK_HARD");
                    mat.DisableKeyword("MASK_OFF");
                    break;
                case Mask_Type.MaskOff:
                    mat.EnableKeyword("MASK_OFF");
                    mat.DisableKeyword("MASK_HARD");
                    mat.DisableKeyword("MASK_SOFT");
                    break;
            }
        }
    }


    // Need to get material properties every update.
    void ReadMaterialProperties()
    {
        Object[] target_Materials = this.targets;


        m_faceColor = GetMaterialProperty(target_Materials, "_FaceColor");
        m_faceTex = GetMaterialProperty(target_Materials, "_FaceTex");
        m_faceDilate = GetMaterialProperty(target_Materials, "_FaceDilate");
        m_faceShininess = GetMaterialProperty(target_Materials, "_FaceShininess");

        // Border Properties
        m_outlineColor = GetMaterialProperty(target_Materials, "_OutlineColor");
        m_outlineThickness = GetMaterialProperty(target_Materials, "_OutlineWidth");
        m_outlineSoftness = GetMaterialProperty(target_Materials, "_OutlineSoftness");
        m_outlineTex = GetMaterialProperty(target_Materials, "_OutlineTex");
        m_outlineShininess = GetMaterialProperty(target_Materials, "_OutlineShininess");


        // Underlay Properties
        m_underlayColor = GetMaterialProperty(target_Materials, "_UnderlayColor");
        m_underlayOffsetX = GetMaterialProperty(target_Materials, "_UnderlayOffsetX");
        m_underlayOffsetY = GetMaterialProperty(target_Materials, "_UnderlayOffsetY");
        m_underlayDilate = GetMaterialProperty(target_Materials, "_UnderlayDilate");
        m_underlaySoftness = GetMaterialProperty(target_Materials, "_UnderlaySoftness");


        // Normal Map Options
        m_bumpMap = GetMaterialProperty(target_Materials, "_BumpMap");
        m_bumpFace = GetMaterialProperty(target_Materials, "_BumpFace");
        m_bumpOutline = GetMaterialProperty(target_Materials, "_BumpOutline");

        // Used by Unlit SDF Shader 
        //m_edgeSharpness = GetMaterialProperty(target_Materials, "_Edge");

        // Material Properties for Beveling Options
        m_bevel = GetMaterialProperty(target_Materials, "_Bevel");
        m_bevelOffset = GetMaterialProperty(target_Materials, "_BevelOffset");
        m_bevelWidth = GetMaterialProperty(target_Materials, "_BevelWidth");
        m_bevelClamp = GetMaterialProperty(target_Materials, "_BevelClamp");
        m_bevelRoundness = GetMaterialProperty(target_Materials, "_BevelRoundness");

        m_specColor = GetMaterialProperty(target_Materials, "_SpecColor"); // Used by Surface Shader
        
        // Bevel properties for Basic Shader & Hidden for Surface Shader
        m_lightAngle = GetMaterialProperty(target_Materials, "_LightAngle");
        m_specularColor = GetMaterialProperty(target_Materials, "_SpecularColor");
        m_specularPower = GetMaterialProperty(target_Materials, "_SpecularPower");
        m_reflectivity = GetMaterialProperty(target_Materials, "_Reflectivity");
        m_diffuse = GetMaterialProperty(target_Materials, "_Diffuse");
        m_ambientLight = GetMaterialProperty(target_Materials, "_Ambient");



        // Material Properties for Glow Options
        m_glowColor = GetMaterialProperty(target_Materials, "_GlowColor");
        m_glowOffset = GetMaterialProperty(target_Materials, "_GlowOffset");
        m_glowInner = GetMaterialProperty(target_Materials, "_GlowInner");
        m_glowOuter = GetMaterialProperty(target_Materials, "_GlowOuter");
        m_glowPower = GetMaterialProperty(target_Materials, "_GlowPower");

        // Cube Map Options
        m_reflectColor = GetMaterialProperty(target_Materials, "_ReflectColor");
        m_reflectTex = GetMaterialProperty(target_Materials, "_Cube");
        m_envTiltX = GetMaterialProperty(target_Materials, "_EnvTiltX");
        m_envTiltY = GetMaterialProperty(target_Materials, "_EnvTiltY");
        // Properties specific to Surface Shader
        //m_shininess = GetMaterialProperty(target_Materials, "_Shininess");


        // Hidden Properties
        m_mainTex = GetMaterialProperty(target_Materials, "_MainTex");
        m_texSampleWidth = GetMaterialProperty(target_Materials, "_TextureWidth");
        m_texSampleHeight = GetMaterialProperty(target_Materials, "_TextureHeight");
        m_gradientScale = GetMaterialProperty(target_Materials, "_GradientScale");
        m_PerspectiveFilter = GetMaterialProperty(target_Materials, "_PerspectiveFilter");
        m_scaleX = GetMaterialProperty(target_Materials, "_ScaleX");
        m_scaleY = GetMaterialProperty(target_Materials, "_ScaleY");


        m_vertexOffsetX = GetMaterialProperty(target_Materials, "_VertexOffsetX");
        m_vertexOffsetY = GetMaterialProperty(target_Materials, "_VertexOffsetY");
        m_maskCoord = GetMaterialProperty(target_Materials, "_MaskCoord");
        m_maskSoftnessX = GetMaterialProperty(target_Materials, "_MaskSoftnessX");
        m_maskSoftnessY = GetMaterialProperty(target_Materials, "_MaskSoftnessY");
        
        //m_weightNormal = GetMaterialProperty(target_Materials, "_WeightNormal");
        //m_weightBold = GetMaterialProperty(target_Materials, "_WeightBold");
      
        m_shaderFlags = GetMaterialProperty(target_Materials, "_ShaderFlags");
        m_scaleRatio_A = GetMaterialProperty(target_Materials, "_ScaleRatioA");
        m_scaleRatio_B = GetMaterialProperty(target_Materials, "_ScaleRatioB");
        m_scaleRatio_C = GetMaterialProperty(target_Materials, "_ScaleRatioC");
        //m_fadeOut = GetMaterialProperty(target_Materials, "_Fadeout");
       
    }
}
