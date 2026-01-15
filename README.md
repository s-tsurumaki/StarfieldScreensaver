# StarfieldScreensaver

[日本語](#日本語) | [English](#english)

Windows 98 時代のスクリーンセーバー **「Starfield Simulation」** を、現代環境向けにアレンジして再実装したスクリーンセーバーです。  
C# / .NET 8 + Visual Studio 2022 で開発しています。

> Note: This project is a fan-made reinterpretation and is not affiliated with Microsoft.

---

## 日本語

### 概要
**StarfieldScreensaver** は、懐かしの「Starfield Simulation」風の星空ワープ表現を、現代の Windows 向けスクリーンセーバーとして作り直したものです。  
マルチモニタの全画面表示、プレビュー（設定画面の小窓）埋め込み、各種パラメータ設定に対応しています。

### 動作環境 / 開発環境
- Windows（スクリーンセーバーが動作する環境）
- Visual Studio 2022
- C#
- .NET 8

### 機能
- `/s` 全画面表示（マルチモニタ対応）
- `/p <HWND>` プレビュー表示（コントロールに埋め込み）
- `/c` 設定画面（密度・速度・視野・トレイル等）
- マウス移動 / キー入力で終了（しきい値あり）
- 設定は次のファイルへ保存  
  `%LOCALAPPDATA%\StarfieldScreensaver\settings.json`

### 使い方（スクリーンセーバーとして）
1. Release ビルドした実行ファイルを `*.scr` にリネームします  
   例: `StarfieldScreensaver.exe` → `StarfieldScreensaver.scr`
2. `StarfieldScreensaver.scr` を任意の場所へ配置します  
   - 右クリック → **インストール**（環境によって表示されます）
   - または Windows の「スクリーン セーバー設定」から参照して選択

> ヒント: 開発中は、`/s` や `/c` を付けて直接起動すると動作確認が楽です。

### コマンドライン引数
```txt
/s                 全画面（スクリーンセーバー本体）
/p <HWND>          プレビュー（設定ダイアログの小窓に埋め込み）
/c                 設定画面
```

例:
```bat
StarfieldScreensaver.scr /s
StarfieldScreensaver.scr /c
StarfieldScreensaver.scr /p 123456
```

### 設定ファイル
設定は以下に保存されます:
- `%LOCALAPPDATA%\StarfieldScreensaver\settings.json`

（例: 密度、速度、視野(FOV)、トレイル、入力しきい値 など）

### ビルド
- Visual Studio 2022 でソリューションを開き、`Release` でビルドしてください。
- 生成物を `*.scr` にリネームすると、スクリーンセーバーとして扱えます。

### ライセンス
MIT License

---

## English

### Overview
**StarfieldScreensaver** is a modern Windows screensaver inspired by the classic Windows 98-era **“Starfield Simulation”**.  
It supports multi-monitor fullscreen mode, the standard Windows screensaver preview host, and configurable visual parameters.

### Requirements / Dev Stack
- Windows (for screensaver hosting)
- Visual Studio 2022
- C#
- .NET 8

### Features
- `/s` Fullscreen mode (multi-monitor supported)
- `/p <HWND>` Preview mode (embedded into the host control)
- `/c` Configuration UI (density, speed, FOV, trail, etc.)
- Exit on mouse movement / key input (with threshold)
- Settings saved to:  
  `%LOCALAPPDATA%\StarfieldScreensaver\settings.json`

### Install / Use as a Screensaver
1. Build the project (Release recommended).
2. Rename the output from `*.exe` to `*.scr`  
   Example: `StarfieldScreensaver.exe` → `StarfieldScreensaver.scr`
3. Place the `.scr` file anywhere you like, then:
   - Right-click → **Install** (if available), or
   - Open Windows “Screen Saver Settings” and browse/select it.

### Command line arguments
```txt
/s                 Fullscreen (screensaver mode)
/p <HWND>          Preview (hosted in a control)
/c                 Configuration
```

Examples:
```bat
StarfieldScreensaver.scr /s
StarfieldScreensaver.scr /c
StarfieldScreensaver.scr /p 123456
```

### Settings
Settings are stored here:
- `%LOCALAPPDATA%\StarfieldScreensaver\settings.json`

(Examples: density, speed, FOV, trail, input threshold, etc.)

### Build
- Open the solution in Visual Studio 2022 and build (Release recommended).
- Rename the output to `.scr` to use it as a Windows screensaver.

### License
MIT License
