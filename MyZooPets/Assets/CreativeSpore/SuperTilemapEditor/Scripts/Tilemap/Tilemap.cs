﻿
// Enable a cache dictionary to improve the access to tilechunks, but it keeps the max distance of a tilechunk to 2^16 tilechunks of distance from origin
// Fair enough unless you have a quantum computer
#define ENABLE_TILECHUNK_CACHE_DIC

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

namespace CreativeSpore.SuperTilemapEditor
{

    public enum eColliderType
    {
        None,
        _2D,
        _3D
    };

    public enum e2DColliderType
    {
        EdgeCollider2D,
        PolygonCollider2D
    };

    [System.Flags]
    public enum eTileFlags
    {
        None = 0,
        Updated = 1,
        Rot90 = 2,
        FlipV = 4,
        FlipH = 8,
    }

    [AddComponentMenu("SuperTilemapEditor/Tilemap", 10)]
    [DisallowMultipleComponent]
    [ExecuteInEditMode] //NOTE: this is needed so OnDestroy is called and there is no memory leaks
    public class Tilemap : MonoBehaviour
    {
        /// <summary>
        /// In the worst case scenario, each tile subdivided in 4 sub tiles, the maximum value should be <= 62.
        /// The mesh vertices have a limit of 65535 62(width)*62(height)*4(subtiles)*4(vertices) = 61504
        /// Warning: changing this value will break the tilemaps made so far. Change this before creating them.
        /// </summary>
        public const int k_chunkSize = 60; 
        public const string k_UndoOpName = "Paint Op. ";

        #region Public Properties
        public Tileset Tileset
        {
            get { return m_tileset; }
            set
            {
                bool hasChanged = m_tileset != value;
                m_tileset = value;
                if (hasChanged)
                {
                    if (Tileset != null && CellSize == default(Vector2))
                    {
                        CellSize = m_tileset.TilePxSize / m_tileset.PixelsPerUnit;
                    }
                }
            }
        }

        public Material Material
        {
            get
            {
                if (m_material == null)
                {
                    m_material = Resources.GetBuiltinResource<Material>("Sprites-Default.mat");
                }
                return m_material;
            }

            set
            {
                if (value != null && m_material != value)
                {
                    m_material = value;
                    Refresh();
                }
            }
        }


        /// <summary>
        /// Color applied to the material before rendering
        /// </summary>
        public Color TintColor 
        { 
            get { return m_tintColor; }
            set { m_tintColor = value;} 
        }

        /// <summary>
        /// Show the tilemap grid
        /// </summary>
        public bool ShowGrid = true;

        //NOTE: the inner padding fix the zoom imperfection, but as long as zoom is bigger, the bigger this value has to be        
        /// <summary>
        /// The size, in pixels, the tile UV will be stretched. Use this to fix pixel precision artifacts when tiles have no padding border in the atlas.
        /// </summary>
        public float InnerPadding = 0.1f;
        /// <summary>
        /// The depth size of the collider. You need to call Refresh(false, true) after changing this value to refresh the collider.
        /// </summary>
        public float ColliderDepth = 0.1f;
        /// <summary>
        /// Set the colliders for this tilemap
        /// </summary>
        public eColliderType ColliderType = eColliderType.None;
        /// <summary>
        /// The type of collider used when ColliderType is eColliderType._2D
        /// </summary>
        public e2DColliderType Collider2DType = e2DColliderType.EdgeCollider2D;
        /// <summary>
        /// Sets the isTrigger property of the collider. You need call Refresh to update the colliders after changing it.
        /// </summary>
        public bool IsTrigger { get { return m_isTrigger; } set { m_isTrigger = true; } }
        /// <summary>
        /// Show the collider normals
        /// </summary>
        public bool ShowColliderNormals = true;
        /// <summary>
        /// The size of the cell containing the tiles. You should call Refresh() after changing this value to apply the effect.
        /// </summary>
        public Vector2 CellSize { get { return m_cellSize; } set { m_cellSize = value; } }
        /// <summary>
        /// Return the size of the map in units
        /// </summary>
        public Bounds MapBounds { get { return m_mapBounds; } }
        /// <summary>
        /// Lock painting tiles only inside the map bounds
        /// </summary>
        public bool AllowPaintingOutOfBounds { get { return m_allowPaintingOutOfBounds; } set { m_allowPaintingOutOfBounds = value; } }
        /// <summary>
        /// If true, undo will be registered when painting to be able to undo any change. But activating this would become a performance killer in big maps.
        /// For these cases, disabling the undo would be a good option to improve the performance while painting
        /// </summary>      
        public bool EnableUndoWhilePainting { get { return m_enableUndoWhilePainting; } set { m_enableUndoWhilePainting = value; } }
        /// <summary>
        /// Return the minimum horizontal grid position of the tilemap area
        /// </summary>
        public int MinGridX { get { return m_minGridX; } }
        /// <summary>
        /// Return the minimum vertical grid position of the tilemap area
        /// </summary>
        public int MinGridY { get { return m_minGridY; } }
        /// <summary>
        /// Return the maximum horizontal grid position of the tilemap area
        /// </summary>
        public int MaxGridX { get { return m_maxGridX; } }
        /// <summary>
        /// Return the maximum vertical grid position of the tilemap area
        /// </summary>
        public int MaxGridY { get { return m_maxGridY; } }      
        /// <summary>
        /// Return the horizontal size of the grid in tiles
        /// </summary>
        public int GridWidth { get { return m_maxGridX - m_minGridX + 1; } }
        /// <summary>
        /// Return the vertical size of the grid in tiles
        /// </summary>
        public int GridHeight { get { return m_maxGridY - m_minGridY + 1; } }
        /// <summary>
        /// Returns the parent tilemap group the tilemap is children of
        /// </summary>
        public TilemapGroup ParentTilemapGroup { get { return m_parentTilemapGroup; } }
        public bool IsUndoEnabled = false;


        public int SortingLayerID
        {
            get { return m_sortingLayer; }
            set
            {
                int prevValue = m_sortingLayer;
                m_sortingLayer = value;
#if UNITY_EDITOR
                m_sortingLayerName = EditorUtils.GetSortingLayerNameById(m_sortingLayer);
#endif
                if (m_sortingLayer != prevValue) 
                    RefreshChunksSortingAttributes();
            }
        }

        public string SortingLayerName
        {
            get { return m_sortingLayerName; }
            set
            {
                m_sortingLayerName = value;
#if UNITY_EDITOR
                m_sortingLayer = EditorUtils.GetSortingLayerIdByName(m_sortingLayerName);                
#endif
                RefreshChunksSortingAttributes();
            }
        }        

        public int OrderInLayer
        {
            get { return m_orderInLayer; }
            set
            {
                int prevValue = m_orderInLayer;
                m_orderInLayer = (value << 16) >> 16; // convert from int32 to int16 keeping sign
                if (m_orderInLayer != prevValue)
                    RefreshChunksSortingAttributes();
            }
        }

        public void RefreshChunksSortingAttributes()
        {
            foreach(TilemapChunk chunk in m_dicChunkCache.Values)
            {
                if (chunk)
                {
                    chunk.SortingLayerID = m_sortingLayer;
                    chunk.OrderInLayer = m_orderInLayer;
                }
            }
        }

        public bool IsVisible
        {
            get
            {
                return m_isVisible;
            }
            set
            {
                bool prevValue = m_isVisible;
                m_isVisible = value;
                if (m_isVisible != prevValue)
                {
                    UpdateMesh();
                }
            }
        }

        /// <summary>
        /// Enables the PixelSnap is the material has the PixelSnap property
        /// </summary>
        public bool PixelSnap { get { return m_pixelSnap; } set { m_pixelSnap = value; } }

        #endregion

        #region Private Fields

        [SerializeField, SortingLayer]
        private int m_sortingLayer = 0;
        [SerializeField]
        private string m_sortingLayerName = "Default";
        [SerializeField]
        private int m_orderInLayer = 0;
        [SerializeField]
        private Material m_material;
        [SerializeField]
        private Color m_tintColor;
        [SerializeField]
        private bool m_pixelSnap;
        [SerializeField]
        private bool m_isVisible = true;
        [SerializeField]
        private bool m_allowPaintingOutOfBounds = true;
        [SerializeField, Tooltip("Set to false when painting on big maps to improve performance.")]
        private bool m_enableUndoWhilePainting = true;
        [SerializeField]
        private bool m_isTrigger = false;

        [SerializeField]
        Vector2 m_cellSize;
        [SerializeField]
        private Bounds m_mapBounds;
        [SerializeField]
        private Tileset m_tileset;
        [SerializeField]
        private int m_minGridX;
        [SerializeField]
        private int m_minGridY;
        [SerializeField]
        private int m_maxGridX;
        [SerializeField]
        private int m_maxGridY;
        [SerializeField]
        private TilemapGroup m_parentTilemapGroup;

        private bool m_markForUpdateMesh = false;
        private bool m_markSceneDirtyOnNextUpdateMesh = false;
        #endregion

        #region Monobehaviour Methods

        void Awake()
        {
            BuildTilechunkDictionary();
        }

        private bool m_applyContactsEmptyFix = false;
        void Update()
        {
            if (Application.isPlaying && m_applyContactsEmptyFix)
            {
                m_applyContactsEmptyFix = false;
                foreach(TilemapChunk tilemapChunk in m_dicChunkCache.Values)
                {
                    if (tilemapChunk)
                    {
                        tilemapChunk.ApplyContactsEmptyFix();
                    }
                }
            }

            if (m_markForUpdateMesh)
            {
                m_markForUpdateMesh = false;
                m_applyContactsEmptyFix = ColliderType == eColliderType._3D;
                UpdateMeshImmediatelly();
            }

        }

        // NOTE: OnDestroy is not called in editor without [ExecuteInEditMode]
        void OnDestroy()
        {
#if UNITY_EDITOR
            if (!AssetDatabase.Contains(m_material))
#endif
            {
                //avoid memory leak
                DestroyImmediate(m_material);
            }
        }

        void OnValidate()
        {
            BuildTilechunkDictionary();
            m_parentTilemapGroup = GetComponentInParent<TilemapGroup>();
#if UNITY_EDITOR
            // fix: for tilemaps created with version 1.3.5 or below
            if(m_tintColor == default(Color))
            {
                Debug.Log("Fixing tilemap made with version below 1.3.5: " + name);
                m_tintColor = m_material.color; //take the color from the material
                m_pixelSnap = Material.HasProperty("PixelSnap") && Material.IsKeywordEnabled("PIXELSNAP_ON");
                bool fixMaterial = string.IsNullOrEmpty(UnityEditor.AssetDatabase.GetAssetPath(m_material));
                if (fixMaterial)
                    m_material = Resources.GetBuiltinResource<Material>("Sprites-Default.mat");
            }
            //---
#endif
            PixelSnap = m_pixelSnap;
        }

        void OnTransformParentChanged()
        {
            m_parentTilemapGroup = GetComponentInParent<TilemapGroup>();
        }

        void Reset()
        {
            ClearMap();
            m_material = Resources.GetBuiltinResource<Material>("Sprites-Default.mat");
            m_tintColor = Color.white;
        }

#if UNITY_EDITOR

        public void OnDrawGizmosSelected()
        {
            if(Selection.activeGameObject == this.gameObject)
            {
                DoDrawGizmos();
            }
        }

        void DoDrawGizmos()
        {
            Rect rBound = new Rect(MapBounds.min, MapBounds.size);
            HandlesEx.DrawRectWithOutline(transform, rBound, new Color(0, 0, 0, 0), new Color(1, 1, 1, 0.5f));

            if ( ShowGrid )
            {
                Plane tilemapPlane = new Plane(this.transform.forward, this.transform.position);
                float distCamToTilemap = 0f;
                Ray rayCamToPlane = new Ray(Camera.current.transform.position, Camera.current.transform.forward);
                tilemapPlane.Raycast(rayCamToPlane, out distCamToTilemap);
                if (HandleUtility.GetHandleSize(rayCamToPlane.GetPoint(distCamToTilemap)) <= 3f)
                {

                    // draw tile cells
                    Gizmos.color = new Color(1f, 1f, 1f, .2f);

                    // Horizontal lines
                    for (float i = 1; i < GridWidth; i++)
                    {
                        Gizmos.DrawLine(
                            this.transform.TransformPoint(new Vector3(MapBounds.min.x + i * CellSize.x, MapBounds.min.y)),
                            this.transform.TransformPoint(new Vector3(MapBounds.min.x + i * CellSize.x, MapBounds.max.y))
                            );
                    }

                    // Vertical lines
                    for (float i = 1; i < GridHeight; i++)
                    {
                        Gizmos.DrawLine(
                            this.transform.TransformPoint(new Vector3(MapBounds.min.x, MapBounds.min.y + i * CellSize.y, 0)),
                            this.transform.TransformPoint(new Vector3(MapBounds.max.x, MapBounds.min.y + i * CellSize.y, 0))
                            );
                    }
                }                
            }

            //Draw Chunk Colliders
            foreach (TilemapChunk chunk in m_dicChunkCache.Values)
            {
                if (chunk)
                {
                    string[] asChunkCoords = chunk.name.Split(new char[] { '_' }, System.StringSplitOptions.RemoveEmptyEntries);
                    int chunkX = int.Parse(asChunkCoords[0]);
                    int chunkY = int.Parse(asChunkCoords[1]);
                    rBound = new Rect(chunkX * k_chunkSize * CellSize.x, chunkY * k_chunkSize * CellSize.y, k_chunkSize * CellSize.x, k_chunkSize * CellSize.y);
                    HandlesEx.DrawRectWithOutline(transform, rBound, new Color(0, 0, 0, 0), new Color(1, 0, 0, 0.2f));
                    chunk.DrawColliders();
                }
            }
            //

        }
#endif
        #endregion

        #region Public Methods

        /// <summary>
        /// Force an update of all tilechunks. This is called after changing sensitive data like CellSize, Collider depth, etc
        /// </summary>
        /// <param name="refreshMesh"></param>
        /// <param name="refreshMeshCollider"></param>
        public void Refresh(bool refreshMesh = true, bool refreshMeshCollider = true, bool refreshTileObjects = false, bool invalidateBrushes = false)
        {
            BuildTilechunkDictionary();
            foreach (TilemapChunk chunk in m_dicChunkCache.Values)
            {
                if (chunk)
                {
                    if (refreshMesh) chunk.InvalidateMesh();
                    if (refreshMeshCollider) chunk.InvalidateMeshCollider();
                    if (refreshTileObjects) chunk.RefreshTileObjects();
                    if (invalidateBrushes) chunk.InvalidateBrushes();
                }
            }
            UpdateMesh();
        }

        /// <summary>
        /// Shrink the map bounds to the minimum area enclosing all visible tiles
        /// </summary>
        public void ShrinkMapBoundsToVisibleArea()
        {
            Bounds mapBounds = new Bounds();
            Vector2 halfCellSize = CellSize / 2f; // used to avoid precission errors
            m_maxGridX = m_maxGridY = m_minGridX = m_minGridY = 0;
            foreach (TilemapChunk chunk in m_dicChunkCache.Values)
            {
                if (chunk)
                {
                    Bounds tilechunkBounds = chunk.MeshFilter.sharedMesh.bounds;
                    Vector2 min = transform.InverseTransformPoint(chunk.transform.TransformPoint(tilechunkBounds.min));
                    Vector2 max = transform.InverseTransformPoint(chunk.transform.TransformPoint(tilechunkBounds.max));
                    mapBounds.Encapsulate(min + halfCellSize);
                    mapBounds.Encapsulate(max - halfCellSize);
                }
            }
            m_minGridX = BrushUtil.GetGridX(mapBounds.min, CellSize);
            m_minGridY = BrushUtil.GetGridY(mapBounds.min, CellSize);
            m_maxGridX = BrushUtil.GetGridX(mapBounds.max, CellSize);
            m_maxGridY = BrushUtil.GetGridY(mapBounds.max, CellSize);
            RecalculateMapBounds();
        }

        /// <summary>
        /// Clear the tilemap from all tilechunks and also remove all objects in the hierarchy
        /// </summary>
        [ContextMenu("Clear Map")]        
        public void ClearMap()
        {
            m_mapBounds = new Bounds();
            m_maxGridX = m_maxGridY = m_minGridX = m_minGridY = 0;
            while (transform.childCount > 0)
            {
#if UNITY_EDITOR
                if (IsUndoEnabled)
                {
                    Undo.DestroyObjectImmediate(transform.GetChild(0).gameObject);
                }
                else
                {
                    DestroyImmediate(transform.GetChild(0).gameObject);
                }
#else
                DestroyImmediate(transform.GetChild(0).gameObject);
#endif
            }
        }

        /// <summary>
        /// Set a tile data using a tilemap local position
        /// </summary>
        /// <param name="vLocalPos"></param>
        /// <param name="tileData"></param>
        public void SetTileData(Vector2 vLocalPos, uint tileData)
        {
            int gridX = BrushUtil.GetGridX(vLocalPos, CellSize);
            int gridY = BrushUtil.GetGridY(vLocalPos, CellSize);
            SetTileData(gridX, gridY, tileData);
        }

        /// <summary>
        /// Set a tile data in the grid position
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <param name="tileData"></param>
        public void SetTileData(int gridX, int gridY, uint tileData)
        {
            TilemapChunk chunk = GetOrCreateTileChunk(gridX, gridY, true);
            int chunkGridX = (gridX < 0 ? -gridX - 1 : gridX) % k_chunkSize;
            int chunkGridY = (gridY < 0 ? -gridY - 1 : gridY) % k_chunkSize;
            if (gridX < 0) chunkGridX = k_chunkSize - 1 - chunkGridX;
            if (gridY < 0) chunkGridY = k_chunkSize - 1 - chunkGridY;
            if (m_allowPaintingOutOfBounds || (gridX >= m_minGridX && gridX <= m_maxGridX && gridY >= m_minGridY && gridY <= m_maxGridY))
            {
                chunk.SetTileData(chunkGridX, chunkGridY, tileData);
                m_markSceneDirtyOnNextUpdateMesh = true;
                // Update map bounds
                //if (tileData != Tileset.k_TileData_Empty) // commented to update the brush bounds when copying empty tiles
                {
                    m_minGridX = Mathf.Min(m_minGridX, gridX);
                    m_maxGridX = Mathf.Max(m_maxGridX, gridX);
                    m_minGridY = Mathf.Min(m_minGridY, gridY);
                    m_maxGridY = Mathf.Max(m_maxGridY, gridY);
                }
                //--
            }
        }

        public void SetTileData(Vector2 vLocalPos, int tileId, int brushId = Tileset.k_BrushId_Default, eTileFlags flags = eTileFlags.None)
        {
            int gridX = BrushUtil.GetGridX(vLocalPos, CellSize);
            int gridY = BrushUtil.GetGridY(vLocalPos, CellSize);
            SetTileData(gridX, gridY, tileId, brushId, flags);
        }

        public void SetTileData(int gridX, int gridY, int tileId, int brushId = Tileset.k_BrushId_Default, eTileFlags flags = eTileFlags.None)
        {
            uint tileData = ((uint)flags << 28) | (((uint)brushId << 16) & Tileset.k_TileDataMask_BrushId) | ((uint)tileId & Tileset.k_TileDataMask_TileId);
            SetTileData(gridX, gridY, tileData);
        }

        public void Erase(Vector2 vLocalPos)
        {
            SetTileData( vLocalPos, Tileset.k_TileData_Empty );
        }

        public void Erase(int gridX, int gridY)
        {
            SetTileData(gridX, gridY, Tileset.k_TileData_Empty);
        }

        /// <summary>
        /// Return the tile data at the local position
        /// </summary>
        /// <param name="vLocalPos"></param>
        /// <returns></returns>
        public uint GetTileData(Vector2 vLocalPos)
        {
            int gridX = BrushUtil.GetGridX(vLocalPos, CellSize);
            int gridY = BrushUtil.GetGridY(vLocalPos, CellSize);
            return GetTileData(gridX, gridY);
        }

        /// <summary>
        /// Return the tile data at the grid position
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <returns></returns>
        public uint GetTileData(int gridX, int gridY)
        {
            TilemapChunk chunk = GetOrCreateTileChunk(gridX, gridY);
            if (chunk == null)
            {
                return Tileset.k_TileData_Empty;
            }
            else
            {
                int chunkGridX = (gridX < 0 ? -gridX - 1 : gridX) % k_chunkSize;
                int chunkGridY = (gridY < 0 ? -gridY - 1 : gridY) % k_chunkSize;
                if (gridX < 0) chunkGridX = k_chunkSize - 1 - chunkGridX;
                if (gridY < 0) chunkGridY = k_chunkSize - 1 - chunkGridY;
                return chunk.GetTileData(chunkGridX, chunkGridY);
            }
        }

        /// <summary>
        /// Return the tile at the local position
        /// </summary>
        /// <param name="vLocalPos"></param>
        /// <returns></returns>
        public Tile GetTile(Vector2 vLocalPos)
        {
            int gridX = BrushUtil.GetGridX(vLocalPos, CellSize);
            int gridY = BrushUtil.GetGridY(vLocalPos, CellSize);
            return GetTile(gridX, gridY);
        }

        /// <summary>
        /// Return the tile at the grid position
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <returns></returns>
        public Tile GetTile(int gridX, int gridY)
        {
            uint tileData = GetTileData(gridX, gridY);
            int tileId = Tileset.GetTileIdFromTileData(tileData);
            return Tileset.GetTile(tileId);
        }

        /// <summary>
        /// Update the render mesh and mesh collider of all tile chunks. This should be called once after making all modifications to the tilemap with SetTileData.
        /// </summary>
        public void UpdateMesh()
        {
            m_markForUpdateMesh = true;
        }     

        /// <summary>
        /// Recalculate the bounding volume of the map from the grid limits
        /// </summary>
        public void RecalculateMapBounds()
        {
            // Fix grid limits if necessary
            m_minGridX = Mathf.Min(m_minGridX, 0);
            m_minGridY = Mathf.Min(m_minGridY, 0);
            m_maxGridX = Mathf.Max(m_maxGridX, 0);
            m_maxGridY = Mathf.Max(m_maxGridY, 0);
            
            Vector2 minTilePos = Vector2.Scale(new Vector2(m_minGridX, m_minGridY), CellSize);
            Vector2 maxTilePos = Vector2.Scale(new Vector2(m_maxGridX, m_maxGridY), CellSize);
            m_mapBounds.min = m_mapBounds.max = Vector2.zero;
            m_mapBounds.Encapsulate(minTilePos);
            m_mapBounds.Encapsulate(minTilePos + CellSize);
            m_mapBounds.Encapsulate(maxTilePos);
            m_mapBounds.Encapsulate(maxTilePos + CellSize);
        }

        /// <summary>
        /// Flip the tilemap vertically
        /// </summary>
        /// <param name="changeFlags"></param>
        public void FlipV(bool changeFlags)
        {
            List<uint> flippedList = new List<uint>(GridWidth * GridHeight);
            for (int gy = MinGridY; gy <= MaxGridY; ++gy)
            {
                for (int gx = MinGridX; gx <= MaxGridX; ++gx)
                {
                    int flippedGy = GridHeight - 1 - gy;
                    flippedList.Add(GetTileData(gx, flippedGy));
                }
            }

            int idx = 0;
            for (int gy = MinGridY; gy <= MaxGridY; ++gy)
            {
                for (int gx = MinGridX; gx <= MaxGridX; ++gx, ++idx)
                {
                    uint flippedTileData = flippedList[idx];
                    if (
                        changeFlags 
                        && (flippedTileData != Tileset.k_TileData_Empty)
                        && (flippedTileData & Tileset.k_TileDataMask_BrushId) == 0 // don't activate flip flags on brushes
                        )
                    {
                        flippedTileData ^= Tileset.k_TileFlag_FlipV;
                    }
                    SetTileData(gx, gy, flippedTileData);
                }
            }
        }

        /// <summary>
        /// Flip the map horizontally
        /// </summary>
        /// <param name="changeFlags"></param>
        public void FlipH(bool changeFlags)
        {
            List<uint> flippedList = new List<uint>(GridWidth * GridHeight);
            for (int gx = MinGridX; gx <= MaxGridX; ++gx)
            {
                for (int gy = MinGridY; gy <= MaxGridY; ++gy)
                {
                    int flippedGx = GridWidth - 1 - gx;
                    flippedList.Add(GetTileData(flippedGx, gy));
                }
            }

            int idx = 0;
            for (int gx = MinGridX; gx <= MaxGridX; ++gx)
            {
                for (int gy = MinGridY; gy <= MaxGridY; ++gy, ++idx)
                {
                    uint flippedTileData = flippedList[idx];
                    if (
                        changeFlags
                        && (flippedTileData != Tileset.k_TileData_Empty)
                        && (flippedTileData & Tileset.k_TileDataMask_BrushId) == 0 // don't activate flip flags on brushes
                        )
                    {
                        flippedTileData ^= Tileset.k_TileFlag_FlipH;
                    }
                    SetTileData(gx, gy, flippedTileData);
                }
            }
        }

        /// <summary>
        /// Rotate the map 90 degrees clockwise
        /// </summary>
        /// <param name="changeFlags"></param>
        public void Rot90(bool changeFlags)
        {
            List<uint> flippedList = new List<uint>(GridWidth * GridHeight);
            for (int gy = MinGridY; gy <= MaxGridY; ++gy)
            {
                for (int gx = MinGridX; gx <= MaxGridX; ++gx)
                {
                    flippedList.Add(GetTileData(gx, gy));
                }
            }

            int minGridX = MinGridX;
            int minGridY = MinGridY;
            int maxGridX = MaxGridY;
            int maxGridY = MaxGridX;
            ClearMap();

            int idx = 0;
            for (int gx = minGridX; gx <= maxGridX; ++gx)
            {
                for (int gy = maxGridY; gy >= minGridY; --gy, ++idx)
                {
                    uint flippedTileData = flippedList[idx];
                    if (
                        changeFlags
                        && (flippedTileData != Tileset.k_TileData_Empty)
                        && (flippedTileData & Tileset.k_TileDataMask_BrushId) == 0 // don't activate flip flags on brushes
                        )
                    {
                        bool rot90 = (flippedTileData & Tileset.k_TileFlag_Rot90) != 0;
                        if (!rot90)
                        {
                            flippedTileData ^= Tileset.k_TileFlag_Rot90;
                        }
                        else
                        {
                            flippedTileData ^= Tileset.k_TileFlag_Rot90;
                            flippedTileData ^= Tileset.k_TileFlag_FlipH;
                            flippedTileData ^= Tileset.k_TileFlag_FlipV;
                        }
                    }
                    SetTileData(gx, gy, flippedTileData);
                }
            }
        }

        public bool InvalidateChunkAt(int gridX, int gridY, bool invalidateMesh = true, bool invalidateMeshCollider = true)
        {
            TilemapChunk chunk = GetOrCreateTileChunk(gridX, gridY);
            if (chunk != null)
            {
                chunk.InvalidateMesh();
                chunk.InvalidateMeshCollider();
                return true;
            }
            return false;
        }

        #endregion

        #region Private Methods      
  


        Dictionary<uint, TilemapChunk> m_dicChunkCache = new Dictionary<uint, TilemapChunk>();
        private TilemapChunk GetOrCreateTileChunk(int gridX, int gridY, bool createIfDoesntExist = false)
        {
            if (m_dicChunkCache.Count == 0 && transform.childCount > 0)
                BuildTilechunkDictionary();

            int chunkX = (gridX < 0 ? (gridX + 1 - k_chunkSize) : gridX) / k_chunkSize;
            int chunkY = (gridY < 0 ? (gridY + 1 - k_chunkSize) : gridY) / k_chunkSize;

            TilemapChunk tilemapChunk = null;

            uint key = (uint)((chunkY << 16) | (chunkX & 0x0000FFFF));
            m_dicChunkCache.TryGetValue(key, out tilemapChunk);

            if (tilemapChunk == null && createIfDoesntExist)
            {
                string chunkName = chunkX + "_" + chunkY;
                GameObject chunkObj = new GameObject(chunkName);
                if (IsUndoEnabled)
                {
#if UNITY_EDITOR
                    Undo.RegisterCreatedObjectUndo(chunkObj, k_UndoOpName + name);
#endif
                }
                chunkObj.transform.parent = transform;
                chunkObj.transform.localPosition = new Vector2(chunkX * k_chunkSize * CellSize.x, chunkY * k_chunkSize * CellSize.y);
                chunkObj.transform.localRotation = Quaternion.identity;
                chunkObj.transform.localScale = Vector3.one;
                chunkObj.hideFlags = gameObject.hideFlags | HideFlags.HideInHierarchy; //NOTE: note the flags inheritance. BrushBehaviour object is not saved, so chunks are left orphans unless this inheritance is done
                tilemapChunk = chunkObj.AddComponent<TilemapChunk>();
                // Reset is not called after AddComponent while in play
                if (Application.isPlaying)
                {
                    tilemapChunk.Reset();
                }
                tilemapChunk.ParentTilemap = this;
                tilemapChunk.GridPosX = chunkX * k_chunkSize;
                tilemapChunk.GridPosY = chunkY * k_chunkSize;
                tilemapChunk.SetDimensions(k_chunkSize, k_chunkSize);
                tilemapChunk.SetSharedMaterial(Material);
                tilemapChunk.SortingLayerID = m_sortingLayer;
                tilemapChunk.OrderInLayer = m_orderInLayer;

                m_dicChunkCache[key] = tilemapChunk;
            }

            return tilemapChunk;
        }

        private void BuildTilechunkDictionary()
        {
            m_dicChunkCache.Clear();
            for(int i = 0; i < transform.childCount; ++i)
            {
                TilemapChunk chunk = transform.GetChild(i).GetComponent<TilemapChunk>();
                if (chunk)
                {
                    int chunkX = (chunk.GridPosX < 0 ? (chunk.GridPosX + 1 - k_chunkSize) : chunk.GridPosX) / k_chunkSize;
                    int chunkY = (chunk.GridPosY < 0 ? (chunk.GridPosY + 1 - k_chunkSize) : chunk.GridPosY) / k_chunkSize;
                    uint key = (uint)((chunkY << 16) | (chunkX & 0x0000FFFF));
                    m_dicChunkCache[key] = chunk;
                }
            }
        }

        /// <summary>
        /// Update the render mesh and mesh collider of all tile chunks. This should be called once after making all modifications to the tilemap with SetTileData.
        /// </summary>
        private void UpdateMeshImmediatelly()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && m_markSceneDirtyOnNextUpdateMesh)
            {
                m_markSceneDirtyOnNextUpdateMesh = false;
#if UNITY_5_3
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
#else
                EditorApplication.MarkSceneDirty();
#endif
            }
#endif

            RecalculateMapBounds();

            List<TilemapChunk> chunkList = new List<TilemapChunk>(m_dicChunkCache.Values.Count);
            foreach (TilemapChunk chunk in m_dicChunkCache.Values)
            {
                if (chunk)                
                {
                    if (!chunk.UpdateMesh())
                    {
#if UNITY_EDITOR
                        if (IsUndoEnabled)
                        {
                            Undo.DestroyObjectImmediate(chunk.gameObject);
                        }
                        else
                        {
                            DestroyImmediate(chunk.gameObject);
                        }
#else
                        DestroyImmediate(chunk.gameObject);
#endif
                    }
                    else
                    {
                        //chunk.UpdateColliderMesh();
                        chunkList.Add(chunk);
                    }
                }
            }

            // UpdateColliderMesh is called after calling UpdateMesh of all tilechunks, because UpdateColliderMesh needs the tileId to be updated 
            // ( remember a brush sets neighbours tile id to empty, so UpdateColliderMesh won't be able to know the collider type )
            for (int i = 0; i < chunkList.Count; ++i)
            {
                chunkList[i].UpdateColliders();
            }
        }

        #endregion
    }
}