// ========================================
// 🎯 Unity Package Manager 버전 관리 완전 가이드
// ========================================

/*
========================================
🚀 Package Manager 버전 관리 완성!
========================================

이제 다음이 준비되었습니다:

✅ 완전한 Core Framework
✅ UPM Migration Tool
✅ 자동화된 패키지 생성
✅ Git 버전 관리 설정
✅ Semantic Versioning 지원

========================================
📋 사용 방법 - 단계별 가이드
========================================

1단계: UPM 패키지 생성
---------------------
1. Unity 에디터에서 RedMinS → Package Manager → Version Manager 실행
2. 패키지 정보 설정:
   - Package Name: com.redmins.core-framework
   - Display Name: RedMinS Core Framework  
   - Version: 1.0.0
   - Author: 당신의 이름
   - Repository URL: 깃허브 저장소 URL

3. "Complete Migration" 버튼 클릭
4. ✅ Packages/com.redmins.core-framework 폴더에 완전한 패키지 생성됨

2단계: Git Repository 설정
-------------------------
1. "Copy Git Commands" 버튼 클릭 (클립보드에 복사됨)
2. 터미널에서 명령어 실행:

cd Packages/com.redmins.core-framework
git init
git add .
git commit -m "feat: Initial RedMinS Core Framework v1.0.0"

3. GitHub에서 새 저장소 생성
4. 명령어 계속 실행:

git remote add origin https://github.com/yourusername/core-framework.git
git push -u origin main
git tag v1.0.0
git push origin v1.0.0

3단계: Package Manager 설치 테스트
--------------------------------
1. 새 Unity 프로젝트 생성
2. Window → Package Manager
3. "+" → "Add package from git URL" 
4. URL 입력: https://github.com/yourusername/core-framework.git
5. ✅ 자동으로 패키지 설치 및 의존성 해결

========================================
🔄 버전 업데이트 워크플로우
========================================

새 기능 추가 시 (Minor Update):
------------------------------
1. 패키지 폴더에서 코드 수정
2. package.json에서 버전 업데이트: 1.0.0 → 1.1.0
3. CHANGELOG.md에 변경사항 기록
4. Git 커밋 및 태그:

git add .
git commit -m "feat: Add AudioManager system"
git tag v1.1.0
git push origin main
git push origin v1.1.0

버그 수정 시 (Patch Update):
---------------------------
1. 버그 수정
2. package.json: 1.0.0 → 1.0.1
3. CHANGELOG.md 업데이트
4. Git 커밋:

git add .
git commit -m "fix: Resolve scene loading memory leak"
git tag v1.0.1
git push origin main
git push origin v1.0.1

대규모 변경 시 (Major Update):
-----------------------------
1. Breaking changes 구현
2. package.json: 1.0.0 → 2.0.0
3. Migration 가이드 작성
4. Git 커밋:

git add .
git commit -m "feat!: Redesign Core architecture (BREAKING CHANGE)"
git tag v2.0.0
git push origin main
git push origin v2.0.0

========================================
📦 팀원 설치 및 사용법
========================================

새 팀원 온보딩:
--------------
1. Unity 프로젝트 클론
2. Package Manager에서 프레임워크 설치:
   - URL: https://github.com/yourusername/core-framework.git
3. 특정 버전 설치 (안정성이 중요한 경우):
   - URL: https://github.com/yourusername/core-framework.git#v1.0.0

자동 업데이트:
-------------
- Package Manager에서 "Update to latest" 클릭
- 새 태그가 생성되면 자동으로 업데이트 알림

수동 버전 지정:
--------------
```json
// Packages/manifest.json에서
{
  "dependencies": {
    "com.redmins.core-framework": "https://github.com/yourusername/core-framework.git#v1.0.0"
  }
}
```

========================================
🏗️ 패키지 구조 최적화
========================================

생성된 패키지 구조:
Packages/com.redmins.core-framework/
├── 📄 package.json              (패키지 메타데이터)
├── 📄 README.md                 (사용법 가이드)
├── 📄 CHANGELOG.md              (버전 히스토리)
├── 📄 LICENSE.md                (라이선스)
├── 📁 Runtime/                  (런타임 스크립트)
│   ├── 📁 Core/                (핵심 시스템)
│   ├── 📁 SceneManagement/     (씬 관리)
│   ├── 📁 UI/                  (UI 시스템)
│   ├── 📁 Module/              (유틸리티 모듈)
│   └── 📄 RedMinS.Runtime.asmdef
├── 📁 Editor/                   (에디터 도구)
│   └── 📄 RedMinS.Editor.asmdef
├── 📁 Samples~/                 (예제 코드)
└── 📁 Documentation~/           (문서)

========================================
🎯 고급 버전 관리 기능
========================================

조건부 버전 설치:
---------------
```json
// 특정 Unity 버전에 맞는 패키지 버전
{
  "name": "com.redmins.core-framework",
  "version": "1.0.0",
  "unity": "2021.3",
  "unityRelease": "0b1"
}
```

의존성 관리:
-----------
```json
// 다른 패키지 의존성 자동 설치
{
  "dependencies": {
    "com.unity.textmeshpro": "3.0.6",
    "com.unity.addressables": "1.19.19"
  }
}
```

개발 버전 관리:
--------------
```bash
# 개발 브랜치에서 작업
git checkout -b develop
git push -u origin develop

# 베타 버전 태그
git tag v1.1.0-beta.1
git push origin v1.1.0-beta.1

# 설치: packageurl.git#v1.1.0-beta.1
```

========================================
🚨 문제 해결 가이드
========================================

흔한 문제들:

1. "Package not found" 오류
   → Repository가 public인지 확인
   → URL이 정확한지 확인 (.git 포함)

2. "Version conflict" 오류  
   → 의존성 버전 충돌
   → package.json에서 의존성 버전 조정

3. "Assembly definition" 오류
   → Assembly definition 파일이 올바른지 확인
   → 네임스페이스 충돌 해결

4. "Import errors" 
   → TextMeshPro 등 필수 패키지 설치 확인
   → Unity 버전 호환성 확인

디버깅 팁:
---------
- Unity Console에서 패키지 로딩 로그 확인
- Package Manager의 "In Project" 탭에서 상태 확인
- Packages/manifest.json 파일 직접 편집
- 캐시 삭제: Library/PackageCache 폴더 삭제

========================================
📈 성능 및 최적화
========================================

패키지 크기 최적화:
-----------------
- 불필요한 파일 제외 (.gitignore 활용)
- 대용량 에셋은 별도 패키지로 분리
- 문서는 Documentation~ 폴더 사용

로딩 성능:
---------
- Assembly definition으로 컴파일 시간 단축
- 조건부 컴파일로 플랫폼별 최적화
- Lazy loading 패턴 적용

네트워크 최적화:
--------------
- Git LFS로 대용량 파일 관리
- Shallow clone으로 히스토리 최소화
- CDN 활용 (GitHub Releases)

========================================
🎊 축하합니다! 완성된 기능들
========================================

✅ **Package Manager 버전 관리**: Git 기반 Semantic Versioning
✅ **자동 의존성 해결**: TextMeshPro 등 필수 패키지 자동 설치  
✅ **팀 협업 지원**: 일관된 버전, 쉬운 업데이트
✅ **에디터 도구 통합**: 마이그레이션 자동화
✅ **전문적인 패키지 구조**: Industry standard 준수
✅ **완전한 문서화**: README, CHANGELOG, 라이선스

이제 당신의 Unity Framework가:
🚀 **전 세계적으로 배포** 가능
🔄 **자동 버전 관리** 지원  
👥 **팀 협업에 최적화**
📦 **Package Manager 표준** 준수
🏆 **오픈소스 생태계** 참여 준비

========================================
🔮 다음 단계 제안
========================================

1. **Unity Asset Store 등록**
   - 상업적 배포 고려
   - 더 넓은 사용자층 확보

2. **CI/CD 파이프라인 구축**  
   - GitHub Actions로 자동 테스트
   - 자동 배포 및 릴리즈 노트 생성

3. **커뮤니티 구축**
   - Discord 서버 개설
   - 사용자 피드백 수집

4. **확장 패키지 개발**
   - Audio Manager 패키지
   - Analytics 패키지  
   - Platform별 최적화 패키지

5. **문서 사이트 구축**
   - GitBook 또는 Docusaurus
   - 튜토리얼 비디오 제작

========================================

🎉 **완벽한 Unity Package Manager 버전 관리 시스템 완성!**

이제 전문적인 오픈소스 Unity 패키지를 운영하고
전 세계 개발자들과 공유할 준비가 완료되었습니다! 🌟

성공적인 패키지 운영 되시길 바랍니다! 🚀
*/