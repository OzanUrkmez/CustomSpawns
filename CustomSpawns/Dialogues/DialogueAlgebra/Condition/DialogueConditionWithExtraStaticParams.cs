using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSpawns.Dialogues.DialogueAlgebra.Condition
{
    public class DialogueConditionWithExtraStaticParams<T> : DialogueCondition
    {

        public override DialogueConditionDelegate ConditionEvaluator { get; protected set; }


        public DialogueConditionWithExtraStaticParams(Func<DialogueParams, T, bool> evaluator, T param, string exposedDefiner): base(exposedDefiner)
        {
            ConditionEvaluator = (p) => evaluator(p, param);
        }
    }

    public class DialogueConditionWithExtraStaticParams<T1, T2> : DialogueCondition
    {

        public override DialogueConditionDelegate ConditionEvaluator { get; protected set; }


        public DialogueConditionWithExtraStaticParams(Func<DialogueParams, T1, T2, bool> evaluator,
            T1 param1, T2 param2, string exposedDefiner) : base(exposedDefiner)
        {
            ConditionEvaluator = (p) => evaluator(p, param1, param2);
        }
    }

    public class DialogueConditionWithExtraStaticParams<T1, T2, T3> : DialogueCondition
    {

        public override DialogueConditionDelegate ConditionEvaluator { get; protected set; }

        public DialogueConditionWithExtraStaticParams(Func<DialogueParams, T1, T2, T3, bool> evaluator,
            T1 param1, T2 param2, T3 param3, string exposedDefiner) : base(exposedDefiner)
        {
            ConditionEvaluator = (p) => evaluator(p, param1, param2, param3);
        }
    }
}
