namespace Application_1.Validations
{
    // Global Validation Class
    class Validation
    {
        // Validate Line is Empty
        public bool ValidateLineIsEmpty(string line)
        {
            return (line.Length == 0);
        }

        // Validate Line is a Comment
        public bool ValidateLineIsComment(string line)
        {
            return line.StartsWith(AllocationsForm.commentPrefix);
        }
    }
}
