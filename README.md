# ASP.NET MVC Application

ASP.NET MVC 웹 애플리케이션 프로젝트입니다.

## 시작하기

1. 프로젝트 클론
2. Visual Studio 또는 VS Code로 열기
3. NuGet 패키지 복원
4. `dotnet run` 명령어로 실행

## 프로젝트 구조

- `Controllers/` - MVC 컨트롤러
- `Models/` - 데이터 모델
- `Views/` - Razor 뷰
- `wwwroot/` - 정적 파일 (CSS, JS, 이미지)

## Development

To run the application in development mode:

```bash
dotnet run
```

The application will be available at `https://localhost:5001` and `http://localhost:5000`

## Building for Production

To build the application for production:

```bash
dotnet publish -c Release
```

## License

This project is licensed under the MIT License. 