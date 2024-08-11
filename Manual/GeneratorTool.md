## Layer & Tagジェネレータ
Unityのレイヤーやタグを、定数として生成する機能です。  
レイヤー名やタグ名の変更による不具合を防止するために、基本的にこの定数を使ってください。


## 使い方
エディタ上部のツールバーのTools→LayerNameGeneratorとTagNameGeneratorをクリックすると生成されます。  
エディタのコンソールに`{path}にLayer(Tag)の定数ファイルを生成しました。`と表示されたら成功です。 
![NameGenerator.png](Images/NameGenerator.png)

ファイルは以下のパスに生成されます  
`Assets/Scripts/Module/Core/Constants/Tag.cs`  
`Assets/Scripts/Module/Core/Constants/Layer.cs`  

`Tag.タグ名`や`Layer.レイヤー名`で使用することができます。  
また、レイヤーマスクを使う場合は、`Layer.Mask.レイヤー名`でレイヤーマスクを取得してください。

### 使用例
![TagAndLayerExample.png](Images/TagAndLayerExample.png)