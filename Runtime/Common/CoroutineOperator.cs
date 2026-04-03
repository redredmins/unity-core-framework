using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedMinS
{
    public class CoroutineOperator
    {
        MonoBehaviour _main;
        Dictionary<string, IEnumerator> _coroutines;

        public CoroutineOperator(MonoBehaviour main)
        {
            _main = main;
            _coroutines = new Dictionary<string, IEnumerator>();
        }

        public void StartMyCoroutine(string key, IEnumerator coroutine)
        {
            if (_coroutines.TryGetValue(key, out var existing))
            {
                if (existing != null) _main.StopCoroutine(existing);
                _coroutines[key] = coroutine;
            }
            else
            {
                _coroutines.Add(key, coroutine);
            }

            _main.StartCoroutine(_coroutines[key]);
        }

        public void StopMyCoroutine(string key)
        {
            if (_coroutines.TryGetValue(key, out var coroutine) && coroutine != null)
            {
                _main.StopCoroutine(coroutine);
                _coroutines.Remove(key);
            }
        }

        public void StopMyAllCoroutine()
        {
            _main.StopAllCoroutines();
            _coroutines.Clear();
        }
    }
}
