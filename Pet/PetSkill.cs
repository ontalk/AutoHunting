using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Unity.VisualScripting;
using Firebase.Firestore;

public class PetSkill : MonoBehaviour
{
    Animator anim;
    PetFollow petFollow;
    private bool isBuffActive = false;
    private float duration = 60f;
    private float CoolDownTime = 180f;
    private float lastExecutionTime; //이전 실행 시간 기록
    void Awake()
    {
        anim = GetComponent<Animator>();
        petFollow = FindObjectOfType<PetFollow>();
        lastExecutionTime = Time.time; // 초기화
    }
    private async void Start() //비동기
    {
        //가져오기
        DocumentReference documentRef = FirebaseFirestore.DefaultInstance.Collection("EnemyData").Document("1001");//컬렉션에있는 EnemyData에서 문서인 1001를 가져온다.
        //var documentRef = FirebaseFirestore.DefaultInstance.Collection("Test").GetSnapshotAsync();//컬렉션에 있는 모든 문서를 가져와라.
        DocumentSnapshot documentSnapshot = await documentRef.GetSnapshotAsync();
        Dictionary<string, object> CoolTime = documentSnapshot.ToDictionary();// string key값 , object value값 //object 는 더 큰 형변환 

        Debug.Log((long)CoolTime["Damage"]);//가져올때 형변환해서 가져와야됨.
                                     //Debug.Log((string)x["Name"]);
    }
   
    void Update()
    {
        StrBuff();
    }


    private void StrBuff()
    {
        if (!isBuffActive && GameManager.Instance.curHealth <= 90)
        {
            StartCoroutine(StartStrBuff());
            // 현재 시간으로 lastExecutionTime 갱신
            lastExecutionTime = Time.time;
        }
    }
    private IEnumerator StartStrBuff()//공격력 버프 
    {
        // 버프가 활성화되었음을 표시
        isBuffActive = true;

        petFollow.nav.enabled = false;
        GameManager.Instance.curDamage += 10;
        anim.SetTrigger("doBuff");

       /* yield return new WaitForSeconds(5f);// 애니메이션 도중 움직임 방지
        petFollow.nav.enabled = true;
*/
        yield return new WaitForSeconds(CoolDownTime); // 재사용 대기시간

        // CoolTime 이후에도 피가 90 이상인 경우에만 데미지 복귀
        if (GameManager.Instance.curHealth >= 90)
        {
            GameManager.Instance.curDamage = GameManager.Instance.MaxDamage;
            // 버프가 해제되었음을 표시
            isBuffActive = false;
        }

        
    }
}
