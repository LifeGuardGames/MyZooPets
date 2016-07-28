using UnityEngine;

// Class that just stores information needed for dragging things
public class DragItemParentMeta : Singleton<DragItemParentMeta> {
	public Transform DragItemParent;
	public Canvas GuiCanvas;
	public Camera GuiCamera;
}
