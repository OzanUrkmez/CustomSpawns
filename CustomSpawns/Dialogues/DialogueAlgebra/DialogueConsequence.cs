namespace CustomSpawns.Dialogues.DialogueAlgebra
{

    public delegate void DialogueConsequenceDelegate(DialogueParams conditionParams);

    public abstract class DialogueConsequence
    {
        public string ExposedDefiningString { get; private set; }

        public abstract DialogueConsequenceDelegate ConsequenceExecutor { get; protected set; }

        public DialogueConsequence(string exposedDefiner)
        {
            ExposedDefiningString = exposedDefiner;
        }

        public static DialogueConsequence operator +(DialogueConsequence d1, DialogueConsequence d2)
        {
            return new DialogueConsequenceBare(
                delegate (DialogueParams p)
                {
                    d1.ConsequenceExecutor(p);
                    d2.ConsequenceExecutor(p);
                },
                "(" + d1.ExposedDefiningString + ") + (" + d2.ExposedDefiningString + ")"
                );
        }
    }

}
