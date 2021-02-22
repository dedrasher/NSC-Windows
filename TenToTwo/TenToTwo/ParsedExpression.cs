namespace NumericSystemConverterApp
{
   public struct ParsedExpression
    {
        public string ToNC, FromNC, Input, Answer;
        public ParsedExpression(string ToNC, string FromNC, string Input, string Answer)
        {
            this.ToNC = ToNC;
            this.FromNC = FromNC;
            this.Input = Input;
            this.Answer = Answer;
        }
    }
}
