namespace CustomSpawns.Dialogues.DialogueAlgebra
{
    public class DialogueConsequenceBare: DialogueConsequence
    {

        public override DialogueConsequenceDelegate ConsequenceExecutor { get; protected set; }

        public DialogueConsequenceBare(DialogueConsequenceDelegate executor, string exposedDefiner) : base(exposedDefiner)
        {
            ConsequenceExecutor = executor;
        }

    }
}
