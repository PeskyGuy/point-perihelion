using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

namespace Perihelion.Editor
{
    public class LonabellaAnimatorSetup
    {
        [MenuItem("Perihelion/Setup Lonabella Animator")]
        public static void CreateAnimator()
        {
            string basePath = "Assets/Art/Characters/Parts/Lonabella/Animations";
            if (!AssetDatabase.IsValidFolder(basePath))
            {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "Art/Characters/Parts/Lonabella/Animations"));
                AssetDatabase.Refresh();
            }

            var controllerPath = $"{basePath}/Lonabella.controller";
            var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

            controller.AddParameter("Direction", AnimatorControllerParameterType.Int);
            controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);

            string[] directions = { "south", "north", "east" };
            string[] states = { "Idle", "Walk" };

            foreach (string dir in directions)
            {
                foreach (string state in states)
                {
                    string clipName = $"Lona_{state}_{dir}";
                    var clipPath = $"{basePath}/{clipName}.anim";
                    
                    var existingClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
                    if (existingClip == null)
                    {
                        var clip = new AnimationClip();
                        clip.name = clipName;
                        AssetDatabase.CreateAsset(clip, clipPath);
                    }
                }
            }

            var rootSM = controller.layers[0].stateMachine;

            // direction values: 0=south, 1=north, 2=east
            for (int d = 0; d < directions.Length; d++)
            {
                string dir = directions[d];
                
                var idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{basePath}/Lona_Idle_{dir}.anim");
                var walkClip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{basePath}/Lona_Walk_{dir}.anim");
                
                var idleState = rootSM.AddState($"Idle_{dir}", new Vector3(300, d * 120, 0));
                idleState.motion = idleClip;
                
                var walkState = rootSM.AddState($"Walk_{dir}", new Vector3(550, d * 120, 0));
                walkState.motion = walkClip;
                
                var toWalk = idleState.AddTransition(walkState);
                toWalk.AddCondition(AnimatorConditionMode.If, 0, "IsMoving");
                toWalk.AddCondition(AnimatorConditionMode.Equals, d, "Direction");
                toWalk.hasExitTime = false;
                toWalk.duration = 0.05f;
                
                var toIdle = walkState.AddTransition(idleState);
                toIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsMoving");
                toIdle.hasExitTime = false;
                toIdle.duration = 0.05f;
                
                var anyToIdle = rootSM.AddAnyStateTransition(idleState);
                anyToIdle.AddCondition(AnimatorConditionMode.Equals, d, "Direction");
                anyToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsMoving");
                anyToIdle.hasExitTime = false;
                anyToIdle.duration = 0.05f;
                anyToIdle.canTransitionToSelf = false;
                
                var anyToWalk = rootSM.AddAnyStateTransition(walkState);
                anyToWalk.AddCondition(AnimatorConditionMode.Equals, d, "Direction");
                anyToWalk.AddCondition(AnimatorConditionMode.If, 0, "IsMoving");
                anyToWalk.hasExitTime = false;
                anyToWalk.duration = 0.05f;
                anyToWalk.canTransitionToSelf = false;
            }

            rootSM.defaultState = rootSM.states[0].state; // Idle_south

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[Perihelion] Created Animator Controller with {rootSM.states.Length} states and 6 animation clips.");
        }
    }
}
