using CustomSpawns.Dialogues.DialogueAlgebra.Condition;

namespace CustomSpawns.Dialogues.DialogueAlgebra
{

    public delegate bool DialogueConditionDelegate(DialogueParams conditionParams);

    public abstract class DialogueCondition
    {
        public string ExposedDefiningString { get; private set; }

        public abstract DialogueConditionDelegate ConditionEvaluator { get; protected set; }

        public DialogueCondition(string exposedDefiner)
        {
            ExposedDefiningString = exposedDefiner;
        }

        public static DialogueCondition operator &(DialogueCondition d1, DialogueCondition d2)
        {
            return new DialogueConditionBare(
                (p) => d1.ConditionEvaluator(p) & d2.ConditionEvaluator(p),
                "(" + d1.ExposedDefiningString + ") & (" + d2.ExposedDefiningString + ")"
                );
        }

        public static DialogueCondition operator |(DialogueCondition d1, DialogueCondition d2)
        {
            return new DialogueConditionBare(
                (p) => d1.ConditionEvaluator(p) | d2.ConditionEvaluator(p),
                "(" + d1.ExposedDefiningString + ") | (" + d2.ExposedDefiningString + ")"
                );
        }

        public static DialogueCondition operator !(DialogueCondition d)
        {
            return new DialogueConditionBare((p) => !d.ConditionEvaluator(p), "!(" + d.ExposedDefiningString + ")");
        }
    }

}
