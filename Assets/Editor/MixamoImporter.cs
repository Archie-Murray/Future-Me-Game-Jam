using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
public class MixamoImporter : EditorWindow {
    private static MixamoImporter _editor;
    private static int _width = 300;
    private static int _height = 500;
    private static int _x = 0;
    private static int _y = 0;
    private static List<string> _allFiles = new List<string>();
    private Vector2 _scrollPos;

    private static Settings _settings = new Settings();
    private const string SettingsPrefsPath = nameof(MixamoImporter) + "_lastsettings";
    private static Color _linecolor = new Color32(128, 128, 128, 64);
    [System.Serializable]
    private class Settings {
        public string Path = string.Empty;
        public bool RenameAnimClips = true;
        public bool renameAnimClipsUnderscores = true;
        public bool renameAnimClipsToLower = true;
        public bool changeLoopAnimClips = true;
        public bool loopAnimClipsTime = true;
        public bool loopAnimClipsPose = false;
        public float loopAnimClipsCycleOffset = 0f;
        public bool rootTransformRotation = false;
        public bool rootTransformRotationBakeIntoPose = false;
        public int rootTransformRotationPopupIndex = 0;
        public float rootTransformRotationOffset = 0f;
        public bool rootTransformPositionY = false;
        public bool rootTransformPositionYBakeIntoPose = false;
        public int rootTransformPositionYPopupIndex = 0;
        public float rootTransformPositionYOffset = 0f;
        public bool rootTransformPositionXZ = false;
        public bool rootTransformPositionXZBakeIntoPose = false;
        public int rootTransformPositionXZPopupIndex = 0;

        public bool disableMaterialImport = true;
        public bool mirror = false;
        public bool setRigToHumanoid = true;
        public Avatar RigCustomAvatar;
    }

    [MenuItem("Window/Animation/Mixamo Batch Import")]
    static void ShowEditor() {
        _editor = EditorWindow.GetWindow<MixamoImporter>();
        CenterWindow();
        LoadSettings();
    }

    private void OnGUI() {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Select directory")) {
            _settings.Path = EditorUtility.OpenFolderPanel("Select directory with files", "", "");
        }
        if (GUILayout.Button("Reset settings")) {
            _settings = new Settings();
            SaveSettings();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Label($"Path: {_settings.Path} ", EditorStyles.boldLabel);
        GUILayout.Label($"Selected: {Selection.gameObjects.Length} assets", EditorStyles.boldLabel);

        bool pathvalid = !string.IsNullOrEmpty(_settings.Path) && Directory.Exists(_settings.Path);
        if (!pathvalid) {
            GUI.color = Color.red;
            GUILayout.Label("Path invalid", EditorStyles.boldLabel);
            GUI.color = Color.white;
        }

        DrawUILine();
        _settings.RenameAnimClips = EditorGUILayout.BeginToggleGroup("Rename animation clips to filename", _settings.RenameAnimClips);
        {
            _settings.renameAnimClipsUnderscores = EditorGUILayout.Toggle("Spaces to underscores", _settings.renameAnimClipsUnderscores);
            _settings.renameAnimClipsToLower = EditorGUILayout.Toggle("To lowercase", _settings.renameAnimClipsToLower);
        }
        EditorGUILayout.EndToggleGroup();
        GUILayout.Space(5);

        DrawUILine();
        _settings.changeLoopAnimClips = EditorGUILayout.BeginToggleGroup("Loop _time", _settings.changeLoopAnimClips);
        {
            _settings.loopAnimClipsTime = EditorGUILayout.Toggle("Loop _time", _settings.loopAnimClipsTime);
            _settings.loopAnimClipsPose = EditorGUILayout.Toggle("Loop Pose", _settings.loopAnimClipsPose);
            _settings.loopAnimClipsCycleOffset = EditorGUILayout.FloatField("Cycle Offset", _settings.loopAnimClipsCycleOffset);
        }
        EditorGUILayout.EndToggleGroup();
        GUILayout.Space(5);

        DrawUILine();
        _settings.rootTransformRotation = EditorGUILayout.BeginToggleGroup("Root Transform Rotation", _settings.rootTransformRotation);
        {
            _settings.rootTransformRotationBakeIntoPose = EditorGUILayout.Toggle("Bake into pose", _settings.rootTransformRotationBakeIntoPose);
            _settings.rootTransformRotationPopupIndex = EditorGUILayout.Popup("Based Upon", _settings.rootTransformRotationPopupIndex, new string[]{
        "Original", "Body Orientation"
      });
            _settings.rootTransformRotationOffset = EditorGUILayout.FloatField("Offset", _settings.rootTransformRotationOffset);
        }
        EditorGUILayout.EndToggleGroup();
        GUILayout.Space(5);

        DrawUILine();
        _settings.rootTransformPositionY = EditorGUILayout.BeginToggleGroup("Root Transform Position (Y)", _settings.rootTransformPositionY);
        {
            _settings.rootTransformPositionYBakeIntoPose = EditorGUILayout.Toggle("Bake into pose", _settings.rootTransformPositionYBakeIntoPose);
            _settings.rootTransformPositionYPopupIndex = EditorGUILayout.Popup("Based Upon", _settings.rootTransformPositionYPopupIndex, new string[]{
        "Original", "Center of Mass", "Feet"
      });
            _settings.rootTransformPositionYOffset = EditorGUILayout.FloatField("Offset", _settings.rootTransformPositionYOffset);
        }
        EditorGUILayout.EndToggleGroup();
        GUILayout.Space(5);

        DrawUILine();
        _settings.rootTransformPositionXZ = EditorGUILayout.BeginToggleGroup("Root Transform Position (XZ)", _settings.rootTransformPositionXZ);
        {
            _settings.rootTransformPositionXZBakeIntoPose = EditorGUILayout.Toggle("Bake into pose", _settings.rootTransformPositionXZBakeIntoPose);
            _settings.rootTransformPositionXZPopupIndex = EditorGUILayout.Popup("Based Upon", _settings.rootTransformPositionXZPopupIndex, new string[]{
        "Original", "Center of Mass"
      });
        }
        EditorGUILayout.EndToggleGroup();
        GUILayout.Space(5);

        DrawUILine();
        _settings.setRigToHumanoid = EditorGUILayout.Toggle("Set Rig to Humanoid", _settings.setRigToHumanoid);
        _settings.disableMaterialImport = EditorGUILayout.Toggle("Disable Material Import", _settings.disableMaterialImport);
        _settings.mirror = EditorGUILayout.Toggle("Mirror", _settings.mirror);
        _settings.RigCustomAvatar = EditorGUILayout.ObjectField("Custom Avatar", _settings.RigCustomAvatar, typeof(Avatar), false) as Avatar;

        GUILayout.Space(30);
        DrawUILine();
        GUILayout.BeginHorizontal();

        GUI.enabled = pathvalid;
        if (GUILayout.Button("Process directory")) {
            process_dir();
            SaveSettings();
        }
        GUI.enabled = Selection.gameObjects.Length > 0;
        if (GUILayout.Button("Process selected assets")) {
            processSelectedAssets();
            SaveSettings();
        }
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
    }

    private static void SaveSettings() {
        string json = EditorJsonUtility.ToJson(_settings);
        EditorPrefs.SetString(SettingsPrefsPath, json);
    }

    private static void LoadSettings() {
        _settings = JsonUtility.FromJson<Settings>(EditorPrefs.GetString(SettingsPrefsPath));
        _settings ??= new Settings();
    }

    public void process_dir() {
        DirSearch(_settings.Path);

        if (_allFiles.Count > 0) {
            for (int i = 0; i < _allFiles.Count; i++) {
                int idx = _allFiles[i].IndexOf("Assets");
                string filename = Path.GetFileName(_allFiles[i]);
                // Use range operator
                string asset = _allFiles[i][idx..];
                // Use range operator
                // dd assignment of a value
                AnimationClip orgClip = (AnimationClip) AssetDatabase.LoadAssetAtPath(
                    asset, typeof(AnimationClip));
                // dd assignment of a value

                // Use explicit type
                string fileName = Path.GetFileNameWithoutExtension(_allFiles[i]);
                // Use explicit type
                // Use explicit type
                ModelImporter importer = (ModelImporter) AssetImporter.GetAtPath(asset);
                // Use explicit type

                EditorUtility.DisplayProgressBar($"Processing {_allFiles.Count} files", filename, (1f / _allFiles.Count) * i);

                RenameAndImport(importer, fileName);
            }
        }

        EditorUtility.DisplayProgressBar($"Processing {_allFiles.Count} files", "Saving assets", 1f);
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();

    }

    // Add a menu item called "rename animation" to a FBX's context menu.
    [MenuItem("Assets/RenameMixamoAnim")]
    static void RenameMixamoAnim() {
        //Rigidbody body = (Rigidbody)command.context;
        //body.mass = body.mass * 2;
        Debug.Log("Context: " + Selection.activeObject.ToString());
        UnityEngine.Object asset = Selection.activeGameObject;
        string assetpath = AssetDatabase.GetAssetPath(asset);
        AnimationClip orgClip = (AnimationClip) AssetDatabase.LoadAssetAtPath(assetpath, typeof(AnimationClip));
        string fileName = asset.name;
        ModelImporter importer = (ModelImporter) AssetImporter.GetAtPath(assetpath);
        RenameAndImport(importer, fileName);
    }

    public void processSelectedAssets() {
        int count = Selection.gameObjects.Length;
        if (count > 0) {
            for (int i = 0; i < count; i++) {
                UnityEngine.Object asset = Selection.gameObjects[i];
                string assetpath = AssetDatabase.GetAssetPath(asset);
                AnimationClip orgClip = (AnimationClip) AssetDatabase.LoadAssetAtPath(
                    assetpath, typeof(AnimationClip));

                string fileName = asset.name;
                ModelImporter importer = (ModelImporter) AssetImporter.GetAtPath(assetpath);

                EditorUtility.DisplayProgressBar($"Processing {count} files", fileName, (1f / count) * i);

                RenameAndImport(importer, fileName);
            }
        }

        EditorUtility.ClearProgressBar();

    }



    static void RenameAndImport(ModelImporter asset, string name) {
        ModelImporter modelImporter = asset as ModelImporter;
        ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;

        if (_settings.disableMaterialImport)
            modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;

        if (_settings.setRigToHumanoid)
            modelImporter.animationType = ModelImporterAnimationType.Human;

        if (_settings.RigCustomAvatar != null)
            modelImporter.sourceAvatar = _settings.RigCustomAvatar;

        if (_settings.renameAnimClipsUnderscores)
            name = name.Replace(' ', '_');

        if (_settings.renameAnimClipsToLower)
            name = name.ToLower();

        for (int i = 0; i < clipAnimations.Length; i++) {
            ModelImporterClipAnimation clip = clipAnimations[i];

            if (_settings.RenameAnimClips)
                clip.name = name;
            if (_settings.changeLoopAnimClips) {
                clip.loopTime = _settings.loopAnimClipsTime;
                clip.loopPose = _settings.loopAnimClipsPose;
                clip.cycleOffset = _settings.loopAnimClipsCycleOffset;
                if (_settings.rootTransformRotation) {
                    clip.lockRootRotation = _settings.rootTransformRotationBakeIntoPose;
                    clip.keepOriginalOrientation = _settings.rootTransformRotationPopupIndex == 0;
                    clip.rotationOffset = _settings.rootTransformRotationOffset;
                }
                if (_settings.rootTransformPositionY) {
                    clip.lockRootHeightY = _settings.rootTransformPositionYBakeIntoPose;
                    clip.keepOriginalPositionY = _settings.rootTransformPositionYPopupIndex == 0;
                    clip.heightFromFeet = _settings.rootTransformPositionYPopupIndex == 2;
                    clip.heightOffset = _settings.rootTransformPositionYOffset;
                }
                if (_settings.rootTransformPositionXZ) {
                    clip.lockRootPositionXZ = _settings.rootTransformPositionXZBakeIntoPose;
                    clip.keepOriginalPositionXZ = _settings.rootTransformPositionXZPopupIndex == 0;
                }
            }
        }

        modelImporter.clipAnimations = clipAnimations;
        modelImporter.SaveAndReimport();
    }

    private static void CenterWindow() {
        _editor = EditorWindow.GetWindow<MixamoImporter>();
        _x = (Screen.currentResolution.width - _width) / 2;
        _y = (Screen.currentResolution.height - _height) / 2;
        _editor.position = new Rect(_x, _y, _width, _height);
    }

    static void DirSearch(string path) {
        string[] fileInfo = Directory.GetFiles(path, "*.fbx", SearchOption.AllDirectories);
        foreach (string file in fileInfo) {
            if (file.EndsWith(".fbx"))
                _allFiles.Add(file);
        }
    }

    private static void DrawUILine(int thickness = 1, int padding = 5) {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, _linecolor);
    }
}