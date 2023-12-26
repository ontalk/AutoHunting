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
    private float lastExecutionTime; //���� ���� �ð� ���
    void Awake()
    {
        anim = GetComponent<Animator>();
        petFollow = FindObjectOfType<PetFollow>();
        lastExecutionTime = Time.time; // �ʱ�ȭ
    }
    private async void Start() //�񵿱�
    {
        //��������
        DocumentReference documentRef = FirebaseFirestore.DefaultInstance.Collection("EnemyData").Document("1001");//�÷��ǿ��ִ� EnemyData���� ������ 1001�� �����´�.
        //var documentRef = FirebaseFirestore.DefaultInstance.Collection("Test").GetSnapshotAsync();//�÷��ǿ� �ִ� ��� ������ �����Ͷ�.
        DocumentSnapshot documentSnapshot = await documentRef.GetSnapshotAsync();
        Dictionary<string, object> CoolTime = documentSnapshot.ToDictionary();// string key�� , object value�� //object �� �� ū ����ȯ 

        Debug.Log((long)CoolTime["Damage"]);//�����ö� ����ȯ�ؼ� �����;ߵ�.
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
            // ���� �ð����� lastExecutionTime ����
            lastExecutionTime = Time.time;
        }
    }
    private IEnumerator StartStrBuff()//���ݷ� ���� 
    {
        // ������ Ȱ��ȭ�Ǿ����� ǥ��
        isBuffActive = true;

        petFollow.nav.enabled = false;
        GameManager.Instance.curDamage += 10;
        anim.SetTrigger("doBuff");

       /* yield return new WaitForSeconds(5f);// �ִϸ��̼� ���� ������ ����
        petFollow.nav.enabled = true;
*/
        yield return new WaitForSeconds(CoolDownTime); // ���� ���ð�

        // CoolTime ���Ŀ��� �ǰ� 90 �̻��� ��쿡�� ������ ����
        if (GameManager.Instance.curHealth >= 90)
        {
            GameManager.Instance.curDamage = GameManager.Instance.MaxDamage;
            // ������ �����Ǿ����� ǥ��
            isBuffActive = false;
        }

        
    }
}
