# DBH - Body Size Waste

RimWorld 1.6용 **Dubs Bad Hygiene 신체 크기별 오물량 패치**입니다.

기본 계산: **기존 발생량 × 실제 Pawn.BodySize × 설정 배율**.
신체 크기가 0.5면 절반, 2면 두 배의 오물이 발생합니다.

## 기능

- 전체 오물 배율: 0~10배
- 신체 크기 반영 강도: 0~2 (0은 크기 무시, 1은 비례, 2는 제곱 비례)
- 바닥 대변·소변 수량에 적용할지 선택
- 한국어·영어 설정 화면 및 배율 미리보기
- 변기·재래식 변기·야외 배변 적용
- 원본 모드 파일을 변경하지 않는 독립 Harmony 패치

## 설치

저장소를 다운로드해 `About`, `Assemblies`, `Languages` 폴더가 들어 있는 최상위 폴더를
`RimWorld/Mods/DBHBodySizeWaste`에 넣으세요. 완성된 DLL을 포함하므로 빌드는 선택 사항입니다.

로드 순서:

1. [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)
2. [Dubs Bad Hygiene](https://steamcommunity.com/sharedfiles/filedetails/?id=836308268)
3. DBH - Body Size Waste

`옵션 → 모드 설정 → DBH - Body Size Waste`에서 조절합니다.
변경은 다음 배출부터 적용됩니다.

## 범위와 제한

실제 `Pawn.BodySize`를 사용합니다. 그래픽 크기만 바꾸는 설정은 반영하지 않습니다.
물 소비량·배변 빈도·시설 용량·정화 속도는 유지합니다. 요강은 제외합니다.
다른 모드가 자체 구현한 배변 작업에는 추가 호환 작업이 필요할 수 있습니다.
자세한 동작과 상한은 [README.txt](README.txt)를 참고하세요.

현재 기준은 **RimWorld 1.6 / Dubs Bad Hygiene 3.1.2800**입니다.
컴파일, 계산 테스트 및 실제 DLL 대상 Harmony 패치 생성 검증을 통과했습니다.
**게임 내 플레이와 멀티플레이는 아직 검증하지 않았습니다.**

## 빌드

Windows의 .NET Framework C# 컴파일러와 로컬 RimWorld·필수 모드 DLL이 필요합니다.

```powershell
./Source/build.ps1 -GameDir 'D:\SteamLibrary\steamapps\common\RimWorld' -WorkshopDir 'D:\SteamLibrary\steamapps\workshop\content\294100'
```

결과: `Assemblies/DBHBodySizeWaste.dll`.
원본 게임·Harmony·Dubs Bad Hygiene DLL은 이 저장소에 포함하지 않습니다.

## English

An independent compatibility patch that scales Dubs Bad Hygiene waste by actual pawn body size.
Includes a configurable overall multiplier, body-size exponent, and optional ground filth scaling.
Applies to toilets, latrines and outdoor defecation. Bedpans are excluded.
Load after Harmony and Dubs Bad Hygiene. This is not an official Dubwise release.
