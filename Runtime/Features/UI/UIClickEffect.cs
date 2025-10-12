using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedMinS
{
    public class UIClickEffect : MonoBehaviour
    {
        [SerializeField] Animator anim;
        [SerializeField] float offTime;

        ObjectPool _pool;


        public void StartEffect(ObjectPool pool)
        {
            _pool = pool;

            if (anim != null)
            {
                //anim.SetTrigger("START");
                Invoke("EffectOff", offTime);
            }
        }

        void EffectOff()
        {
            _pool.RemoveObject(this.gameObject);
        }
    }
}
