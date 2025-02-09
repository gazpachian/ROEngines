﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ROEngines
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class ROEStockInterop : MonoBehaviour
    {

        #region SSTU Interop for Animations
        private static List<Part> dragCubeUpdateParts = new List<Part>();
        private static List<Part> delayedUpdateDragCubeParts = new List<Part>();
        private static List<Part> FARUpdateParts = new List<Part>();

        private static bool fireEditorEvent = false;

        public static ROEStockInterop INSTANCE;
        
        public void Start()
        {
            INSTANCE = this;
            //KSPShaderTools.TexturesUnlimitedLoader.addPostLoadCallback(KSPShaderToolsPostLoad);
            GameObject.DontDestroyOnLoad(this);
            //MonoBehaviour.print("SSTUStockInterop Start");
            //GameEvents.OnGameSettingsApplied.Add(new EventVoid.OnEvent(gameSettingsApplied));
            //GameEvents.onGameStateLoad.Add(new EventData<ConfigNode>.OnEvent(gameStateLoaded));
        }

        public void OnDestroy()
        {
            //MonoBehaviour.print("SSTUStockInterop Destroy");
            //GameEvents.OnGameSettingsApplied.Remove(new EventVoid.OnEvent(gameSettingsApplied));
            //GameEvents.onGameStateLoad.Remove(new EventData<ConfigNode>.OnEvent(gameStateLoaded));
        }

        public static void addFarUpdatePart(Part part)
        {
            if (part != null && !FARUpdateParts.Contains(part))
            {
                FARUpdateParts.Add(part);
            }
        }

        public static void addDragUpdatePart(Part part)
        {
            if(part != null && HighLogic.LoadedSceneIsFlight && !dragCubeUpdateParts.Contains(part))
            {
                dragCubeUpdateParts.Add(part);
            }          
        }

        public static void fireEditorUpdate()
        {
            fireEditorEvent = HighLogic.LoadedSceneIsEditor;
        }

        public void FixedUpdate()
        {
            if (!(HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight))
            {
                return;
            }

            int len = dragCubeUpdateParts.Count;
            Part p;
            for (int i = 0; i < len; i++)
            {
                p = dragCubeUpdateParts[i];
                if (p == null) { continue; }
                updatePartDragCube(p);
                if (p.collider == null) { seatFirstCollider(p); }
            }
            dragCubeUpdateParts.Clear();

            len = FARUpdateParts.Count;
            for (int i = 0; i < len; i++)
            {
                p = FARUpdateParts[i];
                if (p == null) { continue; }
                p.SendMessage("GeometryPartModuleRebuildMeshData");
            }
            FARUpdateParts.Clear();
        }

		/*
        public void LateUpdate()
        {
            if (HighLogic.LoadedSceneIsEditor && fireEditorEvent)
            {
                GameEvents.onEditorShipModified.Fire(EditorLogic.fetch.ship);
            }
            fireEditorEvent = false;
        }
		*/

        private static void seatFirstCollider(Part part)
        {
            Collider[] colliders = part.gameObject.GetComponentsInChildren<Collider>();
            int len = colliders.Length;
            for (int i = 0; i < len; i++)
            {
                if (colliders[i].isTrigger) { continue; }
                if (colliders[i].GetType() == typeof(WheelCollider)) { continue; }
                part.collider = colliders[i];
                break;
            }
        }

        private static void updatePartDragCube(Part part)
        {
            DragCube newDefaultCube = DragCubeSystem.Instance.RenderProceduralDragCube(part);
            newDefaultCube.Weight = 1f;
            newDefaultCube.Name = "Default";
            part.DragCubes.ClearCubes();
            part.DragCubes.Cubes.Add(newDefaultCube);
            part.DragCubes.ResetCubeWeights();
        }

		/*
        public static void updatePartHighlighting(Part part)
        {
            if (!HighLogic.LoadedSceneIsEditor && !HighLogic.LoadedSceneIsFlight) { return; }//noop on prefabs
            if (part.HighlightRenderer != null)
            {
                part.HighlightRenderer = null;
                Transform model = part.transform.FindRecursive("model");
                if (model != null)
                {
                    Renderer[] renders = model.GetComponentsInChildren<Renderer>(false);
                    part.HighlightRenderer = new List<Renderer>(renders);
                }
                else
                {
                    part.HighlightRenderer = new List<Renderer>();
                }
                part.RefreshHighlighter();
            }
        }
		*/

        #endregion

        #region SSTU Interop for SRB's
		/*
        public static void updateEngineThrust(ModuleEngines engine, float minThrust, float maxThrust)
        {
            engine.minThrust = minThrust;
            engine.maxThrust = maxThrust;
            ConfigNode updateNode = new ConfigNode("MODULE");
            updateNode.AddValue("maxThrust", engine.maxThrust);
            updateNode.AddValue("minThrust", engine.minThrust);
            engine.OnLoad(updateNode);
        }
		*/
        #endregion       

    }
}
