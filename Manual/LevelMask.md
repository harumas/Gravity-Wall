## ステージの隠蔽機能
ステージが廊下以外から見えなくする機能です。
Tutorial02のシーンで試しに組んでみたので、参考にしてください。

## 必要なオブジェクト

### MaskVolume (緑色のボックス)
部屋を隠蔽するためのボックスです。緑色に囲われた範囲が隠蔽されます。  
プレイヤーが緑色のボックスにいる間は隠蔽されません。(自分のいる部屋が見えなくなるので)

Prefabのパス: Prefabs/StageObject/MaskVolume

### FilterVolume (赤色のボックス)
MaskVolumeを透過するためのボックスです。このボックスの範囲を通してMaskVolumeの中を見ることができます。  
部屋と部屋を繋ぐ廊下の部分に使ってください。

Prefabのパス: Prefabs/StageObject/FilterVolume

![LevelVolume.png](Images/LevelVolume.png)

## MaskVolumeコンポーネント
Mask VolumeオブジェクトにはMask Volumeコンポーネントが割り当てられています。  
Filter Groupは部屋に接続する廊下に用いた、FilterVolumeを割り当ててください。  
そうすることで、部屋に接続するFilterVolumeのみ有効化することができます。

MaskVolumeを選択することで、接続しているFilterVolumeも表示されます。

![MaskVolumeComponent.png](Images/MaskVolumeComponent.png)

## Volumeを作る際のコツ
部屋や廊下のサイズに出来るだけぴったり合わせるようにすると自然になります。  
少しでも部屋のサイズより小さくなったりすると、カメラの角度によっては見えたりするので気をつけてください。  
グレーボックスの段階は、割と適当でも良い気がします。

