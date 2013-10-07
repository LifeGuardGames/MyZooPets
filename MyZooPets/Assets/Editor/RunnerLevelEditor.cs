using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class RunnerLevelEditor : EditorWindow
{
    private Vector2 mSelectedPointsScrollPosition;
    private Vector2 mSelectedGroupsScrollPosition;
    private Vector2 mSelectedBundlesScrollPosition;

    // Here is some fun in the sun current state variables
    private LevelComponent mLastChosenLevelComponent = null;
    private PointGroup mCurrentSelectedGroup = null;
    private int mSelectedGroupGridIndex = -1;
    private int mSelectedGroupIndex = -1;
    private PointInfo mLastSelectedPointInfo = null;
    private int mSelectedPointIndex = -1;
    private GameObject mLastSelectedObject = null;
    private int mLastSelectedBundleIndex = -1;
    private Bundle mLastSelectedBundle = null;

    private int mNewBundleID = 0;
    private float mNewSpawnChance = 0f;
    
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

                // Tell the editor to re-serialize.
                EditorUtility.SetDirty(mLastChosenLevelComponent);
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

    void OnSelectionChange(){
        // mSelectedGroupIndex = -1;
    }


    // So, like, this method does sort of two things.
    // Set up your gui. What's the layout, where do things go, order, etc.
    // Pull from the gui the current selection values
	void OnGUI()
	{
        GUI.changed = false;
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

            // Depending on if a group is selected
            if (mCurrentSelectedGroup != null)
            {
                // Spawn Type area
                GUILayout.BeginArea(new Rect(300, 0, 200, 75));
                GUILayout.BeginVertical();
                string[] selectionTypes = new string[(int)eSpawnType.Max];
                for (int selectedPurposeIndex = 0; selectedPurposeIndex < (int)eSpawnType.Max; selectedPurposeIndex++)
                {
                    selectionTypes[selectedPurposeIndex] = ((eSpawnType)selectedPurposeIndex).ToString();
                }
                GUILayout.Label("Spawn Type");
                mCurrentSelectedGroup.mSpawnType = (eSpawnType)GUILayout.SelectionGrid((int)mCurrentSelectedGroup.mSpawnType, selectionTypes, 2);
                GUILayout.EndVertical();
                GUILayout.EndArea();

                // Bundle ID area
                GUILayout.BeginArea(new Rect(300, 75, 200, 75));
                GUILayout.BeginVertical();
                GUILayout.Label("Bundle ID");
                mCurrentSelectedGroup.mBundleID = (int)GUILayout.HorizontalSlider(mCurrentSelectedGroup.mBundleID, 0, 100);
                GUILayout.EndVertical();
                GUILayout.EndArea();

                // Curve Type area
                GUILayout.BeginArea(new Rect(300, 150, 200, 75));
                GUILayout.BeginVertical();
                string[] curveTypes = new string[(int)eCurveType.Max];
                for (int curveIndex = 0; curveIndex < (int)eCurveType.Max; curveIndex++) {
                    curveTypes[curveIndex] = ((eCurveType)curveIndex).ToString();
                }
                GUILayout.Label("Curve Type (will modify pts)");
                eCurveType selectedCurve = (eCurveType)GUILayout.SelectionGrid((int)mCurrentSelectedGroup.mCurveType, curveTypes, 2);
                SelectCurveType(mLastChosenLevelComponent, mCurrentSelectedGroup, selectedCurve);
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }

            // Points Area
            GUILayout.BeginArea(new Rect(0, 140, 100, 40));
            // New Point Button
            GUI.enabled = (mCurrentSelectedGroup != null);
            if (GUILayout.Button("New Point"))
                AddNewPoint(mLastChosenLevelComponent, mSelectedGroupIndex);
            GUI.enabled = true;
            GUILayout.EndArea();
            
            // Points Area
            GUILayout.BeginArea(new Rect(100, 140, 200, 100));
            EditorGUILayout.BeginVertical();
			GUILayout.Label("Points");
			mSelectedPointsScrollPosition = GUILayout.BeginScrollView(
				mSelectedPointsScrollPosition, false, true);
            GUILayout.BeginHorizontal();
			GUILayout.Label("Index");
			GUILayout.Label("Pos");
            GUILayout.EndHorizontal();
			
			List<string> pointGridItems = new List<string>();
            PointGroup currentGroup = null;

            currentGroup = mLastChosenLevelComponent.GetGroup(mSelectedGroupIndex);

            if (currentGroup != null)
            {
                List<PointInfo> points = currentGroup.mPoints;
                for (int pointIndex = 0; pointIndex < points.Count; pointIndex++)
                {
                    PointInfo currentPoint = points[pointIndex];

                    string newItem = "index" + pointIndex + " | " + (currentPoint.mPosition + mLastChosenLevelComponent.transform.position);
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
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
            GUILayout.EndArea();

            //====================Area for existing items========================
            GUILayout.BeginArea(new Rect(0, 240, 400, 100));
            mSelectedGroupsScrollPosition = GUILayout.BeginScrollView(
                mSelectedGroupsScrollPosition, false, true);
			GUILayout.Label("Existing Groups");
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Index");
			GUILayout.Label("#pts");
			GUILayout.Label("Bundle");
            GUILayout.Label("SpawnType");
            GUILayout.Label("CurveType");
			EditorGUILayout.EndHorizontal();

			List<string> gridItems = new List<string>();

            for(int groupIndex = 0; groupIndex < selectedGroups.Count; groupIndex++){
                PointGroup currentPointGroup = selectedGroups[groupIndex];

                string purposes = currentPointGroup.mSpawnType.ToString();
				string newItem = "index" + groupIndex + " | " + currentPointGroup.mPoints.Count.ToString() + " | "
                    + currentPointGroup.mBundleID + " | " + purposes + " | " + currentPointGroup.mCurveType.ToString();
				gridItems.Add(newItem);
			}

			mSelectedGroupGridIndex = GUILayout.SelectionGrid(mSelectedGroupGridIndex, gridItems.ToArray(), 1);
			if (mSelectedGroupGridIndex >= 0 && mSelectedGroupGridIndex < gridItems.Count)
                mSelectedGroupIndex = mSelectedGroupGridIndex;

            GUILayout.EndScrollView();
            GUILayout.EndArea();
            
			// Reselect the group
            PointGroup newGroup = null;

            newGroup = mLastChosenLevelComponent.GetGroup(mSelectedGroupIndex);

			if (newGroup != null && newGroup != mCurrentSelectedGroup) {
                // Reset the selected point
                mSelectedPointIndex = -1;
                ObjectDestroyUpdate(true);
                // Select the new point
                mCurrentSelectedGroup = newGroup;
            }
            //==================================================================

            GUILayout.BeginArea(new Rect(0, 340, 300, 50));
            GUILayout.BeginHorizontal();
            GUI.enabled = (mCurrentSelectedGroup != null);
            if (GUILayout.Button("Delete Selected Group"))
                DeletePointGroup(mLastChosenLevelComponent, mCurrentSelectedGroup);
            GUI.enabled = (mLastSelectedPointInfo != null);
            if (GUILayout.Button("Delete Selected Point"))
                DeletePointInfo(mLastChosenLevelComponent, mCurrentSelectedGroup, mLastSelectedPointInfo);
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            // Bundle Area
            GUILayout.BeginArea(new Rect(0, 390, 300, 100));
            mSelectedBundlesScrollPosition = GUILayout.BeginScrollView(
                mSelectedBundlesScrollPosition, false, true);
            GUILayout.Label("Bundles");
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("BundleID");
            GUILayout.Label("Spawn Chance");
            EditorGUILayout.EndHorizontal();

            List<Bundle> currentBundles = mLastChosenLevelComponent.Bundles;
            List<string> bundleGridItems = new List<string>();
            foreach (Bundle currentBundle in currentBundles) {
                string newItem = currentBundle.mBundleID + " | " + currentBundle.mSpawnChance;
                bundleGridItems.Add(newItem);
            }

            mLastSelectedBundleIndex = GUILayout.SelectionGrid(mLastSelectedBundleIndex, bundleGridItems.ToArray(), 4);
            if (mLastSelectedBundleIndex >= 0 && mLastSelectedBundleIndex < currentBundles.Count)
                mLastSelectedBundle = currentBundles[mLastSelectedBundleIndex];
            else
                mLastSelectedBundle = null;

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            // Bundle buttons
            // Add
            GUILayout.BeginArea(new Rect(300, 390, 200, 100));
            GUILayout.BeginVertical();
            GUILayout.Label("ID " + mNewBundleID);
            mNewBundleID = (int)GUILayout.HorizontalSlider(mNewBundleID, 0, 100);
            GUILayout.Label("Spawn Chance " + mNewSpawnChance);
            mNewSpawnChance = GUILayout.HorizontalSlider(mNewSpawnChance, 0f, 100f);
            if (GUILayout.Button("Add/Update Bundle")) {
                mLastChosenLevelComponent.SetBundleChance(mNewBundleID, mNewSpawnChance);
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();

            // Delete
            GUILayout.BeginArea(new Rect(0, 490, 300, 50));
            GUILayout.BeginHorizontal();
            GUI.enabled = (mLastSelectedBundle != null);
            if (GUILayout.Button("Delete Selected Bundle"))
                mLastChosenLevelComponent.RemoveBundle(mLastSelectedBundle.mBundleID);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            // Done with all the layouts.

			OnDrawGizmos();

            if (GUI.changed)
                EditorUtility.SetDirty(mLastChosenLevelComponent);
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
                switch (currentGroup.mCurveType) {
                    case (eCurveType.Point): {
	    	            Handles.color = Color.white;
                        for (int pointIndex = 0; pointIndex < currentGroup.mPoints.Count; pointIndex++)
			            {
                            Vector3 pointPosition = currentGroup.mPoints[pointIndex].mPosition + mLastChosenLevelComponent.transform.position;
                            Handles.CircleCap(0, pointPosition, Quaternion.LookRotation(Vector3.right), 0.75f);
                            Handles.DrawSolidDisc(pointPosition, Vector3.right, 0.1f);
                        }
                    }
                    break;

                    case (eCurveType.Linear): {
                        Handles.color = Color.red;

                        for (int pointIndex = 0; pointIndex < currentGroup.mPoints.Count - 1; pointIndex++)
			            {
                            Handles.DrawLine(currentGroup.mPoints[pointIndex].mPosition + mLastChosenLevelComponent.transform.position,
                                currentGroup.mPoints[pointIndex + 1].mPosition + mLastChosenLevelComponent.transform.position);
			            }
                        break;
                    }

                    case (eCurveType.Quadratic): {
                        Handles.color = Color.cyan;
                        if (currentGroup.mPoints.Count == 3) {
                            Vector3 beginPoint = currentGroup.mPoints[0].mPosition + mLastChosenLevelComponent.transform.position;
                            Vector3 anchorPoint = currentGroup.mPoints[1].mPosition + mLastChosenLevelComponent.transform.position;
                            Vector3 endPoint = currentGroup.mPoints[2].mPosition + mLastChosenLevelComponent.transform.position;
                            DisplayCurve(true, beginPoint, anchorPoint, endPoint, Vector3.zero);
                        } else
                            Debug.LogError("Cannot draw cubic bezier with " + currentGroup.mPoints.Count + " points when expecting 3!");
                        break;
                    }

                    case (eCurveType.Cubic): {
                        Handles.color = Color.green;
                        if (currentGroup.mPoints.Count == 4) {
                            Vector3 beginPoint = currentGroup.mPoints[0].mPosition + mLastChosenLevelComponent.transform.position;
                            Vector3 anchorPoint1 = currentGroup.mPoints[1].mPosition + mLastChosenLevelComponent.transform.position;
                            Vector3 anchorPoint2 = currentGroup.mPoints[2].mPosition + mLastChosenLevelComponent.transform.position;
                            Vector3 endPoint = currentGroup.mPoints[3].mPosition + mLastChosenLevelComponent.transform.position;
                            DisplayCurve(false, beginPoint, anchorPoint1, anchorPoint2, endPoint);
                        } else
                            Debug.LogError("Cannot draw cubic bezier with " + currentGroup.mPoints.Count + " points when expecting 4!");
                        break;
                    }
                }

				
			}
		}
	}

    private void DisplayCurve(bool inbQuadratic, Vector3 inA, Vector3 inB, Vector3 inC, Vector3 inD) {
        Vector3 lastCurvePoint = Vector3.zero;
        bool bOnNext = false;
        for (int pointIndex = 0; pointIndex <= 1000; pointIndex++) {
            float currentTime = (float)pointIndex / 1000f;

            Vector3 nextCurvePoint;
            if (inbQuadratic)
                nextCurvePoint = LevelManager.CalculateQuadtraticPoint(currentTime, inA, inB, inC);
            else
                nextCurvePoint = LevelManager.CalculateBezierPoint(currentTime, inA, inB, inC, inD);

            if (bOnNext)
                Handles.DrawLine(nextCurvePoint, lastCurvePoint);
            else
                bOnNext = true;
            lastCurvePoint = nextCurvePoint;
        }

        // Also display each point
        Handles.CircleCap(0, inA, Quaternion.LookRotation(Vector3.right), 0.75f);
        Handles.DrawSolidDisc(inA, Vector3.right, 0.1f);
        Handles.CircleCap(0, inB, Quaternion.LookRotation(Vector3.right), 0.75f);
        Handles.DrawSolidDisc(inB, Vector3.right, 0.1f);
        Handles.CircleCap(0, inC, Quaternion.LookRotation(Vector3.right), 0.75f);
        Handles.DrawSolidDisc(inC, Vector3.right, 0.1f);
        if (!inbQuadratic) {
            Handles.CircleCap(0, inD, Quaternion.LookRotation(Vector3.right), 0.75f);
            Handles.DrawSolidDisc(inD, Vector3.right, 0.1f);
        }

    }

    private void DeletePointGroup(LevelComponent inSelectedLevel, PointGroup inPointGroupToDelete) {
        inSelectedLevel.DeletePointGroup(inPointGroupToDelete);
        mSelectedGroupIndex = -1;
    }

    private void DeletePointInfo(LevelComponent inSelectedLevel, PointGroup inSelectedPointGroup, PointInfo inPointInfoToDelete) {
        inSelectedLevel.DeletePointInfo(inSelectedPointGroup, inPointInfoToDelete);
    }

	private void CreateNewGroup(LevelComponent inSelectedLevel) {
		// Create new, blank data.
        // string defaultName = "ID" + inSelectedLevel.NextID;

		// PointGroup newGroup = new PointGroup(defaultName);
        PointGroup newGroup = new PointGroup();

		inSelectedLevel.SetPointGroupInfo(newGroup);
	}

	private void AddNewPoint(LevelComponent inSelectedLevel, int inGroupIndex) {
        PointInfo generatedPoint = inSelectedLevel.AddNewPoint(inGroupIndex, Vector3.zero);//inSelectedLevel.transform.position);
        SelectPoint(inSelectedLevel, generatedPoint, inSelectedLevel.GetNextPointNum(inGroupIndex));
	}

    private void AddNewPoint(LevelComponent inSelectedLevel, PointGroup inGroup){
        PointInfo generatedPoint = inSelectedLevel.AddNewPoint(inGroup, Vector3.zero);//inSelectedLevel.transform.position);
        SelectPoint(inSelectedLevel, generatedPoint, inSelectedLevel.GetNextPointNum(inGroup));
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

    private void SelectCurveType(LevelComponent inComponent, PointGroup inGroup, eCurveType inNewSelectedCurve) {
        eCurveType currentCurveType = inGroup.mCurveType;
        if (currentCurveType != inNewSelectedCurve) {
            // Determine the new point.
            switch (inNewSelectedCurve) {
                case eCurveType.Point:
                    CutOrAddGroupPointsTo(inComponent, inGroup, 1);
                    break;
                case eCurveType.Quadratic:
                    CutOrAddGroupPointsTo(inComponent, inGroup, 3);
                    break;
                case eCurveType.Cubic:
                    CutOrAddGroupPointsTo(inComponent, inGroup, 4);
                    break;
            }

            // Set it.
            inGroup.mCurveType = inNewSelectedCurve;
        }
    }

    private void CutOrAddGroupPointsTo(LevelComponent inComponent, PointGroup inGroup, int inNumPoints) {
        List<PointInfo> groupPoints = inGroup.mPoints;

        // Cut our points down to the given number
        while (groupPoints.Count > inNumPoints) {
            groupPoints.RemoveAt(groupPoints.Count - 1);
        }

        // Increment our points up to the given number
        while (groupPoints.Count < inNumPoints) {
            AddNewPoint(inComponent, inGroup);
        }
    }
}
