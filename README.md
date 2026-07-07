# Red Core Framework

Unity 프로젝트용 공용 프레임워크(UPM 패키지). 네임스페이스는 `RedMinS`.

## 선택적 Feature 활성화 조건

각 선택적 Feature는 독립 asmdef + `defineConstraints`로 격리되어 있어, 해당 심볼이
정의되지 않으면 어셈블리 자체가 컴파일되지 않는다(=미설치 프로젝트에 영향 없음).

| Feature | 어셈블리 | 심볼 | 심볼 정의 방식 | 필요한 SDK |
| --- | --- | --- | --- | --- |
| Ads | `RedMinS.RedCoreFramework.Ads` | `GOOGLE_MOBILE_ADS` | **자동** (versionDefines: `com.google.ads.mobile`) | Google Mobile Ads |
| Purchasing | `RedMinS.RedCoreFramework.Purchasing` | `RED_IAP` | **자동** (versionDefines: `com.unity.purchasing`) | Unity IAP 5 |
| Notifications | `RedMinS.RedCoreFramework.Notifications` | `RED_NOTIFICATIONS` | **자동** (versionDefines: `com.unity.mobile.notifications`) | Unity Mobile Notifications |
| GameServices | `RedMinS.RedCoreFramework.GameServices` | `RED_GPGS` | **수동** — 아래 참고 | Google Play Games plugin v2 |

### RED_GPGS 수동 설정

Google Play Games plugin v2는 UPM이 아니라 `.unitypackage`로 설치되므로 `versionDefines`로
자동 심볼 정의를 할 수 없다. GameServices Feature를 사용하려면
**Player Settings > Other Settings > Scripting Define Symbols**(Android 플랫폼)에
`RED_GPGS`를 수동으로 추가해야 `RedMinS.RedCoreFramework.GameServices` 어셈블리가 컴파일된다.

## Auth / Share

`GoogleCredentialService`, `LegacyGoogleIdProvider`, `ShareService`는 메인 Runtime
어셈블리에 포함되어 별도 심볼 없이 항상 컴파일된다(플랫폼 분기 `#if`만 사용).
Android Google 로그인 의존성(credentials/googleid)은 `Editor/RedCoreLoginDependencies.xml`을
EDM4U가 스캔해 해결한다.
