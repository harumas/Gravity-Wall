## 簡易レベルエディタ
ステージの作成を補助するツールです。  
ステージに必要なシーンや、オブジェクトの配置を簡単に行うことができます。

## 基本的な画面構成
![LevelToolOverview.png](Images/LevelToolOverview.png)

最初に表示されていない場合は  
シーンビュー右上の3点ボタン→Overlay Menu→ObjectSelectorとLevelToolbar  
をクリックして有効化してください。

## 1. レベル作成&選択ボタン

### ステージ作成方法

- Create Levelの右側の入力欄にレベル名を入力
- CreateLevelボタンを押すと自動的に1レベルのシーンが作成されます
- ステージはScenes/Template/LevelTemplateのテンプレートを元に作成されます
  - テンプレートの内容を変えたい場合は、@はるに報告してもらえるとありがたいです。

### ステージ削除方法

- Scenes/Level内にある、シーン名のフォルダ、シーンファイルを削除
- この例だとPlaygroundフォルダとPlaygroundシーンを削除する
![RemoveLevel.png](Images/RemoveLevel.png)

### ステージ選択方法

- CreateLevelボタンの右側のドロップダウンメニューを押すとレベル一覧が表示されます
- 任意のレベルを押すと開くことが出来ます

## 2. オブジェクト配置メニュー

- Prefabs/StageObject内のPrefabがアイコンとして表示されます
- クリックすると、マウスカーソルにオブジェクトが追従してくるのでクリックすることで配置できます
- Shiftキーでキャンセル(Escapeじゃないので注意)
- スナッピングの間隔はIncrement SnappingのMoveの部分で変えてください。
![Snapping.png](Images/Snapping.png)