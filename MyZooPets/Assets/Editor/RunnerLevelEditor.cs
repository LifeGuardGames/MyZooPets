using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

//[CustomEditor(typeof(LevelComponent))]
public class RunnerLevelEditor : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    private Vector2 mSelectedPointsScrollPosition;
    private bool[] mSelectedGroupToggles = new bool[3] { false, false, false };
    private bool[] mSelectedPurposeToggles = new bool[3] { false, false, false };
    private int mSelectedGroupGridIndex = 0;
    private string mSelectedGroupID = "";
    private PointGroup mCurrentSelectedGroup = null;

    /*
    public override void OnInspectorGUI()
    {
        LevelComponent levelScript = (LevelComponent)target;

        levelScript.TestValue = EditorGUILayout.IntSlider("Val-fuck", levelScript.TestValue, 1, 10);
    }
     * */

    [MenuItem("Window/Runner Level Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(RunnerLevelEditor));
    }

    void OnGUI()
    {
        LevelComponent selectedLevelComponent = null;
        if (Selection.activeTransform != null)
            selectedLevelComponent = Selection.activeTransform.GetComponent<LevelComponent>();

        if (selectedLevelComponent != null)
        {
            
            //@HACK remove before I submit pls
            PointGroup pdd = new PointGroup("test");
            pdd.mID = "test";
            pdd.mGroups = new List<string>();
            pdd.mGroups.Add("duck");
            pdd.mGroups.Add("hippo");
            pdd.mGroups.Add("giraffe");
            pdd.mPurposes = new List<string>();
            pdd.mPurposes.Add("Coins");
            pdd.mPurposes.Add("Item");
            pdd.mPoints = new List<PointInfo>();
            selectedLevelComponent.SetPointGroupInfo(pdd.mID, pdd);

            // Pull out the point groups
            Dictionary<string, PointGroup> selectedGroups = selectedLevelComponent.PointGroups;

            //--
            //The form is all a vertical layout
            EditorGUILayout.BeginVertical();
            //  --
            //  Top Area
            EditorGUILayout.BeginVertical();

            //      --
            //      New Group
            if (GUILayout.Button("New Group"))
                CreateNewGroup(selectedLevelComponent);
            //      End New Group
            //      --

            //      --
            //      Selected Items
            //          --
            //          Selected Items 1
            EditorGUILayout.BeginHorizontal();

            //              --
            //              ID
            GUILayout.Label("ID");
            string idText = "Enter ID Here";
            if (mCurrentSelectedGroup != null)
                idText = mCurrentSelectedGroup.mID;
            GUILayout.TextArea(idText);
            //              end ID
            //              --

            //              --
            //              Groups
            EditorGUILayout.BeginToggleGroup("Groups", true);
            mSelectedGroupToggles[0] = EditorGUILayout.Toggle("Group 1", mSelectedGroupToggles[0]);
            mSelectedGroupToggles[1] = EditorGUILayout.Toggle("Group 2", mSelectedGroupToggles[1]);
            mSelectedGroupToggles[2] = EditorGUILayout.Toggle("Group 3", mSelectedGroupToggles[2]);
            EditorGUILayout.EndToggleGroup();
            //              End groups
            //              --
            GUILayout.EndHorizontal();
            //          End Selected Items 1
            //          --

            //          --
            //          Selected Items 2
            EditorGUILayout.BeginHorizontal();

            //              --
            //              Points Area
            EditorGUILayout.BeginVertical();
            GUILayout.Label("Points");
            mSelectedPointsScrollPosition = GUILayout.BeginScrollView(
                mSelectedPointsScrollPosition, GUILayout.Width(100), GUILayout.Height(100));
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("#");
            GUILayout.Label("Type");
            GUILayout.Label("Pos");
            EditorGUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            //              Points Area
            //              --

            //              --
            //              New Point Button
            if (GUILayout.Button("New Point"))
                AddNewPoint(selectedLevelComponent, mSelectedGroupID);
            //              End Point Button
            //              --

            //              --
            //              Purpose Area
            EditorGUILayout.BeginToggleGroup("Purpose", true);
            mSelectedPurposeToggles[0] = EditorGUILayout.Toggle("Purpose 1", mSelectedPurposeToggles[0]);
            mSelectedPurposeToggles[1] = EditorGUILayout.Toggle("Purpose 2", mSelectedPurposeToggles[1]);
            mSelectedPurposeToggles[2] = EditorGUILayout.Toggle("Purpose 3", mSelectedPurposeToggles[2]);
            EditorGUILayout.EndToggleGroup();
            //              Purpose Area End
            //              --

            //          Selected Items 2 End
            //          --
            EditorGUILayout.EndHorizontal();

            //      Selected Items End
            //      --
            EditorGUILayout.EndVertical();
            //  Top Area End
            //  --

            //  --
            //  Area for existing items
            EditorGUILayout.BeginVertical();
            GUILayout.Label("Existing Points");
            //mSelectedPointsScrollPosition = GUILayout.BeginScrollView(
                //mSelectedPointsScrollPosition, GUILayout.Width(100), GUILayout.Height(100));
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("ID");
            GUILayout.Label("#pts");
            GUILayout.Label("Groups");
            GUILayout.Label("Purpose");
            EditorGUILayout.EndHorizontal();

            List<string> gridItems = new List<string>();
            List<string> ids = new List<string>();
            foreach (PointGroup currentPointGroup in selectedGroups.Values)
            {
                /*
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(currentPointGroup.mID);
                GUILayout.Label(currentPointGroup.mPoints.Count.ToString());
                GUILayout.Label(string.Join(",", currentPointGroup.mGroups.ToArray()));
                GUILayout.Label(string.Join(",", currentPointGroup.mPurposes.ToArray()));
                EditorGUILayout.EndHorizontal();
                 */
                string newItem = currentPointGroup.mID + " | " + currentPointGroup.mPoints.Count.ToString() + " | "
                    + string.Join(",", currentPointGroup.mGroups.ToArray()) + " | " + string.Join(",", currentPointGroup.mPurposes.ToArray());
                gridItems.Add(newItem);
                ids.Add(currentPointGroup.mID);
            }

            mSelectedGroupGridIndex = GUILayout.SelectionGrid(mSelectedGroupGridIndex, gridItems.ToArray(), 1);
            mSelectedGroupID = ids[mSelectedGroupGridIndex];
            //GUILayout.EndScrollView();
            GUILayout.EndVertical();
            //Whole area end
            //--
            EditorGUILayout.EndVertical();

            /*
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            myString = EditorGUILayout.TextField("Text Field", myString);

            groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            myBool = EditorGUILayout.Toggle("Toggle", myBool);
            myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
            EditorGUILayout.EndToggleGroup();
            */

            // Reselect the group
            mCurrentSelectedGroup = selectedLevelComponent.GetGroup(mSelectedGroupID);
        }
        else
        {
            GUILayout.Label("Please select an item of type LevelComponent from the Hierachy, then click me!");
        }
    }

    private void CreateNewGroup(LevelComponent inSelectedLevel)
    {
        // Create new, blank data.
        string defaultName = "ID" + inSelectedLevel.NextID;
        inSelectedLevel.NextID += 1;

        PointGroup newGroup = new PointGroup(defaultName);

        inSelectedLevel.SetPointGroupInfo(defaultName, newGroup);
    }

    private void AddNewPoint(LevelComponent inSelectedLevel, string inGroupID)
    {

    }
}
