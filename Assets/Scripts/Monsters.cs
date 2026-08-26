using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO.Compression;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Monsters : MonoBehaviour
{

    public string mName;
    public Vector3 rayOrigin { get; set; }
    public SpriteRenderer spriteRenderer { get; set; }
    public Sprite[] moveSprites;//歩く用のスプライトを入れる
    public Sprite[] atkSprites;//攻撃用のスプライトを入れる

    public float atk = 50;//攻撃力
    public float hp = 100;
    public float maxHp { get; set; } //最大のHP
    public float spd = 1;//移動速度
    public float spdRate = 1;//バフやデバフを受けた時ここの値が変更される。ステータスマネージャーを介して変更するため、直接変更してはならない(移動速度)
    public float atkSpdRate = 1;//バフやデバフを受けた時ここの値が変更される。ステータスマネージャーを介して変更するため、直接変更してはならない(攻撃速度)
    public float atkRate = 1;//バフやデバフを受けた時ここの値が変更される。ステータスマネージャーを介して変更するため、直接変更してはならない(攻撃力)
    public float defRate = 1;

    public int player = 0;//左は0,右は1
    public float enemyDistance = 1;//敵を見つける距離(攻撃の当たる距離。攻撃の当たる距離のみを変更したい場合はAttack関数とEnemyCheck関数をオーバーライドして書き換える)
    public float enemyDistanceInCT = 1;
    public bool MoveOk { get; set; }
    public float allyDistance { get; set; } = 0.3f;//使用していない
    public float atkCT = 1;//攻撃のクールタイム
    public float aktCTimer = 0;
    public LayerMask myLayer { get; set; }
    public LayerMask enemyLayer { get; set; }

    public float stanTIme { get; set; } = 0;

    public enum Mode
    {
        idle,
        move,
        atk,
        atkCT,
        stan,
    }

    public Mode mode = Mode.move;

    public float MoveAniTime = 0.3f;
    public float MoveAniTimer { get; set; } = 0f;
    public int MoveAniSpriteNum { get; set; } = 0;

    public GameObject damageText { get; set; }
    public Canvas canvas { get; set; }
    public GameManager gameManager { get; set; }
    public AudioClip defaultAtkSE { get; set; }
    public GameObject monstarDeadPar { get; set; }
    public GameObject hitPar { get; set; }


    [Header("ランダムで攻撃するキャラの設定")]

    public float time = 2;//抽選間隔
    public float timer { get; set; } = 0;
    public float AtkTriggerRate = 50;
    public float AtkTriggerUpRate = 1.1f;

    public void RandomAtk()
    {
        if (time <= timer)
        {

            if (Random.Range(0, 100) <= AtkTriggerRate)
            {

                AtkTriggerRate = 20;
                RandomAtkTrigger();
                aktCTimer = atkCT;
            }
            else
            {
                AtkTriggerRate *= AtkTriggerUpRate;
            }
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    public void RandomAtkCharaUpdate()
    {
        if (isDead) return;
        EnemyCheck();
        if (mode != Mode.atk)
        {
            Move();
        }
        else
        {
            MoveAniTimer = MoveAniTime;
        }

        if (0 >= hp)
        {
            Dead();
        }
        RaycastHit2D[] hit;
        if (Physics2D.Raycast(transform.position + rayOrigin, transform.right, Mathf.Infinity, enemyLayer) && aktCTimer <= 0 && mode == Mode.move)
        {
            RandomAtk();
        }
        else if (aktCTimer > 0 && mode == Mode.move)
        {
            aktCTimer -= Time.deltaTime * atkSpdRate;
        }
        UpdateStatuses();
    }

    public virtual void RandomAtkTrigger()
    {

    }

    void Update()
    {

    }

    public virtual void Move()//移動と異動アニメーションを管理
    {
        if (mode == Mode.move && stanTIme <= 0 && MoveOk)
        {
            if (player == 0)
            {
                transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime * spd * Mathf.Clamp(spdRate, 0, 100), Space.World);

            }
            else
            {
                transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * spd * Mathf.Clamp(spdRate, 0, 100), Space.World);

            }

        }

        if (stanTIme > 0)
        {
            stanTIme -= Time.deltaTime;
        }

        if (MoveAniTimer >= MoveAniTime)
        {
            MoveAniSpriteNum = (MoveAniSpriteNum + 1) % moveSprites.Length;
            spriteRenderer.sprite = moveSprites[MoveAniSpriteNum];
            MoveAniTimer = 0;
            spriteRenderer.color = Color.white;
            ;
        }
        else
        {
            MoveAniTimer += Time.deltaTime * Mathf.Clamp(spdRate, 0, 2);
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

    }

    public virtual void StartSetup()//継承先のStart関数に入れる
    {

        if (CompareTag("Untagged")) tag = "Monster";
        maxHp = hp;
        canvas = GameObject.FindWithTag("DamageTextCanvas").GetComponent<Canvas>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rayOrigin = Vector2.zero;
        if (player == 1)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);

            myLayer = LayerMask.GetMask("Monster1");
            gameObject.layer = LayerMask.NameToLayer("Monster1");
            enemyLayer = LayerMask.GetMask("Monster0");
        }
        else
        {
            myLayer = LayerMask.GetMask("Monster0");
            gameObject.layer = LayerMask.NameToLayer("Monster0");
            enemyLayer = LayerMask.GetMask("Monster1");

        }

        damageText = Resources.Load<GameObject>("DamageText"); // Resources/Enemy.prefab をロード
        monstarDeadPar = Resources.Load<GameObject>("Paticle/MonstarDead"); // Resources/Enemy.prefab をロード
        defaultAtkSE = Resources.Load<AudioClip>("SE/dedaultDamage"); // Resources/Enemy.prefab をロード
        hitPar = Resources.Load<GameObject>("Paticle/HitEffect"); // Resources/Enemy.prefab をロード



        GameObject _gameManager = GameObject.FindWithTag("GameController");
        if (_gameManager != null)
        {
            gameManager = _gameManager.GetComponent<GameManager>();
        }
        GameManager.AddCharacter(this);
    }

    public virtual void Updating()//継承先のUpdate関数に入れる
    {
        if (isDead) return;
        //Allycheck();
        EnemyCheck();
        if (aktCTimer > 0 && mode != Mode.atk)
        {
            aktCTimer -= Time.deltaTime * atkSpdRate;
        }
        if (mode != Mode.atk)
        {
            Move();
        }
        else
        {
            MoveAniTimer = MoveAniTime;
        }

        if (0 >= hp)
        {
            Dead();
        }
        UpdateStatuses();
    }

    public bool isDead = false;
    public void Dead()
    {
        if (isDead)
        {
            return;
        }
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        isDead = true;
        if (gameManager != null) gameManager.resultDatas[(player + 1) % 2].killCount++;
        Dead2();
        GameManager.RemoveCharacter(this);
        Instantiate(monstarDeadPar, transform.position, Quaternion.identity);
        spriteRenderer.enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        transform.SetParent(null);
        Destroy(gameObject, 15);
    }

    public virtual void Dead2()
    {

    }

    public float Attack(Monsters target, float damage = -800, bool Melee = true, StatusManager newStatus = null)//引数に攻撃対象のMonstrsコンポーネントを入れる
    {
        if (damage <= -800) damage = atk;
        if (target == null || !target.gameObject.activeSelf) { return 0; }

        if (!CanHitTarget(target.transform) && Melee) return 0;

        if (hp > 0)
        {
            target.Damaged(damage * atkRate, this, Melee, newStatus);
            if (target.hp >= 0)
            {
                target.DamagedAnimTrigger();
            }
            return damage * atkRate;
        }
        return 0;

    }

    public void InsHitPar(Monsters attacker)
    {
        if (hp <= 0) return;
        if (attacker == null)
        {
            Instantiate(hitPar, transform.position, Quaternion.identity);
            return;
        }

        Instantiate(hitPar, new Vector3(transform.position.x, attacker.transform.position.y, transform.position.z), Quaternion.identity);
    }

    public StatusManager GetEffect(string id, bool stack, StatusManager.StatusType type, float time, float value)
    {
        StatusManager newStatus = new StatusManager(id, stack, type, time, value);
        return newStatus;
    }

    public bool CanHitTarget(Transform target)
    {
        if (target == null) return false;

        // 敵のコライダーの一番近い点を取る
        Collider2D col = target.GetComponent<Collider2D>();
        if (col == null) return false;

        float dist = Vector2.Distance(
            col.ClosestPoint(transform.position), // 自分から見て敵の最も近い点
            transform.position
        );

        return dist <= enemyDistance * 1.1; // attackRange は「当たる距離」
    }


    public virtual void Damaged(float damage, Monsters attacker, bool Melee = true, StatusManager newStatus = null)
    {
        if (newStatus != null) ApplyStatus(newStatus);
        InsHitPar(attacker);


        if (hp > 0)
        {
            if (damage > 0)
            {
                AudioManager.PlaySE(defaultAtkSE, 0.3f);
            }
            else
            {
                return;
            }
            float _damage = damage / defRate;
            hp -= _damage;
            Text _damageText = Instantiate(damageText, transform.position, Quaternion.identity, canvas.transform).GetComponent<Text>();

            if (_damage >= 1)
            {
                _damageText.text = (damage / defRate).ToString("F0");

            }
            else
            {
                _damageText.text = (damage / defRate).ToString("F1");
            }
        }

    }


    public void Healed(float healValue)
    {
        hp += healValue;
        if (hp > maxHp) hp = maxHp;
        Text _damageText = Instantiate(damageText, transform.position, Quaternion.identity, canvas.transform).GetComponent<Text>();
        _damageText.color = Color.green;
        _damageText.text = "+" + healValue.ToString("F0");
    }


    public void DamagedAnimTrigger()
    {
        StartCoroutine(DamagedAnim());
    }


    public IEnumerator DamagedAnim()
    {
        bool flag = true;

        for (int i = 0; i < 10; i++)
        {
            flag = !flag;
            spriteRenderer.color = flag ? new Color(1, 1, 1, 0.5f) : Color.white;
            yield return Wait(0.05f, 2);
        }

        spriteRenderer.color = Color.white;

    }

    public virtual void EnemyCheck()
    {
        //Vector2 origin = (Vector2)transform.position + rayOrigin;

        if (player == 1)
        {
            //origin = (Vector2)transform.position + new Vector2(rayOrigin.x * -1, rayOrigin.y);
        }
        bool _MoveOk = true;

        Vector2 direction = transform.right;

        float _enemyDistance = enemyDistance;
        if (aktCTimer > 0 || enemyDistance == 0) _enemyDistance = enemyDistanceInCT;

        Debug.DrawRay(transform.position + rayOrigin, direction * _enemyDistance, Color.red, 0.1f); // 可視化（長さに注意）

        RaycastHit2D hit = Physics2D.Raycast(transform.position + rayOrigin, direction, _enemyDistance, enemyLayer);
        if (hit.collider != null)
        {
            Monsters targetData = hit.collider.gameObject.GetComponent<Monsters>();
            if (targetData != null && gameObject != hit.collider.gameObject)
            {
                if (targetData.player != player)
                {
                    _MoveOk = false;
                    if (mode != Mode.atk && aktCTimer <= 0 && enemyDistance > 0)
                    {
                        aktCTimer = atkCT;
                        Coroutine atkMotion = StartCoroutine(AtkMotion(targetData));
                    }
                }
            }
        }
        MoveOk = _MoveOk;
    }

    public void Allycheck()
    {
        Vector2 origin = transform.position + rayOrigin;

        if (player == 1)
        {
            origin = (Vector2)transform.position + new Vector2(rayOrigin.x * -1, rayOrigin.y);
        }

        Vector2 direction = transform.right;

        Debug.DrawRay(origin, direction * allyDistance, Color.red, 0.5f); // 可視化（長さに注意）

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, allyDistance, LayerMask.GetMask("Monster"));
        if (hit.collider != null)
        {
            Monsters targetData = hit.collider.gameObject.GetComponent<Monsters>();
            if (targetData != null && gameObject != hit.collider.gameObject)
            {
                if (targetData.player == player)
                {
                    if (mode != Mode.atk)
                    {
                        mode = Mode.idle;
                    }
                }
            }
        }
    }

    public virtual IEnumerator AtkMotion(Monsters target)//攻撃アニメーションなど
    {
        int num = 0;
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }

    }
    /* protected WaitForSeconds Wait(float duration, float rate = 1)
     {
         return new WaitForSeconds(duration / Mathf.Clamp(rate, 0.001f, 100));
     }*/
    public IEnumerator Wait(float duration, int type = 0)
    {
        float time = duration;
        float timer = 0;
        while (time >= timer)
        {
            float rate = 1;
            switch (type)
            {
                case 0:
                    rate = atkSpdRate;
                    break;
                case 1:
                    rate = spdRate;
                    break;
                case >= 2:
                    rate = 1;
                    break;
            }
            timer += Time.deltaTime * Mathf.Clamp(rate, 0.001f, 5);
            yield return null;
        }
    }

    [System.Serializable]
    public class StatusManager
    {
        public string id;
        public bool stack = true;

        public enum StatusType
        {
            spdRate,
            atkRate,
            atkSpdRate,
            heal,
            stan,
            def,
        }

        public StatusType type;
        public float time = 0f;
        public float value = 1f;

        public StatusManager(string id, bool stack, StatusType type, float time, float value)
        {
            this.id = id;
            this.stack = stack;
            this.type = type;
            this.time = time;
            this.value = value;
        }

        public bool UpdateTimer(float deltaTime)
        {
            time -= deltaTime;
            return time <= 0;
        }
    }

    public List<StatusManager> sm = new List<StatusManager>();

    public void ApplyStatusTarget(Monsters monsters, StatusManager newStatus)
    {
        if (monsters == null) return;
        monsters.ApplyStatus(newStatus);
    }


    public void ApplyStatus(StatusManager newStatus)
    {
        if (!newStatus.stack)
        {
            var existing = sm.Find(s => s.id == newStatus.id);
            if (existing != null)
            {
                existing.time = newStatus.time;
                return;
            }
        }

        sm.Add(newStatus);
        ApplyStatusEffect(newStatus);
    }

    public void UpdateStatuses()
    {
        for (int i = sm.Count - 1; i >= 0; i--)
        {
            if (sm[i].UpdateTimer(Time.deltaTime))
            {
                RemoveStatusEffect(sm[i]);
                sm.RemoveAt(i);
            }
        }
    }

    void ApplyStatusEffect(StatusManager status)
    {
        switch (status.type)
        {
            case StatusManager.StatusType.spdRate:
                spdRate += status.value;
                break;
            case StatusManager.StatusType.atkRate:
                atkRate += status.value;
                break;
            case StatusManager.StatusType.atkSpdRate:
                atkSpdRate += status.value;
                break;
            case StatusManager.StatusType.heal:
                Healed(status.value);
                break;
            case StatusManager.StatusType.stan:
                mode = Mode.stan;
                break;
            case StatusManager.StatusType.def:
                defRate += status.value;
                break;
        }
    }

    void RemoveStatusEffect(StatusManager status)
    {
        switch (status.type)
        {
            case StatusManager.StatusType.spdRate:
                spdRate -= status.value;
                break;
            case StatusManager.StatusType.atkRate:
                atkRate -= status.value;
                break;
            case StatusManager.StatusType.atkSpdRate:
                atkSpdRate -= status.value;
                break;
            case StatusManager.StatusType.stan:
                mode = Mode.move;
                break;
            case StatusManager.StatusType.def:
                defRate -= status.value;
                break;
        }
    }
}
