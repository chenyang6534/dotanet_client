﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabLightmapData : MonoBehaviour
{
    [System.Serializable]
    struct RendererInfo
    {
        public Renderer renderer;
        public int lightmapIndex;
        public Vector4 lightmapOffsetScale;
    }
    [System.Serializable]
    struct TerrainRendererInfo
    {
        public Terrain terrain;
        public int lightmapIndex;
        public Vector4 lightmapOffsetScale;
    }

    [SerializeField]
    RendererInfo[] m_RendererInfo;
    [SerializeField]
    TerrainRendererInfo[] m_TerrainRendererInfo;
    [SerializeField]
    Texture2D[] m_Lightmaps;


    void Awake()
    {
        if (m_RendererInfo == null || m_RendererInfo.Length == 0)
            return;

        var lightmaps = LightmapSettings.lightmaps;
        int[] offsetsindexes = new int[m_Lightmaps.Length];
        int counttotal = lightmaps.Length;
        List<LightmapData> combinedLightmaps = new List<LightmapData>();

        for (int i = 0; i < m_Lightmaps.Length; i++)
        {
            bool exists = false;
            for (int j = 0; j < lightmaps.Length; j++)
            {

                if (m_Lightmaps[i] == lightmaps[j].lightmapColor)
                {
                    exists = true;
                    offsetsindexes[i] = j;

                }

            }
            if (!exists)
            {
                offsetsindexes[i] = counttotal;
                var newlightmapdata = new LightmapData();
                newlightmapdata.lightmapColor = m_Lightmaps[i];
                combinedLightmaps.Add(newlightmapdata);

                counttotal += 1;


            }

        }

        var combinedLightmaps2 = new LightmapData[counttotal];

        lightmaps.CopyTo(combinedLightmaps2, 0);
        combinedLightmaps.ToArray().CopyTo(combinedLightmaps2, lightmaps.Length);
        LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
        ApplyRendererInfo(m_RendererInfo, m_TerrainRendererInfo, offsetsindexes);
        LightmapSettings.lightmaps = combinedLightmaps2;

    }


    static void ApplyRendererInfo(RendererInfo[] infos, TerrainRendererInfo[] terraininfos, int[] lightmapOffsetIndex)
    {
        for (int i = 0; i < infos.Length; i++)
        {
            var info = infos[i];
            info.renderer.lightmapIndex = lightmapOffsetIndex[info.lightmapIndex];
            info.renderer.lightmapScaleOffset = info.lightmapOffsetScale;
        }
        for (int i = 0; i < terraininfos.Length; i++)
        {
            var info = terraininfos[i];
            info.terrain.lightmapIndex = lightmapOffsetIndex[info.lightmapIndex];
            info.terrain.lightmapScaleOffset = info.lightmapOffsetScale;
        }


    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Bake Prefab Lightmaps")]
    static void GenerateLightmapInfo()
    {
        if (UnityEditor.Lightmapping.giWorkflowMode != UnityEditor.Lightmapping.GIWorkflowMode.OnDemand)
        {
            Debug.LogError("ExtractLightmapData requires that you have baked you lightmaps and Auto mode is disabled.");
            return;
        }
        UnityEditor.Lightmapping.Bake();

        PrefabLightmapData[] prefabs = FindObjectsOfType<PrefabLightmapData>();

        foreach (var instance in prefabs)
        {
            var gameObject = instance.gameObject;
            var rendererInfos = new List<RendererInfo>();
            var lightmaps = new List<Texture2D>();
            var terrainInfos = new List<TerrainRendererInfo>();

            GenerateLightmapInfo(gameObject, rendererInfos, terrainInfos, lightmaps);

            instance.m_RendererInfo = rendererInfos.ToArray();
            instance.m_TerrainRendererInfo = terrainInfos.ToArray();
            instance.m_Lightmaps = lightmaps.ToArray();

            var targetPrefab = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(gameObject) as GameObject;
            if (targetPrefab != null)
            {
                //UnityEditor.Prefab
                UnityEditor.PrefabUtility.ReplacePrefab(gameObject, targetPrefab);
            }
        }
    }

    static void GenerateLightmapInfo(GameObject root, List<RendererInfo> rendererInfos, List<TerrainRendererInfo> terrainRendererInfos, List<Texture2D> lightmaps)
    {
        var renderers = root.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.lightmapIndex != -1)
            {
                RendererInfo info = new RendererInfo();
                info.renderer = renderer;
                info.lightmapOffsetScale = renderer.lightmapScaleOffset;

                Texture2D lightmap = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapColor;

                info.lightmapIndex = lightmaps.IndexOf(lightmap);
                if (info.lightmapIndex == -1)
                {
                    info.lightmapIndex = lightmaps.Count;
                    lightmaps.Add(lightmap);
                }

                rendererInfos.Add(info);
            }
        }
        var terrains = root.GetComponentsInChildren<Terrain>();
        foreach (Terrain renderer in terrains)
        {
            Debug.Log("----------:" + renderer.name);
            if (renderer.lightmapIndex != -1)
            {
                TerrainRendererInfo info = new TerrainRendererInfo();
                info.terrain = renderer;
                info.lightmapOffsetScale = renderer.lightmapScaleOffset;

                Texture2D lightmap = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapColor;

                info.lightmapIndex = lightmaps.IndexOf(lightmap);
                if (info.lightmapIndex == -1)
                {
                    info.lightmapIndex = lightmaps.Count;
                    lightmaps.Add(lightmap);
                }

                terrainRendererInfos.Add(info);
            }
        }

    }
#endif

}