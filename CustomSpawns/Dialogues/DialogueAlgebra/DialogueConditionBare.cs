using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSpawns.Dialogues.DialogueAlgebra
{
    public class DialogueConditionBare : DialogueCondition
    {
        public override DialogueConditionDelegate ConditionEvaluator { get; protected set; }

        public DialogueConditionBare(DialogueConditionDelegate evaluator, string exposedDefiner) : base(exposedDefiner)
        {
            ConditionEvaluator = evaluator;
        }
    }
}
