using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSpawns.Dialogues.DialogueAlgebra
{
    public class DialogueConsequenceWithExtraStaticParams<T> : DialogueConsequence
    {

        public override DialogueConsequenceDelegate ConsequenceExecutor { get; protected set; }


        public DialogueConsequenceWithExtraStaticParams(Action<DialogueParams, T> evaluator, T param, string exposedDefiner) : base(exposedDefiner)
        {
            ConsequenceExecutor = (p) => evaluator(p, param);
        }
    }

    public class DialogueConsequenceWithExtraStaticParams<T1, T2> : DialogueConsequence
    {

        public override DialogueConsequenceDelegate ConsequenceExecutor { get; protected set; }


        public DialogueConsequenceWithExtraStaticParams(Action<DialogueParams, T1, T2> evaluator, T1 param1, T2 param2, string exposedDefiner) : base(exposedDefiner)
        {
            ConsequenceExecutor = (p) => evaluator(p, param1, param2);
        }
    }

    public class DialogueConsequenceWithExtraStaticParams<T1, T2, T3> : DialogueConsequence
    {

        public override DialogueConsequenceDelegate ConsequenceExecutor { get; protected set; }


        public DialogueConsequenceWithExtraStaticParams(Action<DialogueParams, T1, T2, T3> evaluator, T1 param1, T2 param2, T3 param3, string exposedDefiner) : base(exposedDefiner)
        {
            ConsequenceExecutor = (p) => evaluator(p, param1, param2, param3);
        }
    }
}
