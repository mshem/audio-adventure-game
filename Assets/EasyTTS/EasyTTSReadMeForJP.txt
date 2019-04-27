================
EasyTextToSpeech
================

【概要】
EasyTextToSpeechは
AndroidとiOSの音声読み上げネイティブ機能を簡単に使うためのプラグインです。

## 【動作条件】
iOS7以上
Android1.6以上　Androidは端末にTTS Engineがインストールされている必要があります。


## 【詳細説明と利用方法】
EasyTextToSpeechUtil.Speech(読みあげたい文字列)を呼び出すだけで、
音声読み上げを行う事ができます。

-----Androidの場合の注意事項-----
速やかな発音のための初期化　EasyTextToSpeechUtil.Initialize();
と
メモリ開放のための終了処理　EasyTextToSpeechUtil.Stop ();

を行う事をおすすめします。

Androidの場合、ユーザがTTSエンジンを切り替える事できます。
「設定>言語と文字入力>音声読み上げオプション」から変更でき、
音声読み上げエンジンによっては、端末の言語設定によって読み方を変える場合があります。

EasyTextToSpeechUtil.Initialize(language,engine);
によって、設定言語及び利用するエンジンをプログラムで切り替える事ができます。

言語の種類には、ISO言語コード639-1をご利用ください。英語はen、日本語はjaです。
engineの指定は、読みあげたいengineのパッケージ名を指定します。

EasyTextToSpeechUtil.GetEnginePkgArray() メソッドを利用する事で、
端末にインストールされているpackage名の一覧を取得することができます。

また、端末の読み上げオプション画面を開くメソッド：
EasyTextToSpeechUtil.OpenTTSSetting()

も利用できますので、必要に応じてご利用ください。
------------------------------


## 【デモ】
DemoEasyTextToSpeech.unity
GUIシステムを利用して入力した文字列を読み上げる簡単なサンプルシーンです。

DemoEasyTTS_SelectEngine
Android用にTTSエンジン切替機能を伴った、簡単なサンプルシーンです。


## 【インストール方法】
EasyTextToSpeech.unitypackageのパッケージをインストールしてください。

①以下の３ファイルを下記のパスに配置されているか確認してください。
Plugins/Android/EasyTextToSpeechUtil.jar
Plugins/iOS/EasyTextToSpeechUtil.m
Plugins/iOS/EasyTextToSpeechUtil.h

②EasyTextToSpeechをプロジェクトに含まれている事を確認してください。

## 【作成元】
FreCre Inc.
当社はこのプラグインの動作を保証しませんが、

バグ等が不具合があった場合には、
cebu.english.club@gmail.comまでご連絡ください。
出来る範囲でご対応致します。

## 【参考】
Android-refference
http://developer.android.com/reference/android/speech/tts/TextToSpeech.html

iOS
AVSpeechSynthesizer