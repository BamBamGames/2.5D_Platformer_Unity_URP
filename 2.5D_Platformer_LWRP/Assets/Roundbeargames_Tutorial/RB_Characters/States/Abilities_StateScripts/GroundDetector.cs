﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roundbeargames
{
    [CreateAssetMenu(fileName = "New State", menuName = "Roundbeargames/AbilityData/GroundDetector")]
    public class GroundDetector : StateData
    {
        public float Distance;

        private GameObject testingSphere;

        public GameObject TESTING_SPHERE
        {
            get
            {
                if (testingSphere == null)
                {
                    testingSphere = GameObject.Find("TestingSphere");
                }
                return testingSphere;
            }
        }

        public override void OnEnter(CharacterState characterState, Animator animator, AnimatorStateInfo stateInfo)
        {

        }

        public override void UpdateAbility(CharacterState characterState, Animator animator, AnimatorStateInfo stateInfo)
        {
            if (IsGrounded(characterState.characterControl))
            {
                animator.SetBool(HashManager.Instance.DicMainParams[TransitionParameter.Grounded], true);
            }
            else
            {
                animator.SetBool(HashManager.Instance.DicMainParams[TransitionParameter.Grounded], false);
            }
        }

        public override void OnExit(CharacterState characterState, Animator animator, AnimatorStateInfo stateInfo)
        {

        }

        bool IsGrounded(CharacterControl control)
        {
            if (control.contactPoints != null)
            {
                foreach (ContactPoint c in control.contactPoints)
                {
                    float colliderBottom = (control.transform.position.y + control.boxCollider.center.y)
                        - (control.boxCollider.size.y / 2f);
                    float yDifference = Mathf.Abs(c.point.y - colliderBottom);

                    if (yDifference < 0.01f)
                    {
                        if (Mathf.Abs(control.RIGID_BODY.velocity.y) < 0.001f)
                        {
                            control.animationProgress.Ground = c.otherCollider.transform.root.gameObject;
                            control.animationProgress.LandingPosition = new Vector3(
                                0f,
                                c.point.y,
                                c.point.z);

                            if (control.manualInput.enabled)
                            {
                                TESTING_SPHERE.transform.position = control.animationProgress.LandingPosition;
                            }
                            return true;
                        }
                    }
                }
            }

            if (control.RIGID_BODY.velocity.y < 0f)
            {
                foreach (GameObject o in control.collisionSpheres.BottomSpheres)
                {
                    Debug.DrawRay(o.transform.position, -Vector3.up * Distance, Color.yellow);
                    RaycastHit hit;
                    if (Physics.Raycast(o.transform.position, -Vector3.up, out hit, Distance))
                    {
                        if (!control.RagdollParts.Contains(hit.collider)
                            && !Ledge.IsLedgeChecker(hit.collider.gameObject)
                            && !Ledge.IsCharacter(hit.collider.gameObject))
                        {
                            control.animationProgress.Ground = hit.collider.transform.root.gameObject;
                            control.animationProgress.LandingPosition = new Vector3(
                                0f,
                                hit.point.y,
                                hit.point.z);
                            if (control.manualInput.enabled)
                            {
                                TESTING_SPHERE.transform.position = control.animationProgress.LandingPosition;
                            }
                            return true;
                        }
                    }
                }
            }

            control.animationProgress.Ground = null;
            return false;
        }
    }
}