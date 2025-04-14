# Kakao REST API Asp.Net C# 예제

이 프로젝트는 Kakao REST API를 Asp.Net C#으로 구현한 예제입니다.

## 주요 기능

- 카카오 로그인
- 사용자 정보 가져오기
- 친구 목록 가져오기
- 나에게 메시지 발송
- 친구에게 메시지 발송
- 로그아웃
- 연결 끊기

## 프로젝트 구조

- `Controllers/` - MVC 컨트롤러
- `Models/` - 데이터 모델
- `Views/` - Razor 뷰
- `wwwroot/` - 정적 파일 (CSS, JS, 이미지)

## 설치 방법

1. 프로젝트 클론
- Visual Studio 또는 VS Code로 열기

2. 의존성 설치
```bash
dotnet build 
```

3. 카카오 개발자 설정
- [Kakao Developers](https://developers.kakao.com)에서 애플리케이션 생성
- `appsettings.json`의 `ClientId`를 발급받은 REST API 키로 변경
- 카카오 로그인 활성화 설정
- Redirect URI 설정: `http://localhost:5035/redirect`

4. 서버 실행
```bash
dotnet run
```


## 사용 방법

1. 브라우저에서 `http://localhost:5035` 접속
2. 카카오 로그인 버튼 클릭
3. 각 기능 버튼을 통해 API 테스트

## 주의사항

- 카카오 로그인 Redirect URI가 정확히 설정되어 있어야 합니다.
- 친구 목록 조회와 메시지 발송을 위해서는 추가 동의가 필요합니다.

## 의존성
- Microsoft.AspNetCore.App: ASP.NET Core 프레임워크를 위한 메타패키지.
- Newtonsoft.Json: JSON 직렬화 및 역직렬화를 위한 라이브러리.
- EntityFrameworkCore: 데이터베이스와의 상호작용을 위한 ORM(Object-Relational Mapping) 프레임워크.
- HttpClientFactory: HTTP 요청을 효율적으로 관리하기 위한 라이브러리.

## 스크린샷
![image](https://github.com/user-attachments/assets/a64d2a83-c036-4cb2-88e5-07bba3890ec3)

