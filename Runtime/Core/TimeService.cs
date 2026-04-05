using System;
using UnityEngine;

namespace RedMinS
{
    /// <summary>
    /// 시간 소스 추상화. 로컬 시간, 서버 동기 시간 등 구현체를 스왑하기 위한 인터페이스.
    /// </summary>
    public interface ITimeProvider
    {
        long NowUnixSeconds { get; }
        DateTime NowUtc { get; }
    }

    /// <summary>
    /// 로컬 UTC 시간을 기반으로 하되, 서버 시간과의 오프셋을 보정할 수 있는 기본 구현체.
    /// 서버에서 한 번 현재 시각을 받아 <see cref="SyncOffset"/> 를 호출하면
    /// 이후 모든 조회는 서버 기준 시각을 반환한다.
    /// </summary>
    public class LocalTimeProvider : ITimeProvider
    {
        long _offsetSeconds; // server - local

        public long NowUnixSeconds => DateTimeOffset.UtcNow.ToUnixTimeSeconds() + _offsetSeconds;
        public DateTime NowUtc => DateTime.UtcNow.AddSeconds(_offsetSeconds);
        public long OffsetSeconds => _offsetSeconds;

        /// <summary>서버에서 받은 Unix timestamp(초)로 로컬과의 오프셋을 갱신한다.</summary>
        public void SyncOffset(long serverUnixSeconds)
        {
            _offsetSeconds = serverUnixSeconds - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }

    /// <summary>
    /// AppManager 에 등록되는 시간 모듈. 기본적으로 <see cref="LocalTimeProvider"/> 를 사용하며,
    /// Firebase/커스텀 서버 연동 시 <see cref="SetProvider"/> 로 구현체를 교체할 수 있다.
    /// </summary>
    public class TimeService : MonoBehaviour, ITimeProvider
    {
        ITimeProvider _impl;

        void Awake()
        {
            if (_impl == null) _impl = new LocalTimeProvider();
        }

        public void SetProvider(ITimeProvider provider)
        {
            _impl = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public long NowUnixSeconds => (_impl ??= new LocalTimeProvider()).NowUnixSeconds;
        public DateTime NowUtc => (_impl ??= new LocalTimeProvider()).NowUtc;

        /// <summary>현재 provider 가 <see cref="LocalTimeProvider"/> 인 경우 서버 시각 오프셋을 동기화한다.</summary>
        public void SyncLocalOffset(long serverUnixSeconds)
        {
            if (_impl is LocalTimeProvider local) local.SyncOffset(serverUnixSeconds);
        }
    }
}
