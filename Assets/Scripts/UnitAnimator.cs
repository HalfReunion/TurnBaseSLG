using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform bulletProjectTileTran;
    [SerializeField] Transform shootPointTran;
    [SerializeField] ShootAnimaInfo shootInfo;
    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }
        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShootStart += ShootAction_OnShootStart;
            shootAction.OnShooting += ShootAction_OnShoot;
            shootAction.OnShootEnd += ShootAction_OnShootEnd; 
        }

        var updateVisual = GetComponentInChildren<UnitSelectedVisual>();
        if (updateVisual!=null) {
            updateVisual.UnitSelected += UpdateSelected_SelectedAnima;
        }
    }

    private void UpdateSelected_SelectedAnima(bool isSet) {
        animator.SetBool(Animator.StringToHash("SelectedTrigger"), isSet);

    }
    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool(Animator.StringToHash("IsWalking"), true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking", false);
    }

    private void ShootAction_OnShootStart(object sender, EventArgs e)
    {
        animator.SetBool("AttackActionTrigger", true);
    }

    private async void ShootAction_OnShoot(object sender, EventArgs e)
    {
        animator.SetTrigger("Attacking");
        ShootAction.OnShootEventArgs args = (ShootAction.OnShootEventArgs)e;
        
        await TakeShootAnima(args);
        args.OnDamage(args.Damage);
        args.MakeBuff(args.BuffList);
    }

  

    private void createBullet(ShootAction.OnShootEventArgs args)
    { 
        Transform t = GameObject.Instantiate(bulletProjectTileTran, shootPointTran.position, Quaternion.identity);
        BulletProjectTile bulletProjectTile = t.GetComponent<BulletProjectTile>();
        Vector3 shootTargetPos = args.TargetUnit.GetWorldPosition();
        //提升一个枪口的Y
        shootTargetPos.y += t.position.y;
        bulletProjectTile.Setup(shootTargetPos);
    }



    private void ShootAction_OnShootEnd(object sender, EventArgs e)
    {
        animator.SetBool("AttackActionTrigger", false);
    }
     

    private async UniTask TakeShootAnima(ShootAction.OnShootEventArgs args)
    { 
        List<int> frames = shootInfo.keys;
        int idx = -1;
      
        while (true)
        { 
            var info = animator.GetCurrentAnimatorStateInfo(0);
          
            if (info.IsName("Attack_Ing"))
            {
                for (int i = 0; i < frames.Count;i++)
                {
                    while (true)
                    {
                        float length = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
                        float frameRate = animator.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate;
                        float totalFrame = length / (1 / frameRate);
                        var currentTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                        int currentFrame = (int)(Mathf.Floor(totalFrame * currentTime) % totalFrame);
                         
                        if (currentFrame >= frames[i])
                        {
                            idx = i;
                           
                            createBullet(args);
                            break;
                        }
                        await UniTask.WaitForEndOfFrame(this);
                    }
                }
            }
            if (idx == frames.Count - 1) return;
            await UniTask.WaitForEndOfFrame(this); 
        }
    }
}
