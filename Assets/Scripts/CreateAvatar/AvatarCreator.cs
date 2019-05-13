using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.CreateAvatar
{
    public class AvatarCreator : MonoBehaviour
    {
        public void Awake()
        {
            CreateAvatar();
        }

        [Range(0, 1)]
        public float armStretch;

        [Range(0, 1)]
        public float legStretch;

        private HumanDescription _description;
        private ModelImporter _modelImporter;

        private void UpdateDescription()
        {
            GetHumanDescription(this.gameObject);
            _description.armStretch = armStretch;
            _description.legStretch = legStretch;
            _description.upperArmTwist = 1;
            _description.lowerLegTwist = 1;
            _modelImporter.humanDescription = _description;
        }


        public bool GetHumanDescription(GameObject target)
        {
            if (target != null)
            {
                // todo: find a way to get the path from the object
                var assetPath = "Assets/" + target.name + ".FBX";

                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                if (importer != null)
                {
                    Debug.Log("AssetImporter Type: " + importer.GetType());
                    _modelImporter = importer as ModelImporter;
                    if (_modelImporter != null)
                    {
                        _description = _modelImporter.humanDescription;
                        return true;
                    }
                    else
                    {
                        Debug.LogError("## Please Select Imported Model in Project View not prefab or other things ##");
                    }
                }
            }
            return false;
        }

        public  void CreateAvatar()
        {
            GameObject go = this.gameObject;

            if (go != null && go.GetComponent("Animator") != null)
            {
                HumanDescription hd = new HumanDescription();

                Dictionary<string, string> boneName = new System.Collections.Generic.Dictionary<string, string>();
                boneName["Chest"] = "Ribs";
                boneName["Head"] = "Head";
                boneName["Hips"] = "Hip";
                boneName["LeftFoot"] = "Left_Ankle_Joint_01";
                boneName["LeftHand"] = "Right_Ankle_Joint_01";
                boneName["LeftLowerArm"] = "elbowL";
                boneName["LeftLowerLeg"] = "kneeL";
                boneName["LeftShoulder"] = "clavL";
                boneName["LeftUpperArm"] = "armL";
                boneName["LeftUpperLeg"] = "legL";
                boneName["RightFoot"] = "footR";
                boneName["RightHand"] = "handR";
                boneName["RightLowerArm"] = "elbowR";
                boneName["RightLowerLeg"] = "kneeR";
                boneName["RightShoulder"] = "clavR";
                boneName["RightUpperArm"] = "armR";
                boneName["RightUpperLeg"] = "legR";
                boneName["Spine"] = "spine2";
                string[] humanName = HumanTrait.BoneName;
                HumanBone[] humanBones = new HumanBone[boneName.Count];
                int j = 0;
                int i = 0;
                while (i < humanName.Length)
                {
                    if (boneName.ContainsKey(humanName[i]))
                    {
                        HumanBone humanBone = new HumanBone();
                        humanBone.humanName = humanName[i];
                        humanBone.boneName = boneName[humanName[i]];
                        humanBone.limit.useDefaultValues = true;
                        humanBones[j++] = humanBone;
                    }
                    i++;
                }

                hd.human = humanBones;


                hd.skeleton = new SkeletonBone[18];
                hd.skeleton[0].name = ("Hips") ;
                Avatar avatar = AvatarBuilder.BuildHumanAvatar(go, hd);

                avatar.name = (go.name + "_MyAvatar");
                Debug.Log(avatar.isHuman ? "is human" : "is generic");

                Animator animator = go.GetComponent("Animator") as Animator;
                animator.avatar = avatar;

                string path = AssetDatabase.GenerateUniqueAssetPath("" + avatar.name + ".asset");
                AssetDatabase.CreateAsset(avatar, path);

            }
        }
    }
}
