using UnityEngine;
using System.Collections;

public class GPGHelper {
	private static AndroidJavaClass _helper;

	public static string getString(string text) {
		AndroidJavaClass ajc = new AndroidJavaClass ("joeyp.game.gpg.GPGHelper");
		return ajc.CallStatic<string>("getString", new object[] {text});
	}

	public static void setup() {
		AndroidJavaClass ajc = new AndroidJavaClass ("joeyp.game.gpg.GPGHelper");
		AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
		ajc.Call ("setup", new object[] {activity});

		_helper = ajc;
	}

	public static void start() {
		_helper.Call ("onStart", new object[] {});
	}

	public static void stop() {
		_helper.Call ("onStop", new object[] {});
	}
}
