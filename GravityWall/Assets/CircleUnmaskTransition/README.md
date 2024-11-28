# 円形逆マスクを利用したトランジション
![](Sample/Gif/transitionReverseMask.gif) 
逆マスクを利用した円形上にフェードするトランジションとなります。
Unity上で動作します。
UnmaskForUGUIのインストールが必要です。


<br>
<br>

# 使用方法
1. [こちら](https://github.com/miya123123/CircleUnmaskTransition/releases/download/v.0.1.0/CircleUnmaskTransition.unitypackage)のunitypackageをインポートしてください。

2. [こちら](https://github.com/mob-sakai/UnmaskForUGUI)のUnmaskForUGUIをインストールしてください。ダウンロードしたUnmaskForUGUIというディレクトリをAssetsフォルダの下に配置するだけで大丈夫なようです。

4. Canvasを生成し、Prefab/Mask.prefabをCanvasの下に配置してください。

5. UnMaskオブジェクトにアタッチされているCircleUnmaskTransition AnimatorのParametersのFadeInもしくはFadeOutをtrueにするとフェードイン（フェードアウト）が実行されます。詳細についてはサンプルシーンをご確認ください。

6. フェードイン後に黒い背景画像の一部が残る場合は、FadeIn AnimationのUnMaskオブジェクトのScaleを大きくしてみてください。黒い背景画像の円形に切り取られる面積が大きくなります。合わせてFadeOut AnimationのScale値も同様の値に修正してください。

<br>
<br>

# 関連記事
https://qiita.com/miya_game_developer/items/a4a576f11f3c41558e83