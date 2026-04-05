using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RedMinS
{
    /// <summary>
    /// id → 수량(int) 쌍을 관리하는 범용 컨테이너.
    /// 아이템/쿠폰/스탬프/카운터 등 "뭔가를 세는" 모든 용도에 사용 가능.
    /// JsonUtility 직렬화 호환을 위해 내부 저장은 List&lt;Entry&gt;, 런타임 조회는 Dictionary 캐시.
    ///
    /// 설계 원칙:
    /// - 수량이 0이 되어도 엔트리는 유지한다. (이력 추적 및 일부 백엔드의 "경로 생성" 제약 대응)
    /// - Consume 은 차감 전 수량을 검증해 부족하면 상태를 변경하지 않고 false 를 반환한다 (원자적).
    /// - 특정 게임 타입(ItemInfo 등)에 의존하지 않는 순수 자료구조.
    /// </summary>
    [Serializable]
    public class CountableCollection
    {
        [Serializable]
        struct Entry
        {
            public int id;
            public int count;
        }

        [SerializeField] List<Entry> _entries = new List<Entry>();

        [NonSerialized] Dictionary<int, int> _cache;
        [NonSerialized] bool _cacheBuilt;

        /// <summary>수량이 변경될 때마다 발생. (id, delta, newCount)</summary>
        public event UnityAction<int, int, int> OnChanged;

        void EnsureCache()
        {
            if (_cacheBuilt) return;
            _cache = new Dictionary<int, int>(_entries.Count);
            for (int i = 0; i < _entries.Count; i++)
                _cache[_entries[i].id] = _entries[i].count;
            _cacheBuilt = true;
        }

        /// <summary>JsonUtility 역직렬화 직후 수동으로 캐시를 재빌드해야 할 때 호출.</summary>
        public void RebuildCache()
        {
            _cacheBuilt = false;
            EnsureCache();
        }

        public int GetCount(int id)
        {
            EnsureCache();
            return _cache.TryGetValue(id, out var c) ? c : 0;
        }

        public bool Has(int id, int count = 1) => GetCount(id) >= count;

        /// <summary>주어진 id 의 수량을 증가시킨다. 엔트리가 없으면 새로 생성.</summary>
        public void Add(int id, int count = 1)
        {
            if (count <= 0) return;
            EnsureCache();
            int cur = _cache.TryGetValue(id, out var c) ? c : 0;
            int next = cur + count;
            _cache[id] = next;
            WriteEntry(id, next);
            OnChanged?.Invoke(id, count, next);
        }

        /// <summary>
        /// 주어진 id 의 수량을 차감한다. 현재 수량이 부족하면 상태를 바꾸지 않고 false.
        /// 수량이 0이 되어도 엔트리는 유지된다.
        /// </summary>
        public bool Consume(int id, int count = 1)
        {
            if (count <= 0) return true;
            EnsureCache();
            int cur = _cache.TryGetValue(id, out var c) ? c : 0;
            if (cur < count) return false;
            int next = cur - count;
            _cache[id] = next;
            WriteEntry(id, next);
            OnChanged?.Invoke(id, -count, next);
            return true;
        }

        /// <summary>
        /// 특정 id 의 수량을 지정 값으로 강제 설정한다 (초기화, 서버 동기화 등의 용도).
        /// 음수는 0으로 클램프.
        /// </summary>
        public void Set(int id, int count)
        {
            if (count < 0) count = 0;
            EnsureCache();
            int cur = _cache.TryGetValue(id, out var c) ? c : 0;
            _cache[id] = count;
            WriteEntry(id, count);
            if (cur != count) OnChanged?.Invoke(id, count - cur, count);
        }

        /// <summary>현재 보관 중인 (id, count) 엔트리들의 읽기 전용 뷰.</summary>
        public IReadOnlyDictionary<int, int> Entries
        {
            get { EnsureCache(); return _cache; }
        }

        public int EntryCount
        {
            get { EnsureCache(); return _cache.Count; }
        }

        public void Clear()
        {
            _entries.Clear();
            if (_cache != null) _cache.Clear();
            _cacheBuilt = true;
        }

        // --- 내부: 직렬화용 List<Entry> 갱신 ---
        void WriteEntry(int id, int count)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].id == id)
                {
                    _entries[i] = new Entry { id = id, count = count };
                    return;
                }
            }
            _entries.Add(new Entry { id = id, count = count });
        }
    }
}
