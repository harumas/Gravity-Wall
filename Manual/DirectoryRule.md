## Unityのディレクトリ構成について

```
Assets
├── Contents: デザイナーの素材、マテリアル
├── PlayTest: プレイテストの仮素材、スクリプト
├── Plugins: 外部ライブラリ
├── Prefabs: ゲームで使用するPrefabオブジェクト(素材は置かない！)
├── Resources: 動的にロードするオブジェクト
├── Scenes: シーン関係のファイル
│   ├── Level: 各ステージのシーンデータ
│   ├── Title: タイトルシーンのデータ
│   ├── Template: ステージのテンプレートシーンのデータ
│   
├── Scripts: ゲームのソースコード
│   ├── Editor: エディタ拡張
│   ├── GameMain: 各モジュールを統括するレイヤーのスクリプト
│   ├── Module: モジュールスクリプト
│       ├── Core: ゲームの仕様に依存しない技術基盤
├── Settings: Unityの設定ファイル
```