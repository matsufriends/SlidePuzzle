using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _Develop.Matsu;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace _Develop.Script {
    public class PuzzleManagerMono : SerializedMonoBehaviour {
        private static PuzzleManagerMono sInstance ;
        
        [Header("General")]
        [SerializeField]
        private Transform mParent;

        [SerializeField]
        private Camera mCamera;

        [SerializeField]
        private float mCameraScaleSpeedK;

        [Header("Prefab")]
        [SerializeField]
        private GameObject mBasePrefab;

        [SerializeField]
        private BlockObjMono mBlockObjPrefab;

        [SerializeField]
        private TextObjMono mTextObjPrefab;

        [Header("FromArray")]
        [OdinSerialize]
        private bool[,] mArray;

        [Header("FromString")]
        [SerializeField]
        private static List<string> mSListH;

        [SerializeField]
        private static List<string> mSListW;

        private static List<List<int>>          mIListH;
        private static List<List<int>>          mIListW;
        private        List<List<TextObjMono>>  mTListW = new List<List<TextObjMono>>();
        private        List<List<BlockObjMono>> mBListH = new List<List<BlockObjMono>>();

        private static int mHSize;
        private static int mWSize;

        private Vector2      mGOffset;
        private Vector2      mBlockXRange;
        private float        mDragDifX;
        private BlockObjMono mDragBlock;

        private Vector2 mCameraDif;
        private bool    mIsMoveCamera;

        private const float cGridSpace     = 1f;
        private const float cGridTextSpace = 1.5f;
        private const float cTextSpace     = 1f;

        private void Awake() {
            sInstance = this;
        }
        
        public static void InitFromRandom(int _w, int _h, float _p) {
            var array = new bool[_w, _h];
            for (var x = 0; x < _w; x++) {
                for (var y = 0; y < _h; y++) {
                    var rand = UnityEngine.Random.Range(0, 1f);
                    array[x, y] = rand <= _p;
                }
            }

            InitFromArray(array);
        }

        public static void InitFromArray(bool[,] _array) {
            mIListH = new List<List<int>>();
            mIListW = new List<List<int>>();
            mHSize  = _array.GetLength(1);
            mWSize  = _array.GetLength(0);

            for (var y = 0; y < mHSize; y++) {
                var list   = new List<int>();
                var length = 0;

                for (var x = 0; x < mWSize; x++) {
                    if (_array[x, y]) {
                        length++;
                    } else {
                        if (length > 0) {
                            list.Add(length);
                            length = 0;
                        }
                    }
                }

                if (length > 0) {
                    list.Add(length);
                }

                if (list.Count == 0) list.Add(0);
                mIListH.Add(list);
            }

            for (var x = 0; x < mWSize; x++) {
                var list   = new List<int>();
                var length = 0;

                for (var y = 0; y < mHSize; y++) {
                    if (_array[x, y]) {
                        length++;
                    } else {
                        if (length > 0) {
                            list.Add(length);
                            length = 0;
                        }
                    }
                }

                if (length > 0) {
                    list.Add(length);
                }

                if (list.Count == 0) list.Add(0);
                mIListW.Add(list);
            }

            sInstance.GenerateBlock();
        }

        public static void InitFromString() {
            mIListH = new List<List<int>>();
            mIListW = new List<List<int>>();
            mHSize  = mSListH.Count;
            mWSize  = mSListW.Count;

            foreach (var line in mSListH) {
                var split = line.Split(',').Select(int.Parse).ToList();
                mIListH.Add(split);
            }

            foreach (var line in mSListW) {
                var split = line.Split(',').Select(int.Parse).ToList();
                mIListW.Add(split);
            }

            sInstance.GenerateBlock();
        }

        [Button]
        private void DoArray() {
            InitFromArray(mArray);
        }

        [Button]
        private void DoString() {
            InitFromString();
        }

        private void GenerateBlock() {
            var sum = 0;
            foreach (var line in mIListH) {
                sum += line.Sum();
                if (mWSize < line.Sum() + line.Count - 1) throw new Exception($"ERROR:横幅の超過");
            }

            foreach (var line in mIListW) {
                sum -= line.Sum();
                if (mHSize < line.Sum() + line.Count - 1) throw new Exception($"ERROR:縦幅の超過");
            }

            if (sum != 0) throw new Exception("ERROR:縦横の合計値の不一致");


            if (false) { //log
                var sb = new StringBuilder();
                foreach (var line in mIListH) {
                    foreach (var value in line) {
                        sb.Append(value + " ");
                    }

                    sb.Append("\n");
                }

                sb.Append("\n");

                foreach (var line in mIListW) {
                    foreach (var value in line) {
                        sb.Append(value + " ");
                    }

                    sb.Append("\n");
                }

                Debug.Log(sb.ToString());
            }

            //初期化
            mCamera.orthographicSize   = Math.Max(mWSize, mHSize);
            mCamera.transform.position = new Vector3(0, 0, -10);
            mParent.DestroyImmediateChildren();

            //背景の生成
            mGOffset = new Vector2(-mWSize / 2f + 0.5f, mHSize / 2f - 0.5f) * cGridSpace;
            for (var x = 0; x < mWSize; x++) {
                for (var y = 0; y < mHSize; y++) {
                    var pos = mGOffset + new Vector2(x, -y) * cGridSpace;
                    var obj = Instantiate(mBasePrefab, pos, Quaternion.identity, mParent);
                    obj.name = $"({x + 1},{y + 1})";
                }
            }

            //テキストの生成
            mTListW.Clear();
            for (var x = 0; x < mWSize; x++) {
                var list = new List<TextObjMono>();
                for (var i = 0; i < mIListW[x].Count; i++) {
                    var pos = mGOffset                         +
                              Vector2.right * (x * cGridSpace) +
                              Vector2.up    * (cGridTextSpace + (mIListW[x].Count - i - 1) * cTextSpace);
                    var text = Instantiate(mTextObjPrefab, pos, Quaternion.identity, mParent);
                    text.Init(mIListW[x][i]);
                    list.Add(text);
                }

                mTListW.Add(list);
            }

            for (var y = 0; y < mHSize; y++) {
                for (var i = 0; i < mIListH[y].Count; i++) {
                    var pos = mGOffset                        +
                              Vector2.down * (y * cGridSpace) +
                              Vector2.left * (cGridTextSpace + (mIListH[y].Count - i - 1) * cTextSpace);
                    var text = Instantiate(mTextObjPrefab, pos, Quaternion.identity, mParent);
                    text.Init(mIListH[y][i]);
                    text.SetColor(true);
                }
            }

            //ブロックの生成
            mBListH.Clear();
            for (var y = 0; y < mHSize; y++) {
                var list    = new List<BlockObjMono>();
                var offsetX = 0;
                for (var i = 0; i < mIListH[y].Count; i++) {
                    var value = mIListH[y][i];
                    var pos   = mGOffset + Vector2.down * (y * cGridSpace) + Vector2.right * ((offsetX + (value - 1) / 2f) * cGridSpace);
                    var scale = new Vector3(value            * cGridSpace, cGridSpace, cGridSpace);
                    var block = Instantiate(mBlockObjPrefab, pos, Quaternion.identity, mParent);
                    block.Init(y, i, scale);
                    list.Add(block);
                    offsetX += value + 1;
                }

                mBListH.Add(list);
            }
        }

        private void Update() {
            var mousePos = Input.mousePosition;
            var ray      = mCamera.ScreenPointToRay(mousePos);

            var scroll = Input.GetAxis("Mouse ScrollWheel");
            mCamera.orthographicSize += scroll * mCameraScaleSpeedK;


            if (mDragBlock == null) {
                if (Input.GetMouseButtonDown(0)) {
                    var hit = Physics2D.Raycast(ray.origin, ray.direction, 100, 1 << 6);
                    if (hit) {
                        var comp = hit.transform.GetComponent<BlockObjMono>();
                        mDragDifX = comp.PosX - ray.origin.x;

                        if (comp.IndexW > 0) {
                            var left = mBListH[comp.IndexH][comp.IndexW - 1];
                            mBlockXRange.x = left.PosX + left.ScaleX / 2f + cGridSpace;
                        } else {
                            mBlockXRange.x = mGOffset.x - cGridSpace / 2f;
                        }

                        if (comp.IndexW < mBListH[comp.IndexH].Count - 1) {
                            var right = mBListH[comp.IndexH][comp.IndexW + 1];
                            mBlockXRange.y = right.PosX - right.ScaleX / 2f - cGridSpace;
                        } else {
                            mBlockXRange.y = -mGOffset.x + cGridSpace / 2f;
                        }

                        mBlockXRange.x += comp.ScaleX / 2f;
                        mBlockXRange.y -= comp.ScaleX / 2f;

                        comp.SetDrag(true);
                        mDragBlock = comp;
                    }
                }
            }

            if (mDragBlock) {
                mDragBlock.SetX(Mathf.Clamp(ray.origin.x + mDragDifX, mBlockXRange.x, mBlockXRange.y));

                if (Input.GetMouseButtonUp(0)) {
                    var dif = mDragBlock.PosX - mGOffset.x;
                    if (mDragBlock.ScaleX / cGridSpace % 2 == 0) {
                        var x = (Mathf.RoundToInt(dif / cGridSpace + 0.5f) - 0.5f) * cGridSpace;
                        mDragBlock.SetX(mGOffset.x + x);
                    } else {
                        var x = (Mathf.RoundToInt(dif / cGridSpace)) * cGridSpace;
                        mDragBlock.SetX(mGOffset.x + x);
                    }

                    mDragBlock.SetDrag(false);
                    mDragBlock = null;
                }
            }

            var array = new bool[mWSize, mHSize];
            for (var y = 0; y < mBListH.Count; y++) {
                foreach (var block in mBListH[y]) {
                    var dif   = block.PosX - mGOffset.x;
                    var k     = block.ScaleX / cGridSpace % 2 == 0 ? 0.5f : 0;
                    var x     = (Mathf.RoundToInt(dif / cGridSpace + k) - k);
                    var left  = Mathf.RoundToInt(x - block.ScaleX / cGridSpace / 2f + 0.5f);
                    var right = Mathf.RoundToInt(x + block.ScaleX / cGridSpace / 2f - 0.5f);
                    for (var i = left; i <= right; i++) {
                        array[i, y] = true;
                    }
                }
            }

            for (var x = 0; x < mWSize; x++) {
                var list   = new List<int>();
                var length = 0;

                for (var y = 0; y < mHSize; y++) {
                    if (array[x, y]) {
                        length++;
                    } else {
                        if (length > 0) {
                            list.Add(length);
                            length = 0;
                        }
                    }
                }

                if (length > 0) {
                    list.Add(length);
                }

                if (list.Count == 0) list.Add(0);

                var collect = true;
                if (mTListW[x].Count == list.Count) {
                    for (var i = 0; i < list.Count; i++) {
                        if (mTListW[x][i].Value != list[i]) {
                            collect = false;
                            break;
                        }
                    }
                } else {
                    collect = false;
                }

                foreach (var text in mTListW[x]) {
                    text.SetColor(collect);
                }
            }
        }
    }
}