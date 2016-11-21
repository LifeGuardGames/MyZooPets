//using UnityEditor;
using UnityEngine;

/// <summary>
/// Normalize particles based on the scene's UI canvas camera size
/// </summary>
public class SceneParticleScaler : MonoBehaviour {
	public ParticleSystem particle;
	private float scalingValue;

    void Start() {
		scalingValue = GetScaleFactorByScene(SceneUtils.CurrentScene);
		
		//Scale Shuriken Particles Values
		ParticleSystem[] systems = gameObject.GetComponentsInChildren<ParticleSystem>(true);

		foreach(ParticleSystem ps in systems) {
			ScaleParticleValues(ps, gameObject);
		}
	}
	
	public float GetScaleFactorByScene(string levelName) {
		float baseModifier = 10f;		// Canvas size of the bedroom and yard
		if(levelName == SceneUtils.INHALERGAME) {   // Canvas size: 16.14
			return 16.14f / baseModifier;
		}
		else if(levelName == SceneUtils.TRIGGERNINJA) { // Canvas size: 5
			return 5f / baseModifier;
		}
		else if(levelName == SceneUtils.MEMORY) {   // Canvas size: 10
			return 1f;
		}
		else if(levelName == SceneUtils.RUNNER) {   // Canvas size: 26
			return 26f / baseModifier;
		}
		else if(levelName == SceneUtils.DOCTORMATCH) {  // Canvas size: 150
			return 150f / baseModifier;
		}
		else if(levelName == SceneUtils.SHOOTER) {  // Canvas size: 3.140527
			return 3.140527f / baseModifier;
		}
		else if(levelName == SceneUtils.MICROMIX) { // Canvas size: 5
			return 5f / baseModifier;
		}
		else {
			return 1f;
		}
	}

	private void ScaleParticleValues(ParticleSystem ps, GameObject parent) {
		//Particle System
		ps.startSize *= scalingValue;
		ps.gravityModifier *= scalingValue;
		if(ps.startSpeed > 0.01f)
			ps.startSpeed *= scalingValue;
		if(ps.gameObject != parent)
			ps.transform.localPosition *= scalingValue;

		/*
		SerializedObject psSerial = new SerializedObject(ps);

		//Scale Emission Rate if set on Distance
		if(psSerial.FindProperty("EmissionModule.enabled").boolValue && psSerial.FindProperty("EmissionModule.m_Type").intValue == 1) {
			psSerial.FindProperty("EmissionModule.rate.scalar").floatValue /= scalingValue;
		}

		//Scale Size By Speed Module
		if(psSerial.FindProperty("SizeBySpeedModule.enabled").boolValue) {
			psSerial.FindProperty("SizeBySpeedModule.range.x").floatValue *= scalingValue;
			psSerial.FindProperty("SizeBySpeedModule.range.y").floatValue *= scalingValue;
		}

		//Scale Velocity Module
		if(psSerial.FindProperty("VelocityModule.enabled").boolValue) {
			psSerial.FindProperty("VelocityModule.x.scalar").floatValue *= scalingValue;
			IterateKeys(psSerial.FindProperty("VelocityModule.x.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("VelocityModule.x.maxCurve").animationCurveValue);
			psSerial.FindProperty("VelocityModule.y.scalar").floatValue *= scalingValue;
			IterateKeys(psSerial.FindProperty("VelocityModule.y.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("VelocityModule.y.maxCurve").animationCurveValue);
			psSerial.FindProperty("VelocityModule.z.scalar").floatValue *= scalingValue;
			IterateKeys(psSerial.FindProperty("VelocityModule.z.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("VelocityModule.z.maxCurve").animationCurveValue);
		}

		//Scale Limit Velocity Module
		if(psSerial.FindProperty("ClampVelocityModule.enabled").boolValue) {
			psSerial.FindProperty("ClampVelocityModule.x.scalar").floatValue *= scalingValue;
			IterateKeys(psSerial.FindProperty("ClampVelocityModule.x.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("ClampVelocityModule.x.maxCurve").animationCurveValue);
			psSerial.FindProperty("ClampVelocityModule.y.scalar").floatValue *= scalingValue;
			IterateKeys(psSerial.FindProperty("ClampVelocityModule.y.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("ClampVelocityModule.y.maxCurve").animationCurveValue);
			psSerial.FindProperty("ClampVelocityModule.z.scalar").floatValue *= scalingValue;
			IterateKeys(psSerial.FindProperty("ClampVelocityModule.z.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("ClampVelocityModule.z.maxCurve").animationCurveValue);

			psSerial.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue *= scalingValue;
			IterateKeys(psSerial.FindProperty("ClampVelocityModule.magnitude.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("ClampVelocityModule.magnitude.maxCurve").animationCurveValue);
		}

		//Scale Force Module
		if(psSerial.FindProperty("ForceModule.enabled").boolValue) {
			psSerial.FindProperty("ForceModule.x.scalar").floatValue *= scalingValue;
			IterateKeys(psSerial.FindProperty("ForceModule.x.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("ForceModule.x.maxCurve").animationCurveValue);
			psSerial.FindProperty("ForceModule.y.scalar").floatValue *= scalingValue;
			IterateKeys(psSerial.FindProperty("ForceModule.y.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("ForceModule.y.maxCurve").animationCurveValue);
			psSerial.FindProperty("ForceModule.z.scalar").floatValue *= scalingValue;
			IterateKeys(psSerial.FindProperty("ForceModule.z.minCurve").animationCurveValue);
			IterateKeys(psSerial.FindProperty("ForceModule.z.maxCurve").animationCurveValue);
		}

		//Scale Shape Module
		if(psSerial.FindProperty("ShapeModule.enabled").boolValue) {
			psSerial.FindProperty("ShapeModule.boxX").floatValue *= scalingValue;
			psSerial.FindProperty("ShapeModule.boxY").floatValue *= scalingValue;
			psSerial.FindProperty("ShapeModule.boxZ").floatValue *= scalingValue;
			psSerial.FindProperty("ShapeModule.radius").floatValue *= scalingValue;

			//Create a new scaled Mesh if there is a Mesh reference
			//(ShapeModule.type 6 == Mesh)
			if(psSerial.FindProperty("ShapeModule.type").intValue == 6) {
				Object obj = psSerial.FindProperty("ShapeModule.m_Mesh").objectReferenceValue;
				if(obj != null) {
					Mesh mesh = (Mesh)obj;
					string assetPath = AssetDatabase.GetAssetPath(mesh);
					string name = assetPath.Substring(assetPath.LastIndexOf("/") + 1);

					//Mesh to use
					Mesh meshToUse = null;
					bool createScaledMesh = true;
					float meshScale = scalingValue;

					//Mesh has already been scaled: extract scaling value and re-scale base effect
					if(name.Contains("(scaled)")) {
						string scaleStr = name.Substring(name.LastIndexOf("x") + 1);
						scaleStr = scaleStr.Remove(scaleStr.IndexOf(" (scaled).asset"));

						float oldScale = float.Parse(scaleStr);
						if(oldScale != 0) {
							meshScale = oldScale * scalingValue;

							//Check if there's already a mesh with the correct scale
							string unscaledName = assetPath.Substring(0, assetPath.LastIndexOf(" x"));
							assetPath = unscaledName;
							string newPath = assetPath + " x" + meshScale + " (scaled).asset";
							Mesh alreadyScaledMesh = (Mesh)AssetDatabase.LoadAssetAtPath(newPath, typeof(Mesh));
							if(alreadyScaledMesh != null) {
								meshToUse = alreadyScaledMesh;
								createScaledMesh = false;
							}
							else
							//Load original unscaled mesh
							{
								Mesh orgMesh = (Mesh)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Mesh));
								if(orgMesh != null) {
									mesh = orgMesh;
								}
							}
						}
					}
					else
					//Verify if original mesh has already been scaled to that value
					{
						string newPath = assetPath + " x" + meshScale + " (scaled).asset";
						Mesh alreadyScaledMesh = (Mesh)AssetDatabase.LoadAssetAtPath(newPath, typeof(Mesh));
						if(alreadyScaledMesh != null) {
							meshToUse = alreadyScaledMesh;
							createScaledMesh = false;
						}
					}

					//Duplicate and scale mesh vertices if necessary
					if(createScaledMesh) {
						string newMeshPath = assetPath + " x" + meshScale + " (scaled).asset";
						meshToUse = (Mesh)AssetDatabase.LoadAssetAtPath(newMeshPath, typeof(Mesh));
						if(meshToUse == null) {
							meshToUse = DuplicateAndScaleMesh(mesh, meshScale);
							AssetDatabase.CreateAsset(meshToUse, newMeshPath);
						}
					}

					//Apply new Mesh
					psSerial.FindProperty("ShapeModule.m_Mesh").objectReferenceValue = meshToUse;
				}
			}
		}

		//Apply Modified Properties
		psSerial.ApplyModifiedProperties();
		*/
	}

	//Iterate and Scale Keys (Animation Curve)
	private void IterateKeys(AnimationCurve curve) {
		for(int i = 0; i < curve.keys.Length; i++) {
			curve.keys[i].value *= scalingValue;
		}
	}

	//Create Scaled Mesh
	private Mesh DuplicateAndScaleMesh(Mesh mesh, float Scale) {
		Mesh scaledMesh = new Mesh();

		Vector3[] scaledVertices = new Vector3[mesh.vertices.Length];
		for(int i = 0; i < scaledVertices.Length; i++) {
			scaledVertices[i] = mesh.vertices[i] * Scale;
		}
		scaledMesh.vertices = scaledVertices;

		scaledMesh.normals = mesh.normals;
		scaledMesh.tangents = mesh.tangents;
		scaledMesh.triangles = mesh.triangles;
		scaledMesh.uv = mesh.uv;
		scaledMesh.uv2 = mesh.uv2;
		scaledMesh.colors = mesh.colors;

		return scaledMesh;
	}
}
