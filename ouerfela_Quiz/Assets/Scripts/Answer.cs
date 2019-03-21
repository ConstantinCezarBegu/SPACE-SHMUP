public class Answer
{
    public readonly string Text;
    public readonly bool IsCorrect;

    public Answer(string text, bool isCorrect = false)
    {
        Text = text;
        IsCorrect = isCorrect;
    }
}
