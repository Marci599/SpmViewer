using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEditor.UIElements;
using UnityEngine;

namespace PM.Models.Animations
{
    public class PMAnimator : MonoBehaviour
    {
        public PMAnimation Animation { get; private set; }
        public ModelAnimCurves Curves;
        public bool isStartWithRotate = false;

        public bool IsPlaying { get; private set; }
        float StartTime;

        [SerializeField]
        private float Duration;

        bool flipped = false;

        [SerializeField]
        float speed = 19f;

        public void SetAnimation(PMAnimation anim)
        {
            Animation = anim;

            DeltaCurve c = new(anim);
            c.Bake();
            Curves = new ModelAnimCurves(Animation.Data.Keyframes, c.Frames);
        }

        public void Play()
        {
            if (IsPlaying)
                Stop();

      
            Animation.Model.transform.rotation = Quaternion.identity;

         
            if (isStartWithRotate)
            {
                flipped = false;
                Animation.Model.transform.localScale = new Vector3(-1, 1, 1);
            }

            Animation.Model.ResetVirtualModel();
            IsPlaying = true;
            StartTime = Time.time;
            Duration = Animation.GetDuration();
        }

        public void Stop()
        {
            if (IsPlaying)
                IsPlaying = false;
            //Animation.Model.transform.rotation = Quaternion.identity;
        }

        void Update()
        {
            float actualSpeed = (180 / speed) * 60;
            float deltaSpeed = actualSpeed * Time.deltaTime;
            if (Duration != 0)
            {
                if (IsPlaying)
                {
                  
                    float time = (Time.time - StartTime) * 60f;



                    if (isStartWithRotate && time > Duration - speed - 1)
                    {
                        if (!flipped)
                        {
                            var rot = Animation.Model.transform.rotation.eulerAngles;
                            rot.y -= deltaSpeed;
                            Animation.Model.transform.rotation = Quaternion.Euler(rot);

                            if (Mathf.DeltaAngle(0, rot.y) <= -90f)
                            {
                                flipped = true;
                            }
                        }
                        else if (flipped)
                        {
                            var rot = Animation.Model.transform.rotation.eulerAngles;
                            rot.y += deltaSpeed;
                            Animation.Model.transform.rotation = Quaternion.Euler(rot);
                            Animation.Model.transform.localScale = new Vector3(1, 1, 1);

                            if (Mathf.Abs(Mathf.DeltaAngle(0, rot.y)) < 1f)
                            {
                  
                                isStartWithRotate = false;
                                Animation.Model.transform.rotation = Quaternion.identity;
                            }
                        }
                    }







                    if (Animation.Loop)
                        time %= Duration;
                    else
                    {
                        if (time > Duration)
                        {
                            Stop();
                            return;
                        }
                    }
                    Curves.Evaluate(Animation.Model, time);
                }
            }
            else
            {
                if (isStartWithRotate)
                {
                    var rot = Animation.Model.transform.rotation.eulerAngles;
                    rot.y -= deltaSpeed;
                    Animation.Model.transform.rotation = Quaternion.Euler(rot);
                
                        

                    if (Mathf.DeltaAngle(0, rot.y) <= -90f || Mathf.DeltaAngle(0, rot.y) > 90f)
                    {
                        Animation.Model.transform.localScale = new Vector3(1, 1, -1);
                    }
                    else
                    {
                        Animation.Model.transform.localScale = new Vector3(1, 1, 1);
                    }
                 
             
      
                    
                }
            }
            
        }
    }
}