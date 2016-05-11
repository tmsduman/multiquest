using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TreeSharp
{
    public class BehaviourTest : MonoBehaviour
    {
        [SerializeField]
        private BehaviourTreeManager manager;

        private void Start()
        {
            TestBehaviour behaviour = new TestBehaviour();
            manager.RegisterBehaviour(behaviour, null);
        }
    }
}
