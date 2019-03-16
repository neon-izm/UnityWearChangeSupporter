/**
[UnityWearChangeSupporter]
Copyright (c) 2019 twitter: @izm
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRChatUtilIzm
{
    /// <summary>
    /// 着せ替えスクリプト
    /// 何をするかと言うと、VRChat向けとして売っている洋服を着替える改変をUnity上だけで完結させるための
    /// 補助スクリプト
    /// 正式な着せ替えはBlenderでやった方が綺麗なんだけど、手軽なフィッティングがUnity上だけで出来るとうれしい！
    /// </summary>
    public class UnityWearChangeSupporter : EditorWindow
    {
        public const string Version = "0.0.2";

        private static UnityWearChangeSupporter window;

        [SerializeField] static Animator character;
        [SerializeField] static Animator cloth;

        [SerializeField] static Animator clothModel;

        [MenuItem("VRChatTool/WearChangeSupporter")]
        private static void Create()
        {
            // 生成
            window = GetWindow<UnityWearChangeSupporter>("WearChanger");
            window.minSize = new Vector2(320, 320);
        }

        private void OnGUI()
        {
            using (new GUILayout.VerticalScope())
            {
                GUILayout.Label("UnityWearChangeSupporter v" + Version);

                GUILayout.Label("シーン上のキャラクターと服を選んでください");
                character = EditorGUILayout.ObjectField("character", character, typeof(Animator), true) as Animator;
                cloth = EditorGUILayout.ObjectField("服(シーン)", cloth, typeof(Animator), true) as Animator;
                clothModel =
                    EditorGUILayout.ObjectField("服(プロジェクト)", clothModel, typeof(Animator), true) as
                        Animator;
            }

            using (new GUILayout.HorizontalScope())
            {
                // 書き込みボタン
                if (GUILayout.Button("着せ替え"))
                {
                    if (character == null || cloth == null)
                    {
                        // クリックした位置を視点とするRectを作る
                        // 本来のポップアップの用途として使う場合はボタンのRectを渡す
                        var mouseRect = new Rect(Event.current.mousePosition, Vector2.one);

                        // PopupWindowContentを生成
                        var content = new PopupContentMessage();

                        content.SetMessage("キャラクターと服をシーン上から指定してください");
                        // 開く
                        PopupWindow.Show(mouseRect, content);
                        return;
                    }

                    if (clothModel == null)
                    {
                        // クリックした位置を視点とするRectを作る
                        // 本来のポップアップの用途として使う場合はボタンのRectを渡す
                        var mouseRect = new Rect(Event.current.mousePosition, Vector2.one);

                        // PopupWindowContentを生成
                        var content = new PopupContentMessage();

                        content.SetMessage("プロジェクト内で、服のfbxモデルを服(プロジェクト)\nに指定してください");
                        // 開く
                        PopupWindow.Show(mouseRect, content);
                        return;
                    }

                    var success = ClothChange();
                    if (success)
                    {
                        // クリックした位置を視点とするRectを作る
                        // 本来のポップアップの用途として使う場合はボタンのRectを渡す
                        var mouseRect = new Rect(Event.current.mousePosition, Vector2.one);

                        // PopupWindowContentを生成
                        var content = new PopupContentMessage();

                        content.SetMessage("着せ替え完了 ヒエラルキー上の\n「_kisekae」とついているボーンの\n位置とスケールを調整してください");
                        // 開く
                        PopupWindow.Show(mouseRect, content);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 監視機構
        /// 服(シーン)を指定したら、そこから適したFBXをプロジェクト内から探す
        /// </summary>
        void OnInspectorUpdate()
        {
            if (cloth == null)
            {
                clothModel = null;
                return;
            }

            if (clothModel != null)
            {
                return;
            }


            var filter = cloth.name;
            var findedGuids = AssetDatabase.FindAssets(filter, null);
            if (findedGuids == null || findedGuids.Length == 0)
            {
                Debug.Log("なんもなかった " + cloth.name);
            }

            foreach (var VARIABLE in findedGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(VARIABLE);

                var targetPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (targetPrefab != null)
                {
                    Debug.Log(targetPrefab);
                    var tt = targetPrefab.GetComponent<Animator>();
                    if (tt != null)
                    {
                        clothModel = tt;
                        Repaint();
                    }
                }
                else
                {
                    Debug.Log("さてはprefab無いな！！！：" + path);
                }
            }
        }


        public static HumanBodyBones ConvertStringBoneNameToHumanBodyBones(string boneName)
        {
            switch (boneName)
            {
                case "Hips":
                    return HumanBodyBones.Hips;
                case "LeftUpperLeg":
                    return HumanBodyBones.LeftUpperLeg;
                case "RightUpperLeg":
                    return HumanBodyBones.RightUpperLeg;
                case "LeftLowerLeg":
                    return HumanBodyBones.LeftLowerLeg;
                case "RightLowerLeg":
                    return HumanBodyBones.RightLowerLeg;
                case "LeftFoot":
                    return HumanBodyBones.LeftFoot;
                case "RightFoot":
                    return HumanBodyBones.RightFoot;
                case "Spine":
                    return HumanBodyBones.Spine;
                case "Chest":
                    return HumanBodyBones.Chest;
                case "Neck":
                    return HumanBodyBones.Neck;
                case "Head":
                    return HumanBodyBones.Head;
                case "LeftShoulder":
                    return HumanBodyBones.LeftShoulder;
                case "RightShoulder":
                    return HumanBodyBones.RightShoulder;
                case "LeftUpperArm":
                    return HumanBodyBones.LeftUpperArm;
                case "RightUpperArm":
                    return HumanBodyBones.RightUpperArm;
                case "LeftLowerArm":
                    return HumanBodyBones.LeftLowerArm;
                case "RightLowerArm":
                    return HumanBodyBones.RightLowerArm;
                case "LeftHand":
                    return HumanBodyBones.LeftHand;
                case "RightHand":
                    return HumanBodyBones.RightHand;
                case "LeftToes":
                    return HumanBodyBones.LeftToes;
                case "RightToes":
                    return HumanBodyBones.RightToes;
                case "LeftEye":
                    return HumanBodyBones.LeftEye;
                case "RightEye":
                    return HumanBodyBones.RightEye;
                case "Jaw":
                    return HumanBodyBones.Jaw;

                case "UpperChest":
                    return HumanBodyBones.UpperChest;
                case "LastBone":
                    return HumanBodyBones.LastBone;

                //指は見なかったことにする
                default:
                    break;
            }

            return HumanBodyBones.LastBone;
        }

        public static bool GetHumanDescription(GameObject target, ref HumanDescription des)
        {
            if (target != null)
            {
                AssetImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(target));
                if (importer != null)
                {
                    Debug.Log("AssetImporter Type: " + importer.GetType());
                    ModelImporter modelImporter = importer as ModelImporter;
                    if (modelImporter != null)
                    {
                        des = modelImporter.humanDescription;
                        Debug.Log("## Cool stuff data by ModelImporter ##");
                        foreach (var VARIABLE in des.skeleton)
                        {
                            //Debug.Log(VARIABLE);
                        }

                        //Debug.LogError(des.skeleton);
                        return true;
                    }
                    else
                    {
                        Debug.LogError("## Please Select Imported Model in Project View not prefab or other things ##");
                    }
                }
                else
                {
                    Debug.LogError("importer is null");
                }
            }
            else
            {
                Debug.LogError("target is null");
            }

            return false;
        }

        //Breadth-first search
        public static Transform FindDeepChild(Transform aParent, string aName)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(aParent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == aName)
                    return c;
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }

            return null;
        }


        /// <summary>
        /// 着替え本体
        /// </summary>
        /// <returns></returns>
        private bool ClothChange()
        {
            if (cloth == null || character == null)
            {
                Debug.LogError("キャラクターと服をシーン上から指定してください");
                return false;
            }

            if (character.avatar.isHuman == false)
            {
                Debug.LogError("キャラクターのModelをHumanoidに変更してください！");
                return false;
            }


            cloth.transform.position = character.transform.position;
            /// ううう、本来はValidなHumanoidだったら、Animatorから生えているGetBoneTransformでなんでもいけるんだけど
            /// 残念ながらInvalidな服のHumanoid(足が無かったりする）を使う以上、ModelImporter情報から持ってくる必要がある
            /// めちゃくちゃつれえ…
            HumanDescription dc = new HumanDescription();
            GetHumanDescription(clothModel.gameObject, ref dc);
            //なので、この辞書をGetBoneTransformの代わりに使う、つれえ…
            Dictionary<HumanBodyBones, Transform>
                clothHumanBoneDictionary = new Dictionary<HumanBodyBones, Transform>();

            foreach (var VARIABLE in dc.human)
            {
                //sholder.L が LeftShoulder です。 
                //みたいな情報は取得できる
                Debug.Log(VARIABLE.boneName + " が " + VARIABLE.humanName + " です。 ");

                Transform target = FindDeepChild(cloth.transform, VARIABLE.boneName);
                if (target == null)
                {
                    Debug.LogError(VARIABLE.boneName + " が見つからなかった");
                }
                else
                {
                    clothHumanBoneDictionary.Add(ConvertStringBoneNameToHumanBodyBones(VARIABLE.humanName),
                        target);
                }
            }

            Debug.Log("******Debug Start********");
            foreach (var VARIABLE in clothHumanBoneDictionary)
            {
                Debug.Log(VARIABLE.Key.ToString() + " は " + VARIABLE.Value);
            }

            Debug.Log("******Debug End********");


            cloth.transform.SetParent(character.transform);

            foreach (var VARIABLE in clothHumanBoneDictionary)
            {
                Transform willParent = character.GetBoneTransform(VARIABLE.Key);
                if (willParent != null)
                {
                    //Debug.Log(VARIABLE.Value.name + " の親は " + willParent.name + "　に差し替え");
                    VARIABLE.Value.SetParent(willParent, true);
                    VARIABLE.Value.name += "_kisekae";

                    //spineまわりは座標いじるか微妙だ…
                    //一旦両腕とか左右がある骨は位置を合わせておく、みたいな感じで…
                    if (VARIABLE.Key.ToString().Contains("Left") || VARIABLE.Key.ToString().Contains("Right"))
                    {
                        //肩はモデルによって千差万別なので無視
                        if (VARIABLE.Key.ToString().Contains("Shoulder") == false)
                        {
                            VARIABLE.Value.localPosition = Vector3.zero;
                        }
                    }
                }
            }

            return true;
        }
    }

    /// <summary>
    /// ポップアップしてメッセージをユーザに伝える
    /// Debug.LogErrorは読んでもらえないがち…それはそう…
    /// </summary>
    public class PopupContentMessage : PopupWindowContent
    {
        private string showmessage = "default message";

        /// <summary>
        /// サイズを取得する
        /// </summary>
        public override Vector2 GetWindowSize()
        {
            return new Vector2(300, 150);
        }

        /// <summary>
        /// GUI描画
        /// </summary>
        public override void OnGUI(Rect rect)
        {
            GUIStyle centerbold = new GUIStyle()
            {
                //alignment = TextAnchor.MiddleCenter,
                //fontStyle = FontStyle.Bold,
                wordWrap = true,
                richText = true
            };

            EditorGUILayout.LabelField(showmessage, centerbold);
            //if (GUILayout.Button("OK"))
            {
            }
        }

        public void SetMessage(string newContent)
        {
            showmessage = newContent;
        }

        /// <summary>
        /// 開いたときの処理
        /// </summary>
        public override void OnOpen()
        {
        }

        /// <summary>
        /// 閉じたときの処理
        /// </summary>
        public override void OnClose()
        {
        }
    }
}
