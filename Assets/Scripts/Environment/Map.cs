using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimalManagement;

// The map is divided into n x n regions, with a list of entities for each region
// This allows an entity to more quickly find other nearby entities
public class Map {
    public readonly List<LivingBeing>[, ] map;
    readonly Vector2[, ] centres;
    readonly int regionSize;
    readonly int numRegions;
    public int numEntities;

    public Map (int size, int regionSize) {
        this.regionSize = regionSize;
        numRegions = Mathf.CeilToInt (size / (float) regionSize);
        map = new List<LivingBeing>[numRegions, numRegions];
        centres = new Vector2[numRegions, numRegions];

        for (int y = 0; y < numRegions; y++) {
            for (int x = 0; x < numRegions; x++) {
                Coord regionBottomLeft = new Coord (x * regionSize, y * regionSize);
                Coord regionTopRight = new Coord (x * regionSize + regionSize, y * regionSize + regionSize);
                Vector2 centre = (Vector2) (regionBottomLeft + regionTopRight) / 2f;
                centres[x, y] = centre;
                map[x, y] = new List<LivingBeing> ();
            }
        }
    }

    public List<LivingBeing> GetEntities (Coord origin, float viewDistance) {
        List<RegionInfo> visibleRegions = GetRegionsInView (origin, viewDistance);
        float sqrViewDst = viewDistance * viewDistance;
        var visibleEntities = new List<LivingBeing> ();

        for (int i = 0; i < visibleRegions.Count; i++) {
            Coord regionCoord = visibleRegions[i].coord;

            for (int j = 0; j < map[regionCoord.x, regionCoord.y].Count; j++) {
                LivingBeing entity = map[regionCoord.x, regionCoord.y][j];
                float sqrDst = Coord.SqrDistance (entity.coord, origin);
                if (sqrDst < sqrViewDst) {
                    if (EnvironmentUtility.TileIsVisibile (origin.x, origin.y, entity.coord.x, entity.coord.y)) {
                        visibleEntities.Add (entity);
                    }
                }
            }
        }

        return visibleEntities;
    }

    public LivingBeing ClosestEntity (Coord origin, float viewDistance) {
        List<RegionInfo> visibleRegions = GetRegionsInView (origin, viewDistance);
        LivingBeing closestEntity = null;
        float closestSqrDst = viewDistance * viewDistance + 0.01f;

        for (int i = 0; i < visibleRegions.Count; i++) {
            // Stop searching if current closest entity is closer than the dst to the region edge
            // All remaining regions will be further as well, since sorted by dst
            if (closestSqrDst <= visibleRegions[i].sqrDstToClosestEdge) {
                break;
            }

            Coord regionCoord = visibleRegions[i].coord;

            for (int j = 0; j < map[regionCoord.x, regionCoord.y].Count; j++) {
                LivingBeing entity = map[regionCoord.x, regionCoord.y][j];
                float sqrDst = Coord.SqrDistance (entity.coord, origin);
                if (sqrDst < closestSqrDst) {
                    if (EnvironmentUtility.TileIsVisibile (origin.x, origin.y, entity.coord.x, entity.coord.y)) {
                        closestSqrDst = sqrDst;
                        closestEntity = entity;
                    }
                }
            }
        }

        return closestEntity;
    }

    // Calculates coordinates of all regions that may contain entities within view from the specified viewDoord/viewDstance
    // Regions sorted nearest to farthest
    public List<RegionInfo> GetRegionsInView (Coord origin, float viewDistance) {
        List<RegionInfo> regions = new List<RegionInfo> ();
        int originRegionX = origin.x / regionSize;
        int originRegionY = origin.y / regionSize;
        float sqrViewDst = viewDistance * viewDistance;
        Vector2 viewCentre = origin + Vector2.one * .5f;

        int searchNum = Mathf.Max (1, Mathf.CeilToInt (viewDistance / regionSize));
        // Loop over all regions that might be within the view dst to check if they actually are
        for (int offsetY = -searchNum; offsetY <= searchNum; offsetY++) {
            for (int offsetX = -searchNum; offsetX <= searchNum; offsetX++) {
                int viewedRegionX = originRegionX + offsetX;
                int viewedRegionY = originRegionY + offsetY;

                if (viewedRegionX >= 0 && viewedRegionX < numRegions && viewedRegionY >= 0 && viewedRegionY < numRegions) {
                    // Calculate distance from view coord to closest edge of region to test if region is in range
                    float ox = Mathf.Max (0, Mathf.Abs (viewCentre.x - centres[viewedRegionX, viewedRegionY].x) - regionSize / 2f);
                    float oy = Mathf.Max (0, Mathf.Abs (viewCentre.y - centres[viewedRegionX, viewedRegionY].y) - regionSize / 2f);
                    float sqrDstFromRegionEdge = ox * ox + oy * oy;
                    if (sqrDstFromRegionEdge <= sqrViewDst) {
                        RegionInfo info = new RegionInfo (new Coord (viewedRegionX, viewedRegionY), sqrDstFromRegionEdge);
                        regions.Add (info);
                    }
                }
            }
        }

        // Sort the regions list from nearest to farthest
        regions.Sort ((a, b) => a.sqrDstToClosestEdge.CompareTo (b.sqrDstToClosestEdge));

        return regions;
    }

    public void Add (LivingBeing e, Coord coord) {
        int regionX = coord.x / regionSize;
        int regionY = coord.y / regionSize;

        int index = map[regionX, regionY].Count;
        // store the entity's index in the list inside the entity itself for quick access
        e.mapIndex = index;
        e.coord = coord;
        map[regionX, regionY].Add (e);
        numEntities++;
    }

    public void Remove (LivingBeing e, Coord coord) {
        int regionX = coord.x / regionSize;
        int regionY = coord.y / regionSize;

        int index = e.mapIndex;
        int lastElementIndex = map[regionX, regionY].Count - 1;
        // If this entity is not last in the list, put the last entity in its place
        if (index != lastElementIndex) {

            map[regionX, regionY][index] = map[regionX, regionY][lastElementIndex];
            map[regionX, regionY][index].mapIndex = e.mapIndex;
        }
        // Remove last entity from the list
        map[regionX, regionY].RemoveAt (lastElementIndex);
        numEntities--;
    }

    public void Move (LivingBeing e, Coord fromCoord, Coord toCoord) {
        Remove (e, fromCoord);
        Add (e, toCoord);
    }

    public struct RegionInfo {
        public readonly Coord coord;
        public readonly float sqrDstToClosestEdge;

        public RegionInfo (Coord coord, float sqrDstToClosestEdge) {
            this.coord = coord;
            this.sqrDstToClosestEdge = sqrDstToClosestEdge;
        }
    }
}