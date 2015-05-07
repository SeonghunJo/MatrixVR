using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;


public class WWWHelper : MonoBehaviour {
	
	/** 이벤트 연결을 위한 델리게이터 (대기자) */
	public delegate void HttpRequestDelegate(int id, WWW www);
	
	/** 이벤트 핸들러 */
	public event HttpRequestDelegate OnHttpRequest;
	
	/** 웹 서버로의 요청을 구분하기 위한 ID값 */
	private int requestId;
	
	/** 이 클래스의 싱글톤 객체 */
	static WWWHelper current = null;
	
	/** 객체를 생성하기 위한 GameObject */
	static GameObject container = null;
	
	/** 싱글톤 객체 만들기 */
	public static WWWHelper Instance {
		get {
			if (current == null) {
				container = new GameObject();
				container.name = "WWWHelper";
				current = container.AddComponent(typeof(WWWHelper)) as WWWHelper;
			}
			return current;
		}
	}
	
	/** HTTP GET 방식 통신 처리 */
	public void get(int id, string url) {
		WWW www = new WWW(url);
		StartCoroutine(WaitForRequest(id, www));
	}
	
	/** HTTP POST 방식 통신 처리 */
	public void post(int id, string url, IDictionary<string, string> data) {
		WWWForm form = new WWWForm();
		
		foreach (KeyValuePair<string, string> post_arg in data) {
			form.AddField(post_arg.Key, post_arg.Value);
		}
		
		WWW www = new WWW(url, form);
		StartCoroutine(WaitForRequest(id, www));
	}
	
	/** 통신 처리를 위한 코루틴 */
	private IEnumerator  WaitForRequest(int id, WWW www) {
		// 응답이 올떄까지 기다림
		yield return www;
		
		// 응답이 왔다면, 이벤트 리스너에 응답 결과 전달
		bool hasCompleteListener = (OnHttpRequest != null);
		
		if (hasCompleteListener) {
			OnHttpRequest(id, www);
		}
		
		// 통신 해제
		www.Dispose();
	}
	
}