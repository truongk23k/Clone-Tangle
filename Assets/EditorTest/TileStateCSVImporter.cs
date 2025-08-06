using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TileStateCSVImporter : EditorWindow
{
    private string csvFilePath = "";
    private string outputFolder = "Assets/ScriptableObjects/TileStates/";
    private Vector2 scrollPos;
    private List<TileStateData> tileStates = new List<TileStateData>();

    [System.Serializable]
    public class TileStateData
    {
        public string name;
        public Color backgroundColor;
        public Color textColor;

        public TileStateData(string name, Color bgColor, Color txtColor)
        {
            this.name = name;
            this.backgroundColor = bgColor;
            this.textColor = txtColor;
        }
    }

    [MenuItem("Tools/TileState CSV Importer")]
    public static void ShowWindow()
    {
        GetWindow<TileStateCSVImporter>("TileState CSV Importer");
    }

    void OnGUI()
    {
        GUILayout.Label("TileState CSV Import Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // CSV File Selection
        GUILayout.BeginHorizontal();
        GUILayout.Label("CSV File:", GUILayout.Width(80));
        csvFilePath = GUILayout.TextField(csvFilePath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            csvFilePath = EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Output Folder
        GUILayout.BeginHorizontal();
        GUILayout.Label("Output Folder:", GUILayout.Width(80));
        outputFolder = GUILayout.TextField(outputFolder);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string folder = EditorUtility.OpenFolderPanel("Select Output Folder", "Assets", "");
            if (!string.IsNullOrEmpty(folder))
            {
                outputFolder = "Assets" + folder.Substring(Application.dataPath.Length) + "/";
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load CSV"))
        {
            LoadCSV();
        }
        if (GUILayout.Button("Create TileStates") && tileStates.Count > 0)
        {
            CreateTileStates();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Show CSV Format Example
        if (GUILayout.Button("Show CSV Format Example"))
        {
            ShowCSVExample();
        }

        GUILayout.Space(10);

        // Preview loaded data
        if (tileStates.Count > 0)
        {
            GUILayout.Label($"Loaded {tileStates.Count} TileState(s):", EditorStyles.boldLabel);
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));

            for (int i = 0; i < tileStates.Count; i++)
            {
                var tileState = tileStates[i];

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label($"Name: {tileState.name}", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Background:", GUILayout.Width(80));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ColorField(tileState.backgroundColor);
                EditorGUI.EndDisabledGroup();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Text:", GUILayout.Width(80));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ColorField(tileState.textColor);
                EditorGUI.EndDisabledGroup();
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
                GUILayout.Space(5);
            }

            GUILayout.EndScrollView();
        }
    }

    void LoadCSV()
    {
        if (string.IsNullOrEmpty(csvFilePath) || !File.Exists(csvFilePath))
        {
            EditorUtility.DisplayDialog("Error", "Please select a valid CSV file!", "OK");
            return;
        }

        try
        {
            tileStates.Clear();
            string[] lines = File.ReadAllLines(csvFilePath);

            // Skip header if exists
            int startIndex = 0;
            if (lines.Length > 0 && (lines[0].ToLower().Contains("name") || lines[0].ToLower().Contains("background")))
            {
                startIndex = 1;
            }

            for (int i = startIndex; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                string[] values = ParseCSVLine(line);

                if (values.Length >= 3)
                {
                    string name = values[0].Trim();
                    Color backgroundColor = ParseColor(values[1].Trim());
                    Color textColor = ParseColor(values[2].Trim());

                    tileStates.Add(new TileStateData(name, backgroundColor, textColor));
                }
            }

            EditorUtility.DisplayDialog("Success", $"Loaded {tileStates.Count} TileState configurations!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to load CSV: {e.Message}", "OK");
        }
    }

    string[] ParseCSVLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        string currentField = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(currentField);
                currentField = "";
            }
            else
            {
                currentField += c;
            }
        }

        result.Add(currentField);
        return result.ToArray();
    }

    Color ParseColor(string colorString)
    {
        colorString = colorString.Trim();

        // Try hex format (#RRGGBB or #RRGGBBAA)
        if (colorString.StartsWith("#"))
        {
            if (ColorUtility.TryParseHtmlString(colorString, out Color color))
            {
                return color;
            }
        }

        // Try RGB format (r,g,b) or RGBA format (r,g,b,a)
        if (colorString.StartsWith("(") && colorString.EndsWith(")"))
        {
            string values = colorString.Substring(1, colorString.Length - 2);
            string[] components = values.Split(',');

            if (components.Length >= 3)
            {
                if (float.TryParse(components[0].Trim(), out float r) &&
                    float.TryParse(components[1].Trim(), out float g) &&
                    float.TryParse(components[2].Trim(), out float b))
                {
                    float a = 1.0f;
                    if (components.Length >= 4 && float.TryParse(components[3].Trim(), out float alpha))
                    {
                        a = alpha;
                    }

                    // If values are > 1, assume they're in 0-255 range
                    if (r > 1 || g > 1 || b > 1)
                    {
                        r /= 255f;
                        g /= 255f;
                        b /= 255f;
                        if (a > 1) a /= 255f;
                    }

                    return new Color(r, g, b, a);
                }
            }
        }

        // Default to white if parsing fails
        Debug.LogWarning($"Could not parse color: {colorString}, using white instead");
        return Color.white;
    }

    void CreateTileStates()
    {
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        int createdCount = 0;
        foreach (var data in tileStates)
        {
            string assetPath = Path.Combine(outputFolder, $"{data.name}.asset");

            // Check if asset already exists
            TileState existingAsset = AssetDatabase.LoadAssetAtPath<TileState>(assetPath);

            if (existingAsset != null)
            {
                // Update existing asset
                existingAsset.backgroundColor = data.backgroundColor;
                existingAsset.textColor = data.textColor;
                EditorUtility.SetDirty(existingAsset);
            }
            else
            {
                // Create new asset
                TileState newTileState = CreateInstance<TileState>();
                newTileState.backgroundColor = data.backgroundColor;
                newTileState.textColor = data.textColor;

                AssetDatabase.CreateAsset(newTileState, assetPath);
            }

            createdCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Success",
            $"Created/Updated {createdCount} TileState assets in {outputFolder}", "OK");
    }

    void ShowCSVExample()
    {
        string example = @"CSV Format Examples:
        1. With Header:
        Name,BackgroundColor,TextColor
        Grass,#00FF00,#FFFFFF
        Water,#0000FF,#FFFF00
        Stone,(128,128,128),(255,255,255)

        2. Without Header:
        Grass,#00FF00,#FFFFFF
        Water,#0000FF,#FFFF00
        Stone,(128,128,128),(255,255,255)

        Color Formats Supported:
        - Hex: #RRGGBB or #RRGGBBAA (e.g., #FF0000, #FF0000FF)
        - RGB: (r,g,b) with values 0-1 or 0-255 (e.g., (1,0,0) or (255,0,0))
        - RGBA: (r,g,b,a) with values 0-1 or 0-255 (e.g., (1,0,0,0.5) or (255,0,0,128))";

        EditorUtility.DisplayDialog("CSV Format", example, "OK");
    }
}