using UnityEngine;
using System.Collections.Generic;

namespace RedMinS
{
    public class ObjectPool //: MonoBehaviour
    {
        Dictionary<string, Queue<GameObject>> _objPools = null;


        public ObjectPool()
        {
            _objPools = new Dictionary<string, Queue<GameObject>>();
        }

        //
        void MakePool(string key)
        {
            _objPools.Add(key, new Queue<GameObject>());
        }

        GameObject MakeObject(GameObject prefab, Transform parent)
        {
            GameObject obj = Object.Instantiate(prefab, parent);
            obj.name = prefab.name;

            return obj;
        }

        // 오브젝트풀에서 꺼내서 사용
        public GameObject CreateObject(GameObject prefab, Transform parent)
        {
            if (prefab == null) return null;

            string objName = prefab.name;
            if (!_objPools.ContainsKey(objName))
            {
                MakePool(objName);
            }

            GameObject obj = null;
            if (_objPools[objName].Count > 0)
            {
                obj = _objPools[objName].Dequeue();
                obj.transform.SetParent(parent);
                obj.SetActive(true);
            }
            else
            {
                obj = MakeObject(prefab, parent);
            }

            obj.transform.localScale = new Vector3(1f, 1f, 1f);

            return obj;
        }

        // 오브젝트풀에 다시 넣어둠
        public void RemoveObject(GameObject obj)
        {
            obj.SetActive(false);

            if (!_objPools.ContainsKey(obj.name))
            {
                MakePool(obj.name);
            }

            _objPools[obj.name].Enqueue(obj);
        }

        // 풀의 오브젝트 모두 제거
        public void ClearPool()
        {
            foreach (var objs in _objPools)
            {
                foreach (var obj in objs.Value)
                {
                    Object.Destroy(obj.gameObject);
                }
            }

            _objPools.Clear();
        }

    }
}
