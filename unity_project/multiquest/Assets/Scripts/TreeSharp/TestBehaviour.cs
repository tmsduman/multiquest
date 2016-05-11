using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeSharp
{
    public class TestBehaviour : Composite
    {
        public Composite GetComposite()
        {
            return new Sequence(
                new PrioritySelector(
                    new Decorator((context) => 
                    {
                        System.Random r = new Random();
                        var i = r.Next(0, 10);
                        return i%2 == 0;
                    },
                    new Action((context) => UnityEngine.Debug.Log("Action 1"))),
                new Action((context) => UnityEngine.Debug.Log("Action 2"))));
                //new Action((context) => UnityEngine.Debug.Log("Action 2")));
        }

        public override IEnumerable<RunStatus> Execute(object context)
        {
            return this.GetComposite().Execute(context);
        }
    }
}
