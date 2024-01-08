using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using GameNetcodeStuff;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using LC_Maxwell;
using System;


namespace LC_Maxwell.Patches
{
    [HarmonyPatch(typeof(GrabbableObject))]
    internal class ItemReplacerPatch
    {
        static string itemName = "Maxwell";

        static string assetName = "maxwell.max";

        static Mesh customMesh = null;
        static Material dingus;
        static Material whiskers;

        static float maxwellSize = 6f;
        static string[] affectedObjects = {"largebolt", "bigbolt"};

        static AudioClip pickUp;
        static string pickUpAudioName = "CatPickup.wav";

        static AudioClip drop;
        static string dropAudioName = "CatDrop.wav";

        static Vector3 resting = new Vector3(-90,0,0);

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void ReplaceModel(ref Item ___itemProperties, ref MeshRenderer ___mainObjectRenderer)
        {
            try
            {
                if (customMesh == null)
                {
                    string currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    string path = Path.Combine(currentDirectory, assetName).Replace("\\", "/");
                    MaxwellReplacerMod.mls.LogMessage("Searching this filepath:" + path);

                    AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
                    AssetBundle assets = request.assetBundle;

                    customMesh = assets.LoadAsset<Mesh>("dingus.mesh");

                    dingus = assets.LoadAsset<Material>("dingus.mat");
                    whiskers = assets.LoadAsset<Material>("whiskers.mat");

                    pickUp = assets.LoadAsset<AudioClip>(pickUpAudioName);
                    drop = assets.LoadAsset<AudioClip>(dropAudioName);
                }
            }
            catch
            {
                MaxwellReplacerMod.mls.LogError("Assets did not load");
            }

            if (affectedObjects.Contains(___itemProperties.name.ToLower()))
            {
                MeshFilter mf = ___mainObjectRenderer.GetComponent<MeshFilter>();
                mf.mesh = UnityEngine.Object.Instantiate(customMesh);

                Mesh mesh = mf.mesh;

                Vector3[] vertices = mesh.vertices;
                Vector3 center = mesh.bounds.center;

                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = center + (vertices[i] - center) * maxwellSize;
                }

                mesh.vertices = vertices;
                mesh.RecalculateBounds();

                ___mainObjectRenderer.materials = new Material[2] { dingus, whiskers };
                
                ___itemProperties.restingRotation = resting;

                ___itemProperties.grabSFX = pickUp;
                ___itemProperties.dropSFX = drop;

                ___itemProperties.spawnPrefab.GetComponent<MeshFilter>().mesh = mesh;
                ___mainObjectRenderer.GetComponentInChildren<ScanNodeProperties>().headerText = itemName;

                ___itemProperties.itemName = itemName;

                ___itemProperties.restingRotation = resting;
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Upright(ref Item ___itemProperties, ref MeshRenderer ___mainObjectRenderer)
        {
            try
            {
                if (affectedObjects.Contains(___itemProperties.name.ToLower()))
                {
                    Quaternion a = ___mainObjectRenderer.transform.rotation;
                    ___mainObjectRenderer.transform.rotation = Quaternion.Euler(-90f * Mathf.Sign(___mainObjectRenderer.transform.lossyScale.x), Time.time * 90f, 0);
                }
            }
            catch
            {
                MaxwellReplacerMod.mls.LogMessage(___itemProperties.name.ToLower() + " failed to update rotation");
            }
        }




    }
        
}
