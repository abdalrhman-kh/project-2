namespace project_2.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreateDate {  get; set; }
        public DateTime? EditDate { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}
