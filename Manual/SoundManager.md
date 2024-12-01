# プロジェクト内におけるサウンドの再生方法について


## 1. 再生する座標を決める必要がない場合
SoundManagerを通して再生してください。  
SoundManagerを使用するには`SoundKey`と`MixerType`を指定する必要があります。  

**SoundKey**: 再生するAudioClipを指定するためのキー  
**MixerType**: 再生するサウンドの種類を指定するためのキー

### SoundKeyを生成する
それぞれのキーを設定するためには、`Resources/SoundSettings`でAudioClipを設定してください。  
`Key`の部分にサウンドキーの名前、`Value`の部分にはAudioClipを設定して、`SoundKeyとMixerTypeの生成`ボタンをクリックします。  
そうすると、設定内容を元にSoundKeyが生成されます。  

![SoundSetting.png](Images/SoundSetting.png)

### MixerTypeについて
MixerTypeは今のところ`BGM`, `SE`の二種類です。  
AudioMixerをSoundSettingsに設定することで、MixerTypeもコード生成されます。

### スクリプト上で使用する
生成されたSoundKeyを利用する方法は以下のとおりです。
`SoundManager.Instance.Play(SoundKey.設定した名前, MixerType.サウンドの種類);`

第3引数には`PlayContext`構造体を使用することで、再生時の音量やピッチを設定することができます。  
`volume`: 再生ボリューム  
`pitch`: 再生ピッチ


## 2. 再生する座標が決まっている場合
SoundManagerを使わずに、AudioSourceコンポーネントをつけてください。  
**AudioSourceコンポーネントを直接利用する際には、AudioMixerを忘れずに設定してください！(超重要)**
