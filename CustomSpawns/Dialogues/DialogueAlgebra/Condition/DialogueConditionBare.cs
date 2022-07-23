
namespace CustomSpawns.Dialogues.DialogueAlgebra.Condition
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
