using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFire : MonoBehaviour
{
    // �߻� ��ġ
    public GameObject firePosition;

    // ��ô ���� ������Ʈ
    public GameObject bombFactory;

    // ��ô �Ŀ�
    public float throwPower = 15f;

    // �ǰ� ����Ʈ ������Ʈ
    public GameObject bulletEffect;

    // �ǰ� ����Ʈ ��ƼŬ �ý���
    ParticleSystem ps;

    // �߻� ���� ���ݷ�
    public int weaponPower = 5;

    // �ִϸ����� ����
    Animator anim;

    // ���� ��� ����
    enum WeaponMode
    {
        Normal,
        Sniper
    }
    WeaponMode wMode;

    // ī�޶� Ȯ�� Ȯ�ο� ����
    bool ZoomMode = false;

    // ���� ��� �ؽ�Ʈ
    public Text wModeText;

    // �� �߻� ȿ�� ������Ʈ �迭
    public GameObject[] eff_Flash;

    void Start()
    {
        // �ǰ� ����Ʈ ������Ʈ���� ��ƼŬ �ý��� ������Ʈ ��������
        ps = bulletEffect.GetComponent<ParticleSystem>();

        // �ִϸ����� ������Ʈ ��������
        anim = GetComponentInChildren<Animator>();

        // ���� �⺻ ��带 ��� ���� �����Ѵ�.
        wMode = WeaponMode.Normal;
    }

    void Update()
    {
        // ���� ���°� "���� ��" ������ ������ ���� �����ϰ� �Ѵ�.
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        // ��� ���: ���콺 ������ ��ư�� ������ �ü� �������� ����ź�� ������ �ʹ�.
        // �������� ���: ���콺 ������ ��ư�� ������ ȭ���� Ȯ���ϰ� �ʹ�.

        // ���콺 ������ ��ư �Է��� �޴´�.
        if (Input.GetMouseButtonDown(1))
        {
            switch (wMode)
            {
                case WeaponMode.Normal:
                    // ����ź ������Ʈ�� �����ϰ�, ����ź�� ���� ��ġ�� �߻� ��ġ�� �Ѵ�.
                    GameObject bomb = Instantiate(bombFactory);
                    bomb.transform.position = firePosition.transform.position;

                    // ����ź ������Ʈ�� Rigidbody ������Ʈ�� �����´�.
                    Rigidbody rb = bomb.GetComponent<Rigidbody>();

                    // ī�޶��� ���� �������� ����ź�� �������� ���� ���Ѵ�.
                    rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);
                    break;

                case WeaponMode.Sniper:
                    // ����, �� ��� ���°� �ƴ϶�� ī�޶� Ȯ���ϰ� �� ��� ���·� �����Ѵ�.
                    if (!ZoomMode)
                    {
                        Camera.main.fieldOfView = 15f;
                        ZoomMode = true;
                    }
                    // �׷��� ������, ī�޶� ���� ���·� �ǵ����� �� ��� ���¸� �����Ѵ�.
                    else
                    {
                        Camera.main.fieldOfView = 60f;
                        ZoomMode = false;
                    }
                    break;
            }
        }

        // ���콺 ���� ��ư�� ������ �ü��� �ٶ󺸴� �������� ���� �߻��ϰ� �ʹ�.

        // ���콺 ���� ��ư �Է��� �޴´�.
        if (Input.GetMouseButtonDown(0))
        {
            // ���� �̵� ���� Ʈ�� �Ķ������ ���� 0�̶��, ���� �ִϸ��̼��� �ǽ��Ѵ�.
            if (anim.GetFloat("MoveMotion") == 0)
            {
                anim.SetTrigger("Attack");
            }

            // ���̸� �����ϰ� �߻�� ��ġ�� ���� ������ �����Ѵ�.
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            // ���̰� �ε��� ����� ������ ������ ������ �����Ѵ�.
            RaycastHit hitInfo = new RaycastHit();

            // ���̸� �߻��ϰ�, ���� �ε��� ��ü�� ������...
            if (Physics.Raycast(ray, out hitInfo))
            {
                // ���� ���̿� �ε��� ����� ���̾ "Enemy"��� ������ �Լ��� �����Ѵ�.
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    EnemyFSM eFSM = hitInfo.transform.GetComponent<EnemyFSM>();
                    eFSM.HitEnemy(weaponPower);
                }
                // �׷��� �ʴٸ�, ���̿� �ε��� ������ �ǰ� ����Ʈ�� �÷����Ѵ�.
                else
                {
                    // �ǰ� ����Ʈ�� ��ġ�� ���̰� �ε��� �������� �̵���Ų��.
                    bulletEffect.transform.position = hitInfo.point;

                    // �ǰ� ����Ʈ�� forward ������ ���̰� �ε��� ������ ���� ���Ϳ� ��ġ��Ų��.
                    bulletEffect.transform.forward = hitInfo.normal;

                    // �ǰ� ����Ʈ�� �÷����Ѵ�.
                    ps.Play();
                }
            }
            // �� ����Ʈ�� �ǽ��Ѵ�.
            StartCoroutine(ShootEffectOn(0.05f));
        }

        // ���� Ű������ ���� 1�� �Է��� ������, ���� ��带 �Ϲ� ���� �����Ѵ�.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            wMode = WeaponMode.Normal;

            // ī�޶��� ȭ���� �ٽ� ������� �����ش�.
            Camera.main.fieldOfView = 60f;

            // �Ϲ� ��� �ؽ�Ʈ ���
            wModeText.text = "Normal Mode";
        }
        // ���� Ű������ ���� 2�� �Է��� ������, ���� ��带 �������� ���� �����Ѵ�.
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            wMode = WeaponMode.Sniper;

            // �������� ��� �ؽ�Ʈ ���
            wModeText.text = "Sniper Mode";
        }
    }

    // �ѱ� ����Ʈ �ڷ�ƾ �Լ�
    IEnumerator ShootEffectOn(float duration)
    {
        // �����ϰ� ���ڸ� �̴´�.
        int num = Random.Range(0, eff_Flash.Length - 1);
        // ����Ʈ ������Ʈ �迭���� ���� ���ڿ� �ش��ϴ� ����Ʈ ������Ʈ�� Ȱ��ȭ�Ѵ�.
        eff_Flash[num].SetActive(true);
        // ������ �ð���ŭ ��ٸ���.
        yield return new WaitForSeconds(duration);
        // ����Ʈ ������Ʈ�� �ٽ� ��Ȱ��ȭ�Ѵ�.
        eff_Flash[num].SetActive(false);
    }
}