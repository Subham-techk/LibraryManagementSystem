namespace LibraryManagementSystem.Models
{
    public class IssueBook
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public string IssuedTo { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public bool IsReturned { get; set; }

        public Book Book { get; set; }
    }
}
