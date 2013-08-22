using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class RunnerLevelEditor : EditorWindow
{
    private Vector2 mSelectedPointsScrollPosition;
    private Vector2 mSelectedGroupsScrollPosition;

    // Here is some fun in the sun current state variables
    private LevelComponent mLastChosenLevelComponent = null;
    private PointGroup mCurrentSelectedGroup = null;
    private int mSelectedGroupGridIndex = -1;
    private string mSelectedGroupID = "";
    private PointInfo mLastSelectedPointInfo = null;
    private int mSelectedPointIndex = -1;
    private GameObject mLastSelectedObject = null;

	[MenuItem("Window/Runner Level Editor")]
	public static void ShowWindow()
	{
		RunnerLevelEditor levelWindow = (RunnerLevelEditor)EditorWindow.GetWindow(typeof(RunnerLevelEditor));
        EditorApplication.update += levelWindow.Update;
	}

    void Update()
    {
        ObjectPositionUpdate();
        ObjectDestroyUpdate(false);
    }
    
    void ObjectPositionUpdate()
    {
        if (mLastChosenLevelComponent != null
            && mLastSelectedPointInfo != null 
			&& mLastSelectedObject != null
			&& mSelectedPointIndex != -1)
		{
            Vector3 newPosition = mLastSelectedObject.transform.position - mLastChosenLevelComponent.transform.position;
            Vector3 newLocalPosition = mLastSelectedObject.transform.localPosition;
            if (newPosition != mLastSelectedPointInfo.mPosition) {
                mLastSelectedPointInfo.mPosition = newPosition;
                mLastSelectedPointInfo.mLocalPosition = newLocalPosition;
                //@HACK constanlty re-add
                //mLastChosenLevelComponent.UpdatePointInfo(mSelectedGroupID, mLastSelectedPointInfo, mSelectedPointIndex);
            }
        } else if (mLastSelectedPointInfo == null && mLastSelectedObject != null)
            Debug.LogError("The point is empty, but you're selecting an object. idk something is fucked.");
    }

	void ObjectDestroyUpdate(bool inbForceDestroy = false)
	{
		if (mLastSelectedObject != null)
        {
            if (inbForceDestroy || !Selection.Contains(mLastSelectedObject))
            {
                GameObject.DestroyImmediate(mLastSelectedObject);
                mLastSelectedObject = null;
                mLastSelectedPointInfo = null;

                GameObject[] newSelection = new GameObject[] { };
                Selection.objects = newSelection;

                Resources.UnloadUnusedAssets();
            }
		}
	}


	void OnFocus()
	{
		// Remove delegate listener if it has previously
		// been assigned.
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

		// Add (or re-add) the delegate.
		SceneView.onSceneGUIDelegate += this.OnSceneGUI;
	}

	void OnDestroy()
	{
		// When the window is destroyed, remove the delegate
		// so that it will no longer do any drawing.
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
	}

	void OnSceneGUI(SceneView sceneView)
	{
		// Handles drawing here
		OnDrawGizmos();
		Handles.BeginGUI();
		Repaint();
		Handles.EndGUI();
	}

    // So, like, this method does sort of two things.
    // Set up your gui. What's the layout, where do things go, order, etc.
    // Pull from the gui the current selection values
	void OnGUI()
	{
		if (Selection.activeTransform != null && Selection.activeTransform.GetComponent<LevelComponent>() != null)
			mLastChosenLevelComponent = Selection.activeTransform.GetComponent<LevelComponent>();

		if (mLastChosenLevelComponent != null)
		{
			// Pull out the point groups
			List<PointGroup> selectedGroups = mLastChosenLevelComponent.PointGroups;

            // Here goes the form.

            // New Group
            GUILayout.BeginArea(new Rect(0, 0, 100, 40));
			if (GUILayout.Button("New Group"))
				CreateNewGroup(mLastChosenLevelComponent);
            GUILayout.EndArea();

			// Selected Items
            GUILayout.BeginArea(new Rect(0, 40, 100, 50));
			// ID
			GUILayout.Label("ID");
			string idText = "Enter ID Here";
			if (mCurrentSelectedGroup != null)
				idText = mCurrentSelectedGroup.mID;
			GUILayout.TextArea(idText);
            GUILayout.EndArea();

			// Groups
	        bool[] mSelectedGroupToggles = new bool[3] { false, false, false };

            if (mCurrentSelectedGroup != null && mCurrentSelectedGroup.mPurposes.Length > 0)
            {
                // Purpose Area
                GUILayout.BeginArea(new Rect(300, 40, 200, 100));
                EditorGUILayout.BeginToggleGroup("Purpose", true);
                for (int selectedPurposeIndex = 0; selectedPurposeIndex < (int)eSelectionTypes.Max; selectedPurposeIndex++)
                {
                    mCurrentSelectedGroup.mPurposes[selectedPurposeIndex] = EditorGUILayout.Toggle(
                        ((eSelectionTypes)selectedPurposeIndex).ToString(), 
                        mCurrentSelectedGroup.mPurposes[selectedPurposeIndex] );
                }
                EditorGUILayout.EndToggleGroup();
                GUILayout.EndArea();
            }

            // Points Area
            GUILayout.BeginArea(new Rect(0, 140, 100, 40));
            // New Point Button
            GUI.enabled = (mCurrentSelectedGroup != null);
            if (GUILayout.Button("New Point"))
                AddNewPoint(mLastChosenLevelComponent, mSelectedGroupID);
            GUI.enabled = true;
            GUILayout.EndArea();
            
            // Points Area
            GUILayout.BeginArea(new Rect(100, 140, 200, 100));
            EditorGUILayout.BeginVertical();
			GUILayout.Label("Points");
			mSelectedPointsScrollPosition = GUILayout.BeginScrollView(
				mSelectedPointsScrollPosition, false, true);
            GUILayout.BeginHorizontal();
			GUILayout.Label("#");
			GUILayout.Label("Type");
			GUILayout.Label("Pos");
            GUILayout.EndHorizontal();
			
			if (mLastChosenLevelComponent != null)
			{
				List<string> pointGridItems = new List<string>();
				PointGroup currentGroup = mLastChosenLevelComponent.GetGroup(mSelectedGroupID);
				if (currentGroup != null)
				{
					List<PointInfo> points = currentGroup.mPoints;
					for (int pointIndex = 0; pointIndex < points.Count; pointIndex++)
					{
						PointInfo currentPoint = points[pointIndex];

						string newItem = "id" + pointIndex + " | " + currentPoint.mLineType + " | "
							+ (currentPoint.mPosition + mLastChosenLevelComponent.transform.position);
						pointGridItems.Add(newItem);
					}

					int grabbedPointIndex = GUILayout.SelectionGrid(-1, pointGridItems.ToArray(), 1);

					// Attempt to re-select that item
                    if (grabbedPointIndex >= 0 && grabbedPointIndex < points.Count) {
                        PointInfo selectedPoint = points[grabbedPointIndex];
                        mSelectedPointIndex = grabbedPointIndex;
                        SelectPoint(mLastChosenLevelComponent, selectedPoint, mSelectedPointIndex);
					}
				}
			}


			GUILayout.EndScrollView();
			GUILayout.EndVertical();
            GUILayout.EndArea();

            //  Area for existing items
            GUILayout.BeginArea(new Rect(0, 240, 400, 100));
            mSelectedGroupsScrollPosition = GUILayout.BeginScrollView(
                mSelectedGroupsScrollPosition, false, true);
			GUILayout.Label("Existing Groups");
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("ID");
			GUILayout.Label("#pts");
			GUILayout.Label("Groups");
			GUILayout.Label("Purpose");
			EditorGUILayout.EndHorizontal();

			List<string> gridItems = new List<string>();
			List<string> ids = new List<string>();
			foreach (PointGroup currentPointGroup in selectedGroups) {
                string purposes = "";
                for (int purposeIndex = 0; purposeIndex < (int)eSelectionTypes.Max; purposeIndex++) {
                    if (currentPointGroup.mPurposes[purposeIndex])
                        purposes += ((eSelectionTypes)purposeIndex).ToString();
                }
				string newItem = currentPointGroup.mID + " | " + currentPointGroup.mPoints.Count.ToString() + " | "
                    + string.Join(",", currentPointGroup.mGroups.ToArray()) + " | " + purposes;
				gridItems.Add(newItem);
				ids.Add(currentPointGroup.mID);
			}
            

			mSelectedGroupGridIndex = GUILayout.SelectionGrid(mSelectedGroupGridIndex, gridItems.ToArray(), 1);
			if (mSelectedGroupGridIndex >= 0 && mSelectedGroupGridIndex < gridItems.Count)
				mSelectedGroupID = ids[mSelectedGroupGridIndex];
            else
                mSelectedGroupID = "";

            GUILayout.EndScrollView();
            GUILayout.EndArea();
            
			// Reselect the group
            PointGroup newGroup = mLastChosenLevelComponent.GetGroup(mSelectedGroupID);
			if (newGroup != mCurrentSelectedGroup) {
                // Reset the selected point
                mSelectedPointIndex = -1;
                ObjectDestroyUpdate(true);
                // Select the new point
                mCurrentSelectedGroup = newGroup;
            }

            GUILayout.BeginArea(new Rect(0, 340, 300, 50));
            GUILayout.BeginHorizontal();
            GUI.enabled = (mCurrentSelectedGroup != null);
            if (GUILayout.Button("Delete Selected Group"))
                DeletePointGroup(mLastChosenLevelComponent, mCurrentSelectedGroup);
            GUI.enabled = (mLastSelectedPointInfo != null);
            if (GUILayout.Button("Delete Selected Point"))
                DeletePointInfo(mLastChosenLevelComponent, mCurrentSelectedGroup, mLastSelectedPointInfo);
            GUI.enabled = false;
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            // Done with all the layouts.

			OnDrawGizmos();
		}
		else
		{
			GUILayout.Label("Please select an item of type LevelComponent from the Hierachy, then click me!");
		}
	}

	private void OnDrawGizmos()
	{
		// Iterate through every group, and draw the given gizmo out.
		if (mLastChosenLevelComponent != null)
		{
			foreach (PointGroup currentGroup in mLastChosenLevelComponent.PointGroups)
			{
				for (int pointIndex = 0; pointIndex < currentGroup.mPoints.Count - 1; pointIndex++)
				{
                    Handles.DrawLine(currentGroup.mPoints[pointIndex].mPosition + mLastChosenLevelComponent.transform.position,
                        currentGroup.mPoints[pointIndex + 1].mPosition + mLastChosenLevelComponent.transform.position);
					//Handles.DrawLine(currentGroup.mPoints[pointIndex].mPosition, currentGroup.mPoints[pointIndex + 1].mPosition);
				}
			}
		}
	}

    private void DeletePointGroup(LevelComponent inSelectedLevel, PointGroup inPointGroupToDelete) {
        inSelectedLevel.DeletePointGroup(inPointGroupToDelete);
    }

    private void DeletePointInfo(LevelComponent inSelectedLevel, PointGroup inSelectedPointGroup, PointInfo inPointInfoToDelete) {
        inSelectedLevel.DeletePointInfo(inSelectedPointGroup, inPointInfoToDelete);
    }

	private void CreateNewGroup(LevelComponent inSelectedLevel) {
		// Create new, blank data.
        string defaultName = "ID" + inSelectedLevel.NextID;

		PointGroup newGroup = new PointGroup(defaultName);

		inSelectedLevel.SetPointGroupInfo(defaultName, newGroup);
	}

	private void AddNewPoint(LevelComponent inSelectedLevel, string inGroupID) {
        PointInfo generatedPoint = inSelectedLevel.AddNewPoint(inGroupID, Vector3.zero);//inSelectedLevel.transform.position);
        SelectPoint(inSelectedLevel, generatedPoint, inSelectedLevel.GetNextPointNum(inGroupID));
	}

    private void SelectPoint(LevelComponent inSelectedLevel, PointInfo inPointToselect, int inIndex) {
        // Clean out the old point
        ObjectDestroyUpdate(true);

        // Grab the current
        mLastSelectedPointInfo = inPointToselect;

        // Generate a new transform
        GameObject selectedPoint = new GameObject("point" + inIndex);
        selectedPoint.transform.SetParent(inSelectedLevel.gameObject);
        selectedPoint.transform.position = (mLastSelectedPointInfo.mPosition + inSelectedLevel.transform.position);
		mLastSelectedObject = selectedPoint;

        // Reselect gameobject
        GameObject[] selections = new GameObject[] { selectedPoint };
        Selection.objects = selections;
    }
}
