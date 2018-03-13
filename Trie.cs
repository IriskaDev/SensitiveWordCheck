using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StringUtils
{
#if UNITY_EDITOR
    public static class SensitiveTestRoutine
    {
        public static void StartTest()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            SensitiveWordCheck.Instance.Init();
            watch.Stop();
            Debug.Log("Build Tree Time Cost: " + watch.ElapsedMilliseconds + "ms");

            string content =
                "工工大吃大喝 膨地蝇蝇械历功地 地地厅基本原则圾有我有有的的的人仍厅工13208921209式某月无出售氫化可的松、出售曲马多、出售乳化炸药、出售三箭牌汽枪、出售三箭气枪、出售三唑仑、出售三唑仑片、出售三唑侖、出售森林之豹、出售森林之虎、出售肾、出售手qiang/\n" +
                "123421591344982324312424123424234\n" +
                "箭牌汽枪图纸、三箭牌汽枪制造、三箭牌汽枪转让、三箭气枪qq、三箭气枪到货、三箭气枪电话、三箭气枪订购、三箭气枪技术转让、三箭气枪制造图、三箭\n" +
                "秃鹰汽枪出货asfeeeeeeeeeeeeq wqt wrqer qwerqwrsfa fasfefasdfesadfe\n" +
                "夲枯顶替圾有针有我枯萎 要顶替 基本原则枯地顶替“萨德”烛光集会\n" +
                "奇   顶替  顶替 顶替 《三亚宣言》圆顶  有我有有在地有胸 厅 有\n" +
                "4312424123424173.234.162.152、173.244.222.118、173.83.111.5、173.83.216..85、枯顶替圾有针有我\n" +
                "枯顶替茜村有村枯顶替不查都是领导的脑子、不查他是公仆、不诚模压者勿扰、不枯栽无可奈何花落去\n" +
                "枯基本原则圾霜期地村枯要基本原则圾顶替奇才顶替无可奈何花落去 顶替顶替桂林东走西顾 霜无可奈何花落去顶替顶替顶替\n" +
                "柜橱裁夺要十二月 的捡拾夺1312312321ljsdioeflkjasdi 。lasd 1290123..foa23l34sdfaflk23123lk";

            string[] strs = content.Split('\n');

            watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 10000; ++i)
            {
                for (int j = 0; j < strs.Length; ++j)
                {
                    KeyValuePair<int, string>[] pairs = SensitiveWordCheck.Instance.FindWordsInString(strs[j]);
                }
            }
            watch.Stop();
            Debug.Log("100000 Find Words Time Cost: " + watch.ElapsedMilliseconds + "ms");

            watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 10000; ++i)
            {
                for (int j = 0; j < strs.Length; ++j)
                {
                    bool exist = SensitiveWordCheck.Instance.CheckWordsInString(strs[j]);
                }
            }
            watch.Stop();
            Debug.Log("100000 Check Time Cost: " + watch.ElapsedMilliseconds + "ms");

            string checkResult = "";
            for (int j = 0; j < strs.Length; ++j)
            {
                bool exist = SensitiveWordCheck.Instance.CheckWordsInString(strs[j]);
                checkResult += exist.ToString() + ", ";
            }
            Debug.Log("Check Result: " + checkResult);

        }
    }
#endif

    public class SensitiveWordCheck : Singleton<SensitiveWordCheck>
    {
        private Trie _WordSet;

        public SensitiveWordCheck()
        {
            TextAsset txt = (TextAsset) Resources.Load("IllegalKeyword", typeof (TextAsset));
            string content = txt.text;
            string[] words = content.Split('、');
            _WordSet = new Trie(words);
        }

        public void Init()
        {

        }

        /// <summary>
        /// 查询目标中所有存在于词集中的单词
        /// </summary>
        /// <param name="target">目标字串</param>
        /// <returns>返回键值对，键为目标字串索引值，值为匹配到的单词</returns>
        public KeyValuePair<int, string>[] FindWordsInString(string target)
        {
            return _WordSet.FindWordsInString(target);
        }

        /// <summary>
        /// 查询目标中是否存在单词集中包含的单词
        /// </summary>
        /// <param name="target">目标字串</param>
        /// <returns>目标字串是否存在单词集中包含的单词</returns>
        public bool CheckWordsInString(string target)
        {
            return _WordSet.CheckWordsInString(target);
        }
    }

    public class Trie
    {
        private class Node
        {
            public Dictionary<char, Node> SubTrees;
            public string Value;

            public Node(string value)
            {
                SubTrees = new Dictionary<char, Node>();
                Value = value;
            }
        }

        private Node _Root;

        /// <summary>
        /// 构造函数，输入为单词列表
        /// </summary>
        /// <param name="words">单词列表</param>
        public Trie(string[] words)
        {
            _Root = new Node(null);
            for (int i = 0; i < words.Length; ++i)
                PutWord(words[i]);
        }

        /// <summary>
        /// 往单词集中增加一个单词
        /// </summary>
        /// <param name="word">单词</param>
        public void PutWord(string word)
        {
            Node iter = _Root;
            for (int i = 0; i < word.Length; ++i)
            {
                char uid =  word[i];
                Node tmp;
                if (iter.SubTrees.TryGetValue(uid, out tmp))
                {
                    iter = tmp;
                    if (i == word.Length - 1) iter.Value = word;
                }
                else
                {
                    Node newNode = new Node((i == word.Length - 1) ? word : null);
                    iter.SubTrees.Add(uid, newNode);
                    iter = newNode;
                }
            }
        }

        /// <summary>
        /// 查询目标中所有存在于词集中的单词
        /// </summary>
        /// <param name="target">目标字串</param>
        /// <returns>返回键值对数组，键为目标字串索引值，值为匹配到的单词</returns>
        public KeyValuePair<int, string>[] FindWordsInString(string target)
        {
            List<KeyValuePair<int, string>> results = new List<KeyValuePair<int, string>>();
            for (int i = 0; i < target.Length; ++i)
            {
                Node iter = _Root;
                for (int j = i; j < target.Length; ++j)
                {
                    Node tmp;
                    if (iter.SubTrees.TryGetValue(target[j], out tmp))
                    {
                        iter = tmp;
                        if (iter.Value != null)
                            results.Add(new KeyValuePair<int, string>(i, iter.Value));
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return results.ToArray();
        }

        /// <summary>
        /// 查询目标中是否存在单词集中包含的单词
        /// </summary>
        /// <param name="target">目标字串</param>
        /// <returns>true->目标字符有敏感词; false->目标字串没有敏感词</returns>
        public bool CheckWordsInString(string target)
        {
            for (int i = 0; i < target.Length; ++i)
            {
                Node iter = _Root;
                for (int j = i; j < target.Length; ++j)
                {
                    Node tmp;
                    if (iter.SubTrees.TryGetValue(target[j], out tmp))
                    {
                        iter = tmp;
                        if (iter.Value != null)
                            return true;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return false;
        }
    }
}
