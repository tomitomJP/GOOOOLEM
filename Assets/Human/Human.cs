using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Human : Monsters
{
    [Header("人間の設定")]
    public int level = 1;
    public float growthRate = 0.325f; // 二次関数の係数

    public string name = "ああああ";
    public string job;

    public enum Sex
    {
        man,
        woman,
        unkown
    }

    public Sex sex = Sex.man;




    public void HumanSetUp()
    {
        maxHp = hp;

        monstarDeadPar = Resources.Load<GameObject>("Paticle/HumanDeadParticle");


    }



    public override void Dead2()
    {
        if (SoloManager.instance == null) return;

        //SoloManager.SetHT($"{name}は死んでしまった。", SoloManager.ToHex(Color.red));
    }


    public IEnumerator First()
    {
        yield return new WaitForEndOfFrameUnit();
        float spdDefault = spd;
        spd = 2;
        aktCTimer = 100;
        float saveDef = defRate;
        defRate = 10;

        float saveAtkTriggerRate = AtkTriggerRate;
        float saveAtkTriggerUpRate = AtkTriggerUpRate;
        AtkTriggerRate = 0;
        AtkTriggerUpRate = 0;

        yield return new WaitForSeconds(1.4f);
        spd = spdDefault;
        aktCTimer = 0;
        AtkTriggerRate = saveAtkTriggerRate;
        AtkTriggerUpRate = saveAtkTriggerUpRate;
        defRate = saveDef;
        FirstSkill();
    }

    public virtual void FirstSkill()
    {

    }
}
