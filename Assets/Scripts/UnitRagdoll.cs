using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone;

    public void Setup(Transform originalRootBone) {
        matchAllChildTransforms(originalRootBone, ragdollRootBone);
        //applyExplosionToRagdoll(ragdollRootBone,250f,transform.position,10f);
    }
    /// <summary>
    /// ��ԭ��ģ�͵Ĺ����ڵ�λ��ȫ�����Ƹ������޵Ľڵ�
    /// </summary>
    /// <param name="root"></param>
    /// <param name="clone"></param>
    private void matchAllChildTransforms(Transform root, Transform clone)
    {
        foreach (Transform child in root) {
            Transform cloneChild = clone.Find(child.name);
            if (cloneChild != null) {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;
                matchAllChildTransforms(child, cloneChild);
            }
        }
    }

    /// <summary>
    /// �����������ӱ�ը����
    /// </summary>
    /// <param name="root"></param>
    /// <param name="explosionForce"></param>
    private void applyExplosionToRagdoll(Transform root,float explosionForce,
        Vector3 explosionPosition,float explosionRange) 
    {
        foreach (Transform child in root) {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRidy)) {
                //������
                childRidy.AddExplosionForce(explosionForce,explosionPosition,explosionRange);
            }
            applyExplosionToRagdoll(child,explosionForce,explosionPosition,explosionRange);
        }
       
    }

}
